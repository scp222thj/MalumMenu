using Il2CppSystem.Collections.Generic;
using System;
using UnityEngine;

namespace MalumMenu;
public static class MalumCheats
{
    public static bool murderPlayerActive;
    public static bool teleportPlayerActive;
    public static bool reportBodyActive;

    public static void reportBodyCheat(){
        if (CheatToggles.reportBody){

            if (!reportBodyActive){

                //Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null){
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("reportBody");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.Data.IsDead){
                        playerDataList.Add(player.Data);
                    }
                }

                //New player pick menu to choose any body (alive or dead) to report
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    
                    Utils.ReportDeadBody(PlayerPickMenu.targetPlayerData);
            
                }));

                reportBodyActive = true;
            }

            //Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null){
                CheatToggles.reportBody = false;
            }

        }else{
            if (reportBodyActive){
                reportBodyActive = false;
            }
        }
    }
    public static void closeMeetingCheat()
    {
        if(CheatToggles.closeMeeting){
            
            if (MeetingHud.Instance){ //Closes MeetingHud window if it is open
                MeetingHud.Instance.DespawnOnDestroy = false;
                ExileController exileController = UnityEngine.Object.Instantiate<ExileController>(ShipStatus.Instance.ExileCutscenePrefab);
                UnityEngine.Object.Destroy(MeetingHud.Instance.gameObject);
                exileController.ReEnableGameplay();
                exileController.WrapUp();

            }else if (ExileController.Instance != null){ //Closes ExileController window if it is open
                ExileController.Instance.ReEnableGameplay();
                ExileController.Instance.WrapUp();
            }
            
            CheatToggles.closeMeeting = false; //Button behaviour
        }
    }

    public static void useVentCheat(HudManager hudManager)
    {
        //try-catch to prevent errors when role is null
        try{

			//Engineers & Impostors don't need this cheat so it is disabled for them
			//Ghost venting causes issues so it is also disabled
			if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead){
				hudManager.ImpostorVentButton.gameObject.SetActive(CheatToggles.useVents);
			}

        }catch{}
    }

    public static void sabotageCheat(ShipStatus shipStatus)
    {
        byte currentMapID = Utils.getCurrentMapID();

        MalumSabotageSystem.handleReactor(shipStatus, currentMapID);
        MalumSabotageSystem.handleOxygen(shipStatus, currentMapID);
        MalumSabotageSystem.handleComms(shipStatus, currentMapID);
        MalumSabotageSystem.handleElectrical(shipStatus, currentMapID);
        MalumSabotageSystem.handleMushMix(shipStatus, currentMapID);
        MalumSabotageSystem.handleDoors(shipStatus);
    }

    public static void murderPlayerCheat()
    {
        if (CheatToggles.murderPlayer)
        {
            if (!murderPlayerActive)
            {
                //Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("murderPlayer");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                // All players are saved to playerList, including the localplayer
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    playerDataList.Add(player.Data);
                }

                //New player pick menu made for killing any player by sending a successful MurderPlayer RPC call
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    var HostData = AmongUsClient.Instance.GetHost();
                    if (HostData != null && !HostData.Character.Data.Disconnected)
                    {
                        Utils.MurderPlayer(PlayerPickMenu.targetPlayerData.Object, MurderResultFlags.Succeeded);
                    }
                    CheatToggles.murderPlayer = false;
                }));

                murderPlayerActive = true;
            }

            //Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null){
                CheatToggles.murderPlayer = false;
            }
        }
        else if (murderPlayerActive)
        {
            murderPlayerActive = false;
        }
    }

    public static void murderAllCheat()
    {
        if (CheatToggles.murderAll){

            //Kill all players by sending a successful MurderPlayer RPC call to all clients
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Utils.MurderPlayer(player, MurderResultFlags.Succeeded);
            }

            CheatToggles.murderAll = false;

        }
    }

    public static void teleportCursorCheat()
    {
        if (CheatToggles.teleportCursor)
        {
            if (Input.GetMouseButtonDown(1)) // Register right-click
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }

    public static void speedBoostCheat()
    {
        //try-catch to avoid some errors I was reciving in the logs related to this cheat
        try{

            //PlayerControl.LocalPlayer.MyPhysics.Speed is the base speed of a player
            //Among Us uses this value with the associated game setting to calculate the TrueSpeed of the player
            if(CheatToggles.speedBoost){
                PlayerControl.LocalPlayer.MyPhysics.Speed = 2.5f * 2;
            }else{
                PlayerControl.LocalPlayer.MyPhysics.Speed = 2.5f; //By default, Speed is always 2.5f
            }

        }catch{}
    }

    public static void noClipCheat()
    {
        try{

            PlayerControl.LocalPlayer.Collider.enabled = !(CheatToggles.noClip || PlayerControl.LocalPlayer.onLadder);

        }catch{}
    }

    public static void teleportPlayerCheat()
    {
        if (CheatToggles.teleportPlayer)
        {
            if (!teleportPlayerActive)
            {
                //Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("teleportPlayer");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                // All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!player.AmOwner){
                        playerDataList.Add(player.Data);
                    }
                }

                //New player pick menu made for teleporting yourself to any player's position
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(PlayerPickMenu.targetPlayerData.Object.transform.position);
                }));

                teleportPlayerActive = true;
            }

            //Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null){
                CheatToggles.teleportPlayer = false;
            }
        }
        else if (teleportPlayerActive)
        {
            teleportPlayerActive = false;
        }
    }
}