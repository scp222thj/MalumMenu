using HarmonyLib;
using System;
using UnityEngine;
using System.Text.RegularExpressions;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class ChatController_AddChat
{
	// Prefix patch of ChatController.AddChat to receive ghost messages if CheatSettings.seeGhosts is enabled even if LocalPlayer is alive
	// Basically does what the original method did with the required modifications
    public static bool Prefix(PlayerControl sourcePlayer, string chatText, bool censor, ChatController __instance)
    {
        if (!CheatToggles.seeGhosts || PlayerControl.LocalPlayer.Data.IsDead){
            return true; // Simply run original method if seeGhosts is disabled or LocalPlayer already dead
        }

        if (!sourcePlayer || !PlayerControl.LocalPlayer)
		{
			return true;
		}

		NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
		NetworkedPlayerInfo data2 = sourcePlayer.Data;

		if (data2 == null || data == null) // Remove isDead check for LocalPlayer
		{
			return true;
		}

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
			if (!flag)
			{
				SoundManager.Instance.PlaySound(__instance.messageSound, false, 1f, null).pitch = 0.5f + (float)sourcePlayer.PlayerId / 15f;
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

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class ChatBubble_SetName
{
    public static void Postfix(ChatBubble __instance){
        MalumESP.chatNametags(__instance);
    }
}


[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatController_Update
{
    // Postfix patch of FreeChatInputField.OnFieldChanged to unlock extra chat capabilities
    public static void Postfix(ChatController __instance)
    {
        __instance.freeChatField.textArea.allowAllCharacters = CheatToggles.chatJailbreak; // Not really used by the game's code, but I include it anyway
        __instance.freeChatField.textArea.AllowSymbols = true; // Allow sending certain symbols
        __instance.freeChatField.textArea.AllowEmail = CheatToggles.chatJailbreak; // Allow sending email addresses when chatJailbreak is enabled
        //__instance.freeChatField.textArea.AllowPaste = CheatToggles.chatJailbreak; // Allow pasting from clipboard in chat when chatJailbreak is enabled
        
        if (CheatToggles.chatJailbreak){
            __instance.freeChatField.textArea.characterLimit = 119; // Longer message length when chatJailbreak is enabled
        }else{
            __instance.freeChatField.textArea.characterLimit = 100;
        }
        
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendFreeChat))]
public static class ChatController_SendFreeChat
{
    // Prefix patch of ChatController.SendFreeChat to unlock extra chat capabilities
    public static bool Prefix(ChatController __instance)
    {
        if (!CheatToggles.chatJailbreak){
            return true; // Only works if CheatSettings.chatJailbreak is enabled
        }

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