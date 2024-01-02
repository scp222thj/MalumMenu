using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
class NoBlackoutPatch
{
    public static void Postfix(ref bool __result)
    {
        if (CheatSettings.noBlackOut)
            __result = false;
    }
}
