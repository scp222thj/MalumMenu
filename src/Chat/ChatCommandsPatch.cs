using HarmonyLib;
using Hazel;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class ChatCommands_RpcSendChatPrefix
{
    public static string[] commands = {
        "whisper"
    };
    //Prefix patch of PlayerControl.RpcSendChat to add custom commands
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (!chatText.StartsWith("/") || !CheatToggles.enableCommands)
        {
            return true;
        }
        string command = chatText.Substring(1).Split(' ')[0];
        string[] args = chatText.Substring(command.Length + 3).Split(' ');
        switch (Array.IndexOf(commands, command))
        {
            case 0: // whisper
                string playerName = chatText.Substring(command.Length + 3).Split("'")[0];
                PlayerControl player = Utils.GetPlayerByName(playerName);
                if (!player)
                {
                    HudManager.Instance.Notifier.AddItem("<#fff000>Player doesn't exist</color>");
                    return false;
                }
                PlayerControl whisperSender = __instance;
                if (ChatMimic_PlayerControl_RpcSendChat_Prefix.chatMimicTarget != null)
                    whisperSender = ChatMimic_PlayerControl_RpcSendChat_Prefix.chatMimicTarget;
                if (player == whisperSender)
                {
                    HudManager.Instance.Notifier.AddItem("<#fff000>Can't send a whisper to yourself</color>");
                    return false;
                }
                string whisperText = chatText.Substring(command.Length + playerName.Length + 5);
                string originalName = whisperSender.Data.DefaultOutfit.PlayerName;
                Utils.SetName(whisperSender, "<#ccc>" + originalName + " > " + playerName + "</color>");
                MessageWriter writer1 = AmongUsClient.Instance.StartRpcImmediately(whisperSender.NetId, (byte)RpcCalls.SendChat, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(player));
                MessageWriter writer2 = AmongUsClient.Instance.StartRpcImmediately(whisperSender.NetId, (byte)RpcCalls.SendChat, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(whisperSender));
                writer1.Write(whisperText);
                writer2.Write(whisperText);
                AmongUsClient.Instance.FinishRpcImmediately(writer1);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
                if (whisperSender != __instance && player != __instance)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(whisperSender.NetId, (byte)RpcCalls.SendChat, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(__instance));
                    writer.Write(whisperText);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                Utils.SetName(whisperSender, originalName);
                return false;
            default:
                HudManager.Instance.Notifier.AddItem("<#fff000>Command doesn't exist</color>");
                return false;
        }
    }
}