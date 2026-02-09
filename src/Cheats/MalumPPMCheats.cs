using Il2CppSystem.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using System;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace MalumMenu;
public static class MalumPPMCheats
{
    public static bool telekillPlayerActive;
    public static bool killPlayerActive;
    public static bool spectateActive;
    public static bool teleportPlayerActive;
    public static bool reportBodyActive;
    public static bool ejectPlayerActive;
    public static bool changeRoleActive;
    public static bool forceRoleActive;
    public static RoleTypes? oldRole = null;

    public static void reportBodyPPM(){
        if (CheatToggles.reportBody){

            if (!reportBodyActive){

                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null){
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("reportBody");
                }

                // Player pick menu to choose any body (alive or dead) and report it
                PlayerPickMenu.openPlayerPickMenu(Utils.GetAllPlayerData(), (Action) (() =>
                {
                    PlayerControl.LocalPlayer.CmdReportDeadBody(PlayerPickMenu.targetPlayerData);
                }));

                reportBodyActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null){
                CheatToggles.reportBody = false;
            }

        }else{
            if (reportBodyActive){
                reportBodyActive = false;
            }
        }
    }

    public static void ejectPlayerPPM()
    {
        if (CheatToggles.ejectPlayer)
        {
            if (!ejectPlayerActive)
            {
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

                List<NetworkedPlayerInfo> val = new List<NetworkedPlayerInfo>();
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!player.Data.IsDead && !player.Data.Disconnected)
                    {
                        val.Add(player.Data);
                    }
                }
                PlayerPickMenu.openPlayerPickMenu(val, (Action)(() =>
                {
                    NetworkedPlayerInfo playerToEject = PlayerPickMenu.targetPlayerData;
                    MeetingHud.Instance.RpcVotingComplete(new Il2CppStructArray<MeetingHud.VoterState>(0L), playerToEject, false);
                }));

                ejectPlayerActive = true;
            }
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.ejectPlayer = false;
            }
        }
        else if (ejectPlayerActive)
        {
            ejectPlayerActive = false;
        }
    }

    public static void killPlayerPPM()
    {
        if (CheatToggles.killPlayer)
        {
            if (!killPlayerActive)
            {
                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("killPlayer");
                }

                if (Utils.isLobby){
                    HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
                    CheatToggles.killPlayer = false;
                    return;
                }

                // Player pick menu made for killing any player by sending a successful MurderPlayer RPC call
                PlayerPickMenu.openPlayerPickMenu(Utils.GetAllPlayerData(), (Action)(() =>
                {
                    Utils.murderPlayer(PlayerPickMenu.targetPlayerData.Object, MurderResultFlags.Succeeded);
                }));

                killPlayerActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null){
                CheatToggles.killPlayer = false;
            }
        }
        else if (killPlayerActive)
        {
            killPlayerActive = false;
        }
    }

    public static void telekillPlayerPPM()
    {
        if (CheatToggles.telekillPlayer)
        {
            if (!telekillPlayerActive)
            {
                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("telekillPlayer");
                }

                if (Utils.isLobby){
                    HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
                    CheatToggles.telekillPlayer = false;
                    return;
                }

                // Player pick menu made for killing any player by sending a successful MurderPlayer RPC call
                PlayerPickMenu.openPlayerPickMenu(Utils.GetAllPlayerData(), (Action)(() =>
                {
                    var oldPos = PlayerControl.LocalPlayer.GetTruePosition();
                    Utils.murderPlayer(PlayerPickMenu.targetPlayerData.Object, MurderResultFlags.Succeeded);
                    AmongUsClient.Instance.StartCoroutine(DelayedTeleportBack(oldPos));
                }));

                telekillPlayerActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null){
                CheatToggles.telekillPlayer = false;
            }
        }
        else if (telekillPlayerActive)
        {
            telekillPlayerActive = false;
        }
    }

    /// <summary>
    /// Coroutine to teleport the LocalPlayer back to their original position after a short delay.
    /// </summary>
    /// <param name="position">The position to teleport back to.</param>
    /// <returns>An IEnumerator for the coroutine.</returns>
    public static System.Collections.IEnumerator DelayedTeleportBack(Vector2 position)
    {
        yield return new WaitForSeconds(0.25f);
        PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(position);
    }

    public static void teleportPlayerPPM()
    {
        if (CheatToggles.teleportPlayer)
        {
            if (!teleportPlayerActive)
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
                    if (!player.AmOwner){
                        playerDataList.Add(player.Data);
                    }
                }

                // Player pick menu made for teleporting LocalPlayer to any player's position
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(PlayerPickMenu.targetPlayerData.Object.transform.position);
                }));

                teleportPlayerActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null){
                CheatToggles.teleportPlayer = false;
            }
        }
        else if (teleportPlayerActive)
        {
            teleportPlayerActive = false;
        }
    }

    public static void changeRolePPM()
    {
        if (CheatToggles.changeRole){

            if (!changeRoleActive){

                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null){
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("changeRole");
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                // Shapeshifter role can only be used if it was already assigned at the start of the game
                // This is done to prevent the anticheat from kicking players
                if (oldRole == RoleTypes.Shapeshifter || Utils.isFreePlay){

                    playerDataList.Add(PlayerPickMenu.customPPMChoice("Shapeshifter", Outfits.shapeshifter, Utils.getBehaviourByRoleType(RoleTypes.Shapeshifter)));

                }

                // Phantom role can only be used if it was already assigned at the start of the game
                // This is done to prevent the anticheat from kicking players
                if (oldRole == RoleTypes.Phantom || Utils.isFreePlay){

                    playerDataList.Add(PlayerPickMenu.customPPMChoice("Phantom", Outfits.phantom, Utils.getBehaviourByRoleType(RoleTypes.Phantom)));

                }

                // Viper role can only be used if it was already assigned at the start of the game
                // This is done to prevent the anticheat from kicking players
                if (oldRole == RoleTypes.Viper || Utils.isFreePlay){

                    playerDataList.Add(PlayerPickMenu.customPPMChoice("Viper", Outfits.viper, Utils.getBehaviourByRoleType(RoleTypes.Viper)));

                }

                // Impostor role can only be used if it was already assigned at the start of the game or as host
                // This is done to prevent the anticheat from kicking players
                if (oldRole == RoleTypes.Impostor || Utils.isFreePlay || Utils.isHost){

                    playerDataList.Add(PlayerPickMenu.customPPMChoice("Impostor", Outfits.impostor, Utils.getBehaviourByRoleType(RoleTypes.Impostor)));

                }

                playerDataList.Add(PlayerPickMenu.customPPMChoice("Tracker", Outfits.tracker, Utils.getBehaviourByRoleType(RoleTypes.Tracker)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Noisemaker", Outfits.noisemaker, Utils.getBehaviourByRoleType(RoleTypes.Noisemaker)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Engineer", Outfits.engineer, Utils.getBehaviourByRoleType(RoleTypes.Engineer)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Scientist", Outfits.scientist, Utils.getBehaviourByRoleType(RoleTypes.Scientist)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Detective", Outfits.detective, Utils.getBehaviourByRoleType(RoleTypes.Detective)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Crewmate", Outfits.crewmate, Utils.getBehaviourByRoleType(RoleTypes.Crewmate)));

                // Player pick menu made for changing your roles with a custom choice list
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {

                    // Log the originally assigned role before it gets changed by changeRole cheat
                    if (!Utils.isLobby && !Utils.isFreePlay && oldRole == null){
                        oldRole = PlayerControl.LocalPlayer.Data.RoleType;
                    }

                    if (PlayerControl.LocalPlayer.Data.IsDead){ // Prevent accidential revives
                        if (PlayerPickMenu.targetPlayerData.Role.TeamType == RoleTeamTypes.Impostor){
                            RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, RoleTypes.ImpostorGhost);
                        }else{
                            RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, RoleTypes.CrewmateGhost);
                        }
                    }else{

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

                changeRoleActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null){
                CheatToggles.changeRole = false;
            }

        }else{
            if (changeRoleActive){
                changeRoleActive = false;
            }
        }
    }

    public static void forceRolePPM()
    {
        if (CheatToggles.forceRole)
        {
            if (!forceRoleActive)
            {
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("forceRole");
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                playerDataList.Add(PlayerPickMenu.customPPMChoice("Shapeshifter", Outfits.shapeshifter, Utils.getBehaviourByRoleType(RoleTypes.Shapeshifter)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Phantom", Outfits.phantom, Utils.getBehaviourByRoleType(RoleTypes.Phantom)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Viper", Outfits.viper, Utils.getBehaviourByRoleType(RoleTypes.Viper)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Impostor", Outfits.impostor, Utils.getBehaviourByRoleType(RoleTypes.Impostor)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Tracker", Outfits.tracker, Utils.getBehaviourByRoleType(RoleTypes.Tracker)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Noisemaker", Outfits.noisemaker, Utils.getBehaviourByRoleType(RoleTypes.Noisemaker)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Engineer", Outfits.engineer, Utils.getBehaviourByRoleType(RoleTypes.Engineer)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Scientist", Outfits.scientist, Utils.getBehaviourByRoleType(RoleTypes.Scientist)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Detective", Outfits.detective, Utils.getBehaviourByRoleType(RoleTypes.Detective)));
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Crewmate", Outfits.crewmate, Utils.getBehaviourByRoleType(RoleTypes.Crewmate)));

                // Player pick menu made for forcing a role onto another player
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    CheatToggles.forcedRole = PlayerPickMenu.targetPlayerData.Role.Role;
                }));

                forceRoleActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.forceRole = false;
            }

        }
        else
        {
            if (forceRoleActive)
            {
                forceRoleActive = false;
            }
        }
    }

    public static void spectatePPM()
    {
        if (CheatToggles.spectate){

            if (!spectateActive){

                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null){
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("spectate");
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                // All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerDataList.Add(player.Data);
                    }
                }

                // Player pick menu made for spectating the targeted player
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerPickMenu.targetPlayerData.Object);
                }));

                spectateActive = true;

                PlayerControl.LocalPlayer.moveable = false; // Can't move while spectating

                CheatToggles.freecam = false; // Disable incompatible cheats while spectating

            }

            // Deactivate cheat if menu is closed and no one is getting spectated
            if (PlayerPickMenu.playerpickMenu == null && Camera.main.gameObject.GetComponent<FollowerCamera>().Target == PlayerControl.LocalPlayer){
                CheatToggles.spectate = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }else{
            // Deactivate cheat when it is disabled from the Malum GUI
            if (spectateActive){
                spectateActive = false;
                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            }
        }
    }
}
