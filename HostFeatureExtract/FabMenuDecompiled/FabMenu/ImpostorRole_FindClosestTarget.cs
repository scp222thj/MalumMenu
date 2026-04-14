using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppSystem;
using Sentry.Internal.Extensions;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(ImpostorRole), "FindClosestTarget")]
public static class ImpostorRole_FindClosestTarget
{
	public static bool Prefix(ImpostorRole __instance, ref PlayerControl __result)
	{
		if (!CheatToggles.killReach)
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
