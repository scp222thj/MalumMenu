using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(HatManager), "Initialize")]
public static class HatManager_Initialize
{
	public static void Postfix(HatManager __instance)
	{
		CosmeticsUnlocker.unlockCosmetics(__instance);
	}
}
