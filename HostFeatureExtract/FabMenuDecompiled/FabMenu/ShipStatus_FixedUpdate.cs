using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(ShipStatus), "FixedUpdate")]
public static class ShipStatus_FixedUpdate
{
	public static void Postfix(ShipStatus __instance)
	{
		MalumCheats.sabotageCheat(__instance);
		MalumCheats.closeMeetingCheat();
		MalumCheats.skipMeetingCheat();
		MalumCheats.callMeetingCheat();
		MalumCheats.walkInVentCheat();
		MalumCheats.kickVentsCheat();
		MalumPPMCheats.reportBodyPPM();
	}
}
