using HarmonyLib;
using UnityEngine;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Teleport_CursorPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to teleport to cursor position on right-click
    public static void Postfix(PlayerPhysics __instance)
    {
        if(Input.GetMouseButtonDown(1) && CheatSettings.teleportCursor){

            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Teleport_PlayerPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to teleport to player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.teleportPlayer){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatSettings.spectate = CheatSettings.callMeeting = CheatSettings.shapeshiftAll = CheatSettings.saveSpoofData = CheatSettings.copyOutfit = CheatSettings.kickPlayer = CheatSettings.murderPlayer = false;
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerList.Add(player);
                    }
                }

                //New player pick menu made for teleporting
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Utils_PlayerPickMenu.targetPlayer.transform.position);
                
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatSettings.teleportPlayer = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}