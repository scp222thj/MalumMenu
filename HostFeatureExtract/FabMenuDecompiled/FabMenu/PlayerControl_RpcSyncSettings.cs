using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(PlayerControl), "RpcSyncSettings")]
public static class PlayerControl_RpcSyncSettings
{
	public static bool Prefix(PlayerControl __instance, byte[] optionsByteArray)
	{
		return !CheatToggles.noOptionsLimits;
	}
}
