using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class PlayerControl_FixedUpdate
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance.AmOwner)
        {
            MalumCheats.noKillCdCheat(__instance);
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckMurder))]
public static class PlayerControl_CmdCheckMurder
{
    public static bool Prefix(PlayerControl __instance, PlayerControl target)
    {
        if (Utils.isLobby)
        {
            HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
            return false;
        }

        bool bypassCheck = CheatToggles.killAnyone
                        || CheatToggles.zeroKillCd
                        || Utils.isVanished(__instance.Data)
                        || Utils.isMeeting
                        || (MalumPPMCheats.oldRole != null
                            && !Utils.getBehaviourByRoleType((AmongUs.GameOptions.RoleTypes)MalumPPMCheats.oldRole).IsImpostor);

        if (bypassCheck)
        {
            if (!__instance.Data.Role.IsValidTarget(target.Data)) return true;

            if (target.protectedByGuardianId > -1 && !CheatToggles.killAnyone)
                return true;

            Utils.murderPlayer(target, MurderResultFlags.Succeeded);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.TurnOnProtection))]
public static class PlayerControl_TurnOnProtection
{
    public static void Prefix(ref bool visible)
    {
        if (CheatToggles.seeGhosts)
        {
            visible = true;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckShapeshift))]
public static class ShapeshifterCheats_PlayerControl_CmdCheckShapeshift_Postfix
{
    public static void Prefix(ref bool shouldAnimate)
    {
        if (shouldAnimate && CheatToggles.noShapeshiftAnim)
        {
            shouldAnimate = false;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckRevertShapeshift))]
public static class ShapeshifterCheats_PlayerControl_CmdCheckRevertShapeshift_Postfix
{
    public static void Prefix(ref bool shouldAnimate)
    {
        if (shouldAnimate && CheatToggles.noShapeshiftAnim)
        {
            shouldAnimate = false;
        }
    }
}
