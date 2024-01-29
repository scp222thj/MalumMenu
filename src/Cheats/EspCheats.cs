using UnityEngine;
using System;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;
public static class EspHandler
{
    public static bool spectateActive;
    public static void sporeCloudVision(Mushroom mushroom)
    {
        if (CheatToggles.fullBright)
        {
            mushroom.sporeMask.transform.position = new UnityEngine.Vector3(mushroom.sporeMask.transform.position.x, mushroom.sporeMask.transform.position.y, -1);
            return;
        } 

        mushroom.sporeMask.transform.position = new UnityEngine.Vector3(mushroom.sporeMask.transform.position.x, mushroom.sporeMask.transform.position.y, 5f);
    }

    public static void spectateCheat()
    {
        if (CheatToggles.spectate){

            //Open spectator menu when CheatSettings.spectate is first enabled
            if (!spectateActive){

                //Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null){
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("spectate");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerDataList.Add(player.Data);
                    }
                }

                //New player pick menu made for spectating
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerPickMenu.targetPlayerData.Object);
                }));

                spectateActive = true;

                PlayerControl.LocalPlayer.moveable = false; //Can't move while spectating

                CheatToggles.freeCam = false; //Disable incompatible cheats while spectating

            }

            //Deactivate cheat if menu is closed without spectating anyone
            if (PlayerPickMenu.playerpickMenu == null && Camera.main.gameObject.GetComponent<FollowerCamera>().Target == PlayerControl.LocalPlayer){
                CheatToggles.spectate = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }else{
            //Deactivate cheat when it is disabled from the GUI
            if (spectateActive){
                spectateActive = false;
                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            }
        }
    }
}