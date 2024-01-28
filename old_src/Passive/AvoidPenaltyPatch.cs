using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(StatsManager), nameof(StatsManager.BanMinutesLeft), MethodType.Getter)]
public static class AvoidPenalty_StatsManager_BanMinutesLeft_Getter_Prefix
{
    //Prefix patch of Getter method for StatsManager.BanMinutesLeft to remove disconnect penalty
    public static bool Prefix(StatsManager __instance, ref int __result)
    {
        if (!CheatToggles.avoidBans){
            return true; //Only works if CheatSettings.avoidBans is enabled
        }
        
        __instance.BanPoints = 0f; //Removes all BanPoints
        __result = 0; //Removes all BanMinutes
        return false;
    }
}