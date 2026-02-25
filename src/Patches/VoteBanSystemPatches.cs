using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.AddVote))]
public static class VoteBanSystem_AddVote
{
    // Prefix patch of VoteBanSystem.AddVote to instantly kick players when host votes to kick them
    public static bool Prefix(VoteBanSystem __instance, int srcClient, int clientId)
    {
        if (!Utils.isHost) return true;

        if (AmongUsClient.Instance.ClientId == srcClient)
        {
            AmongUsClient.Instance.KickPlayer(clientId, false);
        }

        return false;
    }
}

[HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.CmdAddVote))]
public static class VoteBanSystem_CmdAddVote
{
    // Prefix patch of VoteBanSystem.CmdAddVote to prevent AddVoteBan RPC from being sent when host votes to kick a player
    public static bool Prefix()
    {
        return !Utils.isHost;
    }
}
