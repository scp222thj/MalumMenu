using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
internal static class CopyCodeOnLeave_OnGameJoinedPatch
{
    [HarmonyPostfix]
    public static void Postfix(string gameIdString)
    {
        CopyCodeOnLeaveService.SetCode(gameIdString);
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnDisconnected))]
internal static class CopyCodeOnLeave_OnDisconnectedPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        CopyCodeOnLeaveService.OnDisconnect();
    }
}
