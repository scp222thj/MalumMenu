using HarmonyLib;
using UnityEngine;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class TeleportCursor_PlayerPhysics_LateUpdate_Postfix
{
    public static bool previousTeleportMeCursorState = false;
    public static bool previousTeleportAllCursorState = false;

    // Postfix patch of PlayerPhysics.LateUpdate to teleport players to cursor position on right-click
    public static void Postfix(PlayerPhysics __instance)
    {
        //Code to make sure only one of these cheats is enabled at a time
        if (CheatToggles.teleportMeCursor && !previousTeleportMeCursorState)
        {
            CheatToggles.teleportAllCursor = false;
            previousTeleportMeCursorState = true;
            previousTeleportAllCursorState = false;
        }
        else if (CheatToggles.teleportAllCursor && !previousTeleportAllCursorState)
        {
            CheatToggles.teleportMeCursor = false;
            previousTeleportAllCursorState = true;
            previousTeleportMeCursorState = false;
        }

        if (Input.GetMouseButtonDown(1)) //Register right-click
        {
            if (CheatToggles.teleportMeCursor) //Teleport LocalPlayer to cursor position
            {
                Utils.TeleportPlayer(PlayerControl.LocalPlayer, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            else if (CheatToggles.teleportAllCursor) //Teleport all players to cursor position
            {
                foreach (var item in PlayerControl.AllPlayerControls)
                {
                    Utils.TeleportPlayer(item, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
        }
    }
}


[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class TeleportMePlayer_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to teleport to player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.teleportMePlayer){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("teleportMePlayer");
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
                    Utils.TeleportPlayer(PlayerControl.LocalPlayer, Utils_PlayerPickMenu.targetPlayer.transform.position);
                
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.teleportMePlayer = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class TeleportAllPlayer_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to teleport all players to one player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.teleportAllPlayer){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("teleportAllPlayer");
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    playerList.Add(player);
                }

                //New player pick menu made for teleporting
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    foreach (var item in PlayerControl.AllPlayerControls){
                        if (item.PlayerId != Utils_PlayerPickMenu.targetPlayer.PlayerId){
                            Utils.TeleportPlayer(item, Utils_PlayerPickMenu.targetPlayer.transform.position);
                        }
                    }
                
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.teleportAllPlayer = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}