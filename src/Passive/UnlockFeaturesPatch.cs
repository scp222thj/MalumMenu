using HarmonyLib;

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

        AmongUs.Data.DataManager.Settings.Multiplayer.ChatMode = InnerNet.QuickChatModes.FreeChatOrQuickChat;
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

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.IsAllowedOnline))]
public static class Passive_OnlineGamePlayPrefix
{
    //Prefix patch of EOSManager.IsAllowedOnline to unlock online gameplay
    public static bool Prefix(bool canOnline, EOSManager __instance)
    {
        if (!CheatSettings.unlockFeatures){
            return true; //Only works if CheatSettings.unlockFeatures is enabled
        }

        if (!canOnline){
            __instance.IsAllowedOnline(true);
            return false;
        }

        return true;
    }
}