using HarmonyLib;
using InnerNet;

namespace FabMenu;

[HarmonyPatch(typeof(EngineerRole), "FixedUpdate")]
public static class EngineerRole_FixedUpdate
{
	public static void Postfix(EngineerRole __instance)
	{
		if (((InnerNetObject)((RoleBehaviour)__instance).Player).AmOwner)
		{
			MalumCheats.engineerCheats(__instance);
		}
	}
}
