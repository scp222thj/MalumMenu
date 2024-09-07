using Il2CppSystem.Collections.Generic;
using System;
using AmongUs.GameOptions;
using UnityEngine;

namespace MalumMenu;
public static class MalumPPMCheats
{
    public static bool telekillPlayerActive;
    public static bool killPlayerActive;
    public static bool spectateActive;
    public static bool teleportPlayerActive;
    public static bool reportBodyActive;
    public static bool changeRoleActive;
    public static float teleKillWaitFrames = -1;
    public static Vector2 teleKillPosition;
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

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                // All players are saved to playerList
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    playerDataList.Add(player.Data);
                }

                // Player pick menu made for killing any player by sending a successful MurderPlayer RPC call
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    teleKillPosition = PlayerControl.LocalPlayer.GetTruePosition();
                    Utils.murderPlayer(PlayerPickMenu.targetPlayerData.Object, MurderResultFlags.Succeeded);
                    teleKillWaitFrames = 40;
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

                    NetworkedPlayerInfo.PlayerOutfit shapeshifterOutfit = new NetworkedPlayerInfo.PlayerOutfit
                    {
                        ColorId = 0,
                        SkinId = "skin_screamghostface",
                        VisorId = "visor_eliksni"
                    };

                    // Custom PPM choice for Shapeshifter role
                    playerDataList.Add(PlayerPickMenu.customPPMChoice("Shapeshifter", shapeshifterOutfit, Utils.getBehaviourByRoleType(RoleTypes.Shapeshifter)));

                }

                // Phantom role can only be used if it was already assigned at the start of the game
                // This is done to prevent the anticheat from kicking players
                if (oldRole == RoleTypes.Phantom || Utils.isFreePlay){

                    NetworkedPlayerInfo.PlayerOutfit phantomOutfit = new NetworkedPlayerInfo.PlayerOutfit
                    {
                        ColorId = 0,
                        HatId = "hat_screamghostface",
                        SkinId = "skin_screamghostface"
                    };

                    // Custom PPM choice for Impostor role
                    playerDataList.Add(PlayerPickMenu.customPPMChoice("Phantom", phantomOutfit, Utils.getBehaviourByRoleType(RoleTypes.Phantom)));

                }

                // Impostor role can only be used if it was already assigned at the start of the game or as host
                // This is done to prevent the anticheat from kicking players
                if (oldRole == RoleTypes.Impostor || Utils.isFreePlay || Utils.isHost){

                    NetworkedPlayerInfo.PlayerOutfit impostorOutfit = new NetworkedPlayerInfo.PlayerOutfit
                    {
                        ColorId = 0
                    };

                    // Custom PPM choice for Impostor role
                    playerDataList.Add(PlayerPickMenu.customPPMChoice("Impostor", impostorOutfit, Utils.getBehaviourByRoleType(RoleTypes.Impostor)));
                
                }

                NetworkedPlayerInfo.PlayerOutfit trackerOutfit = new NetworkedPlayerInfo.PlayerOutfit
                {
                    ColorId = 10,
                    SkinId = "skin_rhm"
                };

                // Custom PPM choice for Tracker role
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Tracker", trackerOutfit, Utils.getBehaviourByRoleType(RoleTypes.Tracker)));

                NetworkedPlayerInfo.PlayerOutfit noisemakerOutfit = new NetworkedPlayerInfo.PlayerOutfit
                {
                    ColorId = 10,
                    HatId = "hat_pk03_Headphones"
                };

                // Custom PPM choice for Noisemaker role
                playerDataList.Add(PlayerPickMenu.customPPMChoice("Noisemaker", noisemakerOutfit, Utils.getBehaviourByRoleType(RoleTypes.Noisemaker)));

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