using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class ChatMimic_PlayerControl_RpcSendChat_Prefix
{
    public static PlayerControl chatMimicTarget = null;

    // Prefix patch of PlayerControl.RpcSendChat to send chat messages as another player
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (chatMimicTarget == null)
        {
            return true; //Only works if the target has been specified
        }


        //Send a fake SendChat RPC with your text to each player from the chatMimicTarget
        foreach (var item in PlayerControl.AllPlayerControls)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(chatMimicTarget.NetId, (byte)RpcCalls.SendChat, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
            writer.Write(chatText);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        return false;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ChatMimic_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to pick a target for ChatMimic
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.chatMimic){

            //Open player pick menu when CheatSettings.chatMimic is first enabled
            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("chatMimic");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerDataList.Add(player.Data);
                    }
                }

                //New player pick menu made for picking a ChatMimic target
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    ChatMimic_PlayerControl_RpcSendChat_Prefix.chatMimicTarget = Utils_PlayerPickMenu.targetPlayerData.Object;

                    Utils.OpenChat();
                }));

                CheatToggles.spamChat = CheatToggles.setNameAll = CheatToggles.setPlayerName = false; //SpamChat, ChatMimic & ChangeName do not work well with each other

                isActive = true;

            }

            //Deactivate cheat if menu is closed and the target is gone
            if (Utils_PlayerPickMenu.playerpickMenu == null && ChatMimic_PlayerControl_RpcSendChat_Prefix.chatMimicTarget == null){
                CheatToggles.chatMimic = false;
            }
        }else{
            //Deactivate cheat when it is disabled from the GUI
            if (isActive){
                isActive = false;

                ChatMimic_PlayerControl_RpcSendChat_Prefix.chatMimicTarget = null;
            }
        }
    }
}