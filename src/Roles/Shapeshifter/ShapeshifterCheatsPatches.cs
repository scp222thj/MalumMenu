using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckShapeshift))]
public static class ShapeshifterCheats_PlayerControl_CmdCheckShapeshift_Postfix
{
    //Prefix patch of PlayerControl.CmdCheckShapeshift to bypass anticheat & prevent SS animation
    public static bool Prefix(PlayerControl __instance, PlayerControl target, bool shouldAnimate){

        if (!target || target.Data == null || __instance.Data.Disconnected)
		{
			return false;
		}

        //Only animate the transformation if the noShapeshiftAnim cheat is disabled
        if (shouldAnimate && CheatToggles.noShapeshiftAnim){
            shouldAnimate = false;
        }

        //Use custom util to bypass anticheat
		Utils.ShapeshiftPlayer(__instance, target, shouldAnimate);

        return false; //The original method is skipped

    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckRevertShapeshift))]
public static class ShapeshifterCheats_PlayerControl_CmdCheckRevertShapeshift_Postfix
{
    //Prefix patch of PlayerControl.CmdCheckRevertShapeshift to bypass anticheat & prevent SS animation
    public static bool Prefix(PlayerControl __instance, bool shouldAnimate){

        if (!__instance || __instance.Data == null || __instance.Data.Disconnected)
		{
			return false;
		}

        //Only animate the transformation if the noShapeshiftAnim cheat is disabled
        if (shouldAnimate && CheatToggles.noShapeshiftAnim){
            shouldAnimate = false;
        }

        //Use custom util to bypass anticheat
		Utils.ShapeshiftPlayer(__instance, __instance, shouldAnimate);

        return false; //The original method is skipped

    }
}

[HarmonyPatch(typeof(ShapeshifterRole), nameof(ShapeshifterRole.FixedUpdate))]
public static class endlessSsDuration_ShapeshifterRole_FixedUpdate_Postfix
{

    //Postfix patch of ShapeshifterRole.FixedUpdate to have endless shapeshift duration
    public static void Postfix(ShapeshifterRole __instance){
        
        //try-catch to avoid breaking when spectator menu is open
        try{

            if(__instance.Player.AmOwner){

                if (CheatToggles.endlessSsDuration){

                    __instance.durationSecondsRemaining = float.MaxValue; //Makes shapeshift duration so incredibly long (float.MaxValue) so that it never ends
                    
                //Makes sure shapeshift duration is resetted to normal value after the cheat is disabled
                }else if (__instance.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetShapeshifterDuration()){
                    
                    __instance.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetShapeshifterDuration();
                
                }

            }

        }catch{};
    }
}

[HarmonyPatch(typeof(ShapeshifterRole), nameof(ShapeshifterRole.FixedUpdate))]
public static class noSsCooldown_ShapeshifterRole_FixedUpdate_Postfix
{

    //Postfix patch of ShapeshifterRole.FixedUpdate to remove shapeshift cooldown
    public static void Postfix(ShapeshifterRole __instance){
        
        //try-catch to avoid breaking when spectator menu is open
        try{

            if(__instance.Player.AmOwner){

                if (CheatToggles.noSsCooldown){

                    __instance.cooldownSecondsRemaining = 0f;
                    
                }

            }

        }catch{};
    }
}