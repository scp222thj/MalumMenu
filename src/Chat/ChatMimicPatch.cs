using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class Chat_ChatMimicPrefix
{
    public static PlayerControl chatMimicTarget = null;

    // Prefix patch of PlayerControl.RpcSendChat to set spamText based on the user's chat messages
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (chatMimicTarget == null)
        {
            return true;
        }

        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(chatMimicTarget.NetId, (byte)RpcCalls.SendChat, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                writer.Write(chatText);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        return false;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Chat_ChatMimicSetTargetPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open spectator menu
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.chatMimic){

            //Open spectator menu when CheatSettings.spectate is first enabled
            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatSettings.teleportPlayer = CheatSettings.spectate = CheatSettings.saveSpoofData = CheatSettings.callMeeting = CheatSettings.copyAllOutfits = CheatSettings.copyOutfit = CheatSettings.kickPlayer = CheatSettings.murderPlayer = false;
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerList.Add(player);
                    }
                }

                //New player pick menu made for spectating
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    Chat_ChatMimicPrefix.chatMimicTarget = Utils_PlayerPickMenu.targetPlayer;
                }));

                isActive = true;

                CheatSettings.spamChat = false;

            }

            //Deactivate cheat if menu is closed without spectating anyone
            if (Utils_PlayerPickMenu.playerpickMenu == null && Chat_ChatMimicPrefix.chatMimicTarget == null){
                CheatSettings.chatMimic = false;
            }
        }else{
            //Deactivate cheat when it is disabled from the GUI
            if (isActive){
                isActive = false;
                Chat_ChatMimicPrefix.chatMimicTarget = null;
            }
        }
    }
}