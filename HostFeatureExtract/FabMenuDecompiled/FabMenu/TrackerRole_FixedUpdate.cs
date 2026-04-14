using HarmonyLib;
using InnerNet;

namespace FabMenu;

[HarmonyPatch(typeof(TrackerRole), "FixedUpdate")]
public static class TrackerRole_FixedUpdate
{
	public static void Postfix(TrackerRole __instance)
	{
		if (((InnerNetObject)((RoleBehaviour)__instance).Player).AmOwner)
		{
			MalumCheats.trackerCheats(__instance);
		}
	}
}
