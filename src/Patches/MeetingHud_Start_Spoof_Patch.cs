using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public static class MeetingHud_Start_Spoof_Patch
{
    public static void Postfix()
    {
        if (SpoofingService.IsAnyShuffleEnabled())
        {
            SpoofingService.ApplyIdentityShuffle();
        }
    }
}
