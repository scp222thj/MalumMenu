using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class AllShapeshift_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to shapeshift all players into the target
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.shapeshiftAll){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("shapeshiftAll");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList
                foreach (var player in PlayerControl.AllPlayerControls){
                    playerDataList.Add(player.Data);
                }

                //New player pick menu made for targeting a player for shapeshifting
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    var HostData = AmongUsClient.Instance.GetHost();
                    if (HostData != null && !HostData.Character.Data.Disconnected){

                        //Each player will shapeshift into targetPlayer
                        foreach (var sender in PlayerControl.AllPlayerControls)
                        {
                            //The target player should not be affected unless they are currently not in their default outfit
                            if(sender.PlayerId != Utils_PlayerPickMenu.targetPlayerData.PlayerId || sender.CurrentOutfitType != PlayerOutfitType.Default){
                                Utils.ShapeshiftPlayer(sender, Utils_PlayerPickMenu.targetPlayerData.Object, true);
                            }
                        }

                    }
                
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.shapeshiftAll = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}