using HarmonyLib;
using InnerNet;

namespace FabMenu;

[HarmonyPatch(typeof(PhantomRole), "FixedUpdate")]
public static class PhantomRole_FixedUpdate
{
	public static void Postfix(PhantomRole __instance)
	{
		if (((InnerNetObject)((RoleBehaviour)__instance).Player).AmOwner)
		{
			MalumCheats.phantomCheats(__instance);
		}
	}
}
