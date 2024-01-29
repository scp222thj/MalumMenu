using Il2CppSystem.Collections.Generic;
using System;
using UnityEngine;

namespace MalumMenu;
public static class PlayerCheatHandler
{
    public static bool murderPlayerActive;
    public static bool teleportPlayerActive;
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