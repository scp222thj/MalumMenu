using System;
using System.Collections.Generic;
using Assets.CoreScripts;
using System.Linq;
using HarmonyLib;
using Hazel;
using UnityEngine;
using System.Text;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
class Passive_ChatLimitPostfix
{
    static Dictionary<string, string> replaceDic = new()
    {
        { "（", " (" },
        { "）", ") " },
        { "，", ", " },
        { "：", ": " },
        { "[", "【" },
        { "]", "】" },
        { "‘", " '" },
        { "’", "' " },
        { "“", " ''" },
        { "”", "'' " },
        { "！", "! " },
        { Environment.NewLine, " " }
    };
    public static void Postfix(ChatController __instance)
    {
        if (!CheatSettings.noChatLimit) return;
        if (!__instance.freeChatField.textArea.hasFocus) return;
        __instance.timeSinceLastMessage = 3f; //This removes the delay of sending chat
        __instance.freeChatField.textArea.characterLimit = AmongUsClient.Instance.AmHost ? 999 : 300;  //Client sending long chats may cause server kick

        //Crtl + C copy from chat box
        //Crtl + V paste from keyboard
        //Crtl + X delete messages in text Area

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
            ClipboardHelper.PutClipboardString(__instance.freeChatField.textArea.text);

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
        {
            if (!string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
            {
                string replacedText = GUIUtility.systemCopyBuffer;
                foreach (var pair in replaceDic)
                {
                    replacedText = replacedText.Replace(pair.Key, pair.Value);
                }

                if ((__instance.freeChatField.textArea.text + replacedText).Length < __instance.freeChatField.textArea.characterLimit)
                    __instance.freeChatField.textArea.SetText(__instance.freeChatField.textArea.text + replacedText);
                else
                {
                    int remainingLength = __instance.freeChatField.textArea.characterLimit - __instance.freeChatField.textArea.text.Length;
                    if (remainingLength > 0)
                    {
                        string text = replacedText.Substring(0, remainingLength);
                        __instance.freeChatField.textArea.SetText(__instance.freeChatField.textArea.text + text);
                    }
                }
            }
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.X))
        {
            ClipboardHelper.PutClipboardString(__instance.freeChatField.textArea.text);
            __instance.freeChatField.textArea.SetText("");
        }
    }
}
//We are not using __instance.freeChatField.textArea.AllowPaste to do paste things bcz it sometimes does not work properly

[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.UpdateCharCount))]
internal class UpdateCharCountPatch
{
    public static void Postfix(FreeChatInputField __instance)
    {
        int length = __instance.textArea.text.Length;
        __instance.charCountText.SetText($"{length}/{__instance.textArea.characterLimit}");
        if (length < (AmongUsClient.Instance.AmHost ? 888 : 250))
            __instance.charCountText.color = Color.black;
        else if (length < (AmongUsClient.Instance.AmHost ? 999 : 300))
            __instance.charCountText.color = new Color(1f, 1f, 0f, 1f);
        else
            __instance.charCountText.color = Color.red;
    }
    //The character count does not effect actual input, it only got visual effects
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
class RpcSendChatPatch
{
    public static bool Prefix(PlayerControl __instance, string chatText, ref bool __result)
    {
        if (string.IsNullOrWhiteSpace(chatText))
        {
            __result = false;
            return false;
        }
        int return_count = PlayerControl.LocalPlayer.name.Count(x => x == '\n');
        chatText = new StringBuilder(chatText).Insert(0, "\n", return_count).ToString();
        if (AmongUsClient.Instance.AmClient && DestroyableSingleton<HudManager>.Instance)
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(__instance, chatText);
        if (chatText.Contains("who", StringComparison.OrdinalIgnoreCase))
            DestroyableSingleton<UnityTelemetry>.Instance.SendWho();
        MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(__instance.NetId, (byte)RpcCalls.SendChat, SendOption.None);
        messageWriter.Write(chatText);
        messageWriter.EndMessage();
        __result = true;
        return false;
    }
}
//Patch rpcsendchat to send with none
//chats longer than around 120 characters with reliable will enjoy fresh kick by anticheat

//From TOHE https://github.com/0xDrMoe/TownofHost-Enhanced/blob/main/Patches/ChatControlPatch.cs
