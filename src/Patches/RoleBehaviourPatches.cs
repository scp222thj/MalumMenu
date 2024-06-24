using HarmonyLib;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sentry.Internal.Extensions;
using AmongUs.GameOptions;

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
    // Prefix patch of PhantomRole.IsValidTarget to allow killing while invisible
    public static void Postfix(NetworkedPlayerInfo target, ref bool __result){

        if (CheatToggles.killVanished){
            __result = Utils.isValidTarget(target);
        }
    }
}

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
public static class ImpostorRole_IsValidTarget
{
    // Prefix patch of ImpostorRole.IsValidTarget to allow forbidden kill targets for killAnyone cheat
    // Allows killing ghosts (with seeGhosts), impostors, players in vents, etc...
    public static void Postfix(NetworkedPlayerInfo target, ref bool __result){

        if (CheatToggles.killAnyone){
           __result = Utils.isValidTarget(target);
        }

    }
}

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.FindClosestTarget))]
public static class ImpostorRole_FindClosestTarget
{
    // Prefix patch of ImpostorRole.FindClosestTarget to allow for infinite kill reach
    public static bool Prefix(ImpostorRole __instance, ref PlayerControl __result){

        if (CheatToggles.killReach){

            List<PlayerControl> playerList = Utils.getPlayersSortedByDistance().Where(player => !player.IsNull() && __instance.IsValidTarget(player.Data) && player.Collider.enabled).ToList();

            __result = playerList[0];

            return false;

        }

        return true;
    }
}