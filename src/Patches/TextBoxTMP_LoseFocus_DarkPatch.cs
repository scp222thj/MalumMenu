using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.LoseFocus))]
public static class TextBoxTMP_LoseFocus_DarkPatch
{
    public static void Postfix(TextBoxTMP __instance)
    {
        if (!CheatToggles.chatDarkMode) return;

        ChatDarkModeService.ApplyTextBoxIfChatOwned();
    }
}
