using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using System;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace MalumMenu;
public static class MalumPPMCheats
{
    private static bool _telekillPlayerActive;
    private static bool _killPlayerActive;
    private static bool _spectateActive;
    private static bool _teleportPlayerActive;
    private static bool _reportBodyActive;
    private static bool _ejectPlayerActive;
    private static bool _setFakeRoleActive;
    private static bool _setFakeAliveActive;
    private static bool _forceRoleActive;
    private static bool _fakeShapeshiftActive;
    private static bool _freezePlayerActive;
    private static PlayerControl _frozenTarget;
    private static Vector2 _frozenPos;
    private static RoleTypes? _oldRole = null;

    public static void ResetOldRole() => _oldRole = null;

    public static void ReportBodyPPM()
    {
        if (CheatToggles.reportBody)
        {

            if (!_reportBodyActive)
            {
                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("reportBody");
                }

                // Player pick menu to choose any body (alive or dead) and report it
                PlayerPickMenu.OpenPlayerPickMenu(Utils.GetAllPlayerData(), (Action) (() =>
                {
                    PlayerControl.LocalPlayer.CmdReportDeadBody(PlayerPickMenu.targetPlayerData);
                }));

                _reportBodyActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.reportBody = false;
            }

        }
        else
        {
            if (_reportBodyActive)
            {
                _reportBodyActive = false;
            }
        }
    }

    public static void EjectPlayerPPM()
    {
        if (CheatToggles.ejectPlayer)
        {
            if (!_ejectPlayerActive)
            {
                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("ejectPlayer");
                }

                if (!Utils.isMeeting)
                {
                    CheatToggles.ejectPlayer = false;
                    return;
                }

                List<NetworkedPlayerInfo> playerInfo = new List<NetworkedPlayerInfo>();
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player == null || player.Data == null) continue;
                    if (!player.Data.IsDead && !player.Data.Disconnected)
                    {
                        playerInfo.Add(player.Data);
                    }
                }

                // Player pick menu to choose any living player and eject them during meeting
                PlayerPickMenu.OpenPlayerPickMenu(playerInfo, (Action)(() =>
                {
                    NetworkedPlayerInfo playerToEject = PlayerPickMenu.targetPlayerData;
                    MeetingHud.Instance.RpcVotingComplete(new Il2CppStructArray<MeetingHud.VoterState>(0L), playerToEject, false);
                }));

                _ejectPlayerActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.ejectPlayer = false;
            }
        }
        else if (_ejectPlayerActive)
        {
            _ejectPlayerActive = false;
        }
    }

    public static void KillPlayerPPM()
    {
        if (CheatToggles.killPlayer)
        {
            if (!_killPlayerActive)
            {
                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("killPlayer");
                }

                if (Utils.isLobby)
                {
                    HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
                    CheatToggles.killPlayer = false;
                    return;
                }

                // Player pick menu made for killing any player by sending a successful MurderPlayer RPC call
                PlayerPickMenu.OpenPlayerPickMenu(Utils.GetAllPlayerData(), (Action)(() =>
                {
                    Utils.MurderPlayer(PlayerPickMenu.targetPlayerData.Object, MurderResultFlags.Succeeded);
                }));

                _killPlayerActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.killPlayer = false;
            }
        }
        else if (_killPlayerActive)
        {
            _killPlayerActive = false;
        }
    }

    public static void TelekillPlayerPPM()
    {
        if (CheatToggles.telekillPlayer)
        {
            if (!_telekillPlayerActive)
            {
                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("telekillPlayer");
                }

                if (Utils.isLobby)
                {
                    HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
                    CheatToggles.telekillPlayer = false;
                    return;
                }

                // Player pick menu made for killing any player by sending a successful MurderPlayer RPC call
                // and immediatly teleporting back to original position
                PlayerPickMenu.OpenPlayerPickMenu(Utils.GetAllPlayerData(), (Action)(() =>
                {
                    var oldPos = PlayerControl.LocalPlayer.GetTruePosition();
                    Utils.MurderPlayer(PlayerPickMenu.targetPlayerData.Object, MurderResultFlags.Succeeded);
                    AmongUsClient.Instance.StartCoroutine(Utils.DelayedSnapTo(oldPos));
                }));

                _telekillPlayerActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.telekillPlayer = false;
            }
        }
        else if (_telekillPlayerActive)
        {
            _telekillPlayerActive = false;
        }
    }

    public static void TeleportPlayerPPM()
    {
        if (CheatToggles.teleportPlayer)
        {
            if (!_teleportPlayerActive)
            {
                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("teleportPlayer");
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                // All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player == null || player.Data == null) continue;
                    if (!player.AmOwner)
                    {
                        playerDataList.Add(player.Data);
                    }
                }

                // Player pick menu made for teleporting LocalPlayer to any player's position
                PlayerPickMenu.OpenPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    var target = PlayerPickMenu.targetPlayerData.Object;
                    if (target != null) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(target.GetTruePosition());
                }));

                _teleportPlayerActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.teleportPlayer = false;
            }
        }
        else if (_teleportPlayerActive)
        {
            _teleportPlayerActive = false;
        }
    }

    public static void SetFakeRolePPM()
    {
        if (CheatToggles.setFakeRole)
        {

            if (!_setFakeRoleActive)
            {

                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("setFakeRole");
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                // Shapeshifter role can only be used if it was already assigned at the start of the game
                // This is done to prevent the anticheat from kicking players
                if (_oldRole == RoleTypes.Shapeshifter || Utils.isFreePlay)
                {
                    playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Shapeshifter", OutfitPreset.Shapeshifter, Utils.GetBehaviourByRoleType(RoleTypes.Shapeshifter)));
                }

                // Phantom role can only be used if it was already assigned at the start of the game
                // This is done to prevent the anticheat from kicking players
                if (_oldRole == RoleTypes.Phantom || Utils.isFreePlay)
                {
                    playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Phantom", OutfitPreset.Phantom, Utils.GetBehaviourByRoleType(RoleTypes.Phantom)));
                }

                // Viper role can only be used if it was already assigned at the start of the game
                // This is done to prevent the anticheat from kicking players
                if (_oldRole == RoleTypes.Viper || Utils.isFreePlay)
                {
                    playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Viper", OutfitPreset.Viper, Utils.GetBehaviourByRoleType(RoleTypes.Viper)));
                }

                // Impostor role can only be used if it was already assigned at the start of the game or as host
                // This is done to prevent the anticheat from kicking players
                bool wasImpostor = false;
                try { wasImpostor = _oldRole != null && Utils.GetBehaviourByRoleType((RoleTypes)_oldRole)?.TeamType == RoleTeamTypes.Impostor; } catch { }
                if (wasImpostor || Utils.isFreePlay || Utils.isHost)
                {
                    playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Impostor", OutfitPreset.Impostor, Utils.GetBehaviourByRoleType(RoleTypes.Impostor)));
                }

                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Tracker", OutfitPreset.Tracker, Utils.GetBehaviourByRoleType(RoleTypes.Tracker)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Noisemaker", OutfitPreset.Noisemaker, Utils.GetBehaviourByRoleType(RoleTypes.Noisemaker)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Engineer", OutfitPreset.Engineer, Utils.GetBehaviourByRoleType(RoleTypes.Engineer)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Scientist", OutfitPreset.Scientist, Utils.GetBehaviourByRoleType(RoleTypes.Scientist)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Detective", OutfitPreset.Detective, Utils.GetBehaviourByRoleType(RoleTypes.Detective)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Crewmate", OutfitPreset.Crewmate, Utils.GetBehaviourByRoleType(RoleTypes.Crewmate)));

                // Player pick menu made for changing your roles with a custom choice list
                PlayerPickMenu.OpenPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    // Log the originally assigned role before it gets changed by setFakeRole cheat
                    if (!Utils.isLobby && !Utils.isFreePlay && _oldRole == null)
                    {
                        _oldRole = PlayerControl.LocalPlayer.Data.RoleType;
                    }

                    if (PlayerControl.LocalPlayer.Data.IsDead) // Prevent accidential revives
                    {
                        if (PlayerPickMenu.targetPlayerData.Role.TeamType == RoleTeamTypes.Impostor)
                        {
                            RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, RoleTypes.ImpostorGhost);
                        }
                        else
                        {
                            RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, RoleTypes.CrewmateGhost);
                        }
                    }
                    else
                    {
                        /* if (PlayerPickMenu.targetPlayerData.Role.Role == RoleTypes.Shapeshifter && oldRole != RoleTypes.Shapeshifter){

                            Utils.showPopup("\n<size=125%>Changing into the Shapeshifter role is not recommended\nsince shapeshifting will get you kicked by the anticheat");

                        } else if (PlayerPickMenu.targetPlayerData.Role.Role == RoleTypes.Noisemaker && oldRole != RoleTypes.Noisemaker){

                            Utils.showPopup("\n<size=125%>Changing into the Noisemaker role is not recommended\nsince dying won't trigger the alert for other players");

                        } else if (oldRole == RoleTypes.Noisemaker){

                            Utils.showPopup("\n<size=125%>Your \"real\" role is still Noisemaker\nso other players will still see the alert when you die");

                        } */

                        RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, PlayerPickMenu.targetPlayerData.Role.Role);
                    }
                }));

                _setFakeRoleActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.setFakeRole = false;
            }

        }
        else
        {
            if (_setFakeRoleActive)
            {
                _setFakeRoleActive = false;
            }
        }
    }

    public static void SetFakeAlivePPM()
    {
        if (CheatToggles.setFakeAlive)
        {

            if (!_setFakeAliveActive)
            {

                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("setFakeAlive");
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Alive", OutfitPreset.Crewmate, Utils.GetBehaviourByRoleType(RoleTypes.Crewmate)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Dead", OutfitPreset.Dead, Utils.GetBehaviourByRoleType(RoleTypes.CrewmateGhost)));

                // Player pick menu made for changing your alive state with a custom choice list
                PlayerPickMenu.OpenPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    if (PlayerPickMenu.targetPlayerData.Role.IsDead)
                    {
                        PlayerControl.LocalPlayer.Die(DeathReason.Exile, true);
                    }
                    else
                    {
                        PlayerControl.LocalPlayer.Revive();
                    }
                }));

                _setFakeAliveActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.setFakeAlive = false;
            }

        }
        else
        {
            if (_setFakeAliveActive)
            {
                _setFakeAliveActive = false;
            }
        }
    }

    public static void ForceRolePPM()
    {
        if (CheatToggles.forceRole)
        {
            if (!_forceRoleActive)
            {
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("forceRole");
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Shapeshifter", OutfitPreset.Shapeshifter, Utils.GetBehaviourByRoleType(RoleTypes.Shapeshifter)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Phantom", OutfitPreset.Phantom, Utils.GetBehaviourByRoleType(RoleTypes.Phantom)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Viper", OutfitPreset.Viper, Utils.GetBehaviourByRoleType(RoleTypes.Viper)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Impostor", OutfitPreset.Impostor, Utils.GetBehaviourByRoleType(RoleTypes.Impostor)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Tracker", OutfitPreset.Tracker, Utils.GetBehaviourByRoleType(RoleTypes.Tracker)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Noisemaker", OutfitPreset.Noisemaker, Utils.GetBehaviourByRoleType(RoleTypes.Noisemaker)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Engineer", OutfitPreset.Engineer, Utils.GetBehaviourByRoleType(RoleTypes.Engineer)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Scientist", OutfitPreset.Scientist, Utils.GetBehaviourByRoleType(RoleTypes.Scientist)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Detective", OutfitPreset.Detective, Utils.GetBehaviourByRoleType(RoleTypes.Detective)));
                playerDataList.Add(PlayerPickMenu.CustomPPMChoice("Crewmate", OutfitPreset.Crewmate, Utils.GetBehaviourByRoleType(RoleTypes.Crewmate)));

                // Player pick menu made for forcing a role onto another player
                PlayerPickMenu.OpenPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    CheatToggles.forcedRole = PlayerPickMenu.targetPlayerData.Role.Role;
                }));

                _forceRoleActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.forceRole = false;
            }

        }
        else
        {
            if (_forceRoleActive)
            {
                _forceRoleActive = false;
            }
        }
    }

    public static void SpectatePPM()
    {
        if (CheatToggles.spectate)
        {

            if (!_spectateActive)
            {

                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("spectate");
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                // All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player == null || player.Data == null) continue;
                    if (!player.AmOwner)
                    {
                        playerDataList.Add(player.Data);
                    }
                }

                // Player pick menu made for spectating the targeted player
                PlayerPickMenu.OpenPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    var target = PlayerPickMenu.targetPlayerData?.Object;
                    if (target != null) Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(target);
                }));

                _spectateActive = true;

                PlayerControl.LocalPlayer.moveable = false; // Can't move while spectating

                CheatToggles.freecam = false; // Disable incompatible cheats while spectating

            }

            // Deactivate cheat if menu is closed and no one is getting spectated
            var followerCam = Camera.main?.gameObject.GetComponent<FollowerCamera>();
            if (PlayerPickMenu.playerpickMenu == null && followerCam != null && followerCam.Target == PlayerControl.LocalPlayer)
            {
                CheatToggles.spectate = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }
        else
        {
            // Deactivate cheat when it is disabled from the Malum GUI
            if (_spectateActive)
            {
                _spectateActive = false;
                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            }
        }
    }

    // Each frame: if freeze is active, snap the frozen target back to their captured position
    public static void TickFreezePlayer()
    {
        if (!CheatToggles.freezePlayer || _frozenTarget == null) return;
        if (!Utils.isHost)
        {
            CheatToggles.freezePlayer = false;
            _frozenTarget = null;
            HudManager.Instance?.Notifier?.AddDisconnectMessage("Freeze Player requires host");
            return;
        }
        if (_frozenTarget.Data == null || _frozenTarget.Data.IsDead || _frozenTarget.Data.Disconnected)
        {
            CheatToggles.freezePlayer = false;
            _frozenTarget = null;
            return;
        }
        try { _frozenTarget.NetTransform.RpcSnapTo(_frozenPos); } catch { }
    }

    public static void FreezePlayerPPM()
    {
        if (CheatToggles.freezePlayer)
        {
            if (!_freezePlayerActive)
            {
                if (!Utils.isHost)
                {
                    HudManager.Instance?.Notifier?.AddDisconnectMessage("Freeze Player requires host");
                    CheatToggles.freezePlayer = false;
                    return;
                }

                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("freezePlayer");
                }

                var playerList = new Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo>();
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player == null || player.Data == null) continue;
                    if (!player.AmOwner && !player.Data.IsDead)
                        playerList.Add(player.Data);
                }

                PlayerPickMenu.OpenPlayerPickMenu(playerList, (Action)(() =>
                {
                    _frozenTarget = PlayerPickMenu.targetPlayerData?.Object;
                    if (_frozenTarget != null)
                    {
                        _frozenPos = _frozenTarget.GetTruePosition();
                        ConsoleUI.Log($"[Freeze] Froze {_frozenTarget.Data.PlayerName}");
                    }
                    CheatToggles.freezePlayer = _frozenTarget != null;
                }));

                _freezePlayerActive = true;
            }

            if (PlayerPickMenu.playerpickMenu == null && _frozenTarget == null)
                CheatToggles.freezePlayer = false;
        }
        else
        {
            if (_freezePlayerActive)
            {
                _freezePlayerActive = false;
                _frozenTarget = null;
            }
        }
    }

    // Opens a PlayerPickMenu to select a target, then broadcasts a fake Shapeshift RPC
    // making all clients see LocalPlayer appear to shapeshift into the chosen target.
    public static void FakeShapeshiftPPM()
    {
        if (CheatToggles.fakeShapeshift)
        {
            if (!_fakeShapeshiftActive)
            {
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("fakeShapeshift");
                }

                PlayerPickMenu.OpenPlayerPickMenu(Utils.GetAllPlayerData(), (Action)(() =>
                {
                    var target = PlayerPickMenu.targetPlayerData?.Object;
                    if (target != null)
                    {
                        Utils.SendFakeShapeshift(target);
                    }
                    CheatToggles.fakeShapeshift = false;
                }));

                _fakeShapeshiftActive = true;
            }

            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.fakeShapeshift = false;
            }
        }
        else
        {
            if (_fakeShapeshiftActive)
            {
                _fakeShapeshiftActive = false;
            }
        }
    }

    private static bool _frameAsShapeshifterActive;
    private static bool _teleportPlayerToPlayerActive;
    private static PlayerControl _teleportP2PSource;
    private static bool _fakeVentOnPlayerActive;

    public static void FrameAsShapeshifterPPM()
    {
        if (CheatToggles.frameAsShapeshifter)
        {
            if (!_frameAsShapeshifterActive)
            {
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("frameAsShapeshifter");
                }

                // Step 1: pick the victim (they will appear to shapeshift)
                PlayerPickMenu.OpenPlayerPickMenu(Utils.GetAllPlayerData(), (Action)(() =>
                {
                    var victim = PlayerPickMenu.targetPlayerData?.Object;
                    if (victim == null) { CheatToggles.frameAsShapeshifter = false; return; }

                    // Step 2: pick what they shapeshift into
                    PlayerPickMenu.OpenPlayerPickMenu(Utils.GetAllPlayerData(), (Action)(() =>
                    {
                        var target = PlayerPickMenu.targetPlayerData?.Object;
                        if (target != null) Utils.FrameAsShapeshifter(victim, target);
                        CheatToggles.frameAsShapeshifter = false;
                    }));
                }));

                _frameAsShapeshifterActive = true;
            }

            if (PlayerPickMenu.playerpickMenu == null)
                CheatToggles.frameAsShapeshifter = false;
        }
        else if (_frameAsShapeshifterActive)
        {
            _frameAsShapeshifterActive = false;
        }
    }

    public static void TeleportPlayerToPlayerPPM()
    {
        if (CheatToggles.teleportPlayerToPlayer)
        {
            if (!_teleportPlayerToPlayerActive)
            {
                if (!Utils.isHost)
                {
                    HudManager.Instance?.Notifier?.AddDisconnectMessage("Teleport Player to Player requires host");
                    CheatToggles.teleportPlayerToPlayer = false;
                    return;
                }

                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("teleportPlayerToPlayer");
                }

                var allOthers = new Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo>();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p == null || p.Data == null || p.AmOwner) continue;
                    allOthers.Add(p.Data);
                }

                // Step 1: pick the player to move
                PlayerPickMenu.OpenPlayerPickMenu(allOthers, (Action)(() =>
                {
                    _teleportP2PSource = PlayerPickMenu.targetPlayerData?.Object;
                    if (_teleportP2PSource == null) { CheatToggles.teleportPlayerToPlayer = false; return; }

                    // Step 2: pick the destination player
                    var destList = new Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo>();
                    foreach (var p in PlayerControl.AllPlayerControls)
                    {
                        if (p == null || p.Data == null || p == _teleportP2PSource) continue;
                        destList.Add(p.Data);
                    }

                    PlayerPickMenu.OpenPlayerPickMenu(destList, (Action)(() =>
                    {
                        var dest = PlayerPickMenu.targetPlayerData?.Object;
                        if (dest != null && _teleportP2PSource != null)
                            _teleportP2PSource.NetTransform.RpcSnapTo(dest.GetTruePosition());
                        _teleportP2PSource = null;
                        CheatToggles.teleportPlayerToPlayer = false;
                    }));
                }));

                _teleportPlayerToPlayerActive = true;
            }

            if (PlayerPickMenu.playerpickMenu == null && _teleportP2PSource == null)
                CheatToggles.teleportPlayerToPlayer = false;
        }
        else if (_teleportPlayerToPlayerActive)
        {
            _teleportPlayerToPlayerActive = false;
            _teleportP2PSource = null;
        }
    }

    public static void FakeVentOnPlayerPPM()
    {
        if (CheatToggles.fakeVentOnPlayer)
        {
            if (!_fakeVentOnPlayerActive)
            {
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("fakeVentOnPlayer");
                }

                var others = new Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo>();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p == null || p.Data == null || p.AmOwner || p.Data.IsDead) continue;
                    others.Add(p.Data);
                }

                PlayerPickMenu.OpenPlayerPickMenu(others, (Action)(() =>
                {
                    var victim = PlayerPickMenu.targetPlayerData?.Object;
                    if (victim != null && ShipStatus.Instance?.AllVents != null)
                    {
                        Vent nearest = null;
                        float best = float.MaxValue;
                        foreach (var v in ShipStatus.Instance.AllVents)
                        {
                            float d = Vector2.Distance(victim.GetTruePosition(), (Vector2)v.transform.position);
                            if (d < best) { best = d; nearest = v; }
                        }
                        if (nearest != null) Utils.FakeVentOnPlayer(victim, nearest.Id);
                    }
                    CheatToggles.fakeVentOnPlayer = false;
                }));

                _fakeVentOnPlayerActive = true;
            }

            if (PlayerPickMenu.playerpickMenu == null)
                CheatToggles.fakeVentOnPlayer = false;
        }
        else if (_fakeVentOnPlayerActive)
        {
            _fakeVentOnPlayerActive = false;
        }
    }
}
