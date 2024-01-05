using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class SetName_PlayerControl_RpcSendChat_Prefix
{
    // Prefix patch of PlayerControl.RpcSendChat to set a custom name to LocalPlayer
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (!CheatToggles.setName)
        {
            return true; //Only works if CheatToggles.setName is enabled
        }

        Utils.SetName(PlayerControl.LocalPlayer, chatText);

        CheatToggles.setName = false;

        return false;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class SetName_PlayerControl_LateUpdate_Postfix
{
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.setName){
            if (!isActive){
                CheatToggles.chatMimic = CheatToggles.spamChat = CheatToggles.setNameAll = false;
                isActive = true;
            }
        }else{
            isActive = false;
        }
    }
}
