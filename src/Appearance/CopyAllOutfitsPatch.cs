using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class MimicAllOutfits_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to pick the player outfit that all other players will mimic 
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.mimicAllOutfits){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.spectate = CheatToggles.copyPlayerFC = CheatToggles.chatMimic = CheatToggles.reportBody = CheatToggles.murderPlayer = CheatToggles.mimicOutfit = CheatToggles.teleportPlayer = CheatToggles.copyPlayerPUID = CheatToggles.kickPlayer = false;
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList
                foreach (var player in PlayerControl.AllPlayerControls){
                    playerList.Add(player);
                }

                //New player pick menu made for targeting a player outfit
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    //Each player will copy the targetPlayer's outfit
                    foreach (var sender in PlayerControl.AllPlayerControls)
                    {
                        //The target player should not be affected unless they are not currently in their default outfit
                        if(sender.PlayerId != Utils_PlayerPickMenu.targetPlayer.PlayerId || sender.CurrentOutfitType != PlayerOutfitType.Default){
                            Utils.CopyOutfit(sender, Utils_PlayerPickMenu.targetPlayer);
                        }
                    }
                
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.mimicAllOutfits = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}