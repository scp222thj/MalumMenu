using HarmonyLib;
using Hazel;
using System.Text.RegularExpressions;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class ChatJailBreak_PlayerControl_RpcSendChat_Prefix
{
    //Prefix patch of PlayerControl.RpcSendChat to unlock extra chat capabilities
    [HarmonyPriority(Priority.VeryHigh)]
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (!CheatToggles.chatJailbreak || CheatToggles.chatMimic || CheatToggles.setNameAll || CheatToggles.spamChat || CheatToggles.setName){
            return true; //Only works if CheatSettings.chatJailbreak is enabled & CheatSettings.chatMimic is disabled
        }

        //Removal of some checks for special characters and whitespaces

		if (AmongUsClient.Instance.AmClient && DestroyableSingleton<HudManager>.Instance)
		{
			DestroyableSingleton<HudManager>.Instance.Chat.AddChat(__instance, chatText, true);
		}

        //Removal of UnityTelemetry.SendWho()

        //Modded version of the original RPC call that lets users bypass the character limit and the message ratelimit
		MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)RpcCalls.SendChat, SendOption.None, -1);
        writer.Write(chatText);
        AmongUsClient.Instance.FinishRpcImmediately(writer);

		return false;
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
public static class ChatJailBreak_ChatController_SendChat_Prefix
{
    //Prefix patch of ChatController.SendChat to unlock extra chat capabilities
    public static bool Prefix(ChatController __instance)
    {
        if (!CheatToggles.chatJailbreak){
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
public static class ChatJailBreak_ChatController_SendFreeChat_Prefix
{
    //Prefix patch of ChatController.SendFreeChat to unlock extra chat capabilities
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


[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.OnFieldChanged))]
public static class ChatJailBreak_FreeChatInputField_OnFieldChanged_Prefix
{
    //Postfix patch of FreeChatInputField.OnFieldChanged to unlock extra chat capabilities
    public static void Postfix(FreeChatInputField __instance)
    {
        __instance.textArea.allowAllCharacters = CheatToggles.chatJailbreak; //Not really used by the game's code, but I include it anyway
        __instance.textArea.AllowPaste = CheatToggles.chatJailbreak; //Allow pasting from clipboard in chat when chatJailbreak is enabled
        __instance.textArea.AllowSymbols = true; //Allow sending certain symbols
        __instance.textArea.AllowEmail = CheatToggles.chatJailbreak; //Allow sending email addresses when chatJailbreak is enabled
        
        if (CheatToggles.chatJailbreak){
            __instance.textArea.characterLimit = int.MaxValue; //Longer message length when chatJailbreak is enabled
        }else{
            __instance.textArea.characterLimit = 100;
        }
        
    }
}


[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
public static class ChatJailBreak_TextBoxTMP_IsCharAllowed_Prefix
{
    //Postfix patch of TextBoxTMP.IsCharAllowed to allow all characters
    public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
    {
        if (!CheatToggles.chatJailbreak){
            return true;
        }
        
        __result = !(i == '\b'); //Bugfix: '\b' messing with chat message
        return false;
    }
}


// Updated charCount when using cheat
[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.UpdateCharCount))]
public static class ChatJailBreak_FreeChatInputField_UpdateCharCount_Postfix
{
    public static void Postfix(FreeChatInputField __instance)
    {
        if (!CheatToggles.chatJailbreak){
            return; //Only works if CheatToggles.chatJailbreak is enabled
        }

        //Update charCountText to account for longer characterLimit
        int length = __instance.textArea.text.Length;
        __instance.charCountText.SetText($"{length}/{__instance.textArea.characterLimit}");

        //Update charCountText.color to account for longer characterLimit
        if (length < 1610612735) //Under 75%
            __instance.charCountText.color = UnityEngine.Color.black;
        else if (length < 2147483647) //Under 100%
            __instance.charCountText.color = new UnityEngine.Color(1f, 1f, 0f, 1f);
        else //Over or equal to 100%
            __instance.charCountText.color = UnityEngine.Color.red;
    }
}