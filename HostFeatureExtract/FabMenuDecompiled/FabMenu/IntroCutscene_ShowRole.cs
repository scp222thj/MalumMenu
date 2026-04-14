using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(IntroCutscene), "ShowRole")]
public static class IntroCutscene_ShowRole
{
	public static bool Prefix()
	{
		return !CheatToggles.skipRoleReveal;
	}
}
