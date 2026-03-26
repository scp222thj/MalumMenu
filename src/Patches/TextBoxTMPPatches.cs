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
        if (!CheatToggles.unlockClipboard || !__instance.hasFocus) return;

        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            GUIUtility.systemCopyBuffer = __instance.text;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Utils.isPastingInput = true;

            __instance.SetText(__instance.text + GUIUtility.systemCopyBuffer);

            Utils.isPastingInput = false;
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
    private static int _currentCharPos = 0;

    // Prefix patch of TextBoxTMP.IsCharAllowed to unlock extra characters
    public static bool Prefix(TextBoxTMP __instance, ref bool __result)
    {
        // If user is writing through IME composition, then always allow the inputted characters
        // Fixes issues for users of CJK languages

        string compositionString = Input.compositionString;
        if (compositionString.Length > 0)
        {
            __result = true;
            return false;
        }

        // If the user pasted text, read from clipboard. Otherwise use typed input
        var input = Utils.isPastingInput ? GUIUtility.systemCopyBuffer : Input.inputString;

        // Allow all characters if there is no user input, as validation is not needed then
        if (input.Length == 0)
        {
            __result = true;
            return false;
        }

        // Reconstruct the full string being processed by TextBoxTMP.SetText

        string currentText = __instance.text ?? string.Empty;

        int caretPos = Mathf.Clamp(__instance.caretPos, 0, currentText.Length);

        string text = currentText.Insert(caretPos, input);

        // Get character that is currently being checked by keeping track
        // of each TextBoxTMP.IsCharAllowed call made within TextBoxTMP.SetText foreach loop

        _currentCharPos = Mathf.Clamp(_currentCharPos, 0, text.Length - 1);

        char currentChar = text[_currentCharPos];

        if (_currentCharPos >= text.Length - 1)
        {
            _currentCharPos = 0; // Reset position when loop finishes
        }
        else
        {
            _currentCharPos++; // Increment position to next character in loop
        }

        if (CheatToggles.unlockCharacters)
        {
            // Blocked characters to avoid breaking text input / getting kicked by anticheat
            HashSet<char> blockedSymbols = new() { '\b', '\r', '>', '<', '[' };

            if (blockedSymbols.Contains(currentChar))
            {
                __result = false;
                return false;
            }

            __result = true;
        }
        else // Normal IsCharAllowed logic
        {
            if (__instance.IpMode)
            {
                __result = (currentChar >= '0' && currentChar <= '9') || currentChar == '.';
                return false;
            }

            __result = currentChar == ' ' ||
            (currentChar >= 'A' && currentChar <= 'Z') ||
            (currentChar >= 'a' && currentChar <= 'z') ||
            (currentChar >= '0' && currentChar <= '9') ||
            (currentChar >= 'À' && currentChar <= 'ÿ') ||
            (currentChar >= 'Ѐ' && currentChar <= 'џ') ||
            (currentChar >= '぀' && currentChar <= '㆟') ||
            (currentChar >= 'ⱡ' && currentChar <= '힣') ||
            (__instance.AllowSymbols && TextBoxTMP.SymbolChars.Contains(currentChar)) ||
            (__instance.AllowEmail && TextBoxTMP.EmailChars.Contains(currentChar));
        }

        return false;
    }
}
