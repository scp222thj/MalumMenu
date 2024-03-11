using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class PlayerControl_FixedUpdate
{
    public static void Postfix(PlayerControl __instance){

        if (__instance.AmOwner){
            MalumCheats.noKillCdCheat(__instance);
        }

    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckMurder))]
public static class PlayerControl_CmdCheckMurder
{
    // Prefix patch of PlayerControl.CmdCheckMurder to always bypass checks when killing players
    public static bool Prefix(PlayerControl __instance, PlayerControl target){

        if (Utils.isLobby || CheatToggles.killAnyone || Utils.isMeeting || (MalumPPMCheats.oldRole != null && Utils.getBehaviourByRoleType((AmongUs.GameOptions.RoleTypes)MalumPPMCheats.oldRole).IsImpostor)){
            __instance.isKilling = false;

            if (!__instance.Data.Role.IsValidTarget(target.Data))
            {
                return true;
            }

            // Protected players can only be killed if killAnyone is enabled
            if (target.protectedByGuardianId > -1 && !CheatToggles.killAnyone){
                return true;
            }

            __instance.isKilling = true;

            Utils.murderPlayer(target, MurderResultFlags.Succeeded);

            return false;
        }

        return true;

    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.TurnOnProtection))]
public static class PlayerControl_TurnOnProtection
{
	// Prefix patch of PlayerControl.ProtectPlayer to render all protections visible if CheatToggles.seeGhosts is enabled
    public static void Prefix(ref bool visible){
		if (CheatToggles.seeGhosts){
            visible = true;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckShapeshift))]
public static class ShapeshifterCheats_PlayerControl_CmdCheckShapeshift_Postfix
{
    // Prefix patch of PlayerControl.CmdCheckShapeshift to prevent SS animation
    public static void Prefix(ref bool shouldAnimate){

        shouldAnimate = !CheatToggles.noShapeshiftAnim;

    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckRevertShapeshift))]
public static class ShapeshifterCheats_PlayerControl_CmdCheckRevertShapeshift_Postfix
{
    // Prefix patch of PlayerControl.CmdCheckRevertShapeshift to prevent SS animation
    public static void Prefix(ref bool shouldAnimate){

        shouldAnimate = !CheatToggles.noShapeshiftAnim;

    }
}