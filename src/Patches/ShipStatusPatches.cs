using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
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

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class FungleShipStatus_FixedUpdate
{
    public static void Postfix(FungleShipStatus __instance)
    {
        MalumCheats.fungleSabotageCheat(__instance);
    }
}
