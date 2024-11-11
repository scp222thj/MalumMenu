using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyLib.HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
class Pastepatch
{
    //Allows to copy/paste/delete the text
    static void Postfix()
    {
        if (DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.V))
                { 
                    DestroyableSingleton<HudManager>.Instance.Chat.freeChatField.textArea.SetText(DestroyableSingleton<HudManager>.Instance.Chat.freeChatField.textArea.text + GUIUtility.systemCopyBuffer); 
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    GUIUtility.systemCopyBuffer = DestroyableSingleton<HudManager>.Instance.Chat.freeChatField.textArea.text;
                    DestroyableSingleton<HudManager>.Instance.Chat.freeChatField.textArea.Clear();
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    GUIUtility.systemCopyBuffer = DestroyableSingleton<HudManager>.Instance.Chat.freeChatField.textArea.text;
                }
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

[HarmonyPatch(typeof(BanMenu), nameof(BanMenu.Select))]
class BanMenuSelectPatch
{
    //Allows to ban/kick a player during the game
    public static void Postfix(BanMenu __instance, int clientId)
    {
        InnerNet.ClientData recentClient = AmongUsClient.Instance.GetRecentClient(clientId);
        if (recentClient == null) return;
        __instance.BanButton.GetComponent<ButtonRolloverHandler>().SetEnabledColors();
    }
}
