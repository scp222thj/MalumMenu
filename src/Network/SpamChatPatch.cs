using HarmonyLib;
using Hazel;
using System.Text.RegularExpressions;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class Network_SpamChatPostfix
{
    //Prefix patch of PlayerControl.RpcSendChat
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (CheatSettings.spamChat){ //Run custom code if the spamChat cheat is active
            chatText = Regex.Replace(chatText, "<.*?>", string.Empty);

            if (string.IsNullOrWhiteSpace(chatText))
            {
                return false;
            }

            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected){

                //Sends a forged SendChat RPC call from each player to all players
                //This generates an identical fake chat message coming from each player, spamming the chat
                foreach (var sender in PlayerControl.AllPlayerControls)
                {
                    foreach (var recipient in PlayerControl.AllPlayerControls)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SendChat, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient.Data.Object));
                        writer.Write(chatText);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
                
            }

            return false; //Skip the original method when the cheat is active

        }

        return true; //Send a normal chat message if the cheat is not active
    }
}