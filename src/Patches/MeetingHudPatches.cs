using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class MeetingHud_Update
{
    public static List<int> votedPlayers = new List<int>();

    // Prefix patch of MeetingHud.Update to constantly bloop new vote icons for 
    // each new vote being cast during the meeting
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

            if (__instance.SkippedVoting)
            {
                __instance.SkippedVoting.SetActive(CheatToggles.revealVotes);
            }
        }
    }

    public static void Postfix(MeetingHud __instance){

        MalumESP.meetingNametags(__instance);

        // Bugfix: NoClip staying active if meeting is called whilst climbing ladder
        PlayerControl.LocalPlayer.onLadder = false;
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
public static class MeetingHud_PopulateResults
{
    // Prefix patch of MeetingHud.PopulateResults to clear all vote icons before repopulating them for final results
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