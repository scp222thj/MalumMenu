using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChatWarning))]
public static class ChatController_AddChatWarning_DarkPatch
{
    public static void Postfix(ChatController __instance)
    {
        if (!CheatToggles.chatDarkMode) return;

        ChatDarkModeService.SetOwner(__instance);
        ChatDarkModeService.ApplyAll();
    }
}
