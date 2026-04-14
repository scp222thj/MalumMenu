using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(LogicGameFlowNormal), "CheckEndCriteria")]
public static class LogicGameFlowNormal_CheckEndCriteria
{
	public static bool Prefix()
	{
		return !CheatToggles.noGameEnd;
	}
}
