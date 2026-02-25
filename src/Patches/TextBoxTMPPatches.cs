using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Update))]
public static class TextBoxTMP_Update
{
    // Postfix patch of TextBoxTMP.Update to allow copying, pasting and cutting text between the chatbox and the device's clipboard
    public static void Postfix(TextBoxTMP __instance)
    {
        if (!CheatToggles.chatJailbreak || !__instance.hasFocus) return;

        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            GUIUtility.systemCopyBuffer = __instance.text;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            __instance.SetText(__instance.text + GUIUtility.systemCopyBuffer);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GUIUtility.systemCopyBuffer = __instance.text;
            __instance.SetText("");
        }
    }
}

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
public static class TextBoxTMP_IsCharAllowed
{
    // Prefix patch of TextBoxTMP.IsCharAllowed to allow all characters
    public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
    {
        if (!CheatToggles.chatJailbreak) return true;

        HashSet<char> blockedSymbols = new() { '\b', '\r' };

        if (blockedSymbols.Contains(i))
        {
            __result = false;
            return false;
        }

        __result = true;
        return false;
    }
}
