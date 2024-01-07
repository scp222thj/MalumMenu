using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class SeeRoles_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to get colored names in-game 
    public static void Postfix(PlayerPhysics __instance){
        //try-catch to prevent errors when role is null
        try{

            //Get appropriate name color depending on if CheatSettings.seeRoles is enabled
            __instance.myPlayer.cosmetics.SetNameColor(Utils.getColorName(CheatToggles.seeRoles, __instance.myPlayer.Data));
            
        }catch{}
    }
}    

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class SeeRoles_MeetingHud_Update_Postfix
{
    //Postfix patch of MeetingHud.Update to get colored names in meetings
    public static void Postfix(MeetingHud __instance){
        try{
            foreach (PlayerVoteArea playerState in __instance.playerStates)
            {
                //Fetching the GameData.PlayerInfo of each playerState to get the player's role
                GameData.PlayerInfo data = GameData.Instance.GetPlayerById(playerState.TargetPlayerId);

                //Get appropriate name color depending on if CheatSettings.seeRoles is enabled
                playerState.NameText.color = Utils.getColorName(CheatToggles.seeRoles, data);

            }
        }catch{}
    }
}    

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class SeeRoles_ChatBubble_SetName_Postfix
{
    //Postfix patch of ChatBubble.SetName to get colored names in chat messages
    public static void Postfix(ChatBubble __instance){

        //Get appropriate name color depending on if CheatSettings.seeRoles is enabled
        __instance.NameText.color = Utils.getColorName(CheatToggles.seeRoles, __instance.playerInfo);
    
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