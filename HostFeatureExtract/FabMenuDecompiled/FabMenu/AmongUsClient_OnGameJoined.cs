using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(AmongUsClient), "OnGameJoined")]
public static class AmongUsClient_OnGameJoined
{
	public static string lastGameIdString = "";

	public static void Postfix(string gameIdString)
	{
		lastGameIdString = gameIdString;
	}
}
