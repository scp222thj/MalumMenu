using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
internal static class ChatColorLocalPatch
{
    internal static void Prefix(PlayerControl sourcePlayer, ref string chatText)
    {
        if (string.IsNullOrEmpty(chatText))
            return;

        if (MiscConfig.ChatColorEnabled == null || !MiscConfig.ChatColorEnabled.Value)
            return;

        PlayerControl local = PlayerControl.LocalPlayer;
        if (local == null || sourcePlayer == null)
            return;

        if (sourcePlayer.PlayerId != local.PlayerId)
            return;

        if (chatText.Length >= 8 && chatText.StartsWith("<color=#"))
            return;

        string hex = ValidateHex(MiscConfig.ChatColorHex?.Value);
        if (hex == null)
            return;

        chatText = "<color=#" + hex + ">" + chatText + "</color>";
    }

    private static string ValidateHex(string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return null;

        if (hex[0] == '#')
            hex = hex[1..];

        if (hex.Length != 6)
            return null;

        for (int i = 0; i < 6; i++)
        {
            char c = hex[i];
            if ((c < '0' || c > '9') && (c < 'a' || c > 'f') && (c < 'A' || c > 'F'))
                return null;
        }

        return hex.ToUpperInvariant();
    }
}
