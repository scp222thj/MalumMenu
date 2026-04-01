using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using UnityEngine;

namespace MalumMenu;

public static class ForceRole
{
    private static readonly Dictionary<byte, RoleTypes> preGameRoleAssignments = new Dictionary<byte, RoleTypes>();
    private static readonly System.Random random = new System.Random();
    
    public static bool RoleOverrideEnabled { get; private set; } = false;
    public static RoleTypes SelectedRoleForHost { get; private set; } = RoleTypes.Crewmate;
    
    public static RoleTypes[] SupportedRoles = new RoleTypes[]
    {
        RoleTypes.Crewmate,
        RoleTypes.Impostor,
        RoleTypes.Scientist,
        RoleTypes.Engineer,
        RoleTypes.GuardianAngel,
        RoleTypes.Shapeshifter,
        RoleTypes.Tracker,
        RoleTypes.Noisemaker,
        RoleTypes.Phantom,
        RoleTypes.Detective,
        (RoleTypes)18
    };

    [HarmonyPatch(typeof(RoleManager), "SelectRoles")]
    public static class RoleSelectionPatch
    {
        public static bool Prefix(RoleManager __instance)
        {
            try
            {
                if (!((InnerNetClient)AmongUsClient.Instance).AmHost)
                {
                    return true;
                }

                PlayerControl localPlayer = PlayerControl.LocalPlayer;
                if (localPlayer == null)
                {
                    return true;
                }

                if (preGameRoleAssignments.Count > 0)
                {
                    AssignPreGameRolesAsHost();
                    return false;
                }

                return true;
            }
            catch (System.Exception value)
            {
                Debug.LogError($"[ForceRole] Error in RoleSelectionPatch: {value}");
                return true;
            }
        }
    }

    public static void SetRoleForPlayer(PlayerControl player, RoleTypes role)
    {
        if (player == null || player.Data == null) return;

        if (preGameRoleAssignments.TryGetValue(player.PlayerId, out var existingRole) && existingRole == role)
        {
            ClearRoleForPlayer(player.PlayerId);
            Debug.Log($"[ForceRole] Role assignment REMOVED: {player.Data.PlayerName}");
            return;
        }

        preGameRoleAssignments[player.PlayerId] = role;
        Debug.Log($"[ForceRole] Role assigned: {player.Data.PlayerName} -> {role}");
    }

    public static void ClearRoleForPlayer(byte playerId)
    {
        if (preGameRoleAssignments.Remove(playerId))
        {
            Debug.Log($"[ForceRole] Role assignment removed for PlayerId {playerId}");
        }
    }

    public static void ClearAllAssignments()
    {
        preGameRoleAssignments.Clear();
        Debug.Log("[ForceRole] All role assignments cleared");
    }

    public static void SetRoleOverrideEnabled(bool enabled)
    {
        RoleOverrideEnabled = enabled;
        Debug.Log($"[ForceRole] Role Override (Host) {(enabled ? "ON" : "OFF")}");
    }

    public static void SetSelectedRoleForHost(RoleTypes role)
    {
        SelectedRoleForHost = role;
        Debug.Log($"[ForceRole] Selected role for Host: {role}");
    }

    public static void HostApplySelectedRoleNow()
    {
        if (!((InnerNetClient)AmongUsClient.Instance).AmHost)
        {
            Debug.LogWarning("[ForceRole] HostApplySelectedRoleNow: not host.");
            return;
        }

        AssignPreGameRolesAsHost(clearAfter: false);
    }

    public static void ApplyHostRoleOnly()
    {
        if (!((InnerNetClient)AmongUsClient.Instance).AmHost) return;
        
        PlayerControl localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null) return;
        
        preGameRoleAssignments[localPlayer.PlayerId] = SelectedRoleForHost;
        Debug.Log($"[ForceRole] Host role updated to {SelectedRoleForHost}");
    }

    public static RoleTypes? GetAssignedRole(byte playerId)
    {
        return preGameRoleAssignments.TryGetValue(playerId, out var role) ? role : (RoleTypes?)null;
    }

    public static bool HasAssignedRole(byte playerId)
    {
        return preGameRoleAssignments.ContainsKey(playerId);
    }

    private static void AssignPreGameRolesAsHost(bool clearAfter = true)
    {
        if (!((InnerNetClient)AmongUsClient.Instance).AmHost) return;
        if (preGameRoleAssignments.Count == 0) return;

        var allPlayers = PlayerControl.AllPlayerControls;
        if (allPlayers == null || allPlayers.Count == 0) return;

        int numImpostors = GameOptionsManager.Instance?.CurrentGameOptions?.NumImpostors ?? 2;
        
        Debug.Log($"[ForceRole] Starting role assignment. Players: {allPlayers.Count}, NumImpostors: {numImpostors}");
        Debug.Log($"[ForceRole] Pre-assignments: {string.Join(", ", preGameRoleAssignments.Select(kvp => $"{kvp.Key}->{kvp.Value}"))}");

        var impostorTeamRoleIds = new List<byte>();
        var specialRoleAssignments = new Dictionary<byte, RoleTypes>();

        foreach (var kvp in preGameRoleAssignments)
        {
            if (IsImpostorTeam(kvp.Value))
            {
                impostorTeamRoleIds.Add(kvp.Key);
                if (kvp.Value != RoleTypes.Impostor)
                {
                    specialRoleAssignments[kvp.Key] = kvp.Value;
                    Debug.Log($"[ForceRole] Special impostor team role: {kvp.Key} -> {kvp.Value}");
                }
            }
            else
            {
                if (kvp.Value != RoleTypes.Crewmate)
                {
                    specialRoleAssignments[kvp.Key] = kvp.Value;
                    Debug.Log($"[ForceRole] Special crewmate team role: {kvp.Key} -> {kvp.Value}");
                }
            }
        }

        if (impostorTeamRoleIds.Count > numImpostors)
        {
            Debug.LogWarning($"[ForceRole] Too many impostor team players ({impostorTeamRoleIds.Count}) for {numImpostors} impostors!");
            
            var regularImpostors = impostorTeamRoleIds.Where(id => !specialRoleAssignments.ContainsKey(id)).ToList();
            var toDemote = regularImpostors.Skip(Mathf.Max(0, numImpostors - specialRoleAssignments.Count)).ToList();
            
            foreach (var id in toDemote)
            {
                impostorTeamRoleIds.Remove(id);
                Debug.Log($"[ForceRole] Demoting {id} from impostor to crewmate due to limit");
            }
        }

        var finalImpostorIds = new List<byte>(impostorTeamRoleIds);
        if (finalImpostorIds.Count < numImpostors)
        {
            for (int i = 0; i < allPlayers.Count && finalImpostorIds.Count < numImpostors; i++)
            {
                var p = allPlayers[i];
                if (p != null && !impostorTeamRoleIds.Contains(p.PlayerId) && !preGameRoleAssignments.ContainsKey(p.PlayerId))
                {
                    finalImpostorIds.Add(p.PlayerId);
                    Debug.Log($"[ForceRole] Auto-filling impostor slot with {p.Data.PlayerName}");
                }
            }
        }

        Debug.Log($"[ForceRole] Final impostor IDs: {string.Join(", ", finalImpostorIds)}");
        Debug.Log($"[ForceRole] Special roles: {string.Join(", ", specialRoleAssignments.Select(kvp => $"{kvp.Key}->{kvp.Value}"))}");

        for (int i = 0; i < allPlayers.Count; i++)
        {
            var player = allPlayers[i];
            if (player == null || player.Data == null) continue;

            RoleTypes roleToSet;
            
            if (specialRoleAssignments.ContainsKey(player.PlayerId))
            {
                roleToSet = specialRoleAssignments[player.PlayerId];
                Debug.Log($"[ForceRole] {player.Data.PlayerName} gets SPECIAL role: {roleToSet}");
            }
            else if (finalImpostorIds.Contains(player.PlayerId))
            {
                roleToSet = RoleTypes.Impostor;
                Debug.Log($"[ForceRole] {player.Data.PlayerName} gets IMPOSTOR");
            }
            else
            {
                roleToSet = RoleTypes.Crewmate;
                Debug.Log($"[ForceRole] {player.Data.PlayerName} gets CREWMATE");
            }

            player.RpcSetRole(roleToSet, false);
            UpdateRoleLocally(player, roleToSet);
        }

        var impNames = new List<string>();
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (finalImpostorIds.Contains(allPlayers[i].PlayerId) && allPlayers[i].Data != null)
                impNames.Add(allPlayers[i].Data.PlayerName);
        }
        
        var specialRoleText = specialRoleAssignments.Any() 
            ? $" | Special: {string.Join(", ", specialRoleAssignments.Select(kvp => $"{GetPlayerName(allPlayers.ToArray(), kvp.Key)}({kvp.Value})"))}"
            : "";
        
        NotificationManager.Show($"Roles: {string.Join(", ", impNames)}{specialRoleText}", 3f);
        
        if (clearAfter)
        {
            preGameRoleAssignments.Clear();
        }
    }

    private static string GetPlayerName(PlayerControl[] allPlayers, byte playerId)
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            if (allPlayers[i] != null && allPlayers[i].PlayerId == playerId && allPlayers[i].Data != null)
            {
                return allPlayers[i].Data.PlayerName;
            }
        }
        return "Unknown";
    }

    public static void UpdateRoleLocally(PlayerControl player, RoleTypes roleType)
    {
        if (player == null || player.Data == null) return;

        try
        {
            Debug.Log($"[ForceRole] Updating locally {player.Data.PlayerName} to {roleType}");

            player.Data.RoleType = roleType;
            player.Data.IsDead = false;

            RoleBehaviour roleBehaviour = player.GetComponent<RoleBehaviour>();
            if (roleBehaviour != null)
            {
                FieldInfo roleField = typeof(RoleBehaviour).GetField("role", BindingFlags.Instance | BindingFlags.NonPublic);
                roleField?.SetValue(roleBehaviour, roleType);

                FieldInfo teamTypeField = typeof(RoleBehaviour).GetField("teamType", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?? typeof(RoleBehaviour).GetField("<TeamType>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                teamTypeField?.SetValue(roleBehaviour, IsImpostorTeam(roleType) ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate);
            }

            NetworkedPlayerInfo playerInfo = GameData.Instance?.GetPlayerById(player.PlayerId);
            if (playerInfo != null)
            {
                playerInfo.RoleType = roleType;
                if (playerInfo.Role != null)
                {
                    playerInfo.Role.TeamType = IsImpostorTeam(roleType) ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
                    playerInfo.Role.Role = roleType;
                }
                playerInfo.IsDead = false;
                playerInfo.MarkDirty();
            }

            Debug.Log($"[ForceRole] Local update for {player.Data.PlayerName} completed");
        }
        catch (System.Exception value)
        {
            Debug.LogError($"[ForceRole] Error in UpdateRoleLocally: {value}");
        }
    }

    private static bool IsImpostorTeam(RoleTypes role)
    {
        if (role == RoleTypes.Impostor || role == RoleTypes.Shapeshifter || role == RoleTypes.Phantom)
            return true;
        
        if ((int)role == 18)
            return true;
            
        return false;
    }

    private static int GetMaxImpostorsForPlayerCount(int playerCount)
    {
        if (playerCount <= 5) return 1;
        if (playerCount <= 8) return 2;
        return 3;
    }
}
