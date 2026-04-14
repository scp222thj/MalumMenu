using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(PlatformSpecificData), "Serialize")]
public static class PlatformSpecificData_Serialize
{
	public static void Prefix(PlatformSpecificData __instance)
	{
		MalumSpoof.spoofPlatform(__instance);
	}
}
