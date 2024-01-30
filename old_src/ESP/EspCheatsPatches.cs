using HarmonyLib;
using Sentry.Internal.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
public static class RevealVotes_MeetingHud_BloopAVoteIcon_Prefix
{
    //Prefix patch of MeetingHud.BloopAVoteIcon to show all anonymous votes
	//Basically does what the original method did with the required modifications
    public static bool Prefix(GameData.PlayerInfo voterPlayer, int index, Transform parent, MeetingHud __instance){
        if (!CheatToggles.revealVotes){
            return true; //Run original method if CheatSettings.seeAnon is turned off
        }

        SpriteRenderer spriteRenderer = Object.Instantiate<SpriteRenderer>(__instance.PlayerVotePrefab);

		PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer); // Skip check for GameManager.Instance.LogicOptions.GetAnonymousVotes()
		
        spriteRenderer.transform.SetParent(parent);
		spriteRenderer.transform.localScale = Vector3.zero;
		PlayerVoteArea component = parent.GetComponent<PlayerVoteArea>();
		if (component != null)
		{
			spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);
		}
		__instance.StartCoroutine(Effects.Bloop((float)index * 0.3f, spriteRenderer.transform, 1f, 0.5f));
		parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
        
        return false; //Skip original method
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class RevealVotes_MeetingHud_Update_Prefix
{
    public static List<int> votedPlayers = new List<int>(); //avoid duplicate votes
    //Prefix patch of MeetingHud.Update to show live vote
    public static bool Prefix(MeetingHud __instance)
    {
        if (__instance.state < MeetingHud.VoteStates.Results)
        {
            foreach (var playerVoteArea in __instance.playerStates)
            {
                var playerData = GameData.Instance.GetPlayerById(playerVoteArea.TargetPlayerId);
                if (!playerData.Disconnected && playerVoteArea.VotedFor != PlayerVoteArea.HasNotVoted && playerVoteArea.VotedFor != PlayerVoteArea.MissedVote && playerVoteArea.VotedFor != PlayerVoteArea.DeadVote && !votedPlayers.Contains(playerVoteArea.TargetPlayerId))
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

        return true;
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
public static class RevealVotes_MeetingHud_PopulateResults_Prefix
{
    public static bool Prefix(MeetingHud __instance)
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
            var length = voteSpreader.Votes.Count;
            foreach (var spriteRenderer in voteSpreader.Votes)
            {
                Object.DestroyImmediate(spriteRenderer);
            }
            voteSpreader.Votes.Clear();
        }
        RevealVotes_MeetingHud_Update_Prefix.votedPlayers.Clear();
        return true;
    }
}