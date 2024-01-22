using HarmonyLib;
using Sentry.Internal.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class SeeRoles_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to get colored names in-game 
    public static void Postfix(PlayerPhysics __instance){
        //try-catch to prevent errors when role is null
        try
        {

            //Get appropriate name color depending on if CheatSettings.seeRoles is enabled
            __instance.myPlayer.cosmetics.SetName(Utils.getNameTag(__instance.myPlayer, __instance.myPlayer.CurrentOutfit.PlayerName));

        }
        catch { }
    }
}    

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class SeeRoles_MeetingHud_Update_Postfix
{
    //Postfix patch of MeetingHud.Update to get colored names in meetings
    public static void Postfix(MeetingHud __instance){
        //seeRoles code
        try{
            foreach (PlayerVoteArea playerState in __instance.playerStates)
            {
                //Fetching the GameData.PlayerInfo of each playerState to get the player's role
                GameData.PlayerInfo data = GameData.Instance.GetPlayerById(playerState.TargetPlayerId);

                if (!data.IsNull() && !data.Disconnected && !data.Outfits[PlayerOutfitType.Default].IsNull())
                {
                    //Get appropriate name color depending on if CheatSettings.seeRoles is enabled
                    playerState.NameText.text = Utils.getNameTag(data.Object, data.DefaultOutfit.PlayerName);
                }

            }
        }catch{}

        //Bugfix: NoClip staying active if meeting is called whilst climbing ladder
        PlayerControl.LocalPlayer.onLadder = false;
    }
}    

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class SeeRoles_ChatBubble_SetName_Postfix
{
    //Postfix patch of ChatBubble.SetName to get colored names in chat messages
    public static void Postfix(ChatBubble __instance){
        __instance.NameText.text = Utils.getNameTag(__instance.playerInfo.Object, __instance.NameText.text, true);
        __instance.NameText.ForceMeshUpdate(true, true);
        __instance.Background.size = new Vector2(5.52f, 0.2f + __instance.NameText.GetNotDumbRenderedHeight() + __instance.TextArea.GetNotDumbRenderedHeight());
        __instance.MaskArea.size = __instance.Background.size - new Vector2(0f, 0.03f);
    }
}    

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class VentVision_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to make names visible for players inside vents
    //if CheatSettings.ventVision is enabled
    public static void Postfix(PlayerPhysics __instance){
        if (__instance.myPlayer.inVent){
            __instance.myPlayer.cosmetics.nameText.gameObject.SetActive(CheatToggles.ventVision);
        }
    }
}    

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

[HarmonyPatch(typeof(Mushroom), nameof(Mushroom.FixedUpdate))]
public static class SporeVision_Mushroom_FixedUpdate_Postfix
{
    //Postfix patch of Mushroom.FixedUpdate that slightly moves spore clouds on the Z axis when sporeVision is enabled
    //This allows sporeVision users to see players even when they are inside a spore cloud
    public static void Postfix(Mushroom __instance)
    {
        if (CheatToggles.fullBright)
        {
            __instance.sporeMask.transform.position = new UnityEngine.Vector3(__instance.sporeMask.transform.position.x, __instance.sporeMask.transform.position.y, -1);
            return;
        } 

        __instance.sporeMask.transform.position = new UnityEngine.Vector3(__instance.sporeMask.transform.position.x, __instance.sporeMask.transform.position.y, 5f);
    }
}