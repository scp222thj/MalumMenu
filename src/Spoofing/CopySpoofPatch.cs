using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System;
using BepInEx;
using UnityEngine;
using System.IO;
using BepInEx.Configuration;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class CopyPUID_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to copy a player's PUID
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.copyPlayerPUID){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats();
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerList.Add(player);
                    }
                }

                //New player pick menu made for copying a player's PUID to clipboard
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    GUIUtility.systemCopyBuffer = AmongUsClient.Instance.GetClientFromCharacter(Utils_PlayerPickMenu.targetPlayer).ProductUserId;
                
                    Utils.showPopup($"\n{Utils_PlayerPickMenu.targetPlayer.Data.PlayerName}'s PUID\nhas been copied to your clipboard");
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.copyPlayerPUID = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class CopyFriendCode_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to copy a player's friend code
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.copyPlayerFC){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats();
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerList.Add(player);
                    }
                }

                //New player pick menu made for copying a player's friend code to clipboard
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    GUIUtility.systemCopyBuffer = AmongUsClient.Instance.GetClientFromCharacter(Utils_PlayerPickMenu.targetPlayer).FriendCode;
                
                    Utils.showPopup($"\n{Utils_PlayerPickMenu.targetPlayer.Data.PlayerName}'s friend code\nhas been copied to your clipboard");
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.copyPlayerFC = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}