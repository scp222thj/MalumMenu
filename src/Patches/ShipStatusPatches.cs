using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class ShipStatus_FixedUpdate
{
    public static void Postfix(ShipStatus __instance)
    {
        MalumSabotageCheats.Process(__instance);
        MalumCheats.OpenSabotageMapCheat();

        MalumCheats.CloseMeetingCheat();
        MalumCheats.SkipMeetingCheat();
        MalumCheats.CallMeetingCheat();
        MalumCheats.WalkInVentCheat();
        MalumCheats.KickVentsCheat();

        MalumPPMCheats.ReportBodyPPM();
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class FungleShipStatus_FixedUpdate
{
    public static void Postfix(FungleShipStatus __instance)
    {
        MalumSabotageCheats.ProcessFungle(__instance);
    }
}
