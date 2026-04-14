using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(PlayerControl), "CmdCheckShapeshift")]
public static class PlayerControl_CmdCheckShapeshift
{
	public static void Prefix(ref bool shouldAnimate)
	{
		if (shouldAnimate && CheatToggles.noShapeshiftAnim)
		{
			shouldAnimate = false;
		}
	}
}
