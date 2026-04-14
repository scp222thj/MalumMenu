using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppSystem;
using Sentry.Internal.Extensions;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(TrackerRole), "FindClosestTarget")]
public static class TrackerRole_FindClosestTarget
{
	public static bool Prefix(TrackerRole __instance, ref PlayerControl __result)
	{
		if (!CheatToggles.trackReach)
		{
			return true;
		}
		List<PlayerControl> list = (from player in Utils.getPlayersSortedByDistance()
			where !MiscExtensions.IsNull((Object)(object)player) && ((RoleBehaviour)__instance).IsValidTarget(player.Data) && ((Behaviour)player.Collider).enabled
			select player).ToList();
		__result = list[0];
		return false;
	}
}
