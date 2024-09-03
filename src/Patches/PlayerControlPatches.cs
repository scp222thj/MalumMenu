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

        if (Utils.isLobby){
            HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
            return false;
        }

        // Direct kill RPC should only be used when absolutely necessary as to avoid detection from anticheat mods
        if (CheatToggles.killAnyone || CheatToggles.zeroKillCd || Utils.isVanished(__instance.Data) || Utils.isMeeting || (MalumPPMCheats.oldRole != null && !Utils.getBehaviourByRoleType((AmongUs.GameOptions.RoleTypes)MalumPPMCheats.oldRole).IsImpostor)){
            if (!__instance.Data.Role.IsValidTarget(target.Data))
            {
                return true;
            }

            if (target.protectedByGuardianId > -1 && !CheatToggles.killAnyone){
                return true;
            }
            
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

        if (shouldAnimate && CheatToggles.noShapeshiftAnim){
            shouldAnimate = false;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckRevertShapeshift))]
public static class ShapeshifterCheats_PlayerControl_CmdCheckRevertShapeshift_Postfix
{
    // Prefix patch of PlayerControl.CmdCheckRevertShapeshift to prevent SS animation
    public static void Prefix(ref bool shouldAnimate){

        if (shouldAnimate && CheatToggles.noShapeshiftAnim){
            shouldAnimate = false;
        }
    }
}
