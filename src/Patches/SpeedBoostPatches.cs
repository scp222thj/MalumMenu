using HarmonyLib;
using MalumMenu;
using UnityEngine;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetNormalizedVelocity))]
public static class SpeedBoostCheatPatch
{
    public static bool Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] Vector2 direction)
    {
        __instance.body.velocity = direction * TrueSpeed(__instance);
        return false;
    }

    public static float TrueSpeed(PlayerPhysics instance)
    {
        float realSpeed = instance.Speed;
        if (CheatToggles.speedBoost)
        {
            realSpeed *= 1.8f;
        }
        return realSpeed * instance.SpeedMod;
    }
}