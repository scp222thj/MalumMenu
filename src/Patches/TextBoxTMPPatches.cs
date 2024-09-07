using HarmonyLib;
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
    // Postfix patch of TextBoxTMP.IsCharAllowed to allow all characters
    public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
    {
        if (!CheatToggles.chatJailbreak){
            return true;
        }

        __result = !(i == '\b' || i == '>' || i == '<' || i == ']' || i == '[' || i == '\r'); // Some characters cause issues and must therefore be removed
        return false;
    }
}