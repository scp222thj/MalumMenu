using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace MalumMenu;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class MeetingHud_Update
{
    public static List<int> votedPlayers = new List<int>();

    /// <summary>
    /// Prefix patch of MeetingHud.Update to constantly bloop new vote icons for each new vote being cast during the meeting
    /// </summary>
    /// <param name="__instance">The <c>MeetingHud</c> instance.</param>
    public static void Prefix(MeetingHud __instance)
    {
        if (__instance.state < MeetingHud.VoteStates.Results)
        {
            foreach (var playerVoteArea in __instance.playerStates)
            {
                if (!playerVoteArea) continue;
                var playerData = GameData.Instance.GetPlayerById(playerVoteArea.TargetPlayerId);
                if (playerData != null && !playerData.Disconnected && playerVoteArea.VotedFor != PlayerVoteArea.HasNotVoted && playerVoteArea.VotedFor != PlayerVoteArea.MissedVote && playerVoteArea.VotedFor != PlayerVoteArea.DeadVote && !votedPlayers.Contains(playerVoteArea.TargetPlayerId))
                {
                    votedPlayers.Add(playerVoteArea.TargetPlayerId);
                    if (playerVoteArea.VotedFor != PlayerVoteArea.SkippedVote)
                    {
                        foreach (var votedForArea in __instance.playerStates)
                        {
                            if (votedForArea.TargetPlayerId == playerVoteArea.VotedFor)
                            {
                                __instance.BloopAVoteIcon(playerData, 0, votedForArea.transform);
                                break;
                            }
                        }
                    }
                    else if (__instance.SkippedVoting)
                    {
                        __instance.BloopAVoteIcon(playerData, 0, __instance.SkippedVoting.transform);
                    }
                }
            }

            foreach (var votedForArea in __instance.playerStates)
            {
                if (!votedForArea) continue;
                var voteSpreader = votedForArea.transform.GetComponent<VoteSpreader>();
                if (!voteSpreader) continue;
                foreach (var spriteRenderer in voteSpreader.Votes)
                {
                    spriteRenderer.gameObject.SetActive(CheatToggles.revealVotes);
                }
            }

            // This is required to see who skipped the voting
            if (__instance.SkippedVoting)
            {
                __instance.SkippedVoting.SetActive(CheatToggles.revealVotes);
            }
        }
    }

    public static void Postfix(MeetingHud __instance){

        MalumESP.MeetingNametags(__instance);

        // Bugfix: NoClip staying active if meeting is called whilst climbing ladder
        PlayerControl.LocalPlayer.onLadder = false;
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
public static class MeetingHud_PopulateResults
{
    /// <summary>
    /// Prefix patch of MeetingHud.PopulateResults to clear all vote icons before repopulating them for final results
    /// </summary>
    /// <param name="__instance">The <c>MeetingHud</c> instance.</param>
    public static void Prefix(MeetingHud __instance)
    {
        foreach (var votedForArea in __instance.playerStates)
        {
            if (!votedForArea) continue;
            var voteSpreader = votedForArea.transform.GetComponent<VoteSpreader>();
            if (!voteSpreader) continue;
            var length = voteSpreader.Votes.Count;
            if (length == 0) continue;
            foreach (var spriteRenderer in voteSpreader.Votes)
            {
                Object.DestroyImmediate(spriteRenderer);
            }
            voteSpreader.Votes.Clear();
        }
        if (__instance.SkippedVoting)
        {
            var voteSpreader = __instance.SkippedVoting.transform.GetComponent<VoteSpreader>();
            foreach (var spriteRenderer in voteSpreader.Votes)
            {
                Object.DestroyImmediate(spriteRenderer);
            }
            voteSpreader.Votes.Clear();
        }
        MeetingHud_Update.votedPlayers.Clear();
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
public static class MeetingHud_CheckForEndVoting
{
    /// <summary>
    /// Prefix patch of MeetingHud.CheckForEndVoting to make the local player immune to being voted out
    /// </summary>
    /// <param name="__instance">The <c>MeetingHud</c> instance.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(MeetingHud __instance)
    {
        if (!CheatToggles.voteImmune) return true; // We don't need to check whether we are host because this method only runs on the host's side

        if (!__instance.playerStates.All(ps => ps.AmDead || ps.DidVote)) return true;
        var max = __instance.CalculateVotes().MaxPair(out var tie);
        var exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == max.Key);

        // This is the only change from the original method - make sure local player is not exiled
        if (exiled != null && exiled == PlayerControl.LocalPlayer.Data)
        {
            exiled = null;
        }

        var states = new MeetingHud.VoterState[__instance.playerStates.Length];
        for (var index = 0; index < __instance.playerStates.Length; ++index)
        {
            var playerState = __instance.playerStates[index];
            states[index] = new MeetingHud.VoterState
            {
                VoterId = playerState.TargetPlayerId,
                VotedForId = playerState.VotedFor
            };
        }
        __instance.RpcVotingComplete(states, exiled, tie);

        return false;
    }
}
