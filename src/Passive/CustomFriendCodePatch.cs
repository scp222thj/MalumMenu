using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.ProductUserId), MethodType.Getter)]
    public static class Passive_CustomPUIDPostfix
    {
        public static void Postfix(ref string __result)
        {
            if (CheatSettings.customUserData){
                if (MalumPlugin.customPuid.Value != ""){
                    __result = MalumPlugin.customPuid.Value;
                }
            }
        }
    }

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Update))]
public static class Passive_CustomFriendCodePostfix
{
    //Prefix patch of Getter method for StatsManager.BanMinutesLeft to remove disconnect penalty
    public static void Postfix(EOSManager __instance)
    {
        if (CheatSettings.customUserData){
            if (MalumPlugin.customFriendCode.Value != ""){
                __instance.FriendCode = MalumPlugin.customFriendCode.Value;
            }
        }
    }
}