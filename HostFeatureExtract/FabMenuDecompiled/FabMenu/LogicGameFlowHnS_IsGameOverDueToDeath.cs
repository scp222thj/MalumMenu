using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(LogicGameFlowHnS), "IsGameOverDueToDeath")]
public static class LogicGameFlowHnS_IsGameOverDueToDeath
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.noGameEnd)
		{
			__result = false;
		}
	}
}
