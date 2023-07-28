using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ESP_PlayerPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to get colored names in-game 
    public static void Postfix(PlayerPhysics __instance){
        //try-catch to prevent errors when role is null
        try{

            //Get appropriate name color depending on if CheatSettings.seeRoles is enabled
            __instance.myPlayer.cosmetics.SetNameColor(Utils.getColorName(CheatSettings.seeRoles, __instance.myPlayer.Data));
            
        }catch{}
    }
}    

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class ESP_MeetingHudPostfix
{
    //Postfix patch of MeetingHud.Update to get colored names in meetings
    public static void Postfix(MeetingHud __instance){
        foreach (PlayerVoteArea playerState in __instance.playerStates)
        {
            //Fetching the GameData.PlayerInfo of each playerState to get the player's role
            GameData.PlayerInfo data = GameData.Instance.GetPlayerById(playerState.TargetPlayerId);

            //Get appropriate name color depending on if CheatSettings.seeRoles is enabled
            playerState.NameText.color = Utils.getColorName(CheatSettings.seeRoles, data);

        }
    }
}    

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class ESP_ChatBubblePostfix
{
    //Postfix patch of ChatBubble.SetName to get colored names in chat messages
    public static void Postfix(ChatBubble __instance){

        //Get appropriate name color depending on if CheatSettings.seeRoles is enabled
        __instance.NameText.color = Utils.getColorName(CheatSettings.seeRoles, __instance.playerInfo);
    
    }
}    