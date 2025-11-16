using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(LogicOptions), nameof(LogicOptions.GetAnonymousVotes))]
public static class LogicOptions_GetAnonymousVotes
{
    /// <summary>
    /// Postfix patch of LogicOptions.GetAnonymousVotes to disable anonymous votes for revealVotes cheat
    /// </summary>
    /// <param name="__result">Original return value of <c>GetAnonymousVotes</c>.</param>
    public static void Postfix(ref bool __result){
        if (CheatToggles.revealVotes){
            __result = false;
        }
    }
}

[HarmonyPatch(typeof(LogicOptionsNormal), nameof(LogicOptionsNormal.GetAnonymousVotes))]
public static class LogicOptionsNormal_GetAnonymousVotes
{
    /// <summary>
    /// Postfix patch of LogicOptionsNormal.GetAnonymousVotes to disable anonymous votes for revealVotes cheat
    /// </summary>
    /// <param name="__result">Original return value of <c>GetAnonymousVotes</c>.</param>
    public static void Postfix(ref bool __result){
        if (CheatToggles.revealVotes){
            __result = false;
        }
    }
}
