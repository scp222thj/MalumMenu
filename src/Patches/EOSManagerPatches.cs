using AmongUs.Data;
using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.StartInitialLoginFlow))]
public static class EOSManager_StartInitialLoginFlow
{
    public static bool Prefix(EOSManager __instance)
    {
        __instance.DeleteDeviceID(new System.Action(__instance.EndMergeGuestAccountFlow));
        if (!MalumMenu.guestMode.Value) return true;
        __instance.StartTempAccountFlow();
        __instance.CloseStartupWaitScreen();
        return false;
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFreechatAllowed))]
public static class EOSManager_IsFreechatAllowed
{
    //Prefix patch of EOSManager.IsFreechatAllowed to unlock freechat
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures){
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFriendsListAllowed))]
public static class EOSManager_IsFriendsListAllowed
{
    //Prefix patch of EOSManager.IsFriendsListAllowed to unlock friend list
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures){
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsAllowedOnline))]
public static class EOSManager_IsAllowedOnline
{
    //Prefix patch of AccountManager.CanPlayOnline to allow online games
    public static void Prefix(ref bool canOnline)
    {
        if (CheatToggles.unlockFeatures){
            canOnline = true; 
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsMinorOrWaiting))]
public static class EOSManager_IsMinorOrWaiting
{
    //Prefix patch of EOSManager.IsMinorOrWaiting to remove minor status
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures){
            __result = false;
        }
    }
}
