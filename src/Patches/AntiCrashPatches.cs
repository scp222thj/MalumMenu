using System;
using HarmonyLib;
using Hazel;
using InnerNet;
using UnityEngine;

namespace MalumMenu;

public static class AntiCrashLimiter
{
    private const int MaxBuckets = 32;

    private const int MaxRpcPerSecondGlobal = 300;
    private const int MaxRpcPerSecondPerOwner = 120;
    private const int MaxUpdateSystemPerSecondPerClient = 120;
    private const int MaxStrikesBeforeBan = 3;
    private const float StrikeWindowSeconds = 3f;

    private static readonly float[] OwnerWindowStart = new float[MaxBuckets];
    private static readonly int[] OwnerRpcCount = new int[MaxBuckets];
    private static readonly float[] OwnerLastPopup = new float[MaxBuckets];
    private static readonly int[] StrikeClientId = new int[MaxBuckets];
    private static readonly float[] StrikeWindowStart = new float[MaxBuckets];
    private static readonly int[] StrikeCount = new int[MaxBuckets];
    private static readonly bool[] StrikeBanned = new bool[MaxBuckets];

    private static float GlobalWindowStart;
    private static int GlobalRpcCount;

    private static readonly string OwnerIdResolution = ResolveOwnerIdResolution();
    private static readonly Func<InnerNetObject, int> GetOwnerId = CreateOwnerIdGetter();

    public static string GetDiagnosticsSummary()
    {
        return $"OwnerIdResolver={OwnerIdResolution}, Buckets={MaxBuckets}, RpcGlobal={MaxRpcPerSecondGlobal}/s, RpcPerOwner={MaxRpcPerSecondPerOwner}/s, UpdateSystemPerClient={MaxUpdateSystemPerSecondPerClient}/s, Strikes={MaxStrikesBeforeBan} in {StrikeWindowSeconds}s";
    }

    public static int GetClientIdFromObject(InnerNetObject obj)
    {
        if (obj == null) return -1;
        return GetOwnerId(obj);
    }

    private static string ResolveOwnerIdResolution()
    {
        var getter = AccessTools.PropertyGetter(typeof(InnerNetObject), "OwnerId");
        if (getter != null && getter.ReturnType == typeof(int))
        {
            return "Property:OwnerId";
        }

        var field = AccessTools.Field(typeof(InnerNetObject), "OwnerId");
        if (field != null && field.FieldType == typeof(int))
        {
            return "Field:OwnerId";
        }

        field = AccessTools.Field(typeof(InnerNetObject), "ownerId");
        if (field != null && field.FieldType == typeof(int))
        {
            return "Field:ownerId";
        }

        return "Missing";
    }

    private static Func<InnerNetObject, int> CreateOwnerIdGetter()
    {
        var getter = AccessTools.PropertyGetter(typeof(InnerNetObject), "OwnerId");
        if (getter != null && getter.ReturnType == typeof(int))
        {
            return (Func<InnerNetObject, int>)Delegate.CreateDelegate(typeof(Func<InnerNetObject, int>), null, getter);
        }

        var field = AccessTools.Field(typeof(InnerNetObject), "OwnerId") ?? AccessTools.Field(typeof(InnerNetObject), "ownerId");
        if (field != null && field.FieldType == typeof(int))
        {
            return obj => (int)field.GetValue(obj);
        }

        return _ => -1;
    }

    public static bool ShouldAllowRpc(InnerNetObject obj, byte callId)
    {
        if (!CheatToggles.antiCrashProtection) return true;
        if (obj == null) return true;

        var now = Time.realtimeSinceStartup;

        if (now - GlobalWindowStart >= 1f)
        {
            GlobalWindowStart = now;
            GlobalRpcCount = 0;
        }

        GlobalRpcCount++;
        if (GlobalRpcCount > MaxRpcPerSecondGlobal)
        {
            TryShowPopup(-1, callId, "Global RPC rate limit");
            return false;
        }

        var ownerId = GetOwnerId(obj);
        if (ownerId < 0) return true;

        var bucket = ownerId & (MaxBuckets - 1);
        var windowStart = OwnerWindowStart[bucket];
        if (now - windowStart >= 1f)
        {
            OwnerWindowStart[bucket] = now;
            OwnerRpcCount[bucket] = 0;
        }

        var count = ++OwnerRpcCount[bucket];
        if (count > MaxRpcPerSecondPerOwner)
        {
            RecordStrike(ownerId, callId, "RPC rate limit");
            TryShowPopup(ownerId, callId, "RPC rate limit");
            return false;
        }

        return true;
    }

    public static bool ShouldAllowUpdateSystem(PlayerControl player, SystemTypes systemType)
    {
        if (!CheatToggles.antiCrashProtection) return true;
        if (player == null || player.Data == null) return true;

        var clientId = player.Data.ClientId;
        if (clientId < 0) return true;

        var now = Time.realtimeSinceStartup;
        var bucket = clientId & (MaxBuckets - 1);
        var windowStart = OwnerWindowStart[bucket];
        if (now - windowStart >= 1f)
        {
            OwnerWindowStart[bucket] = now;
            OwnerRpcCount[bucket] = 0;
        }

        var count = ++OwnerRpcCount[bucket];
        if (count > MaxUpdateSystemPerSecondPerClient)
        {
            RecordStrike(clientId, (byte)systemType, "UpdateSystem rate limit");
            TryShowPopup(clientId, (byte)systemType, "UpdateSystem rate limit");
            return false;
        }

        return true;
    }

    public static void NotifyException(string where, int clientId, byte code, Exception ex)
    {
        if (!CheatToggles.antiCrashProtection) return;
        if (ex == null) return;

        if (clientId >= 0)
        {
            RecordStrike(clientId, code, $"{where} exception: {ex.GetType().Name}");
        }

        if (!CheatToggles.antiCrashPopups) return;
        Utils.ShowNewPopup($"{where} exception blocked: {ex.GetType().Name}");
    }

    private static void RecordStrike(int clientId, byte code, string reason)
    {
        if (clientId < 0) return;
        if (IsLocalClientId(clientId)) return;

        var now = Time.realtimeSinceStartup;
        var bucket = clientId & (MaxBuckets - 1);

        var trackedClientId = StrikeClientId[bucket];
        var windowStart = StrikeWindowStart[bucket];
        if (trackedClientId != clientId || now - windowStart >= StrikeWindowSeconds)
        {
            StrikeClientId[bucket] = clientId;
            StrikeWindowStart[bucket] = now;
            StrikeCount[bucket] = 0;
            StrikeBanned[bucket] = false;
        }

        StrikeCount[bucket]++;

        if (!CheatToggles.autoBanCrashers) return;
        if (!Utils.isHost) return;
        if (CheatToggles.runOverload) return;
        if (StrikeBanned[bucket]) return;
        if (StrikeCount[bucket] < MaxStrikesBeforeBan) return;

        StrikeBanned[bucket] = true;
        TryAutoBanClient(clientId, code, reason);
    }

    private static void TryAutoBanClient(int clientId, byte code, string reason)
    {
        if (!Utils.isHost) return;
        if (AmongUsClient.Instance == null) return;
        if (CheatToggles.runOverload) return;
        if (IsLocalClientId(clientId)) return;

        try
        {
            AmongUsClient.Instance.KickPlayer(clientId, true);
        }
        catch (Exception ex)
        {
            if (CheatToggles.antiCrashPopups)
            {
                Utils.ShowNewPopup($"Auto ban failed: {ex.GetType().Name}");
            }
            return;
        }

        if (!CheatToggles.antiCrashPopups) return;

        var name = "Unknown";
        var playerData = Utils.GetPlayerDataFromClientId(clientId);
        if (playerData != null)
        {
            name = playerData.PlayerName;
        }

        Utils.ShowNewPopup($"Banned {name} (ClientId {clientId}) {reason} [{code}]");
    }

    private static void TryShowPopup(int clientId, byte code, string reason)
    {
        if (!CheatToggles.antiCrashPopups) return;

        var now = Time.realtimeSinceStartup;
        var bucket = clientId < 0 ? 0 : (clientId & (MaxBuckets - 1));
        if (now - OwnerLastPopup[bucket] < 1.0f) return;
        OwnerLastPopup[bucket] = now;

        var name = "Unknown";
        if (clientId >= 0)
        {
            var playerData = Utils.GetPlayerDataFromClientId(clientId);
            if (playerData != null)
            {
                name = playerData.PlayerName;
            }
        }

        var idString = clientId >= 0 ? $" {name} (ClientId {clientId})" : "";
        Utils.ShowNewPopup($"{reason}{idString} [{code}]");
    }

    private static bool IsLocalClientId(int clientId)
    {
        if (clientId < 0) return false;
        if (!Utils.isPlayer) return false;
        if (PlayerControl.LocalPlayer.Data == null) return false;
        return PlayerControl.LocalPlayer.Data.ClientId == clientId;
    }
}

[HarmonyPatch(typeof(InnerNetObject), nameof(InnerNetObject.HandleRpc))]
public static class InnerNetObject_HandleRpc_AntiCrash
{
    public static bool Prefix(InnerNetObject __instance, byte callId, MessageReader reader)
    {
        return AntiCrashLimiter.ShouldAllowRpc(__instance, callId);
    }

    public static Exception Finalizer(Exception __exception, InnerNetObject __instance, byte callId)
    {
        if (__exception == null) return null;
        AntiCrashLimiter.NotifyException("HandleRpc", AntiCrashLimiter.GetClientIdFromObject(__instance), callId, __exception);
        return CheatToggles.antiCrashProtection ? null : __exception;
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.UpdateSystem), typeof(SystemTypes), typeof(PlayerControl), typeof(MessageReader))]
public static class ShipStatus_UpdateSystem_AntiCrash
{
    public static bool Prefix(SystemTypes systemType, PlayerControl player)
    {
        return AntiCrashLimiter.ShouldAllowUpdateSystem(player, systemType);
    }

    public static Exception Finalizer(Exception __exception, SystemTypes systemType, PlayerControl player)
    {
        if (__exception == null) return null;
        var clientId = player != null && player.Data != null ? player.Data.ClientId : -1;
        AntiCrashLimiter.NotifyException("UpdateSystem", clientId, (byte)systemType, __exception);
        return CheatToggles.antiCrashProtection ? null : __exception;
    }
}
