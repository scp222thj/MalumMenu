using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
public static class LogicGameFlowNormal_CheckEndCriteria
{
    /// <summary>
    /// Prefix patch of LogicGameFlowNormal.CheckEndCriteria to prevent the game from ending
    /// </summary>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix()
    {
        return !CheatToggles.noGameEnd;
    }
}

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
public static class LogicGameFlowNormal_IsGameOverDueToDeath
{
    /// <summary>
    /// Postfix patch of LogicGameFlowNormal.IsGameOverDueToDeath to prevent the game from ending
    /// </summary>
    /// <param name="__result">Original return value of <c>IsGameOverDueToDeath</c>.</param>
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
    /// <summary>
    /// Prefix patch of LogicGameFlowHnS.CheckEndCriteria to prevent the game from ending
    /// </summary>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix()
    {
        return !CheatToggles.noGameEnd;
    }
}

[HarmonyPatch(typeof(LogicGameFlowHnS), nameof(LogicGameFlowHnS.IsGameOverDueToDeath))]
public static class LogicGameFlowHnS_IsGameOverDueToDeath
{
    /// <summary>
    /// Postfix patch of LogicGameFlowHnS.IsGameOverDueToDeath to prevent the game from ending
    /// </summary>
    /// <param name="__result">Original return value of <c>IsGameOverDueToDeath</c>.</param>
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.noGameEnd){
            __result = false;
        }

    }
}
