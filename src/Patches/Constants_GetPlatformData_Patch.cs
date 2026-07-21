using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(Constants), nameof(Constants.GetPlatformData))]
public static class Constants_GetPlatformData_Spoof_Patch
{
    public static void Postfix(ref PlatformSpecificData __result)
    {
        SpoofingService.ApplyPlatformSpoof(__result);
    }
}
