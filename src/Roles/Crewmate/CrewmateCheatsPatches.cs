using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class CrewmateCheats_PlayerPhysics_LateUpdate_Postfix
{

    //Postfix patch of EngineerRole.FixedUpdate to remove vent cooldown & have endless vent time
    public static void Postfix(PlayerPhysics __instance){

        if (CheatToggles.completeMyTasks){
            Utils.CompleteAllTasks();

            CheatToggles.completeMyTasks = false;
        }
    }
}