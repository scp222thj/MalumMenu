using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Update))]
public static class TextBoxTMP_Update
{
    /// <summary>
    /// Postfix patch of TextBoxTMP.Update to allow copying from the chatbox
    /// </summary>
    /// <param name="__instance">The <c>TextBoxTMP</c> instance.</param>
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
    /// <summary>
    /// Prefix patch of TextBoxTMP.IsCharAllowed to allow all characters
    /// </summary>
    /// <param name="__instance">The <c>TextBoxTMP</c> instance.</param>
    /// <param name="i">The character to be checked.</param>
    /// <param name="__result">Original return value of <c>IsCharAllowed</c>.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
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
