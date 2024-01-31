using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class ShipStatus_FixedUpdate
{
    public static void Postfix(ShipStatus __instance)
    {
        MalumCheats.sabotageCheat(__instance);
        MalumCheats.closeMeetingCheat();
        MalumCheats.reportBodyCheat();
        MalumCheats.walkInVentCheat();
        MalumCheats.kickVentsCheat();
    }
}