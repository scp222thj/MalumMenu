using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(FullAccount), "CanSetCustomName")]
public static class FullAccount_CanSetCustomName
{
	public static void Prefix(ref bool canSetName)
	{
		if (CheatToggles.unlockFeatures)
		{
			canSetName = true;
		}
	}
}
