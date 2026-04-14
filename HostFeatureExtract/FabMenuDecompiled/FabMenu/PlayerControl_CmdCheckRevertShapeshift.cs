using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(PlayerControl), "CmdCheckRevertShapeshift")]
public static class PlayerControl_CmdCheckRevertShapeshift
{
	public static void Prefix(ref bool shouldAnimate)
	{
		if (shouldAnimate && CheatToggles.noShapeshiftAnim)
		{
			shouldAnimate = false;
		}
	}
}
