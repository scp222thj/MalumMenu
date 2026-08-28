using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MalumMenu;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class MeetingHud_Update
{
    public static List<int> votedPlayers = new List<int>();

    public static void Prefix(MeetingHud __instance)
    {
        if (__instance.state < MeetingHud.MeetingStates.Results)
        {
            foreach (var playerVoteArea in __instance.playerStates)
            {
                if (!playerVoteArea) continue;

                var playerData = GameData.Instance.GetPlayerById(playerVoteArea.PlayerId);

                if (playerData != null && !playerData.Disconnected && playerVoteArea.VotedForId != PlayerVoteArea.HasNotVoted && playerVoteArea.VotedForId != PlayerVoteArea.MissedVote && playerVoteArea.VotedForId != PlayerVoteArea.DeadVote && !votedPlayers.Contains(playerVoteArea.PlayerId))
                {
                    votedPlayers.Add(playerVoteArea.PlayerId);
                    RevealVote(__instance, playerVoteArea, playerData);
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

    public static void Postfix(MeetingHud __instance)
    {
        MalumESP.MeetingNametags(__instance);

        PlayerControl.LocalPlayer.onLadder = false;
    }

    public static void RevealVote(MeetingHud __instance, PlayerVoteArea area, NetworkedPlayerInfo voter)
    {
        if (!CheatToggles.revealVotes) return;

        Transform parent = null;

        if (area.VotedForId == PlayerVoteArea.SkippedVote)
        {
            parent = __instance.SkippedVoting ? __instance.SkippedVoting.transform : null;
        }
        else
        {
            foreach (var a in __instance.playerStates)
            {
                if (a.PlayerId == area.VotedForId)
                {
                    parent = a.transform;
                    break;
                }
            }
        }

        if (parent == null) return;

        __instance.BloopAVoteIcon(voter, 0, parent);

        var voteSpreader = parent.GetComponent<VoteSpreader>();
        if (voteSpreader && voteSpreader.Votes != null)
        {
            foreach (var spriteRenderer in voteSpreader.Votes)
            {
                if (spriteRenderer) spriteRenderer.gameObject.SetActive(true);
            }
        }
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
public static class MeetingHud_PopulateResults
{
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
    public static bool Prefix(MeetingHud __instance)
    {
        if (!CheatToggles.voteImmune) return true;

        if (!__instance.playerStates.All(ps => ps.AmDead || ps.DidVote)) return true;

        var max = __instance.CalculateVotes().MaxPair(out var tie);
        var exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == max.Key);

        bool wasOverruled = false;
        ushort overruleNonce = 0;
        JudgeOverrule judgeOverrule;
        NetworkedPlayerInfo networkedPlayerInfo;
        NetworkedPlayerInfo networkedPlayerInfo2;

        if (__instance.TryGetWinningOverrule(out judgeOverrule, out networkedPlayerInfo, out networkedPlayerInfo2))
        {
            wasOverruled = true;
            overruleNonce = judgeOverrule.OverruleNonce;

            if (networkedPlayerInfo2.Role.TeamType == RoleTeamTypes.Impostor)
            {
                exiled = GameData.Instance.GetPlayerById(judgeOverrule.OverruledPlayerId);
            }
            else
            {
                exiled = networkedPlayerInfo;
            }
        }

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
                VoterId = playerState.PlayerId,
                VotedForId = playerState.VotedForId
            };
        }

        __instance.RpcVotingComplete(states, exiled, tie, wasOverruled, overruleNonce);

        return false;
    }
}

[HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Deserialize))]
public static class PlayerVoteArea_Deserialize
{
    public static void Postfix(PlayerVoteArea __instance)
    {
        if (MeetingHud.Instance == null || !CheatToggles.revealVotes) return;
        if (__instance.VotedForId == PlayerVoteArea.HasNotVoted || __instance.VotedForId == PlayerVoteArea.MissedVote || __instance.VotedForId == PlayerVoteArea.DeadVote) return;
        if (MeetingHud_Update.votedPlayers.Contains((int)__instance.PlayerId)) return;
        if (!DestroyableSingleton<GameData>.InstanceExists || PlayerControl.LocalPlayer == null) return;
        var voter = GameData.Instance.GetPlayerById(__instance.PlayerId);
        if (voter == null || voter.Disconnected) return;
        MeetingHud_Update.votedPlayers.Add((int)__instance.PlayerId);
        MeetingHud_Update.RevealVote(MeetingHud.Instance, __instance, voter);
    }
}
