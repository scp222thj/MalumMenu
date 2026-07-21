using AmongUs.Data;
using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.Update))]
public static class InnerNetClient_Update_Spoof_Patch
{
    private static float _lastCheck;

    public static void Postfix()
    {
        if (Time.time - _lastCheck < 2f) return;
        _lastCheck = Time.time;

        try
        {
            if (SpoofingService.EnableLevelSpoof)
            {
                if (DataManager.Player?.Stats != null)
                {
                    uint spoofed = SpoofingService.GetEffectiveLevel();
                    if (DataManager.Player.Stats.Level != spoofed)
                    {
                        DataManager.Player.Stats.Level = spoofed;
                        DataManager.Player.Save();
                    }
                }
            }

            if (SpoofingService.EnableFriendCodeSpoof && PlayerControl.LocalPlayer != null)
            {
                SpoofingService.ApplyFriendCodeSpoof();
            }
        }
        catch { }
    }
}
