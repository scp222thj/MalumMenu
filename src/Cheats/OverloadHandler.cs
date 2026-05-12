using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace MalumMenu;
public static class OverloadHandler
{
    public static float cooldown;
    public static int strength;
    private static HashSet<int> _customTargets = new();
    private static float _timer;
    private static float _attackLogTimer;
    private static Dictionary<int, int> _rpcCounters = new();
    private static int _nextTarget = int.MinValue;
    private static bool _hasRun;
    private static float _bypassOffset;

    public static void Run()
    {
        if (!CheatToggles.runOverload || OverloadUI.currentTargets.Count <= 0)
        {
            _timer = cooldown;
            _attackLogTimer = MalumMenu.attackLogDelay.Value;
            _nextTarget = int.MinValue;
            _hasRun = false;
            _rpcCounters.Clear();
            return;
        }

        _timer += Time.unscaledDeltaTime;
        _attackLogTimer += Time.unscaledDeltaTime;

        if (_timer >= (cooldown + _bypassOffset))
        {
            _bypassOffset = UnityEngine.Random.Range(-0.02f, 0.035f);
            int variedStrength = strength + UnityEngine.Random.Range(-2, 3);
            if (variedStrength < 1) variedStrength = 1;

            // If all possible targets are selected...

            if (OverloadUI.maxPossibleTargets == OverloadUI.currentTargets.Count)
            {
                int broadcastId = -1; // ... it is more efficient to broadcast RPCs

                Utils.Overload(broadcastId, variedStrength);
                _timer = 0f;

                if (CheatToggles.olLogAttack)
                {
                    string colorStr = ColorUtility.ToHtmlStringRGB(Palette.Orange);

                    if (!CheatToggles.olVerboseLogs)
                    {
                        _rpcCounters.TryAdd(broadcastId, 0);

                        _rpcCounters.TryGetValue(broadcastId, out var rpcCount);

                        int newRpcCount = rpcCount + variedStrength;

                        // Log number of broadcasted RPCs since last log
                        // At most logs once per attackLogDelay interval (in seconds)

                        if (_attackLogTimer >= MalumMenu.attackLogDelay.Value)
                        {
                            OverloadUI.LogConsole($"> <b><color=#{colorStr}>Broadcasted {newRpcCount} malformed RPCs to all players (ID : {broadcastId})</color></b>");

                            _attackLogTimer -= MalumMenu.attackLogDelay.Value;

                            _rpcCounters.Clear();
                        }
                        else
                        {
                            _rpcCounters[broadcastId] = newRpcCount;
                        }
                    }
                    else // Log every single broadcast instead if using verbose logging
                    {
                        OverloadUI.LogConsole($"> <b><color=#{colorStr}>Broadcasted {variedStrength} malformed RPCs to all players (ID : {broadcastId})</color></b>");
                    }
                }

                return;
            }

            var currentTargets = OverloadUI.currentTargets;

            foreach (NetworkedPlayerInfo targetData in currentTargets)
            {
                int clientId = targetData.ClientId;

                if (!_hasRun)
                {
                    if (_nextTarget == int.MinValue || clientId == _nextTarget) // No marked target (new cycle) OR clientId is the marked target
                    {
                        Utils.Overload(clientId, variedStrength);
                        _timer = 0f;

                        if (CheatToggles.olLogAttack)
                        {
                            string colorStr = ColorUtility.ToHtmlStringRGB(Palette.Orange);

                            if (!CheatToggles.olVerboseLogs)
                            {
                                _rpcCounters.TryAdd(clientId, 0);

                                _rpcCounters.TryGetValue(clientId, out var rpcCount);

                                int newRpcCount = rpcCount + variedStrength;

                                _rpcCounters[clientId] = newRpcCount;
                            }
                            else // Log every single send if using verbose logging
                            {
                                OverloadUI.LogConsole($"> <b><color=#{colorStr}>Sent {variedStrength} malformed RPCs to {targetData.DefaultOutfit.PlayerName} (ID : {clientId})</color></b>");
                            }
                        }

                        _hasRun = true; // Mark that an overload has run this iteration
                    }
                }
                else // If an overload has run this iteration...
                    // (will always have been previous player sequentially)
                {
                    // Mark current player to be target in following iteration
                    _nextTarget = clientId;
                    _hasRun = false;

                    // End so following iteration can directly start after cooldown
                    return;
                }
            }

            // After a full iteration (cycle ended)...

            // ... (1) Reset state to begin cycle again from first player

            _nextTarget = int.MinValue;
            _hasRun = false;

            // ... (2) Log number of sent RPCs since last log for all currentTargets
            // At most logs once per attackLogDelay interval (in seconds)

            if (!CheatToggles.olVerboseLogs)
            {
                if (_attackLogTimer >= MalumMenu.attackLogDelay.Value)
                {
                    string colorStr = ColorUtility.ToHtmlStringRGB(Palette.Orange);

                    foreach (KeyValuePair<int, int> entry in _rpcCounters)
                    {
                        int clientId = entry.Key;
                        int rpcCount = entry.Value;

                        NetworkedPlayerInfo playerData = OverloadUI.currentTargets.FirstOrDefault(pd => pd.ClientId == clientId);

                        if (playerData != null)
                        {
                            OverloadUI.LogConsole($"> <b><color=#{colorStr}>Sent {rpcCount} malformed RPCs to {playerData.DefaultOutfit.PlayerName} (ID : {clientId})</color></b>");
                        }
                    }

                    _attackLogTimer -= MalumMenu.attackLogDelay.Value;

                    _rpcCounters.Clear();
                }
            }
            else
            {
                _rpcCounters.Clear();
            }
        }
    }

    public static void AddCustomTarget(NetworkedPlayerInfo playerData)
    {
        int clientId = playerData.ClientId;
        _customTargets.Add(clientId);
    }

    public static void RemoveCustomTarget(NetworkedPlayerInfo playerData)
    {
        int clientId = playerData.ClientId;
        _customTargets.Remove(clientId);
    }

    public static bool IsCustomTarget(NetworkedPlayerInfo playerData)
    {
        return _customTargets.Contains(playerData.ClientId);
    }

    public static (HashSet<TargetType> targetTypes, bool isTarget) GetTarget(NetworkedPlayerInfo playerData)
    {
        bool isTarget = false;
        var targetTypes = new HashSet<TargetType>();

        if (CheatToggles.overloadAll)
        {
            targetTypes.Add(TargetType.All);
            isTarget = true;
        }

        bool hostTarget = CheatToggles.overloadHost && AmongUsClient.Instance.HostId == playerData.ClientId;
        if (hostTarget)
        {
            targetTypes.Add(TargetType.Host);
            isTarget = true;
        }

        if (playerData.Role != null)
        {
            RoleTeamTypes roleTeamType = playerData.Role.TeamType;

            bool crewTarget = CheatToggles.overloadCrew && roleTeamType.Equals(RoleTeamTypes.Crewmate);
            if (crewTarget)
            {
                targetTypes.Add(TargetType.Crewmate);
                isTarget = true;
            }

            bool impTarget = CheatToggles.overloadImps && roleTeamType.Equals(RoleTeamTypes.Impostor);
            if (impTarget)
            {
                targetTypes.Add(TargetType.Impostor);
                isTarget = true;
            }
        }

        bool customTarget = IsCustomTarget(playerData);
        if (customTarget)
        {
            targetTypes.Add(TargetType.Custom);
            isTarget = true;
        }

        if (!isTarget)
        {
            targetTypes.Add(TargetType.None);
        }

        return (targetTypes, isTarget);
    }

    public static void ClearCustomTargets()
    {
        _customTargets.Clear();
    }

    // Iterates through all given players and
    // adds all that are marked as targets and match given targetType to _customTargets
    public static void PopulateCustomTargets(PlayerControl[] players, TargetType targetType)
    {
        int playerCount = players.Length;

        for (int i = 0; i < playerCount; i++)
        {
            NetworkedPlayerInfo playerData = players[i].Data;
            var playerTarget = GetTarget(playerData);
            bool isTarget = playerTarget.isTarget;

            if (isTarget && !IsCustomTarget(playerData))
            {
                HashSet<TargetType> currentTargetTypes = playerTarget.targetTypes;
                if (currentTargetTypes.Contains(targetType))
                {
                    AddCustomTarget(playerData);
                }
            }
        }
    }

    // Returns adapted strength and cooldown using number of currentTargets and AmongUsClient ping
    // Should balance them to aim for low lag but effective output
    public static (int strength, float cooldown) CalculateAdaptedValues()
    {
        int targetCount = OverloadUI.maxPossibleTargets == OverloadUI.currentTargets.Count
                        ? 1 // Broadcast mode counts as one target
                        : Math.Max(1, OverloadUI.currentTargets.Count); // Prevents division by 0

        float maxCooldown = MalumMenu.adaptMaxCooldown.Value;
        float cooldown = maxCooldown / targetCount;

        int pingLevel = Math.Max(1, Utils.GetPing() / 100); // 0-99 ms = Lvl 1, 100-199 ms = Lvl 1, 200-299 ms = Lvl 2, ...

        int maxStrength = MalumMenu.adaptMaxStrength.Value;
        int strength = Math.Max(1, maxStrength / pingLevel / targetCount);

        return (strength, cooldown);
    }

    public enum TargetType
    {
        None,
        All,
        Custom,
        Host,
        Impostor,
        Crewmate
    }
}
