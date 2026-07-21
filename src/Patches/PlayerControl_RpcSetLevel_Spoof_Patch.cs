using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetLevel))]
public static class PlayerControl_RpcSetLevel_Spoof_Patch
{
    public static void Prefix(ref uint level)
    {
        if (SpoofingService.EnableLevelSpoof)
        {
            uint spoofed = SpoofingService.GetEffectiveLevel();
            Debug.Log($"[SpoofingService] RpcSetLevel intercepted: {level} -> {spoofed}");
            level = spoofed;
        }
    }
}
