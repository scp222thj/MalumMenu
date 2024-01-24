using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckShapeshift))]
public static class ShapeshifterCheats_PlayerControl_CmdCheckShapeshift_Postfix
{
    // Prefix patch of PlayerControl.CmdCheckShapeshift to prevent SS animation
    public static bool Prefix(PlayerControl __instance, ref bool shouldAnimate)
    {
        // Remove the shapeshift animation if noShapeshiftAnim is enabled
        if (shouldAnimate && CheatToggles.noShapeshiftAnim)
        {
            shouldAnimate = false;
        }

        return true;

    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckRevertShapeshift))]
public static class ShapeshifterCheats_PlayerControl_CmdCheckRevertShapeshift_Postfix
{
    // Prefix patch of PlayerControl.CmdCheckRevertShapeshift to prevent SS animation
    public static bool Prefix(PlayerControl __instance, ref bool shouldAnimate)
    {
        // Remove the shapeshift animation if noShapeshiftAnim is enabled
        if (CheatToggles.noShapeshiftAnim)
        {
            shouldAnimate = false;
        }

        return true;

    }
}

[HarmonyPatch(typeof(ShapeshifterRole), nameof(ShapeshifterRole.FixedUpdate))]
public static class endlessSsDuration_ShapeshifterRole_FixedUpdate_Postfix
{

    // Postfix patch of ShapeshifterRole.FixedUpdate to have endless shapeshift duration
    public static void Postfix(ShapeshifterRole __instance)
    {
        // try-catch to avoid breaking when spectator menu is open
        try
        {
            if (__instance.Player.AmOwner)
            {
                if (CheatToggles.endlessSsDuration)
                {
                    __instance.durationSecondsRemaining = float.MaxValue; // Makes shapeshift duration so incredibly long (float.MaxValue) so that it never ends
                }
                else if (__instance.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetShapeshifterDuration())
                {
                    __instance.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetShapeshifterDuration(); // Makes sure shapeshift duration is reset to normal value after the cheat is disabled
                }

            }

        }
        catch { };
    }
}