using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.ProductUserId), MethodType.Getter)]
public static class Passive_SpoofedPUIDPostfix
{
    public static void Postfix(ref string __result)
    {
        if (MalumPlugin.spoofPuid.Value != ""){
            __result = MalumPlugin.spoofPuid.Value;
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Update))]
public static class Passive_SpoofedFriendCodePostfix
{
    public static void Postfix(EOSManager __instance)
    {
        if (MalumPlugin.spoofPuid.Value != "" && MalumPlugin.spoofPuid.Value != __instance.FriendCode){
            __instance.FriendCode = MalumPlugin.spoofPuid.Value;
        }
    }
}