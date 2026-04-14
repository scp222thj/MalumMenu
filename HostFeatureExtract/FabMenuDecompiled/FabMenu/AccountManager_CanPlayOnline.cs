using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(AccountManager), "CanPlayOnline")]
public static class AccountManager_CanPlayOnline
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.unlockFeatures)
		{
			__result = true;
		}
	}
}
