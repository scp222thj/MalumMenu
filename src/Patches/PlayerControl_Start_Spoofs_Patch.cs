using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
public static class PlayerControl_Start_Spoofs_Patch
{
    public static void Prefix(PlayerControl __instance)
    {
        if (!__instance.AmOwner) return;
        if (SpoofingService.EnableLevelSpoof)
        {
            SpoofingService.ApplyLevelSpoof();
        }
    }

    public static void Postfix(PlayerControl __instance)
    {
        if (!__instance.AmOwner) return;
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
