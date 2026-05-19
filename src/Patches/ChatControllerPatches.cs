using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class ChatController_AddChat
{
    // Tracks message timestamps per player (playerId → list of Time.time values)
    private static readonly Dictionary<byte, List<float>> _msgTimes = new();

    // Prefix patch of ChatController.AddChat to receive ghost messages if CheatSettings.seeGhosts is enabled even if LocalPlayer is alive
    // Basically does what the original method did with the required modifications
    public static bool Prefix(PlayerControl sourcePlayer, string chatText, bool censor, ChatController __instance)
    {
        // Anti-Bot Kick: rate-limit chat messages per player when host (Feature 2)
        if (CheatToggles.antiBotKick && Utils.isHost && sourcePlayer != null && sourcePlayer != PlayerControl.LocalPlayer && sourcePlayer.Data != null)
        {
            byte pid = sourcePlayer.PlayerId;
            float now = Time.time;

            if (!_msgTimes.ContainsKey(pid))
                _msgTimes[pid] = new List<float>();

            _msgTimes[pid].Add(now);
            _msgTimes[pid].RemoveAll(t => now - t > 1.5f);

            if (_msgTimes[pid].Count > 4)
            {
                int clientId = sourcePlayer.Data.ClientId;
                ConsoleUI.Log($"[AntiBotKick] Kicked {sourcePlayer.Data.PlayerName} for spamming ({_msgTimes[pid].Count} msgs/1.5s)");
                AmongUsClient.Instance.KickPlayer(clientId, false);
                _msgTimes.Remove(pid);
            }
        }

        // Chat Logger
        if (CheatToggles.chatLogger && sourcePlayer != null && sourcePlayer.Data != null)
        {
            try
            {
                string logPath = Path.Combine(BepInEx.Paths.ConfigPath, "MalumChat.txt");
                string line    = $"[{DateTime.Now:HH:mm:ss}] {sourcePlayer.Data.PlayerName}: {chatText}";
                File.AppendAllText(logPath, line + Environment.NewLine);
            }
            catch { }
        }

        // Simply run original method if seeGhosts is disabled or LocalPlayer already dead
        if (!CheatToggles.seeGhosts || PlayerControl.LocalPlayer.Data.IsDead) return true;

        if (!sourcePlayer || !PlayerControl.LocalPlayer) return true;

		NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
		NetworkedPlayerInfo data2 = sourcePlayer.Data;

		if (data2 == null || data == null) return true; // Remove isDead check for LocalPlayer

		ChatBubble pooledBubble = __instance.GetPooledBubble();

		try
		{
			pooledBubble.transform.SetParent(__instance.scroller.Inner);
			pooledBubble.transform.localScale = Vector3.one;
			bool flag = sourcePlayer == PlayerControl.LocalPlayer;
			if (flag)
			{
				pooledBubble.SetRight();
			}
			else
			{
				pooledBubble.SetLeft();
			}
			bool didVote = MeetingHud.Instance && MeetingHud.Instance.DidVote(sourcePlayer.PlayerId);
			pooledBubble.SetCosmetics(data2);
			__instance.SetChatBubbleName(pooledBubble, data2, data2.IsDead, didVote, PlayerNameColor.Get(data2), null);
			if (censor && AmongUs.Data.DataManager.Settings.Multiplayer.CensorChat)
			{
				chatText = BlockedWords.CensorWords(chatText, false);
			}
			pooledBubble.SetText(chatText);
			pooledBubble.AlignChildren();
			__instance.AlignAllBubbles();
			if (!__instance.IsOpenOrOpening && __instance.notificationRoutine == null)
			{
				__instance.notificationRoutine = __instance.StartCoroutine(__instance.BounceDot());
			}
			if (!flag && !__instance.IsOpenOrOpening)
			{
				SoundManager.Instance.PlaySound(__instance.messageSound, false).pitch = 0.5f + sourcePlayer.PlayerId / 15f;
				__instance.chatNotification.SetUp(sourcePlayer, chatText);
			}
		}
		catch (Exception message)
		{
			ChatController.Logger.Error(message.ToString(), null);
			__instance.chatBubblePool.Reclaim(pooledBubble);
		}

        return false; // Skips the original method completly
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatController_Update
{
    // Postfix patch of ChatController.Update to unlock longer message length
    public static void Postfix(ChatController __instance)
    {
        //__instance.freeChatField.textArea.allowAllCharacters = CheatToggles.chatJailbreak; // Not really used by the game's code, but I include it anyway
        //__instance.freeChatField.textArea.AllowSymbols = true; // Allow sending certain symbols
        //__instance.freeChatField.textArea.AllowEmail = CheatToggles.chatJailbreak; // Allow sending email addresses when chatJailbreak is enabled
        //__instance.freeChatField.textArea.AllowPaste = CheatToggles.chatJailbreak; // Allow pasting from clipboard in chat when chatJailbreak is enabled

        if (CheatToggles.longerMessages)
		{
			// Increasing the maximum length by 20 characters still avoids anticheat kicks
            __instance.freeChatField.textArea.characterLimit = 120;
        }
		else
		{
            __instance.freeChatField.textArea.characterLimit = 100;
        }
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
public static class ChatController_SendChat
{
    // Postfix patch of ChatController.SendChat to unlock lower chat rate limits
    public static void Postfix(ChatController __instance)
    {
        if (!CheatToggles.lowerRateLimits) return;

		if (__instance.timeSinceLastMessage == 0f)
		{
			// Decreasing rate limit by 1 sec max still avoids anticheat kicks
			__instance.timeSinceLastMessage += 1f;
		}
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendFreeChat))]
public static class ChatController_SendFreeChat
{
    // Prefix patch of ChatController.SendFreeChat to allow sending URLs without being censored
    public static bool Prefix(ChatController __instance)
    {
		// Only works if CheatSettings.bypassUrlBlock is enabled
        if (!CheatToggles.bypassUrlBlock) return true;

        string text = __instance.freeChatField.Text;

        // Replace periods in URLs and email addresses with commas to avoid censorship
        string modifiedText = CensorUrlsAndEmails(text);

        ChatController.Logger.Debug("SendFreeChat () :: Sending message: '" + modifiedText + "'", null);
        PlayerControl.LocalPlayer.RpcSendChat(modifiedText);

        return false;
    }

    private static string CensorUrlsAndEmails(string text)
    {
        // Regular expression pattern to match URLs and email addresses
        string pattern = @"(http[s]?://)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}(/[\w-./?%&=]*)?|([a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+)";
        Regex regex = new Regex(pattern);

        // Censor periods in each match
        return regex.Replace(text, match =>
        {
            var censored = match.Value;
            censored = censored.Replace('.', ',');
            return censored;
        });
    }
}
