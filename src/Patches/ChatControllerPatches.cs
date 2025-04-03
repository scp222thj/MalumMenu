using HarmonyLib;
using System;
using UnityEngine;
using System.Text.RegularExpressions;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class ChatController_AddChat
{
    public static bool Prefix(PlayerControl sourcePlayer, string chatText, bool censor, ChatController __instance)
    {
        if (!CheatToggles.seeGhosts || PlayerControl.LocalPlayer.Data.IsDead)
            return true;

        if (!sourcePlayer || !PlayerControl.LocalPlayer)
            return true;

        NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
        NetworkedPlayerInfo data2 = sourcePlayer.Data;

        if (data2 == null || data == null)
            return true;

        ChatBubble pooledBubble = __instance.GetPooledBubble();

        try
        {
            pooledBubble.transform.SetParent(__instance.scroller.Inner);
            pooledBubble.transform.localScale = Vector3.one;

            bool isLocal = sourcePlayer == PlayerControl.LocalPlayer;
            if (isLocal)
                pooledBubble.SetRight();
            else
                pooledBubble.SetLeft();

            bool didVote = MeetingHud.Instance?.DidVote(sourcePlayer.PlayerId) ?? false;

            pooledBubble.SetCosmetics(data2);
            __instance.SetChatBubbleName(pooledBubble, data2, data2.IsDead, didVote, PlayerNameColor.Get(data2), null);

            if (censor && AmongUs.Data.DataManager.Settings.Multiplayer.CensorChat)
                chatText = BlockedWords.CensorWords(chatText, false);

            pooledBubble.SetText(chatText);
            pooledBubble.AlignChildren();
            __instance.AlignAllBubbles();

            if (!__instance.IsOpenOrOpening && __instance.notificationRoutine == null)
                __instance.notificationRoutine = __instance.StartCoroutine(__instance.BounceDot());

            if (!isLocal)
                SoundManager.Instance.PlaySound(__instance.messageSound, false, 1f, null).pitch = 0.5f + (float)sourcePlayer.PlayerId / 15f;
        }
        catch (Exception message)
        {
            ChatController.Logger.Error(message.ToString(), null);
            __instance.chatBubblePool.Reclaim(pooledBubble);
        }

        return false;
    }
}

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class ChatBubble_SetName
{
    public static void Postfix(ChatBubble __instance)
    {
        MalumESP.chatNametags(__instance);
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatController_Update
{
    private static bool hasLogged = false;

    public static void Postfix(ChatController __instance)
    {
        TextBoxTMP textArea = __instance?.freeChatField?.textArea;
        if (textArea == null)
        {
            Debug.LogWarning("[MalumMenu] textArea is null. Cannot apply jailbreak.");
            return;
        }

        textArea.allowAllCharacters = CheatToggles.chatJailbreak;
        textArea.AllowSymbols = true;
        textArea.AllowEmail = true;
        textArea.AllowPaste = true;
        textArea.ForceUppercase = false;
        textArea.characterLimit = CheatToggles.chatJailbreak ? 119 : 100;

        if (Debug.isDebugBuild || !hasLogged)
        {
            hasLogged = true;
            Debug.Log("[MalumMenu] Jailbreak flags applied to TextBoxTMP:");
            Debug.Log($"  allowAllCharacters = {textArea.allowAllCharacters}");
            Debug.Log($"  AllowSymbols = {textArea.AllowSymbols}");
            Debug.Log($"  AllowEmail = {textArea.AllowEmail}");
            Debug.Log($"  ForceUppercase = {textArea.ForceUppercase}");
            Debug.Log($"  characterLimit = {textArea.characterLimit}");
        }
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendFreeChat))]
public static class ChatController_SendFreeChat
{
    public static bool Prefix(ChatController __instance)
    {
        if (!CheatToggles.chatJailbreak)
            return true;

        string text = __instance.freeChatField?.textArea?.text;
        if (string.IsNullOrEmpty(text))
            return false;

        string modifiedText = CensorUrlsAndEmails(text);
        ChatController.Logger.Debug("SendFreeChat () :: Sending message: '" + modifiedText + "'", null);
        PlayerControl.LocalPlayer.RpcSendChat(modifiedText);

        return false;
    }

    private static string CensorUrlsAndEmails(string text)
    {
        string pattern = @"(http[s]?://)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}(/[\w\-./?%&=]*)?|([a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+)";
        Regex regex = new Regex(pattern);

        return regex.Replace(text, match =>
        {
            return match.Value.Replace('.', ',');
        });
    }
}
