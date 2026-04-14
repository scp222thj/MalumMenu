using HarmonyLib;
using InnerNet;

namespace FabMenu;

[HarmonyPatch(typeof(VoteBanSystem), "AddVote")]
public static class VoteBanSystem_AddVote
{
	public static bool Prefix(VoteBanSystem __instance, int srcClient, int clientId)
	{
		if (!Utils.isHost)
		{
			return true;
		}
		if (((InnerNetClient)AmongUsClient.Instance).ClientId == srcClient)
		{
			((InnerNetClient)AmongUsClient.Instance).KickPlayer(clientId, false);
		}
		return false;
	}
}
