using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
public static class Vent_CanUse
{
    // Postfix patch of Vent.CanUse to allow usage of vents when useVents cheat is enabled
    public static void Postfix(Vent __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
    {
        if (!PlayerControl.LocalPlayer || !PlayerControl.LocalPlayer.Data) return;
        if (PlayerControl.LocalPlayer.Data.Role.CanVent || PlayerControl.LocalPlayer.Data.IsDead) return;
        if (!CheatToggles.unlockVents) return;

        var @object = pc.Object;

        var center = @object.Collider.bounds.center;
        var position = __instance.transform.position;
        var num = Vector2.Distance(center, position);

        // Allow usage of vents unless the vent is too far or there are objects blocking the player's path
        canUse = num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false);
        couldUse = true;
        __result = num;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
public static class Vent_EnterVent
{
    // Postfix patch of Vent.EnterVent to log on ConsoleUI when a player enters a vent
    // along with the room they entered it in, and optionally kick impostor venters (host-only).
    public static void Postfix(Vent __instance, PlayerControl pc)
    {
        if (!Utils.isShip || pc == null || pc.Data == null) return;

        var roleInfo = pc.Data?.Role;
        bool isImp = roleInfo != null && roleInfo.IsImpostor;
        string roleLabel = isImp ? "Imp" : "Crew";

        if (CheatToggles.logVents)
        {
            var (realPlayerName, displayPlayerName, isDisguised) = Utils.GetPlayerIdentity(pc);
            var room = Utils.GetRoomFromPosition(__instance.transform.position);
            var roomName = room != null ? room.RoomId.ToString() : "an unknown location";

            ConsoleUI.Log(isDisguised
                ? $"[{roleLabel}] {realPlayerName} (as {displayPlayerName}) entered a vent in {roomName}"
                : $"[{roleLabel}] {realPlayerName} entered a vent in {roomName}");
        }

        // Auto-boot: eject all impostors from vents when an impostor (not engineer) vents
        if (CheatToggles.autoBootVents && roleInfo != null
            && isImp && roleInfo.Role != RoleTypes.Engineer)
        {
            foreach (var vent in ShipStatus.Instance.AllVents)
                VentilationSystem.Update(VentilationSystem.Operation.BootImpostors, vent.Id);
        }

        // Auto-kick: host kicks any impostor/phantom who vents — Engineers are exempt
        if (CheatToggles.autoKickVentImpostors && Utils.isHost && pc != PlayerControl.LocalPlayer)
        {
            if (roleInfo == null) return;
            if (roleInfo.Role == RoleTypes.Engineer) return;
            if (!isImp) return;

            ConsoleUI.Log($"[Imp] Kicked {pc.Data.PlayerName} for venting");
            AmongUsClient.Instance.KickPlayer(pc.Data.ClientId, false);
        }
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
public static class Vent_ExitVent
{
    // Postfix patch of Vent.ExitVent to log on ConsoleUI when a player exits a vent
    // along with the room they exited it in
    public static void Postfix(Vent __instance, PlayerControl pc)
    {
        if (!CheatToggles.logVents || !Utils.isShip) return;
        if (pc == null || pc.Data == null) return;

        var (realPlayerName, displayPlayerName, isDisguised) = Utils.GetPlayerIdentity(pc);

        var room = Utils.GetRoomFromPosition(__instance.transform.position); //- (Vector3) pc.Collider.offset);
        var roomName = room != null ? room.RoomId.ToString() : "an unknown location";

        ConsoleUI.Log(isDisguised
            ? $"{realPlayerName} (as {displayPlayerName}) exited a vent in {roomName}"
            : $"{realPlayerName} exited a vent in {roomName}");
    }
}
