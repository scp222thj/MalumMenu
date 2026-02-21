using HarmonyLib;
using UnityEngine;

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
    /// <summary>
    /// Prefix patch of PlayerControl.CmdCheckMurder to always bypass checks when killing players
    /// </summary>
    /// <param name="__instance">The <c>PlayerControl</c> instance.</param>
    /// <param name="target">The target player to be killed.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(PlayerControl __instance, PlayerControl target)
    {
        /*if (Utils.isLobby){
            HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
            return false;
        }

        // Direct kill RPC should only be used when absolutely necessary as to avoid detection from anticheat mods
        if (!CheatToggles.killAnyone && !CheatToggles.zeroKillCd && !Utils.isVanished(__instance.Data) &&
            !Utils.isMeeting &&
            (MalumPPMCheats.oldRole == null ||
             Utils.getBehaviourByRoleType((AmongUs.GameOptions.RoleTypes)MalumPPMCheats.oldRole).IsImpostor))
            return true;
        if (!__instance.Data.Role.IsValidTarget(target.Data))
        {
            return true;
        }

        if (target.protectedByGuardianId > -1 && !CheatToggles.killAnyone){
            return true;
        }

        Utils.murderPlayer(target, MurderResultFlags.Succeeded);

        return false;*/

        if (!Utils.IsHost) return true;
        // __instance.isKilling = true;
        PlayerControl.LocalPlayer.RpcMurderPlayer(target, true);
        return false;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
public static class PlayerControl_MurderPlayer
{
    /// <summary>
    /// Prefix patch of PlayerControl.MurderPlayer to log when a player tries to kill another player, who the killer and target are,
    /// and where the kill happened. Also logs when a kill gets saved by a guardian angel.
    /// </summary>
    /// <param name="__instance">The <c>PlayerControl</c> instance.</param>
    /// <param name="target">The player being killed.</param>
    public static void Prefix(PlayerControl __instance, PlayerControl target)
    {
        if (!CheatToggles.logDeaths || target == null) return;

        var (realKillerName, displayKillerName, isDisguised) = Utils.GetPlayerIdentity(__instance);
        var targetName = $"<color=#{ColorUtility.ToHtmlStringRGB(target.Data.Color)}>{target.CurrentOutfit.PlayerName}</color>";
        var room = Utils.GetRoomFromPosition(target.GetTruePosition());
        var roomName = room != null ? room.RoomId.ToString() : "an unknown location";

        if (target.protectedByGuardianId != -1)
        {
            ConsoleUI.Log(isDisguised ? $"{realKillerName} (as {displayKillerName}) tried to kill {targetName} in {roomName} (Saved)"
                : $"{realKillerName} tried to kill {targetName} in {roomName} (Saved)");
        }
        else
        {
            ConsoleUI.Log(isDisguised ? $"{realKillerName} (as {displayKillerName}) killed {targetName} in {roomName}"
                : $"{realKillerName} killed {targetName} in {roomName}");
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.TurnOnProtection))]
public static class PlayerControl_TurnOnProtection
{
    /// <summary>
    /// Prefix patch of PlayerControl.TurnOnProtection to render all protections visible if CheatToggles.seeGhosts is enabled
    /// </summary>
    /// <param name="visible">Whether the protection should be visible.</param>
    public static void Prefix(ref bool visible){
		if (CheatToggles.seeGhosts){
            visible = true;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckShapeshift))]
public static class PlayerControl_CmdCheckShapeshift
{
    /// <summary>
    /// Prefix patch of PlayerControl.CmdCheckShapeshift to prevent SS animation
    /// </summary>
    /// <param name="shouldAnimate">Whether the shapeshift animation should play.</param>
    public static void Prefix(ref bool shouldAnimate){

        if (shouldAnimate && CheatToggles.noShapeshiftAnim){
            shouldAnimate = false;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckRevertShapeshift))]
public static class PlayerControl_CmdCheckRevertShapeshift
{
    /// <summary>
    /// Prefix patch of PlayerControl.CmdCheckRevertShapeshift to prevent SS animation
    /// </summary>
    /// <param name="shouldAnimate">Whether the revert shapeshift animation should play.</param>
    public static void Prefix(ref bool shouldAnimate){

        if (shouldAnimate && CheatToggles.noShapeshiftAnim){
            shouldAnimate = false;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Shapeshift))]
public static class PlayerControl_Shapeshift
{
    /// <summary>
    /// Postfix patch of PlayerControl.Shapeshift to log when a player shapeshifts into another player,
    /// and who they shapeshifted into. Also logs when a shapeshift gets reverted.
    /// </summary>
    /// <param name="__instance">The <c>PlayerControl</c> instance.</param>
    /// <param name="targetPlayer">The player that is being shapeshifted into.</param>
    /// <param name="animate">Used in the original method to determine whether the shapeshift animation should play.</param>
    public static void Postfix(PlayerControl __instance, PlayerControl targetPlayer, bool animate)
    {
        if (!CheatToggles.logShapeshifts) return;

        if (__instance.CurrentOutfitType == PlayerOutfitType.MushroomMixup) return;
        var targetPlayerInfo = targetPlayer.Data;
        if (targetPlayerInfo.PlayerId == __instance.Data.PlayerId)
        {
            ConsoleUI.Log($"<color=#{ColorUtility.ToHtmlStringRGB(GameData.Instance.GetPlayerById(__instance.PlayerId).Color)}>" +
                          $"{GameData.Instance.GetPlayerById(__instance.PlayerId)._object.Data.PlayerName}</color> Shapeshift was reverted");
        }
        else
        {
            ConsoleUI.Log($"<color=#{ColorUtility.ToHtmlStringRGB(GameData.Instance.GetPlayerById(__instance.PlayerId).Color)}>" +
                          $"{GameData.Instance.GetPlayerById(__instance.PlayerId)._object.Data.PlayerName}</color> shapeshifted into " +
                          $"<color=#{ColorUtility.ToHtmlStringRGB(GameData.Instance.GetPlayerById(targetPlayerInfo.PlayerId).Color)}>" +
                          $"{GameData.Instance.GetPlayerById(targetPlayerInfo.PlayerId)._object.Data.PlayerName}</color>");
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
public static class PlayerControl_RpcSyncSettings
{
    /// <summary>
    /// Prefix patch of PlayerControl.RpcSyncSettings to prevent the anti-cheat from kicking you
    /// for some settings that are out of the "original" valid range.
    /// </summary>
    /// <param name="__instance">The <c>PlayerControl</c> instance.</param>
    /// <param name="optionsByteArray">The byte array containing the options to sync.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(PlayerControl __instance, byte[] optionsByteArray)
    {
        return !CheatToggles.noOptionsLimits;
    }
}
