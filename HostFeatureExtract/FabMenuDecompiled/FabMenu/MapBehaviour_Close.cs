using HarmonyLib;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(MapBehaviour), "Close")]
public static class MapBehaviour_Close
{
	public static void Postfix(MapBehaviour __instance)
	{
		try
		{
			MinimapHandler.herePoints.ForEach(delegate(HerePoint x)
			{
				Object.Destroy((Object)(object)((Component)x.sprite).gameObject);
			});
			MinimapHandler.herePoints.Clear();
		}
		catch
		{
		}
	}
}
