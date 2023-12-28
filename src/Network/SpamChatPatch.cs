using HarmonyLib;
using Hazel;
using System.Text.RegularExpressions;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class Network_SpamChatPostfix
{
    //Prefix patch of MeetingHud.CastVote
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (CheatSettings.spamChat){
            chatText = Regex.Replace(chatText, "<.*?>", string.Empty);

            if (string.IsNullOrWhiteSpace(chatText))
            {
                return false;
            }

            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected){
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

            return false;

        }

        return true;
    }
}