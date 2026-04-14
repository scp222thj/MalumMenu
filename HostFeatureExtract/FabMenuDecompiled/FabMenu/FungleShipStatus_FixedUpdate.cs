using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(ShipStatus), "FixedUpdate")]
public static class FungleShipStatus_FixedUpdate
{
	public static void Postfix(FungleShipStatus __instance)
	{
		MalumCheats.fungleSabotageCheat(__instance);
	}
}
