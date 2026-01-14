using HarmonyLib;
using System.Linq;
using Sentry.Internal.Extensions;

namespace MalumMenu;

[HarmonyPatch(typeof(EngineerRole), nameof(EngineerRole.FixedUpdate))]
public static class EngineerRole_FixedUpdate
{
    public static void Postfix(EngineerRole __instance){

        if(__instance.Player.AmOwner){

            MalumCheats.engineerCheats(__instance);
        }
    }
}

[HarmonyPatch(typeof(ShapeshifterRole), nameof(ShapeshifterRole.FixedUpdate))]
public static class ShapeshifterRole_FixedUpdate
{
    public static void Postfix(ShapeshifterRole __instance){

        try{
            if(__instance.Player.AmOwner){

                MalumCheats.shapeshifterCheats(__instance);
            }
        }catch{}
    }
}

[HarmonyPatch(typeof(ScientistRole), nameof(ScientistRole.Update))]
public static class ScientistRole_Update
{

    public static void Postfix(ScientistRole __instance){

        if(__instance.Player.AmOwner){

            MalumCheats.scientistCheats(__instance);
        }
    }
}

[HarmonyPatch(typeof(TrackerRole), nameof(TrackerRole.FixedUpdate))]
public static class TrackerRole_FixedUpdate
{

    public static void Postfix(TrackerRole __instance){

        if(__instance.Player.AmOwner){

            MalumCheats.trackerCheats(__instance);
        }
    }
}

[HarmonyPatch(typeof(PhantomRole), nameof(PhantomRole.FixedUpdate))]
public static class PhantomRole_FixedUpdate
{

    public static void Postfix(PhantomRole __instance){

        if(__instance.Player.AmOwner){

            MalumCheats.phantomCheats(__instance);
        }
    }
}

[HarmonyPatch(typeof(PhantomRole), nameof(PhantomRole.IsValidTarget))]
public static class PhantomRole_IsValidTarget
{
    /// <summary>
    /// Postfix patch of PhantomRole.IsValidTarget to allow killing while invisible
    /// </summary>
    /// <param name="target">The target player info.</param>
    /// <param name="__result">Original return value of <c>IsValidTarget</c>.</param>
    public static void Postfix(NetworkedPlayerInfo target, ref bool __result){

        if (CheatToggles.killVanished){
            __result = Utils.isValidTarget(target);
        }
    }
}

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
public static class ImpostorRole_IsValidTarget
{
    /// <summary>
    /// Postfix patch of ImpostorRole.IsValidTarget to allow forbidden kill targets for killAnyone cheat
    /// Allows killing ghosts (with seeGhosts), impostors, players in vents, etc...
    /// </summary>
    /// <param name="target">The target player info.</param>
    /// <param name="__result">Original return value of <c>IsValidTarget</c>.</param>
    public static void Postfix(NetworkedPlayerInfo target, ref bool __result){

        if (CheatToggles.killAnyone){
           __result = Utils.isValidTarget(target);
        }

    }
}

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.FindClosestTarget))]
public static class ImpostorRole_FindClosestTarget
{
    /// <summary>
    /// Prefix patch of ImpostorRole.FindClosestTarget to allow for infinite kill reach
    /// </summary>
    /// <param name="__instance">The <c>ImpostorRole</c> instance.</param>
    /// <param name="__result">The closest valid target.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(ImpostorRole __instance, ref PlayerControl __result){
        if (!CheatToggles.killReach) return true;
        var playerList = Utils.getPlayersSortedByDistance().Where(player => !player.IsNull() && __instance.IsValidTarget(player.Data) && player.Collider.enabled).ToList();

        __result = playerList[0];

        return false;

    }
}

[HarmonyPatch(typeof(DetectiveRole), nameof(DetectiveRole.FindClosestTarget))]
public static class DetectiveRole_FindClosestTarget
{
    /// <summary>
    /// Prefix patch of DetectiveRole.FindClosestTarget to allow for infinite interrogate reach
    /// </summary>
    /// <param name="__instance">The <c>DetectiveRole</c> instance.</param>
    /// <param name="__result">The closest valid target.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(DetectiveRole __instance, ref PlayerControl __result)
    {
        if (!CheatToggles.interrogateReach) return true;
        var playerList = Utils.getPlayersSortedByDistance().Where(player => !player.IsNull() && __instance.IsValidTarget(player.Data) && player.Collider.enabled).ToList();

        __result = playerList[0];

        return false;
    }
}

[HarmonyPatch(typeof(TrackerRole), nameof(TrackerRole.FindClosestTarget))]
public static class TrackerRole_FindClosestTarget
{
    /// <summary>
    /// Prefix patch of TrackerRole.FindClosestTarget to allow for infinite track reach
    /// </summary>
    /// <param name="__instance">The <c>TrackerRole</c> instance.</param>
    /// <param name="__result">The closest valid target.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(TrackerRole __instance, ref PlayerControl __result)
    {
        if (!CheatToggles.trackReach) return true;
        var playerList = Utils.getPlayersSortedByDistance().Where(player => !player.IsNull() && __instance.IsValidTarget(player.Data) && player.Collider.enabled).ToList();

        __result = playerList[0];

        return false;
    }
}
