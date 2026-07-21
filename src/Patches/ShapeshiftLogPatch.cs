using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Shapeshift))]
public static class PlayerControl_Shapeshift_ShapeshiftLog
{
    public static void Postfix(PlayerControl __instance, PlayerControl targetPlayer, bool animate)
    {
        if (__instance.CurrentOutfitType == PlayerOutfitType.MushroomMixup) return;

        var killerName = __instance.Data.PlayerName;
        var killerRole = __instance.Data.Role?.Role.ToString() ?? "";
        var targetName = targetPlayer.Data.PlayerName;
        var targetRole = targetPlayer.Data.Role?.Role.ToString() ?? "";

        var room = Utils.GetRoomFromPosition(__instance.GetTruePosition());
        var location = room != null ? room.RoomId.ToString() : "Unknown";

        if (targetPlayer.PlayerId == __instance.PlayerId)
        {
            EventLogger.Log(GameEventType.Shapeshift, $"{killerName} reverted their shapeshift", killerName, killerRole, location);
        }
        else
        {
            EventLogger.Log(GameEventType.Shapeshift, $"{killerName} shapeshifted into {targetName}", killerName, killerRole, location);
        }
    }
}
