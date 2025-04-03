using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(LogicOptions), nameof(LogicOptions.GetAnonymousVotes))]
public static class LogicOptions_GetAnonymousVotes
{
    // Postfix patch to disable anonymous voting when revealVotes is enabled
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.revealVotes)
        {
            __result = false;
        }
    }
}

[HarmonyPatch(typeof(LogicOptionsNormal), nameof(LogicOptionsNormal.GetAnonymousVotes))]
public static class LogicOptionsNormal_GetAnonymousVotes
{
    // Postfix patch to ensure compatibility across different LogicOptions variants
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.revealVotes)
        {
            __result = false;
        }
    }
}