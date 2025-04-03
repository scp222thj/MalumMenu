using AmongUs.Data.Player;
using BepInEx;
using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.StartInitialLoginFlow))]
public static class EOSManager_StartInitialLoginFlow
{
    public static bool Prefix(EOSManager __instance)
    {
        if (!MalumMenu.guestMode.Value)
            return true;

        // Only delete Device ID and start guest login if login hasn't completed yet
        if (!__instance.loginFlowFinished)
        {
            __instance.DeleteDeviceID(new System.Action(__instance.EndMergeGuestAccountFlow));
            __instance.StartTempAccountFlow();
        }

        __instance.CloseStartupWaitScreen();
        return false; // Skip original method
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFreechatAllowed))]
public static class EOSManager_IsFreechatAllowed
{
    // Allows free chat when unlockFeatures is enabled
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures)
        {
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFriendsListAllowed))]
public static class EOSManager_IsFriendsListAllowed
{
    // Allows friend list access when unlockFeatures is enabled
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures)
        {
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsAllowedOnline))]
public static class EOSManager_IsAllowedOnline
{
    // Overrides online restriction if unlockFeatures is enabled
    public static void Prefix(ref bool canOnline)
    {
        if (CheatToggles.unlockFeatures)
        {
            canOnline = true;
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsMinorOrWaiting))]
public static class EOSManager_IsMinorOrWaiting
{
    // Removes minor account limitation if unlockFeatures is enabled
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures)
        {
            __result = false;
        }
    }
}
