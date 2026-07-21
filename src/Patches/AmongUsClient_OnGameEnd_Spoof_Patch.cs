using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
public static class AmongUsClient_OnGameEnd_Spoof_Patch
{
    public static void Postfix()
    {
        SpoofingService.MarkForReapplication();
    }
}
