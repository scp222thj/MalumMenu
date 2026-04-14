using HarmonyLib;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(MapBehaviour), "FixedUpdate")]
public static class MapBehaviour_FixedUpdate
{
	public static void Postfix(MapBehaviour __instance)
	{
		if (MinimapHandler.isCheatEnabled() != MinimapHandler.minimapActive && !((Component)__instance.infectedOverlay).gameObject.active)
		{
			__instance.Close();
			__instance.ShowNormalMap();
		}
		foreach (HerePoint herePoint in MinimapHandler.herePoints)
		{
			MinimapHandler.handleHerePoint(herePoint);
		}
		foreach (HerePoint item in MinimapHandler.herePointsToRemove)
		{
			MinimapHandler.herePoints.Remove(item);
		}
	}
}
