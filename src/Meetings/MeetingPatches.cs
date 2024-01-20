using HarmonyLib;
using UnityEngine;
using Hazel;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class Meetings_MainPostfix
{
    //Postfix patch of ShipStatus.FixedUpdate
    public static void Postfix(ShipStatus __instance)
    {
        if(CheatSettings.closeMeeting){
            
            if (MeetingHud.Instance){ //Closes MeetingHud window if it is open
                MeetingHud.Instance.DespawnOnDestroy = false;
                ExileController exileController = UnityEngine.Object.Instantiate<ExileController>(ShipStatus.Instance.ExileCutscenePrefab);
                UnityEngine.Object.Destroy(MeetingHud.Instance.gameObject);
                exileController.ReEnableGameplay();
                exileController.WrapUp();
            }
            
            CheatSettings.closeMeeting = false; //Button behaviour
        }
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
public static class Meetings_VoteIconPrefix
{
    //Prefix patch of MeetingHud.BloopAVoteIcon to show all anonymous votes
	//Basically does what the original method did with the required modifications
    public static bool Prefix(GameData.PlayerInfo voterPlayer, int index, Transform parent, MeetingHud __instance){
        if (!CheatSettings.revealVotes){
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