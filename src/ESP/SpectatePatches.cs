using HarmonyLib;
using UnityEngine;
using System;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Spectate_MainPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open spectator menu
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.spectate){

            //Open spectator menu when CheatSettings.spectate is first enabled
            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatSettings.teleportPlayer = CheatSettings.saveSpoofData = CheatSettings.callMeeting = CheatSettings.shapeshiftAll = CheatSettings.copyOutfit = CheatSettings.kickPlayer = CheatSettings.murderPlayer = false;
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerList.Add(player);
                    }
                }

                //New player pick menu made for spectating
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(Utils_PlayerPickMenu.targetPlayer);
                }));

                isActive = true;

                PlayerControl.LocalPlayer.moveable = false; //Can't move while spectating

                CheatSettings.freeCam = false; //Disable incompatible cheats while spectating

            }

            //Deactivate cheat if menu is closed without spectating anyone
            if (Utils_PlayerPickMenu.playerpickMenu == null && Camera.main.gameObject.GetComponent<FollowerCamera>().Target == PlayerControl.LocalPlayer){
                CheatSettings.spectate = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }else{
            //Deactivate cheat when it is disabled from the GUI
            if (isActive){
                isActive = false;
                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            }
        }
    }
}