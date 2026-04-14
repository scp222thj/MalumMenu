using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(LogicGameFlowHnS), "CheckEndCriteria")]
public static class LogicGameFlowHnS_CheckEndCriteria
{
	public static bool Prefix()
	{
		return !CheatToggles.noGameEnd;
	}
}
