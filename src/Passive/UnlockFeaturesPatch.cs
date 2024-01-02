using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFreechatAllowed))]
public static class Passive_FreechatPrefix
{
    //Prefix patch of EOSManager.IsFreechatAllowed to unlock freechat
    public static bool Prefix(EOSManager __instance, ref bool __result)
    {
        if (!CheatSettings.unlockFeatures){
            return true; //Only works if CheatSettings.unlockFeatures is enabled
        }

        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsFriendsListAllowed))]
public static class Passive_FriendListPrefix
{
    //Prefix patch of EOSManager.IsFriendsListAllowed to unlock friend list
    public static bool Prefix(EOSManager __instance, ref bool __result)
    {
        if (!CheatSettings.unlockFeatures){
            return true; //Only works if CheatSettings.unlockFeatures is enabled
        }
        
        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsMinorOrWaiting))]
public static class Passive_MinorPrefix
{
    //Prefix patch of EOSManager.IsMinorOrWaiting to remove minor status
    public static bool Prefix(EOSManager __instance, ref bool __result)
    {
        if (!CheatSettings.unlockFeatures){
            return true; //Only works if CheatSettings.unlockFeatures is enabled
        }
        
        __result = false;
        return false;
    }
}

[HarmonyPatch(typeof(FullAccount), nameof(FullAccount.CanSetCustomName))]
public static class Passive_CustomNamePrefix
{
    //Prefix patch of FullAccount.CanSetCustomName to unlock custom names
    public static bool Prefix(bool canSetName, FullAccount __instance)
    {
        if (!CheatSettings.unlockFeatures){
            return true; //Only works if CheatSettings.unlockFeatures is enabled
        }

        if (!canSetName){
            __instance.CanSetCustomName(true);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(AccountManager), nameof(AccountManager.CanPlayOnline))]
public static class Passive_CanPlayOnlinePrefix
{
    public static void Postfix(ref bool __result)
    {
        if (CheatSettings.unlockFeatures){
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsAllowedOnline))]
public static class Passive_IsAllowedOnlinePrefix
{
    public static void Prefix(ref bool canOnline)
    {
        canOnline = true;
    }
}

[HarmonyPatch(typeof(InnerNet.InnerNetClient), nameof(InnerNet.InnerNetClient.JoinGame))]
public static class Passive_JoinGamePrefix
{
    public static void Prefix()
    {
        AmongUs.Data.DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;
    }
}