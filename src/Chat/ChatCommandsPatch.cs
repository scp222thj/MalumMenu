using HarmonyLib;
using Hazel;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class ChatCommands_RpcSendChatPrefix
{
    //Prefix patch of PlayerControl.RpcSendChat to add custom commands
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (!chatText.StartsWith("/") || !CheatToggles.enableCommands)
        {
            return true;
        }
        string command = chatText.Substring(1).Split(' ')[0];
        string[] args = chatText.Substring(command.Length + 2).Split(' ');
        PlayerControl sender = __instance;
        if (ChatMimic_PlayerControl_RpcSendChat_Prefix.chatMimicTarget != null)
            sender = ChatMimic_PlayerControl_RpcSendChat_Prefix.chatMimicTarget;
        if (command == "whisper")
        {
            string playerName = chatText.Substring(command.Length + 2).Split("'")[1];
            PlayerControl player = Utils.GetPlayerByName(playerName);
            if (!player)
            {
                HudManager.Instance.Notifier.AddItem("<#fff000>Player doesn't exist</color>");
                return false;
            }
            if (player == sender)
            {
                HudManager.Instance.Notifier.AddItem("<#fff000>Can't send a whisper to yourself</color>");
                return false;
            }
            string whisperText = chatText.Substring(command.Length + playerName.Length + 5);
            string originalName = sender.Data.DefaultOutfit.PlayerName;
            Utils.SetName(sender, "<#ccc>" + originalName + " > " + playerName + "</color>");
            MessageWriter writer1 = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SendChat, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(player));
            MessageWriter writer2 = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SendChat, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(sender));
            writer1.Write(whisperText);
            writer2.Write(whisperText);
            AmongUsClient.Instance.FinishRpcImmediately(writer1);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);
            if (sender != __instance && player != __instance)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SendChat, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(__instance));
                writer.Write(whisperText);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            Utils.SetName(sender, originalName);
            return false;
        }
        else if (command == "setname")
        {
            string inputName = chatText.Substring(command.Length + 2);
            Utils.SetName(sender, inputName);
            return false;
        }
        else if (command == "setnameall")
        {
            string inputName = chatText.Substring(command.Length + 2);
            foreach (var player in PlayerControl.AllPlayerControls)
                Utils.SetName(player, inputName);
            return false;
        }
        else
        {
            HudManager.Instance.Notifier.AddItem("<#fff000>Command doesn't exist</color>");
            return false;
        }
    }
}