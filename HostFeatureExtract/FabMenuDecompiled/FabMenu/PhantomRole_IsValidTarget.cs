using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(PhantomRole), "IsValidTarget")]
public static class PhantomRole_IsValidTarget
{
	public static void Postfix(NetworkedPlayerInfo target, ref bool __result)
	{
		if (CheatToggles.killVanished)
		{
			__result = Utils.isValidTarget(target);
		}
	}
}
