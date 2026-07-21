using System.Collections.Generic;
using HarmonyLib;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.AddVote))]
internal static class VoteBanIncomingNotifyPatch
{
    private static readonly HashSet<int> _myVoteSources = new();
    private static int _lastGameId;

    [HarmonyPostfix]
    internal static void Postfix(int srcClient, int clientId)
    {
        try
        {
            AmongUsClient aucClient = AmongUsClient.Instance;
            if (aucClient == null) return;

            int myClientId = aucClient.ClientId;
            if (clientId != myClientId || srcClient == myClientId) return;

            int gid = aucClient.GameId;
            if (gid != _lastGameId) { _myVoteSources.Clear(); _lastGameId = gid; }

            if (_myVoteSources.Add(srcClient))
            {
                int count = _myVoteSources.Count;
                string srcName = "Unknown";
                try
                {
                    ClientData c = aucClient.GetClient(srcClient);
                    if (c != null && !string.IsNullOrEmpty(c.PlayerName)) srcName = c.PlayerName;
                }
                catch { }
                NotifyUtils.Warning($"Vote ban warning: {srcName} voted to ban you ({count}/3)");
            }
        }
        catch { }
    }
}
