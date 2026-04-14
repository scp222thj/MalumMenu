using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(Mushroom), "FixedUpdate")]
public static class Mushroom_FixedUpdate
{
	public static void Postfix(Mushroom __instance)
	{
		MalumESP.sporeCloudVision(__instance);
	}
}
