using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Shapeshift_ShapeshiftAllPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to murder any player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.shapeshiftAll){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatSettings.spectate = CheatSettings.callMeeting = CheatSettings.murderPlayer = CheatSettings.copyOutfit = CheatSettings.teleportPlayer = CheatSettings.saveSpoofData = CheatSettings.kickPlayer = false;
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    playerList.Add(player);
                }

                //New player pick menu made for killing players
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    var HostData = AmongUsClient.Instance.GetHost();
                    if (HostData != null && !HostData.Character.Data.Disconnected){

                        foreach (var sender in PlayerControl.AllPlayerControls)
                        {
                            if(sender.PlayerId != Utils_PlayerPickMenu.targetPlayer.PlayerId || sender.CurrentOutfitType != PlayerOutfitType.Default){
                                foreach (var recipient in PlayerControl.AllPlayerControls)
                                {
                                        MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.Shapeshift, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient));
                                        messageWriter.WriteNetObject(Utils_PlayerPickMenu.targetPlayer);
                                        messageWriter.Write(true);
                                        AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
                                }
                            }
                        }

                    }
                
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatSettings.shapeshiftAll = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}