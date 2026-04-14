using HarmonyLib;
using InnerNet;

namespace FabMenu;

[HarmonyPatch(typeof(ShapeshifterRole), "FixedUpdate")]
public static class ShapeshifterRole_FixedUpdate
{
	public static void Postfix(ShapeshifterRole __instance)
	{
		try
		{
			if (((InnerNetObject)((RoleBehaviour)__instance).Player).AmOwner)
			{
				MalumCheats.shapeshifterCheats(__instance);
			}
		}
		catch
		{
		}
	}
}
