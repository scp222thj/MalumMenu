using Il2CppSystem.Collections.Generic;
using System;
using AmongUs.GameOptions;
using UnityEngine;

namespace MalumMenu;
public static class MalumPPMCheats
{
    public static bool murderPlayerActive;
    public static bool spectateActive;
    public static bool teleportPlayerActive;
    public static bool reportBodyActive;
    public static bool changeRoleActive;
    public static RoleTypes? oldRole = null;

    public static void reportBodyPPM(){
        if (CheatToggles.reportBody){

            if (!reportBodyActive){

                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null){
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("reportBody");
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                // All players are saved to playerList
                foreach (var player in PlayerControl.AllPlayerControls){
                    playerDataList.Add(player.Data);
                }

                // Player pick menu to choose any body (alive or dead) and report it
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    
                    Utils.reportDeadBody(PlayerPickMenu.targetPlayerData);
            
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

    public static void murderPlayerPPM()
    {
        if (CheatToggles.murderPlayer)
        {
            if (!murderPlayerActive)
            {
                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("murderPlayer");
                }

                if (Utils.isLobby){
                    HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
                    CheatToggles.murderPlayer = false;
                    return;
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                // All players are saved to playerList
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    playerDataList.Add(player.Data);
                }

                // Player pick menu made for killing any player by sending a successful MurderPlayer RPC call
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    Utils.murderPlayer(PlayerPickMenu.targetPlayerData.Object, MurderResultFlags.Succeeded);
                }));

                murderPlayerActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null){
                CheatToggles.murderPlayer = false;
            }
        }
        else if (murderPlayerActive)
        {
            murderPlayerActive = false;
        }
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
                if (oldRole == RoleTypes.Shapeshifter){

                    NetworkedPlayerInfo.PlayerOutfit shapeshifterOutfit = new NetworkedPlayerInfo.PlayerOutfit
                    {
                        ColorId = 0,
                        SkinId = "skin_screamghostface",
                        VisorId = "visor_eliksni"
                    };

                    // Custom PPM choice for Shapeshifter role
                    playerDataList.Add(PlayerPickMenu.customPPMChoice("Shapeshifter", shapeshifterOutfit, Utils.getBehaviourByRoleType(RoleTypes.Shapeshifter)));

                }

                NetworkedPlayerInfo.PlayerOutfit impostorOutfit = new NetworkedPlayerInfo.PlayerOutfit
                {
                    ColorId = 0
                };

                // Custom PPM choice for Impostor role
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Impostor", impostorOutfit, Utils.getBehaviourByRoleType(RoleTypes.Impostor)));

                NetworkedPlayerInfo.PlayerOutfit engineerOutfit = new NetworkedPlayerInfo.PlayerOutfit
                {
                    ColorId = 10,
                    SkinId = "skin_Mech",
                    VisorId = "visor_D2CGoggles"
                };

                // Custom PPM choice for Engineer role
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Engineer", engineerOutfit, Utils.getBehaviourByRoleType(RoleTypes.Engineer)));

                NetworkedPlayerInfo.PlayerOutfit scientistOutfit = new NetworkedPlayerInfo.PlayerOutfit
                {
                    ColorId = 10,
                    SkinId = "skin_Science",
                    VisorId = "visor_pk01_PaperMaskVisor"
                };

                // Custom PPM choice for Scientist role
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Scientist", scientistOutfit, Utils.getBehaviourByRoleType(RoleTypes.Scientist)));

                NetworkedPlayerInfo.PlayerOutfit crewmateOutfit = new NetworkedPlayerInfo.PlayerOutfit
                {
                    ColorId = 10
                };

                // Custom PPM choice for Crewmate role
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Crewmate", crewmateOutfit, Utils.getBehaviourByRoleType(RoleTypes.Crewmate)));

                // Player pick menu made for changing your roles with a custom choice list
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {

                    // Log the originally assigned role before it gets changed by changeRole cheat
                    if (!Utils.isLobby && oldRole == null){
                        oldRole = PlayerControl.LocalPlayer.Data.RoleType;
                    }

                    if (PlayerControl.LocalPlayer.Data.IsDead){ // Prevent accidential revives
                        if (PlayerPickMenu.targetPlayerData.Role.TeamType == RoleTeamTypes.Impostor){
                            RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, RoleTypes.ImpostorGhost);
                        }else{
                            RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, RoleTypes.CrewmateGhost);
                        }
                    }else{
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