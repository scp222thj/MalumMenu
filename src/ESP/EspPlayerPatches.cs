using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ESP_PlayerPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to set colored names for roles in-game 
    public static void Postfix(PlayerPhysics __instance){
        //try-catch to prevent errors when role is null
        try{

            if (CheatSettings.seeRoles){
                
                __instance.myPlayer.cosmetics.SetNameColor(__instance.myPlayer.Data.Role.TeamColor); //seeRoles Vision

            }else if(PlayerControl.LocalPlayer.Data.Role.IsImpostor){

                __instance.myPlayer.cosmetics.SetNameColor(__instance.myPlayer.Data.Role.NameColor); //Normal Impostor Vision

            }else {

                __instance.myPlayer.cosmetics.SetNameColor(PlayerControl.LocalPlayer.Data.Role.NameColor); //Normal Crewmate Vision
            }
            
        }catch{}
    }
}    

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class ESP_MeetingHudPostfix
{
    //Postfix patch of MeetingHud.Update to set colored names for roles in meetings
    public static void Postfix(MeetingHud __instance){
        foreach (PlayerVoteArea playerState in __instance.playerStates)
        {
            //Fetching the GameData.PlayerInfo of each playerState to get the player's role
            GameData.PlayerInfo data = GameData.Instance.GetPlayerById(playerState.TargetPlayerId);

            if (CheatSettings.seeRoles){
                
                playerState.NameText.color = data.Role.TeamColor; //seeRoles Vision

            }else if(PlayerControl.LocalPlayer.Data.Role.IsImpostor){

                playerState.NameText.color = data.Role.NameColor; //Normal Impostor Vision

            }else {

                playerState.NameText.color = PlayerControl.LocalPlayer.Data.Role.NameColor; //Normal Crewmate Vision
            }
        }
    }
}    

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class ESP_ChatBubblePostfix
{
    //Postfix patch of ChatBubble.SetName to set colored names for roles in chat messages
    public static void Postfix(ChatBubble __instance){
        if (CheatSettings.seeRoles){
                
            __instance.NameText.color = __instance.playerInfo.Role.TeamColor; //seeRoles Vision

        }else if(PlayerControl.LocalPlayer.Data.Role.IsImpostor){

            __instance.NameText.color = __instance.playerInfo.Role.NameColor; //Normal Impostor Vision

        }else {

            __instance.NameText.color = PlayerControl.LocalPlayer.Data.Role.NameColor; //Normal Crewmate Vision
        }
    }
}    