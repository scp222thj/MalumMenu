using System.Collections.Generic;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(MapBehaviour), "ShowNormalMap")]
public static class MapBehaviour_ShowNormalMap
{
	public static void Postfix(MapBehaviour __instance)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		MinimapHandler.minimapActive = MinimapHandler.isCheatEnabled();
		if (!MinimapHandler.minimapActive)
		{
			return;
		}
		__instance.ColorControl.SetColor(Palette.Purple);
		__instance.DisableTrackerOverlays();
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
		List<HerePoint> list = new List<HerePoint>();
		Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl current = enumerator.Current;
			if (!((InnerNetObject)current).AmOwner)
			{
				SpriteRenderer sprite = Object.Instantiate<SpriteRenderer>(__instance.HerePoint, ((Component)__instance.HerePoint).transform.parent);
				list.Add(new HerePoint(current, sprite));
			}
		}
		MinimapHandler.herePoints = list;
	}
}
