using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Update))]
public static class Passive_CustomFriendCodePostfix
{
    //Prefix patch of Getter method for StatsManager.BanMinutesLeft to remove disconnect penalty
    public static void Postfix(EOSManager __instance)
    {
        if (CheatSettings.customFriendCode){
            if (MalumPlugin.customFriendCode.Value != ""){
                __instance.FriendCode = MalumPlugin.customFriendCode.Value;
            }
        }
    }
}