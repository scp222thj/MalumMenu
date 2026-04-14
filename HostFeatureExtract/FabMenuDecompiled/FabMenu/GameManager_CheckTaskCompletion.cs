using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(GameManager), "CheckTaskCompletion")]
public static class GameManager_CheckTaskCompletion
{
	public static bool Prefix(ref bool __result)
	{
		if (!CheatToggles.noGameEnd)
		{
			return true;
		}
		__result = false;
		return false;
	}
}
