using HarmonyLib;
using Hazel;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class ChatJailbreak_RpcSendChatPostfix
{
    //Prefix patch of PlayerControl.RpcSendChat to unlock extra chat capabilities
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (!CheatSettings.chatJailbreak || CheatSettings.chatMimic){
            return true; //Only works if CheatSettings.chatJailbreak is enabled
        }

        //Removal of some checks for special characters and whitespaces

		if (AmongUsClient.Instance.AmClient && DestroyableSingleton<HudManager>.Instance)
		{
			DestroyableSingleton<HudManager>.Instance.Chat.AddChat(__instance, chatText, true);
		}

        //Removal of UnityTelemetry.SendWho()
        
        if(CheatSettings.spamChat){

            RPC_SpamTextPostfix.spamText = chatText;
            return true;

        }

        //Modded version of the original RPC call that lets users bypass the character limit and the message ratelimit
		MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)RpcCalls.SendChat, SendOption.None, -1);
        writer.Write(chatText);
        AmongUsClient.Instance.FinishRpcImmediately(writer);

		return false;
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
public static class ChatJailbreak_SendChatPostfix
{
    //Prefix patch of ChatController.SendChat to unlock extra chat capabilities
    public static bool Prefix(ChatController __instance)
    {
        if (!CheatSettings.chatJailbreak){
            return true; //Only works if CheatSettings.chatJailbreak is enabled
        }

        //Removal of message rate-limit

        if (__instance.quickChatMenu.CanSend)
        {
            __instance.SendQuickChat();
        }
        else
        {
            if (__instance.quickChatMenu.IsOpen) // Removal of some other message-related checks
            {
                return false;
            }
            __instance.SendFreeChat();
        }
        __instance.timeSinceLastMessage = 0f;
        __instance.freeChatField.Clear();
        __instance.quickChatMenu.Clear();
        __instance.quickChatField.Clear();
        __instance.UpdateChatMode();

		return false;
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendFreeChat))]
public static class ChatJailbreak_SendFreeChatPostfix
{
    //Prefix patch of ChatController.SendFreeChat to unlock extra chat capabilities
    public static bool Prefix(ChatController __instance)
    {
        if (!CheatSettings.chatJailbreak){
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


[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.OnFieldChanged))]
public static class ChatJailbreak_FreeChatInputField_UpdateStatePostfix
{
    //Postfix patch of FreeChatInputField.UpdateState to unlock extra chat capabilities
    public static void Postfix(FreeChatInputField __instance)
    {
        __instance.textArea.allowAllCharacters = CheatSettings.chatJailbreak; //Not really used by the game's code, but I include it anyway
        __instance.textArea.AllowPaste = CheatSettings.chatJailbreak; //Allow pasting from clipboard in chat when chatJailbreak is enabled
        __instance.textArea.AllowSymbols = true; //Allow sending certain symbols
        __instance.textArea.AllowEmail = CheatSettings.chatJailbreak; //Allow sending email addresses when chatJailbreak is enabled
        
        if (CheatSettings.chatJailbreak){
            __instance.textArea.characterLimit = int.MaxValue; //Unlimited message length when chatJailbreak is enabled
        }else{
            __instance.textArea.characterLimit = 100;
        }
        
    }
}


[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
public static class ChatJailbreak_IsCharAllowedPostfix
{
    //Postfix patch of TextBoxTMP.IsCharAllowed to allow all characters
    public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
    {
        if (!CheatSettings.chatJailbreak){
            return true;
        }
        
        __result = !(i == '\b'); //Bugfix: '\b' messing with chat message
        return false;
    }
}