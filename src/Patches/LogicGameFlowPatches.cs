using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
public static class LogicGameFlowNormal_CheckEndCriteria
{
    // Prefix patch of LogicGameFlowNormal.CheckEndCriteria to prevent a running game from ending
    public static bool Prefix()
    {
        return !CheatToggles.noGameEnd;
    }
}

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
public static class LogicGameFlowNormal_IsGameOverDueToDeath
{
    // Postfix patch of LogicGameFlowNormal.IsGameOverDueToDeath to ensure the game does not stall
    // after an exile that should have triggered game over
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.noGameEnd){
            __result = false;
        }

    }
}

[HarmonyPatch(typeof(LogicGameFlowHnS), nameof(LogicGameFlowHnS.CheckEndCriteria))]
public static class LogicGameFlowHnS_CheckEndCriteria
{
    // Prefix patch of LogicGameFlowHnS.CheckEndCriteria to prevent a running HnS game from ending
    public static bool Prefix()
    {
        return !CheatToggles.noGameEnd;
    }
}

[HarmonyPatch(typeof(LogicGameFlowHnS), nameof(LogicGameFlowHnS.IsGameOverDueToDeath))]
public static class LogicGameFlowHnS_IsGameOverDueToDeath
{
    // Postfix patch of LogicGameFlowNormal.IsGameOverDueToDeath to ensure the HnS game does not stall
    // after an exile that should have triggered game over
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.noGameEnd){
            __result = false;
        }

    }
}
