using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
public static class LobbyBehaviour_Start_Spoof_Patch
{
    public static void Postfix()
    {
        if (SpoofingService.EnableLevelSpoof)
        {
            SpoofingService.ApplyLevelSpoof();
        }
        if (SpoofingService.EnableFriendCodeSpoof)
        {
            SpoofingService.ApplyFriendCodeSpoof();
        }
        if (SpoofingService.IsAnyShuffleEnabled())
        {
            SpoofingService.ApplyIdentityShuffle();
        }
    }
}
