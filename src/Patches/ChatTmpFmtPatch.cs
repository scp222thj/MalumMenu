using System;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
internal static class ChatTmpFmtPatch
{
    private static readonly Regex BracketColor8 = new("\\[([0-9A-Fa-f]{8})\\]([^\\[]*?)\\[\\]", RegexOptions.Compiled);
    private static readonly Regex BracketColor6 = new("\\[([0-9A-Fa-f]{6})\\]([^\\[]*?)\\[\\]", RegexOptions.Compiled);

    internal static void Prefix(ref string chatText)
    {
        if (string.IsNullOrEmpty(chatText) || chatText.IndexOf('[') < 0)
            return;

        if (chatText.IndexOf("<color", StringComparison.OrdinalIgnoreCase) >= 0)
            return;

        string converted = BracketColor8.Replace(chatText, m =>
            "<color=#" + m.Groups[1].Value + ">" + m.Groups[2].Value + "</color>");
        converted = BracketColor6.Replace(converted, m =>
            "<color=#" + m.Groups[1].Value + ">" + m.Groups[2].Value + "</color>");

        chatText = converted;
    }
}
