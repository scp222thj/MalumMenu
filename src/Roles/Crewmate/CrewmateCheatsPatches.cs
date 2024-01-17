using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class CrewmateCheats_PlayerPhysics_LateUpdate_Postfix
{

    //Postfix patch of EngineerRole.FixedUpdate to remove vent cooldown & have endless vent time
    public static void Postfix(PlayerPhysics __instance){

        if (CheatToggles.completeMyTasks){
            Utils.CompleteAllTasks(PlayerControl.LocalPlayer);

            CheatToggles.completeMyTasks = false;
        }

        if (CheatToggles.completeAllTasks){

            foreach (PlayerControl item in PlayerControl.AllPlayerControls){
                Utils.CompleteAllTasks(item);
            }

            CheatToggles.completeAllTasks = false;
        }
    }
}

[HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
public static class doTasksAsGhost_CrewmateRole_CanUse_Prefix
{
    //Prefix patch of Console.CanUse to do tasks as a ghost
    //Can fix sabotage as a ghost
    public static bool Prefix(Console __instance)
    {
        __instance.GhostsIgnored = !CheatToggles.doTasksAsGhost;
        return true;
    }
}