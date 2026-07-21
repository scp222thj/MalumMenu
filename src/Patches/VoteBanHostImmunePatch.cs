using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.AddVote))]
internal static class VoteBanHostImmunePatch
{
    internal static bool Prefix(int srcClient, int clientId)
    {
        try
        {
            if (!AmongUsClient.Instance || !AmongUsClient.Instance.AmHost)
                return true;

            if (clientId != AmongUsClient.Instance.ClientId)
                return true;

            if (srcClient == AmongUsClient.Instance.ClientId)
                return true;

            return false;
        }
        catch
        {
            return true;
        }
    }
}
