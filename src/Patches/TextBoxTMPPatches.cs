using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

// Allow copying from the chatbox
[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Update))]
public static class TextBoxTMP_Update
{
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
