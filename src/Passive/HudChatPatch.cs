using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class Passive_HudChatPostfix
{
    //Postfix patch of HudManager.Update to enable the chat icon in-game
    //if CheatSettings.alwaysChat is turned on
    public static void Postfix(HudManager __instance){
        __instance.Chat.gameObject.SetActive(CheatSettings.alwaysChat);
    }
}