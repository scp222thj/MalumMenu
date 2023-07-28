using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Player_NoClipPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate that disables player collider when NoClipping
    public static void Postfix(PlayerPhysics __instance)
    {
        try{

        PlayerControl.LocalPlayer.Collider.enabled = !CheatSettings.noClip;

        }catch{}

    }
}

//This patch is currently inactive, but it used to change a crewmate role to an impostor role
//when CheatSettings.impostorHack was toggled
[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Player_ImpostorHackPostfix
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
public static class Player_SpeedBoostPostfix
{

    //Postfix patch of PlayerPhysics.LateUpdate to double player speed
    public static void Postfix(PlayerPhysics __instance)
    {
        //try-catch to avoid some errors I was reciving in the logs related to this cheat
        try{

        //PlayerControl.LocalPlayer.MyPhysics.Speed is the base speed of a player
        //Among Us uses this value with the associated game setting to calculate the TrueSpeed of the player
        if(CheatSettings.speedBoost){
            PlayerControl.LocalPlayer.MyPhysics.Speed = 2.5f * 2;
        }else{
            PlayerControl.LocalPlayer.MyPhysics.Speed = 2.5f; //By default, Speed is always 2.5f
        }

        }catch{}
    }
}