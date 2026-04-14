using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(LogicOptionsNormal), "GetAnonymousVotes")]
public static class LogicOptionsNormal_GetAnonymousVotes
{
	public static void Postfix(ref bool __result)
	{
		if (CheatToggles.revealVotes)
		{
			__result = false;
		}
	}
}
