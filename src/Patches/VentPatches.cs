using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
public static class Vent_CanUse
{
    /// <summary>
    /// Postfix patch of Vent.CanUse to allow venting.
    /// </summary>
    /// <param name="__instance">The <c>Vent</c> instance.</param>
    /// <param name="pc">The <c>PlayerControl</c> of the player trying to use the vent.</param>
    /// <param name="canUse">Whether the player can currently use the vent, accounting for distance and physics obstacles.</param>
    /// <param name="couldUse">Whether the player's role and game state theoretically allow vent usage.</param>
    /// <param name="__result">The distance from the player to the vent, or -1 if the vent cannot be used.</param>
    public static void Postfix(Vent __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
    {
        if (!PlayerControl.LocalPlayer || !PlayerControl.LocalPlayer.Data) return;
        if (PlayerControl.LocalPlayer.Data.Role.CanVent || PlayerControl.LocalPlayer.Data.IsDead) return;
        if (!CheatToggles.useVents) return;
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
    /// <summary>
    /// Postfix patch of Vent.EnterVent to log when a player enters a vent, along with the room they entered it in.
    /// </summary>
    /// <param name="__instance">The <c>Vent</c> instance.</param>
    /// <param name="pc">The <c>PlayerControl</c> of the player entering the vent.</param>
    public static void Postfix(Vent __instance, PlayerControl pc)
    {
        if (!CheatToggles.logVents || !Utils.isShip) return;

        var (realPlayerName, displayPlayerName, isDisguised) = Utils.GetPlayerIdentity(pc);
        var room = Utils.GetRoomFromPosition(__instance.transform.position - (Vector3) pc.Collider.offset);
        var roomName = room != null ? room.RoomId.ToString() : "an unknown location";
        ConsoleUI.Log(isDisguised
            ? $"{realPlayerName} (as {displayPlayerName}) entered a vent in {roomName}"
            : $"{realPlayerName} entered a vent in {roomName}");
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
public static class Vent_ExitVent
{
    /// <summary>
    /// Postfix patch of Vent.ExitVent to log when a player exits a vent, along with the room they exited it in.
    /// </summary>
    /// <param name="__instance">The <c>Vent</c> instance.</param>
    /// <param name="pc">The <c>PlayerControl</c> of the player exiting the vent.</param>
    public static void Postfix(Vent __instance, PlayerControl pc)
    {
        if (!CheatToggles.logVents || !Utils.isShip) return;

        var (realPlayerName, displayPlayerName, isDisguised) = Utils.GetPlayerIdentity(pc);
        var room = Utils.GetRoomFromPosition(__instance.transform.position - (Vector3) pc.Collider.offset);
        var roomName = room != null ? room.RoomId.ToString() : "an unknown location";
        ConsoleUI.Log(isDisguised
            ? $"{realPlayerName} (as {displayPlayerName}) exited a vent in {roomName}"
            : $"{realPlayerName} exited a vent in {roomName}");
    }
}
