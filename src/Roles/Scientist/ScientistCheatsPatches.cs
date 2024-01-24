using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ScientistRole), nameof(ScientistRole.Update))]
public static class ScientistCheats_ScientistRole_Update_Postfix
{
    // Postfix patch of ScientistRole.Update to have endless vitals battery and no cooldown
    public static void Postfix(ScientistRole __instance)
    {
        if (__instance.Player.AmOwner)
        {
            if (CheatToggles.noVitalsCooldown)
            {
                __instance.currentCooldown = 0f; // tricks the client into thinking the vitals CD is 0
            }

            if (CheatToggles.endlessBattery)
            {
                __instance.currentCharge = float.MaxValue; //Makes vitals battery so incredibly long (float.MaxValue) so that it never ends
            }
            else if (__instance.currentCharge > GameManager.Instance.LogicOptions.GetScientistBatteryCharge())
            {
                __instance.currentCharge = GameManager.Instance.LogicOptions.GetScientistBatteryCharge(); // Makes sure battery is reset to normal value after the cheat is disabled
            }
        }
    }
}