using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
public static class TextBoxTMP_SetText_DarkPatch
{
    public static void Postfix(TextBoxTMP __instance)
    {
        if (!CheatToggles.chatDarkMode) return;

        ChatDarkModeService.ApplyTextBoxIfChatOwned();
    }
}
