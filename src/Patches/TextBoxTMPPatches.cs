using HarmonyLib;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;

namespace MalumMenu
{
    // Postfix patch to allow copying chat text via Ctrl+C
    [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Update))]
    public static class TextBoxTMP_Update
    {
        public static void Postfix(TextBoxTMP __instance)
        {
            // Only execute if chat jailbreak is enabled and the text box has focus
            if (!CheatToggles.chatJailbreak || !__instance.hasFocus) return;

            // Handle Ctrl+C keypress to copy text to clipboard
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
            {
                ClipboardHelper.PutClipboardString(__instance.text);
            }
        }
    }

    // Prefix patch to allow almost all characters during chat input
[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
    public static class TextBoxTMP_IsCharAllowed
    {
        // Prefix patch to allow specific Cyrillic characters mapped from US layout
        public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
        {
            // Get the current system keyboard layout
            string currentLayout = CultureInfo.CurrentCulture.Name;

            // Check if we are in jailbreak mode
            if (CheatToggles.chatJailbreak)
            {
                // Define problematic English symbols to block
                HashSet<char> blockedSymbols = new HashSet<char> { '\b', '\r', '>', '<', '[', ']' };

                // Define Cyrillic equivalents of the blocked symbols
                HashSet<char> allowedCyrillicSymbols = new HashSet<char> { 'Б', 'Ю', 'х', 'ъ' };

                // If the current keyboard layout is US English or another layout that uses these symbols
                if (currentLayout.StartsWith("en"))
                {
                    // Block the specific symbols if we are in the US layout or similar
                    if (blockedSymbols.Contains(i))
                    {
                        // Allow the Cyrillic equivalent of the blocked symbols
                        if (allowedCyrillicSymbols.Contains(i))
                        {
                            __result = true; // Allow the Cyrillic character
                            return false; // Skip the original IsCharAllowed method
                        }
                        else
                        {
                            __result = false; // Block the symbol
                            return false; // Skip the original IsCharAllowed method
                        }
                    }
                }

                // Allow all other characters in Jailbreak mode
                __result = true;
                return false; // Skip the original IsCharAllowed method
            }

            // If not in Jailbreak mode, continue with the original method logic
            return true;
        }
    }
}
