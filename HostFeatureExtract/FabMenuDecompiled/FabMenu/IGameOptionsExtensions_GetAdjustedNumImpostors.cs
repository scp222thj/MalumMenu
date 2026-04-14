using AmongUs.GameOptions;
using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(IGameOptionsExtensions), "GetAdjustedNumImpostors")]
public static class IGameOptionsExtensions_GetAdjustedNumImpostors
{
	public static bool Prefix(IGameOptions __instance, int playerCount, ref int __result)
	{
		if (!CheatToggles.noOptionsLimits)
		{
			return true;
		}
		__result = GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;
		return false;
	}
}
