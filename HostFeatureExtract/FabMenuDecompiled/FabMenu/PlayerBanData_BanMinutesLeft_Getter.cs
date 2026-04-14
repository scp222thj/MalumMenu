using AmongUs.Data.Player;
using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class PlayerBanData_BanMinutesLeft_Getter
{
	public static void Postfix(PlayerBanData __instance, ref int __result)
	{
		if (CheatToggles.avoidBans)
		{
			__instance.BanPoints = 0f;
			__result = 0;
		}
	}
}
