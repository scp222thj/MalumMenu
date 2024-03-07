using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(LogicOptions), nameof(LogicOptions.GetAnonymousVotes))]
public static class LogicOptions_GetAnonymousVotes
{
    // Postfix patch of LogicOptions.GetAnonymousVotes to disable anonymous votes for revealVotes cheat
    public static void Postfix(ref bool __result){
        if (CheatToggles.revealVotes){
            __result = false;
        }
    }
}

[HarmonyPatch(typeof(LogicOptionsNormal), nameof(LogicOptionsNormal.GetAnonymousVotes))]
public static class LogicOptionsNormal_GetAnonymousVotes
{
    // Postfix patch of LogicOptionsNormal.GetAnonymousVotes to disable anonymous votes for revealVotes cheat
    public static void Postfix(ref bool __result){
        if (CheatToggles.revealVotes){
            __result = false;
        }
    }
}