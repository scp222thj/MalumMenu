using HarmonyLib;
using AmongUs.GameOptions;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;

namespace MalumMenu;

/// <summary>
/// Intercepts RpcSetRole at game start so the host can swap roles safely.
/// Buffers assignments, applies swap logic once, then releases. Never leaves clients
/// without a role RPC — timeout, disconnect, and ResetState all flush the buffer.
/// </summary>
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetRole))]
public static class HostRoleSwapManager
{
    private static readonly ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("HostRoleSwap");

    private enum SwapState
    {
        Inactive,
        Buffering,
        Releasing,
        Done
    }

    private static SwapState _state = SwapState.Inactive;

    private static readonly Dictionary<byte, RoleTypes> _bufferedAssignments = new();
    private static readonly Dictionary<byte, bool> _bufferedOverrideFlags = new();
    private static readonly HashSet<byte> _seenPlayers = new();

    private static float _bufferStartTime;
    private const float BUFFER_TIMEOUT_SEC = 3f;

    private static RoleTypes _expectedLocalRole;
    private static bool _pendingVerification;
    private static bool _swapLogicApplied;

    // ========================================================================
    // HARMONY PREFIX
    // ========================================================================

    private static RoleTypes GetTargetRole() => CheatToggles.roleSwapTarget ?? RoleTypes.Crewmate;

    public static bool Prefix(PlayerControl __instance, ref RoleTypes roleType, bool canOverrideRole)
    {
        if (!Utils.isHost || !CheatToggles.roleSwap)
            return true;

        if (_state == SwapState.Done || _state == SwapState.Releasing)
            return true;

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
            return true;

        var targetRole = GetTargetRole();

        if (_state == SwapState.Inactive)
        {
            _state = SwapState.Buffering;
            _bufferStartTime = Time.time;
            _swapLogicApplied = false;

            Log.LogInfo(
                $"[HostRoleSwap] Buffering started | target={targetRole} | players={PlayerControl.AllPlayerControls.Count} | timeout={BUFFER_TIMEOUT_SEC}s");
        }

        if (__instance == localPlayer && roleType == targetRole)
        {
            Log.LogInfo("[HostRoleSwap] Local player already has target role — flushing unchanged");
            FlushBuffer(applyRoleSwap: false);
            return true;
        }

        BufferAssignment(__instance, roleType, canOverrideRole);

        if (ShouldReleaseBatch(localPlayer))
            FlushBuffer(applyRoleSwap: true);

        return false;
    }

    // ========================================================================
    // FRAME TICK — timeout even when no more RpcSetRole calls arrive (disconnect)
    // ========================================================================

    public static void Tick()
    {
        if (_state != SwapState.Buffering)
            return;

        if (!Utils.isHost || !CheatToggles.roleSwap)
        {
            Log.LogWarning("[HostRoleSwap] Tick: feature off while buffering — flushing unchanged");
            FlushBuffer(applyRoleSwap: false);
            return;
        }

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
            return;

        if (!HasTimedOut())
            return;

        var activeCount = PlayerControl.AllPlayerControls.Count;
        Log.LogWarning(
            $"[HostRoleSwap] Tick timeout ({BUFFER_TIMEOUT_SEC}s) | buffered={_bufferedAssignments.Count} | seen={_seenPlayers.Count}/{activeCount}");

        FlushBuffer(applyRoleSwap: true);
    }

    // ========================================================================
    // DISCONNECT / LOBBY LIFECYCLE
    // ========================================================================

    public static void OnPlayerDisconnected(PlayerControl player)
    {
        if (_state != SwapState.Buffering)
            return;

        if (player == null)
            return;

        var playerId = player.PlayerId;
        var activeCount = PlayerControl.AllPlayerControls.Count;

        Log.LogWarning(
            $"[HostRoleSwap] Player disconnected during buffering | id={playerId} | seen={_seenPlayers.Count} | active={activeCount} | buffered={_bufferedAssignments.Count}");

        _seenPlayers.Remove(playerId);
        _bufferedAssignments.Remove(playerId);
        _bufferedOverrideFlags.Remove(playerId);

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
            return;

        if (!ShouldReleaseBatch(localPlayer))
            return;

        Log.LogInfo("[HostRoleSwap] Disconnect reduced lobby — releasing batch now");
        FlushBuffer(applyRoleSwap: true);
    }

    /// <summary>Call when joining a lobby or leaving a game — always safe to clear.</summary>
    public static void ResetState()
    {
        if (_state == SwapState.Buffering || _state == SwapState.Releasing)
        {
            Log.LogWarning(
                $"[HostRoleSwap] ResetState during {_state} — flushing buffer first (swapApplied={_swapLogicApplied})");

            FlushBuffer(applyRoleSwap: _swapLogicApplied && CheatToggles.roleSwap);
            return;
        }

        ClearAll(SwapState.Inactive, "ResetState");
    }

    /// <summary>Call on game start — do not abort an in-flight role assignment batch.</summary>
    public static void ResetStateForNewGame()
    {
        if (_state == SwapState.Buffering || _state == SwapState.Releasing)
        {
            Log.LogInfo($"[HostRoleSwap] ResetStateForNewGame skipped — batch in progress ({_state})");
            return;
        }

        ClearAll(SwapState.Inactive, "new game");
    }

    // ========================================================================
    // BUFFER / RELEASE
    // ========================================================================

    private static void BufferAssignment(PlayerControl player, RoleTypes roleType, bool canOverrideRole)
    {
        var playerId = player.PlayerId;
        _seenPlayers.Add(playerId);
        _bufferedAssignments[playerId] = roleType;
        _bufferedOverrideFlags[playerId] = canOverrideRole;

        Log.LogInfo(
            $"[HostRoleSwap] Buffered id={playerId} name={player.Data?.PlayerName ?? "?"} role={roleType} override={canOverrideRole} | progress={_seenPlayers.Count}/{PlayerControl.AllPlayerControls.Count}");
    }

    private static bool ShouldReleaseBatch(PlayerControl localPlayer)
    {
        var activeCount = PlayerControl.AllPlayerControls.Count;
        if (activeCount <= 0)
            return false;

        var allPlayersSeen = _seenPlayers.Count >= activeCount;
        var hasLocal = _bufferedAssignments.ContainsKey(localPlayer.PlayerId);
        var timedOut = HasTimedOut();

        if (allPlayersSeen && hasLocal)
        {
            Log.LogInfo($"[HostRoleSwap] All {activeCount} active players buffered — releasing");
            return true;
        }

        if (timedOut)
        {
            Log.LogWarning(
                $"[HostRoleSwap] Timeout with partial batch | seen={_seenPlayers.Count}/{activeCount} | hasLocal={hasLocal} | will release buffered as-is or swapped");
            return _bufferedAssignments.Count > 0;
        }

        return false;
    }

    private static bool HasTimedOut() => Time.time - _bufferStartTime > BUFFER_TIMEOUT_SEC;

    private static void FlushBuffer(bool applyRoleSwap)
    {
        if (_state != SwapState.Buffering)
            return;

        if (_bufferedAssignments.Count == 0)
        {
            Log.LogWarning("[HostRoleSwap] FlushBuffer called with empty buffer — clearing state");
            ClearAll(SwapState.Inactive, "empty flush");
            return;
        }

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
        {
            Log.LogError("[HostRoleSwap] FlushBuffer: no local player — releasing unchanged");
            ReleaseAssignments(applySwap: false);
            return;
        }

        if (applyRoleSwap && CheatToggles.roleSwap)
            ApplySwapAndRelease(localPlayer, GetTargetRole());
        else
            ReleaseAssignments(applySwap: false);
    }

    private static void ApplySwapAndRelease(PlayerControl localPlayer, RoleTypes targetRole)
    {
        _state = SwapState.Releasing;

        if (!_bufferedAssignments.TryGetValue(localPlayer.PlayerId, out var localOriginalRole))
        {
            Log.LogError("[HostRoleSwap] Local player missing from buffer — releasing all unchanged");
            ReleaseAssignments(applySwap: false);
            return;
        }

        Log.LogInfo($"[HostRoleSwap] Applying swap logic | target={targetRole} | localOriginal={localOriginalRole} | batch={DescribeBuffer()}");

        if (localOriginalRole == targetRole)
        {
            Log.LogInfo("[HostRoleSwap] Local already has target role — no swap");
        }
        else if (TryFindExactMatchPartner(localPlayer.PlayerId, targetRole, out var exactPartnerId))
        {
            var partnerOriginal = _bufferedAssignments[exactPartnerId];
            _bufferedAssignments[localPlayer.PlayerId] = targetRole;
            _bufferedAssignments[exactPartnerId] = localOriginalRole;
            _expectedLocalRole = targetRole;
            Log.LogInfo(
                $"[HostRoleSwap] EXACT swap: local({localOriginalRole}→{targetRole}) ↔ id{exactPartnerId}({partnerOriginal}→{localOriginalRole})");
        }
        else if (CheatToggles.roleSwapLegit && TryFindSameTeamPartner(localPlayer.PlayerId, targetRole, out var legitPartnerId))
        {
            var theirRole = _bufferedAssignments[legitPartnerId];
            _bufferedAssignments[localPlayer.PlayerId] = theirRole;
            _bufferedAssignments[legitPartnerId] = localOriginalRole;
            _expectedLocalRole = theirRole;
            Log.LogInfo(
                $"[HostRoleSwap] LEGIT swap: local({localOriginalRole}→{theirRole}) ↔ id{legitPartnerId}({theirRole}→{localOriginalRole})");
        }
        else
        {
            if (TryFindSameTeamPartner(localPlayer.PlayerId, targetRole, out var teamPartnerId))
            {
                var teamRole = _bufferedAssignments[teamPartnerId];
                _bufferedAssignments[localPlayer.PlayerId] = teamRole;
                _bufferedAssignments[teamPartnerId] = localOriginalRole;
                Log.LogInfo(
                    $"[HostRoleSwap] NORMAL team swap: local({localOriginalRole}→{teamRole}) ↔ id{teamPartnerId}({teamRole}→{localOriginalRole})");
            }

            _bufferedAssignments[localPlayer.PlayerId] = targetRole;
            _expectedLocalRole = targetRole;
            Log.LogInfo($"[HostRoleSwap] NORMAL force-upgrade: local → {targetRole}");
        }

        _swapLogicApplied = true;
        _pendingVerification = true;
        ReleaseAssignments(applySwap: true);
    }

    private static void ReleaseAssignments(bool applySwap)
    {
        if (_state == SwapState.Inactive && _bufferedAssignments.Count == 0)
            return;

        var previousState = _state;
        _state = SwapState.Releasing;

        Log.LogInfo($"[HostRoleSwap] Releasing {_bufferedAssignments.Count} assignment(s) | applySwap={applySwap} | was={previousState}");

        foreach (var assignment in _bufferedAssignments.ToList())
        {
            var player = FindPlayerById(assignment.Key);
            if (player == null)
            {
                Log.LogWarning($"[HostRoleSwap] Skip release — player id={assignment.Key} not in scene (disconnected?)");
                continue;
            }

            try
            {
                _bufferedOverrideFlags.TryGetValue(assignment.Key, out var canOverrideRole);
                Log.LogInfo(
                    $"[HostRoleSwap] RpcSetRole → id={assignment.Key} name={player.Data?.PlayerName ?? "?"} role={assignment.Value} override={canOverrideRole}");

                player.RpcSetRole(assignment.Value, canOverrideRole);
            }
            catch (Exception ex)
            {
                Log.LogError($"[HostRoleSwap] Release failed for id={assignment.Key}: {ex}");
            }
        }

        ClearAll(SwapState.Done, "release complete");
        Log.LogInfo("[HostRoleSwap] Batch done — further RpcSetRole calls pass through unchanged");
    }

    // ========================================================================
    // HELPERS
    // ========================================================================

    private static PlayerControl FindPlayerById(byte playerId)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.PlayerId == playerId)
                return player;
        }

        return null;
    }

    private static bool TryFindExactMatchPartner(byte localPlayerId, RoleTypes targetRole, out byte partnerId)
    {
        foreach (var entry in _bufferedAssignments)
        {
            if (entry.Key == localPlayerId)
                continue;
            if (entry.Value != targetRole)
                continue;

            partnerId = entry.Key;
            return true;
        }

        partnerId = byte.MaxValue;
        return false;
    }

    private static bool TryFindSameTeamPartner(byte localPlayerId, RoleTypes targetRole, out byte partnerId)
    {
        foreach (var entry in _bufferedAssignments)
        {
            if (entry.Key == localPlayerId)
                continue;
            if (!IsSameTeam(entry.Value, targetRole))
                continue;

            partnerId = entry.Key;
            return true;
        }

        partnerId = byte.MaxValue;
        return false;
    }

    private static string DescribeBuffer()
    {
        var sb = new StringBuilder();
        foreach (var entry in _bufferedAssignments.OrderBy(e => e.Key))
            sb.Append($"[{entry.Key}:{entry.Value}] ");

        return sb.ToString().TrimEnd();
    }

    private static bool IsSameTeam(RoleTypes role, RoleTypes targetRole) =>
        IsImpostorRole(role) == IsImpostorRole(targetRole);

    private static bool IsImpostorRole(RoleTypes role) =>
        role == RoleTypes.Impostor
        || role == RoleTypes.Shapeshifter
        || role == RoleTypes.Phantom
        || role == RoleTypes.Viper;

    private static void ClearAll(SwapState nextState, string reason)
    {
        if (_bufferedAssignments.Count > 0 || _state != SwapState.Inactive)
            Log.LogInfo($"[HostRoleSwap] Clear ({reason}) | was={_state} | next={nextState}");

        _bufferedAssignments.Clear();
        _bufferedOverrideFlags.Clear();
        _seenPlayers.Clear();
        _state = nextState;
        _bufferStartTime = 0f;
        _expectedLocalRole = default;
        _pendingVerification = false;
        _swapLogicApplied = false;
    }

    public static void VerifySwap()
    {
        if (!_pendingVerification)
            return;

        _pendingVerification = false;

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null || localPlayer.Data == null)
        {
            Log.LogWarning("[HostRoleSwap] Verification skipped — local player not available");
            return;
        }

        var actualRole = localPlayer.Data.RoleType;
        if (actualRole == _expectedLocalRole)
            Log.LogInfo($"[HostRoleSwap] VERIFIED local role={actualRole}");
        else
            Log.LogError($"[HostRoleSwap] MISMATCH local role={actualRole} expected={_expectedLocalRole}");
    }
}
