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
                    CheatSettings.spectate = CheatSettings.callMeeting = CheatSettings.shapeshiftAll = CheatSettings.murderPlayer = CheatSettings.teleportPlayer = CheatSettings.kickPlayer = false;
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

                        //Shapeshift into any player by sending a fake Shapeshift RPC call to all clients
                        foreach (var item in PlayerControl.AllPlayerControls)
                        {
                            MessageWriter colorWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                            colorWriter.Write(Utils_PlayerPickMenu.targetPlayer.Data.DefaultOutfit.ColorId);
                            AmongUsClient.Instance.FinishRpcImmediately(colorWriter);

                            MessageWriter nameWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetName, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                            nameWriter.Write(Utils_PlayerPickMenu.targetPlayer.Data.DefaultOutfit.PlayerName);
                            AmongUsClient.Instance.FinishRpcImmediately(nameWriter);

                            MessageWriter hatWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetHatStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                            hatWriter.Write(Utils_PlayerPickMenu.targetPlayer.Data.DefaultOutfit.HatId);
                            AmongUsClient.Instance.FinishRpcImmediately(hatWriter);

                            MessageWriter petWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetPetStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                            petWriter.Write(Utils_PlayerPickMenu.targetPlayer.Data.DefaultOutfit.PetId);
                            AmongUsClient.Instance.FinishRpcImmediately(petWriter);

                            MessageWriter visorWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetVisorStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                            visorWriter.Write(Utils_PlayerPickMenu.targetPlayer.Data.DefaultOutfit.VisorId);
                            AmongUsClient.Instance.FinishRpcImmediately(visorWriter);

                            MessageWriter skinWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetSkinStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                            skinWriter.Write(Utils_PlayerPickMenu.targetPlayer.Data.DefaultOutfit.SkinId);
                            AmongUsClient.Instance.FinishRpcImmediately(skinWriter);
                        }

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