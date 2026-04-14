using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(PlayerControl), "CmdCheckMurder")]
public static class PlayerControl_CmdCheckMurder
{
	public static bool Prefix(PlayerControl __instance, PlayerControl target)
	{
		if (!Utils.isHost)
		{
			return true;
		}
		PlayerControl.LocalPlayer.RpcMurderPlayer(target, true);
		return false;
	}
}
