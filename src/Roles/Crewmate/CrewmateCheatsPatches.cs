using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class CrewmateCheats_PlayerPhysics_LateUpdate_Postfix
{

    // Postfix patch of PlayerPhysics.LateUpdate to complete all of the user's tasks
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.completeMyTasks)
        {
            Utils.CompleteAllTasks();
            CheatToggles.completeMyTasks = false;
        }
    }
}