using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(EOSManager), "IsFreechatAllowed")]
public static class EOSManager_IsFreechatAllowed
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.unlockFeatures)
		{
			__result = true;
		}
	}
}
