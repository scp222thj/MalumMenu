using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EngineerRole), nameof(EngineerRole.FixedUpdate))]
public static class EngineerCheats_EngineerRole_FixedUpdate_Postfix
{
    // Postfix patch of EngineerRole.FixedUpdate to remove vent cooldown & have endless vent time
    public static void Postfix(EngineerRole __instance)
    {
        if (__instance.Player.AmOwner)
        {
            if (CheatToggles.endlessVentTime)
            {
                __instance.inVentTimeRemaining = float.MaxValue; // Makes vent time so incredibly long (float.MaxValue) so that it never ends
                
            }
            else if (__instance.inVentTimeRemaining > GameManager.Instance.LogicOptions.GetEngineerCooldown())
            {
                __instance.inVentTimeRemaining = GameManager.Instance.LogicOptions.GetEngineerCooldown(); // Makes sure vent time is resetted to normal value after the cheat is disabled
            }

            if (CheatToggles.noVentCooldown)
            {
                __instance.cooldownSecondsRemaining = 0f; // Tricks the client into thinking the cooldown is always 0 seconds.
            }
        }
    }
}