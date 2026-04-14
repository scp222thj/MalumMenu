using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(LogicOptions), "GetAnonymousVotes")]
public static class LogicOptions_GetAnonymousVotes
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.revealVotes)
		{
			__result = false;
		}
	}
}
