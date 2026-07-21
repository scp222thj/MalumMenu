using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.OnEnable))]
public static class ChatController_OnEnable_DarkPatch
{
    public static void Postfix(ChatController __instance)
    {
        if (!CheatToggles.chatDarkMode) return;

        ChatDarkModeService.SetOwner(__instance);

        bool hasManager = false;
        foreach (var comp in __instance.GetComponents<Component>())
        {
            if (comp != null && comp.GetIl2CppType().FullName == "MalumMenu.ChatDarkModeManager")
            {
                hasManager = true;
                break;
            }
        }

        if (!hasManager)
        {
            ChatDarkModeManager.Create(__instance);
        }
    }
}
