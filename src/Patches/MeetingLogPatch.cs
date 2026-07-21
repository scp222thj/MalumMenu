using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
public static class PlayerControl_StartMeeting_MeetingLog
{
    public static void Postfix(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        var callerName = __instance.Data.PlayerName;

        string reportedPlayerName;
        if (target != null)
        {
            var reportedData = GameData.Instance.GetPlayerById(target.PlayerId);
            reportedPlayerName = reportedData != null ? reportedData.PlayerName : "Unknown";
        }
        else
        {
            reportedPlayerName = "None";
        }

        EventLogger.Log(GameEventType.Meeting, $"{callerName} started a meeting (Reported: {reportedPlayerName})", callerName, __instance.Data.Role?.Role.ToString() ?? "");
    }
}
