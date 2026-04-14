using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace FabMenu
{
    [HarmonyPatch(typeof(ChatBubble), "SetName")]
    public static class ChatBubble_SetName
    {
        public static void Postfix(ChatBubble __instance)
        {
            if (!CheatToggles.chatDarkMode)
            {
                return;
            }

            try
            {
                if (__instance.Background != null)
                {
                    __instance.Background.color = new Color(0.08f, 0.08f, 0.08f, 0.95f);
                }

                if (__instance.NameText != null)
                {
                    ((Graphic)__instance.NameText).color = Color.white;
                }

                if (__instance.TextArea != null)
                {
                    ((Graphic)__instance.TextArea).color = Color.white;
                }
            }
            catch
            {
            }
        }
    }
}
