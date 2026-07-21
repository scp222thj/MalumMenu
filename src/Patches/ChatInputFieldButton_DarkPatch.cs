using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatInputFieldButton), nameof(ChatInputFieldButton.SetButtonEnabled))]
public static class ChatInputFieldButton_DarkPatch
{
    public static void Postfix(ChatInputFieldButton __instance)
    {
        if (!CheatToggles.chatDarkMode) return;

        var img = __instance.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.color = ChatDarkModeService.DarkSubmitButton;
            return;
        }

        var sr = __instance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = ChatDarkModeService.DarkSubmitButton;
        }
    }
}
