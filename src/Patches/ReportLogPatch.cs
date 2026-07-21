using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
public static class PlayerControl_CmdReportDeadBody_ReportLog
{
    public static void Postfix(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        if (target == null) return;

        var reporterName = __instance.Data.PlayerName;
        var deadPlayerName = target.PlayerName;

        var room = Utils.GetRoomFromPosition(__instance.GetTruePosition());
        var location = room != null ? room.RoomId.ToString() : "Unknown";

        EventLogger.Log(GameEventType.Report, $"{reporterName} reported {deadPlayerName}", reporterName, __instance.Data.Role?.Role.ToString() ?? "", location);
    }
}
