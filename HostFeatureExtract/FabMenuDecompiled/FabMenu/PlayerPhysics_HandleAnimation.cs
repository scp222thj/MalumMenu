using HarmonyLib;
using InnerNet;

namespace FabMenu;

[HarmonyPatch(typeof(PlayerPhysics), "HandleAnimation")]
public static class PlayerPhysics_HandleAnimation
{
	public static bool Prefix(PlayerPhysics __instance, ref bool amDead)
	{
		if (CheatToggles.moonwalk && ((InnerNetObject)__instance).AmOwner)
		{
			return false;
		}
		return true;
	}
}
