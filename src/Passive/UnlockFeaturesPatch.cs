using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFreechatAllowed))]
public static class UnlockFeatures_EOSManager_IsFreechatAllowed_Prefix
{
    //Prefix patch of EOSManager.IsFreechatAllowed to unlock freechat
    public static bool Prefix(EOSManager __instance, ref bool __result)
    {
        if (!CheatToggles.unlockFeatures)
        {
            return true; // Only works if CheatSettings.unlockFeatures is enabled
        }

        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFriendsListAllowed))]
public static class UnlockFeatures_EOSManager_IsFriendsListAllowed_Prefix
{
    // Prefix patch of EOSManager.IsFriendsListAllowed to unlock friend list
    public static bool Prefix(EOSManager __instance, ref bool __result)
    {
        if (!CheatToggles.unlockFeatures)
        {
            return true; // Only works if CheatSettings.unlockFeatures is enabled
        }

        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsMinorOrWaiting))]
public static class UnlockFeatures_EOSManager_IsMinorOrWaiting_Prefix
{
    // Prefix patch of EOSManager.IsMinorOrWaiting to remove minor status
    public static bool Prefix(EOSManager __instance, ref bool __result)
    {
        if (!CheatToggles.unlockFeatures)
        {
            return true; // Only works if CheatSettings.unlockFeatures is enabled
        }

        __result = false;
        return false;
    }
}

[HarmonyPatch(typeof(FullAccount), nameof(FullAccount.CanSetCustomName))]
public static class UnlockFeatures_EOSManager_CanSetCustomName_Prefix
{
    // Prefix patch of FullAccount.CanSetCustomName to allow the usage of custom names
    public static void Prefix(ref bool canSetName)
    {
        if (CheatToggles.unlockFeatures)
        { // Only works if CheatSettings.unlockFeatures is enabled
            canSetName = true;
        }
    }
}

[HarmonyPatch(typeof(AccountManager), nameof(AccountManager.CanPlayOnline))]
public static class UnlockFeatures_AccountManager_CanPlayOnline_Postfix
{
    // Prefix patch of AccountManager.CanPlayOnline to allow online games
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures)
        { // Only works if CheatSettings.unlockFeatures is enabled
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsAllowedOnline))]
public static class UnlockFeatures_EOSManager_IsAllowedOnline_Prefix
{
    // Prefix patch of AccountManager.CanPlayOnline to allow online games
    public static void Prefix(ref bool canOnline)
    {
        if (CheatToggles.unlockFeatures)
        { // Only works if CheatSettings.unlockFeatures is enabled
            canOnline = true;
        }
    }
}

[HarmonyPatch(typeof(InnerNet.InnerNetClient), nameof(InnerNet.InnerNetClient.JoinGame))]
public static class UnlockFeatures_InnerNetClient_JoinGame_Prefix
{
    // Prefix patch of InnerNet.InnerNetClient.JoinGame to allow online games
    public static void Prefix()
    {
        if (CheatToggles.unlockFeatures)
        { // Only works if CheatSettings.unlockFeatures is enabled
            AmongUs.Data.DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;
        }
    }
}