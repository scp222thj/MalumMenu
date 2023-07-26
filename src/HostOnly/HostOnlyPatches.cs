using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class HostOnly_GodmodePostfix
{
    //Postfix patch of PlayerControl.Die to revive LocalPlayer when CheatSettings.godMode enabled
    public static void Postfix(PlayerControl __instance)
    {
        if(CheatSettings.godMode){
            if (__instance.Data.IsDead && __instance.AmOwner){
                __instance.Revive();
            }
        }
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
public static class HostOnly_CastVotePrefix
{
    //Prefix patch of MeetingHud.CastVote
    public static bool Prefix(byte srcPlayerId, byte suspectPlayerId, MeetingHud __instance)
    {
        if (CheatSettings.evilVote){
            //Detects votes from LocalPlayer
            if (PlayerControl.LocalPlayer.PlayerId == srcPlayerId){

                for (int i = 0; i < __instance.playerStates.Length; i++) //Loops through all players
                {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    
                    if (!playerVoteArea.AmDead){ //Dead players shouldn't be able to vote

                        playerVoteArea.VotedFor = suspectPlayerId; //and makes them vote for whoever LocalPlayer voted for (including skipping)
                    
                    }    
                }
                
                __instance.CheckForEndVoting(); //Ends the vote since everyone as voted
                    
                return false; //Skips original method
            }

        } else if (CheatSettings.voteImmune){
            if (PlayerControl.LocalPlayer.PlayerId == suspectPlayerId){
                __instance.CastVote(srcPlayerId, (byte)254); //Redirects all votes to LocalPlayer so that they skip instead
                return false; //Skips original method
            }
        }

        //If no cheat is enabled, original MeetingHud.CastVote is ran
        return true;
    }
}