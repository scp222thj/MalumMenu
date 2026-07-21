using System.Reflection;
using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
public static class ChatBubble_SetText_DarkPatch
{
    private static readonly FieldInfo _playerInfoField = typeof(ChatBubble)
        .GetField("playerInfo", BindingFlags.NonPublic | BindingFlags.Instance);

    public static void Postfix(ChatBubble __instance)
    {
        if (!CheatToggles.chatDarkMode) return;

        var playerInfo = _playerInfoField?.GetValue(__instance) as NetworkedPlayerInfo;
        bool isSelf = playerInfo != null && PlayerControl.LocalPlayer != null
            && playerInfo.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        ChatDarkModeService.ApplyToPanel(__instance.Background, true, isSelf);

        if (__instance.NameText != null)
        {
            ChatDarkModeService.ApplyToText(__instance.NameText);
        }
    }
}
