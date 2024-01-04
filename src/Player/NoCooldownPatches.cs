using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckShapeshift))]
public static class NoShapeshifterCooldowns_PlayerControl_CmdCheckShapeshift_Prefix
{
    //Prefix patch of PlayerControl.CmdCheckShapeshift to remove shapeshift animation
    public static bool Prefix(PlayerControl target, bool shouldAnimate, PlayerControl __instance)
    {
        //Skips the original method if animation is enabled & 
        //Invokes it again with animation disabled
        //Only works for LocalPlayer
        if (__instance.AmOwner && shouldAnimate && CheatToggles.noCooldowns_shapeshifter){
            __instance.CmdCheckShapeshift(target, false);
            return false;
        }

        //Once animation has been disabled the original method runs normally
        return true;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckRevertShapeshift))]
public static class NoShapeshifterCooldowns_PlayerControl_CmdCheckRevertShapeshift_Prefix
{
    //Prefix patch of PlayerControl.CmdCheckRevertShapeshift to remove reverse shapeshift animation
    public static bool Prefix(bool shouldAnimate, PlayerControl __instance)
    {
        //Skips the original method if animation is enabled & 
        //Invokes it again with animation disabled
        //Only works for LocalPlayer
        if (__instance.AmOwner && shouldAnimate && CheatToggles.noCooldowns_shapeshifter){
            __instance.CmdCheckRevertShapeshift(false);
            return false;
        }

        //Once animation has been disabled the original method runs normally
        return true;
    }
}

[HarmonyPatch(typeof(ShapeshifterRole), nameof(ShapeshifterRole.FixedUpdate))]
public static class NoShapeshifterCooldowns_ShapeshifterRole_FixedUpdate_Postfix
{

    //Postfix patch of ShapeshifterRole.FixedUpdate to remove shapeshift duration
    public static void Postfix(ShapeshifterRole __instance){
        
        //try-catch to avoid breaking when spectator menu is open
        try{

            if(__instance.Player.AmOwner){

                if (CheatToggles.noCooldowns_shapeshifter){

                    __instance.durationSecondsRemaining = float.MaxValue; //Makes shapeshift duration so incredibly long (float.MaxValue) so that it never ends
                    
                //Once CheatSettings.noCooldowns_shapeshifter is off it checks if the duration is still too long
                //If so it resets it to the appropriate setting
                }else if (__instance.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetShapeshifterDuration()){
                    
                    __instance.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetShapeshifterDuration();
                }

            }

        }catch{};
    }
}

[HarmonyPatch(typeof(EngineerRole), nameof(EngineerRole.FixedUpdate))]
public static class NoEngineerCooldowns_EngineerRole_FixedUpdate_Postfix
{

    //Postfix patch of EngineerRole.FixedUpdate to remove vent time & vent cooldown
    public static void Postfix(EngineerRole __instance){

        if(__instance.Player.AmOwner){

            if (CheatToggles.noCooldowns_engineer){

                __instance.cooldownSecondsRemaining = 0f; //Resets vent cooldown
                __instance.inVentTimeRemaining = float.MaxValue; //Makes vent time so incredibly long (float.MaxValue) so that it never ends
            
            //Once CheatSettings.noCooldowns_engineer is off it checks if vent time is still too long
            //If so it resets it to the appropriate setting
            }else if (__instance.inVentTimeRemaining > GameManager.Instance.LogicOptions.GetEngineerCooldown()){
                
                __instance.inVentTimeRemaining = GameManager.Instance.LogicOptions.GetEngineerCooldown();
            
            }
        }
    }
}

[HarmonyPatch(typeof(ScientistRole), nameof(ScientistRole.Update))]
public static class NoScientistCooldowns_ScientistRole_Update_Postfix
{

    //Postfix patch of ScientistRole.Update to remove vitals battery and cooldown
    public static void Postfix(ScientistRole __instance){

        if(__instance.Player.AmOwner){

            if (CheatToggles.noCooldowns_scientist){

                __instance.currentCooldown = 0f; //Resets vitals cooldown
                __instance.currentCharge = float.MaxValue; //Makes vitals battery so incredibly long (float.MaxValue) so that it never ends

            //Once CheatSettings.noCooldowns_scientist is off it checks if vent time is still too long
            //If so it resets it to the appropriate setting
            }else if (__instance.currentCharge > GameManager.Instance.LogicOptions.GetScientistBatteryCharge()){
                
                __instance.currentCharge = GameManager.Instance.LogicOptions.GetScientistBatteryCharge();
            
            }
        }
    }
}