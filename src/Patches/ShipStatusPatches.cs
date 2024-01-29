using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class ShipStatus_FixedUpdate
{
    public static void Postfix(ShipStatus __instance)
    {
        ShipCheatHandler.sabotageCheat(__instance);
        ShipCheatHandler.closeMeetingCheat();
        ShipCheatHandler.reportBodyCheat();
    }
}