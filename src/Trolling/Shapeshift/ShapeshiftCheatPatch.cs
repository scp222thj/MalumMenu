using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ShapeshiftCheat_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to shapeshift into any player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.shapeshiftCheat){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("shapeshiftCheat");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerDataList.Add(player.Data);
                    }
                }

                //New player pick menu made for shapeshifting into any player
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {

                    var HostData = AmongUsClient.Instance.GetHost();
                    if (HostData != null && !HostData.Character.Data.Disconnected){

                        Utils.ShapeshiftPlayer(PlayerControl.LocalPlayer, Utils_PlayerPickMenu.targetPlayerData.Object, !CheatToggles.noShapeshiftAnim); //Compatible with noShapeshiftAnim

                    }
                
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.shapeshiftCheat = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}