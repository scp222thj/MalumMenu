using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class RPC_CopyOutfitCheatPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to murder any player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.copyOutfit){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatSettings.spectate = CheatSettings.chatMimic = CheatSettings.callMeeting = CheatSettings.murderPlayer = CheatSettings.copyAllOutfits = CheatSettings.teleportPlayer = CheatSettings.saveSpoofData = CheatSettings.kickPlayer = false;
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if(!player.AmOwner){
                        playerList.Add(player);
                    }
                }

                //New player pick menu made for killing players
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    var HostData = AmongUsClient.Instance.GetHost();
                    if (HostData != null && !HostData.Character.Data.Disconnected){
                        
                        Utils.CopyOutfit(PlayerControl.LocalPlayer, Utils_PlayerPickMenu.targetPlayer);
                    
                    }
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatSettings.copyOutfit = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}