using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class SetAllName_PlayerControl_RpcSendChat_Prefix
{
    // Prefix patch of PlayerControl.RpcSendChat to set a custom name to LocalPlayer
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (!CheatToggles.setNameAll)
        {
            return true; //Only works if CheatToggles.setName is enabled
        }

        foreach (var sender in PlayerControl.AllPlayerControls)
        {
            Utils.SetName(sender, chatText);
        }

        CheatToggles.setNameAll = false;

        return false;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class SetNameAll_PlayerControl_LateUpdate_Postfix
{
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.setNameAll){
            if (!isActive){
                CheatToggles.chatMimic = CheatToggles.spamChat = CheatToggles.setPlayerName = false;

                Utils.OpenChat();

                isActive = true;
            }
        }else{
            if (isActive){
                if (!CheatToggles.chatMimic && !CheatToggles.spamChat && !CheatToggles.setPlayerName){
                    Utils.CloseChat();
                }

                isActive = false;
            }
        }
    }
}