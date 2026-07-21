using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
public static class ChatController_Awake_DarkPatch
{
    public static void Postfix(ChatController __instance)
    {
        if (!CheatToggles.chatDarkMode) return;

        ChatDarkModeService.SetOwner(__instance);
        ChatDarkModeManager.Create(__instance);
    }
}
