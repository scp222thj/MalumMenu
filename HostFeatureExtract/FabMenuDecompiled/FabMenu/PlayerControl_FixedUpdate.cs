using HarmonyLib;
using InnerNet;

namespace FabMenu;

[HarmonyPatch(typeof(PlayerControl), "FixedUpdate")]
public static class PlayerControl_FixedUpdate
{
	public static void Postfix(PlayerControl __instance)
	{
		if (((InnerNetObject)__instance).AmOwner)
		{
			MalumCheats.noKillCdCheat(__instance);
		}
	}
}
