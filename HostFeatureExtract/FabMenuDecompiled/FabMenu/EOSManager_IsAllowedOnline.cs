using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(EOSManager), "IsAllowedOnline")]
public static class EOSManager_IsAllowedOnline
{
	public static void Prefix(ref bool canOnline)
	{
		if (CheatToggles.unlockFeatures)
		{
			canOnline = true;
		}
	}
}
