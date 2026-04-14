using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(VoteBanSystem), "CmdAddVote")]
public static class VoteBanSystem_CmdAddVote
{
	public static bool Prefix(int clientIdToVoteBan)
	{
		return !Utils.isHost;
	}
}
