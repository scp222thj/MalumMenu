using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.GiveFocus))]
public static class TextBoxTMP_GiveFocus_DarkPatch
{
    public static void Postfix(TextBoxTMP __instance)
    {
        if (!CheatToggles.chatDarkMode) return;

        ChatDarkModeService.ApplyTextBoxIfChatOwned();
    }
}
