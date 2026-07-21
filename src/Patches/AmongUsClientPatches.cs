using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Update))]
public static class AmongUsClient_Update
{
    public static void Postfix()
    {
        MalumSpoof.SpoofLevel();
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
public static class AmongUsClient_OnGameJoined
{
    public static string lastGameIdString = "";

    public static void Postfix(string gameIdString)
    {
        lastGameIdString = gameIdString;
    }
}
