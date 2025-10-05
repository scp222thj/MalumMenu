using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Update))]
public static class TextBoxTMP_Update
{
    // Postfix patch of TextBoxTMP.Update to allow copying from the chatbox
    public static void Postfix(TextBoxTMP __instance)
    {
        if (CheatToggles.chatJailbreak)
        { 
            if (!__instance.hasFocus){return;}

            // If the user is pressing Ctrl + C, copy the text from the chatbox to the device's clipboard
            if((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
            {
                ClipboardHelper.PutClipboardString(__instance.text);
            }
        }
    }
}

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
public static class TextBoxTMP_IsCharAllowed
{
    // Prefix patch of TextBoxTMP.IsCharAllowed to allow all characters
    public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
    {
        if (CheatToggles.chatJailbreak)
        {
            HashSet<char> blockedSymbols = new() { '\b', '\r' };

            if (blockedSymbols.Contains(i))
            {
                __result = false;
                return false;
            }

            __result = true;
            return false;
        }

        return true;
    }
}
