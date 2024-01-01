using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Meetings_CallMeetingPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to murder any player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.callMeeting){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatSettings.spectate = CheatSettings.murderPlayer = CheatSettings.massShapeshift = CheatSettings.shapeshiftCheat = CheatSettings.teleportPlayer = CheatSettings.kickPlayer = false;
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

                        //Report the dead body by sending a (fake) ReportDeadBody RPC call to all clients
                        foreach (var item in PlayerControl.AllPlayerControls)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(Utils_PlayerPickMenu.targetPlayer.NetId, (byte)RpcCalls.ReportDeadBody, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                            writer.Write(Utils_PlayerPickMenu.targetPlayer.Data.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                        }

                    }
            
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatSettings.callMeeting = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}