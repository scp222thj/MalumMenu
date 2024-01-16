using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class MurderPlayer_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to murder any player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.murderPlayer){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("murderPlayer");
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    playerList.Add(player);
                }

                //New player pick menu made for killing players
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    Utils.MurderPlayer(Utils_PlayerPickMenu.targetPlayer, Utils_PlayerPickMenu.targetPlayer);
                
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.murderPlayer = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}