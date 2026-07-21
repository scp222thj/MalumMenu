using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class ChatController_AddChat_DarkPatch
{
    public static void Postfix(ChatController __instance)
    {
        if (!CheatToggles.chatDarkMode) return;

        if (HudManager.InstanceExists && HudManager.Instance.Chat != null)
        {
            ChatDarkModeService.SetOwner(HudManager.Instance.Chat);
        }

        ChatDarkModeManager existing = null;
        foreach (var comp in __instance.GetComponents<Component>())
        {
            if (comp != null && comp.GetIl2CppType().FullName == "MalumMenu.ChatDarkModeManager")
            {
                existing = comp.TryCast<ChatDarkModeManager>();
                break;
            }
        }

        if (existing == null)
        {
            ChatDarkModeManager.Create(__instance);
        }

        ChatDarkModeService.ApplyAll();
    }
}
