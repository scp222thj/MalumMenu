using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(PlayerControl), "TurnOnProtection")]
public static class PlayerControl_TurnOnProtection
{
	public static void Prefix(ref bool visible)
	{
		if (CheatToggles.seeGhosts)
		{
			visible = true;
		}
	}
}
