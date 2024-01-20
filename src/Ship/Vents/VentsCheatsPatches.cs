using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class WalkInVents_PlayerPhysics_LateUpdate_Postfix
{

    //Postfix patch of PlayerPhysics.LateUpdate to allow movement and interactions from within vents
    //if CheatSettings.walkVent is enabled
    public static void Postfix(PlayerPhysics __instance)
    {
        try{

        if (PlayerControl.LocalPlayer.inVent && CheatToggles.walkVent){
            PlayerControl.LocalPlayer.inVent = false;
            PlayerControl.LocalPlayer.moveable = true;
        }

        }catch{}

    }
}

//This patch contains some old commented-out code that I might re-add in the future
//However I removed it because it slowed down the operation
[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class KickVents_ShipStatus_FixedUpdate_Postfix
{

    //Postfix patch of ShipStatus.FixedUpdate that loops through all vents and kicks players within
    public static void Postfix(ShipStatus __instance)
    {
        if (CheatToggles.kickVents){

            //var ventSystem = __instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();

            foreach(var vent in ShipStatus.Instance.AllVents){

                //if (ventSystem.IsImpostorInsideVent(vent.Id)){

                VentilationSystem.Update(VentilationSystem.Operation.BootImpostors, vent.Id); //Unlike PlayerPhysics.RpcBootFromVent, this code also works for clients
                
                //}

            }

            CheatToggles.kickVents = false; //Button behaviour
        }
    }
}