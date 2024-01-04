using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class MimicOutfit_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to pick a player outfit to mimic
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.mimicOutfit){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.spectate = CheatToggles.copyPlayerFC = CheatToggles.chatMimic = CheatToggles.reportBody = CheatToggles.murderPlayer = CheatToggles.mimicAllOutfits = CheatToggles.teleportPlayer = CheatToggles.copyPlayerPUID = CheatToggles.kickPlayer = false;
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if(!player.AmOwner){
                        playerList.Add(player);
                    }
                }

                //New player pick menu made for mimicking outfits
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    Utils.CopyOutfit(PlayerControl.LocalPlayer, Utils_PlayerPickMenu.targetPlayer);
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.mimicOutfit = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}