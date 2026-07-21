using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlatformSpecificData), nameof(PlatformSpecificData.Serialize))]
public static class PlatformSpecificData_Serialize_Spoof_Patch
{
    public static void Prefix(PlatformSpecificData __instance)
    {
        SpoofingService.ApplyPlatformSpoof(__instance);
    }
}
