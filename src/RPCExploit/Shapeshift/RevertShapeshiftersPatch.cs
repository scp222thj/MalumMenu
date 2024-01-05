using HarmonyLib;
using Hazel;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class RevertShapeshifters_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to revert all shapeshifters to their normal outfit
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.revertShapeshifters){

            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected){

                //Each shapeshifted player will be forced to shapeshift into themselves, removing their disguises
                foreach (var sender in PlayerControl.AllPlayerControls)
                {
                    if (sender.CurrentOutfitType != PlayerOutfitType.Default){
                        Utils.ShapeshiftPlayer(sender, sender, false); //Don't animate the shapeshifting 
                                                                       //to avoid player scale bug from affecting other players
                    }
                }

            }

            CheatToggles.revertShapeshifters = false;

        }

        //Bugfix: In case the player is dwarfed after the shapeshift is reverted, this is meant to reset their scale to the default value
        try{
            if (!PlayerControl.LocalPlayer.shapeshifting && PlayerControl.LocalPlayer.transform.localScale != PlayerControl.LocalPlayer.defaultCosmeticsScale){
                PlayerControl.LocalPlayer.cosmetics.SetScale(PlayerControl.LocalPlayer.MyPhysics.Animations.DefaultPlayerScale, PlayerControl.LocalPlayer.defaultCosmeticsScale);
            }
        }catch{}
                
    }
}