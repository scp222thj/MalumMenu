using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class SeeRoles_ChatBubble_SetName_Postfix
{
    //Postfix patch of ChatBubble.SetName to get colored names in chat messages
    public static void Postfix(ChatBubble __instance){
        MalumESP.chatNametags(__instance);
    }
}    