using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.AddVote))]
public static class VoteBanSystem_AddVote
{
    /// <summary>
    /// Prefix patch of VoteBanSystem.AddVote to instantly kick players when host votes to kick them
    /// </summary>
    /// <param name="__instance">The <c>VoteBanSystem</c> instance.</param>
    /// <param name="srcClient">The client ID of the player who is voting.</param>
    /// <param name="clientId">The client ID of the player being voted to kick.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(VoteBanSystem __instance, int srcClient, int clientId)
    {
        if (!Utils.IsHost) return true;

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
    /// <summary>
    /// Prefix patch of VoteBanSystem.CmdAddVote to prevent AddVoteBan RPC from being sent when host votes to kick a player
    /// </summary>
    /// <param name="clientIdToVoteBan">The client ID of the player being voted to kick.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(int clientIdToVoteBan)
    {
        return !Utils.IsHost;
    }
}
