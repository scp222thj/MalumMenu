using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetCosmetics))]
public static class ChatBubble_SetCosmetics_DarkPatch
{
    public static void Postfix(ChatBubble __instance, NetworkedPlayerInfo playerInfo)
    {
        if (!CheatToggles.chatDarkMode) return;

        bool isSelf = playerInfo != null && PlayerControl.LocalPlayer != null
            && playerInfo.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        ChatDarkModeService.ApplyToPanel(__instance.Background, true, isSelf);

        if (__instance.NameText != null)
        {
            ChatDarkModeService.ApplyToText(__instance.NameText);
        }
    }
}
