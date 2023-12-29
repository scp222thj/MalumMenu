using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class HostOnly_GodmodePostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to revive LocalPlayer when CheatSettings.godMode enabled
    public static void Postfix(PlayerPhysics __instance)
    {
        if(CheatSettings.godMode){
            if (__instance.myPlayer.Data.IsDead && __instance.AmOwner){
                __instance.myPlayer.Revive();
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

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class HostOnly_ImpostorHackPostfix
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
            
            if (CheatSettings.impostorHack){
                isActive = true;
                
                //Prevents accidental revival of dead players when turning into impostor
                if (PlayerControl.LocalPlayer.Data.IsDead){
                    
                    DestroyableSingleton<RoleManager>.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                
                } else {
                    
                    DestroyableSingleton<RoleManager>.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.Impostor);
                
                }
            }

        }else{

            //If the cheat toggle is disabled but the hack is still active
            //Disable the hack by resetting to lastRole
            if(!CheatSettings.impostorHack && isActive){

                isActive = false;
                DestroyableSingleton<RoleManager>.Instance.SetRole(PlayerControl.LocalPlayer, (AmongUs.GameOptions.RoleTypes)lastRole);
            
            }
        }

        }catch{};

    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
class HostOnly_ForceStartGamePatch
{
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatSettings.forceStartGame)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                if (GameStates.IsLobby)
                    AmongUsClient.Instance.SendStartGame();
                else
                    HudManager.Instance.Notifier.AddItem("Already in game!"); //Change text to what you like
            }
            else
            {
                HudManager.Instance.Notifier.AddItem("You are not Host!"); //Change text to what you like
            }

            CheatSettings.forceStartGame = false;
        }
    }
}

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
class HostOnly_NoGameEndPatch_CheckEndCriteria
{
    public static bool Prefix(LogicGameFlow __instance)
    {
        if (!AmongUsClient.Instance.AmHost) return true;

        if (CheatSettings.noGameEnd)
            return false;
        //Pls Help add a notifier which only send once in a round
        //Probably put a bool and change it in amongusclient.ongamejoined
        return true;
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.CheckTaskCompletion))]
class HostOnly_NoGameEndPatch_CheckTaskCompletion
{
    public static bool Prefix(ref bool __result)
    {
        if (CheatSettings.noGameEnd)
        {
            __result = false;
            return false;
        }
        return true;
    }
}
