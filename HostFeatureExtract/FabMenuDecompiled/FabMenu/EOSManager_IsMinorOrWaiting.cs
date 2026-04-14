using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(EOSManager), "IsMinorOrWaiting")]
public static class EOSManager_IsMinorOrWaiting
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.unlockFeatures)
		{
			__result = false;
		}
	}
}
