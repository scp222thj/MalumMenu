using AmongUs.Data.Player;
using BepInEx;
using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.StartInitialLoginFlow))]
public static class EOSManager_StartInitialLoginFlow
{
    // Prefix patch of EOSManager.StartInitialLoginFlow to automatically play with a guest account
    // when loading the game with guestMode enabled
    public static bool Prefix(EOSManager __instance)
    {
        // Always delete old guest accounts to avoid merge account popup
        __instance.DeleteDeviceID(new System.Action(__instance.EndMergeGuestAccountFlow));
        
        // Log into a new temp account if the user is playing in guest mode
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
