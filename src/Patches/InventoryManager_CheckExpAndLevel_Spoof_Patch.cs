using AmongUs.Data;
using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.CheckExpAndLevel))]
public static class InventoryManager_CheckExpAndLevel_Spoof_Patch
{
    public static void Postfix()
    {
        if (SpoofingService.EnableLevelSpoof)
        {
            uint spoofed = SpoofingService.GetEffectiveLevel();
            if (DataManager.Player.Stats.Level != spoofed)
            {
                DataManager.Player.Stats.Level = spoofed;
                DataManager.Player.Save();
                Debug.Log($"[SpoofingService] CheckExpAndLevel override: Level {spoofed} saved.");
            }
        }
    }
}
