using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class GodMode_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to revive LocalPlayer when CheatSettings.godMode enabled
    public static void Postfix(PlayerPhysics __instance)
    {
        if(CheatToggles.godMode){
            if (__instance.myPlayer.Data.IsDead && __instance.AmOwner){
                __instance.myPlayer.Revive();
            }
        }
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
public static class VotingCheats_MeetingHud_CastVote_Prefix
{
    //Prefix patch of MeetingHud.CastVote
    public static bool Prefix(byte srcPlayerId, byte suspectPlayerId, MeetingHud __instance)
    {
        if (CheatToggles.evilVote){
            //Detects votes from LocalPlayer
            if (PlayerControl.LocalPlayer.PlayerId == srcPlayerId){

                for (int i = 0; i < __instance.playerStates.Length; i++) //Loops through all players
                {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    
                    if (!playerVoteArea.AmDead){ //Dead players shouldn't be able to vote

                        playerVoteArea.VotedFor = suspectPlayerId; //Makes them vote for whoever LocalPlayer voted for (including skipping)
                    
                    }    
                }
                
                __instance.CheckForEndVoting(); //Ends the vote since everyone as voted
                    
                return false; //Skips original method
            }

        } else if (CheatToggles.voteImmune){
            if (PlayerControl.LocalPlayer.PlayerId == suspectPlayerId){
                __instance.CastVote(srcPlayerId, (byte)254); //Redirects all votes to LocalPlayer so that they skip instead
                return false; //Skips original method
            }
        }

        //If no cheat is enabled, original MeetingHud.CastVote is ran
        return true;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ImpostorHack_PlayerPhysics_LateUpdate_Postfix
{
    public static int lastRole;
    public static bool isActive;

    //Postfix patch of PlayerPhysics.LateUpdate that turns player into an impostor
    //when CheatSettings.impostorHack is enabled
    public static void Postfix(PlayerPhysics __instance)
    {
        //try-catch to prevent errors when role is null
        try{

        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor){ //Only works if the player is not already an impostor
            
            lastRole = (int)PlayerControl.LocalPlayer.Data.RoleType; //Saves role before the modification
            
            if (CheatToggles.impostorHack){
                isActive = true;
                
                //Prevents accidental revival of dead players when turning into impostor
                if (PlayerControl.LocalPlayer.Data.IsDead){
                    
                    PlayerControl.LocalPlayer.RpcSetRole(AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    
                    //Old system (Fake Role):
                    //DestroyableSingleton<RoleManager>.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                
                } else {
                    
                    PlayerControl.LocalPlayer.RpcSetRole(AmongUs.GameOptions.RoleTypes.Impostor);
                    
                    //Old system (Fake Role):
                    //DestroyableSingleton<RoleManager>.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.Impostor);
                
                }
            }

        }else{

            //If the cheat toggle is disabled but the hack is still active
            //Disable the hack by resetting to lastRole
            if(!CheatToggles.impostorHack && isActive){

                isActive = false;
                PlayerControl.LocalPlayer.RpcSetRole((AmongUs.GameOptions.RoleTypes)lastRole);
                
                //Old system (Fake Role):
                //DestroyableSingleton<RoleManager>.Instance.SetRole(PlayerControl.LocalPlayer, (AmongUs.GameOptions.RoleTypes)lastRole);
            
            }
        }

        }catch{};

    }
}