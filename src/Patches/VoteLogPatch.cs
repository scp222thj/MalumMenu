using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
public static class MeetingHud_CastVote_VoteLog
{
    public static void Postfix(MeetingHud __instance, byte srcPlayerId, byte suspectPlayerId)
    {
        var voterData = GameData.Instance.GetPlayerById(srcPlayerId);
        if (voterData == null) return;

        var voterName = voterData.PlayerName;

        string targetName;
        if (suspectPlayerId == 253)
        {
            targetName = "Skip";
        }
        else if (suspectPlayerId == 255)
        {
            targetName = "No Vote";
        }
        else
        {
            var targetData = GameData.Instance.GetPlayerById(suspectPlayerId);
            targetName = targetData != null ? targetData.PlayerName : "Unknown";
        }

        EventLogger.Log(GameEventType.Vote, $"{voterName} voted for {targetName}", voterName, voterData.Role?.Role.ToString() ?? "");
    }
}
