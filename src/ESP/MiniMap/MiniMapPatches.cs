using HarmonyLib;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
public static class Minimap_ShowMapPostfix
{
	public static bool isActive()
	{
		return CheatToggles.mapCrew || CheatToggles.mapGhosts || CheatToggles.mapImps;
	}
	public static bool miniMap;
	public static List<HerePoint> herePoints = new List<HerePoint>();

	// Postfix patch of MapBehaviour.ShowNormalMap if CheatSettings.miniMap is enabled
	// Creates players icons for each player on the ship
	public static void Postfix(MapBehaviour __instance)
	{
		if (!isActive())
		{
			miniMap = false;
			return;
		}
		else
		{
			miniMap = true;
		}

		__instance.ColorControl.SetColor(Palette.Purple); // Custom map color

		// Destroy old player icons (herePoints)
		try
		{
			herePoints.ForEach(x => UnityEngine.Object.Destroy(x.sprite.gameObject));
			herePoints.Clear();
		}
		catch { }

		// Create new ones for each player
		var temp = new List<HerePoint>();
		foreach (var t in PlayerControl.AllPlayerControls)
		{
			if (!t.AmOwner)
			{ // LocalPlayer is treated normally
				var herePoint = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
				temp.Add(new HerePoint(t, herePoint));
			}
		}
		herePoints = temp;
	}
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
public static class Minimap_FixedUpdatePostfix
{
	public static Color color;
	public static List<HerePoint> herePointsToRemove = new List<HerePoint>();

	// Postfix patch of MapBehaviour.FixedUpdate if CheatSettings.miniMap is enabled
	// Sets each player icon's color and position on the map based on their respective player
	public static void Postfix(MapBehaviour __instance)
	{
		// Resets map if CheatSettings.miniMap changes value
		if (Minimap_ShowMapPostfix.isActive() != Minimap_ShowMapPostfix.miniMap)
		{
			if (!__instance.infectedOverlay.gameObject.active)
			{ // Do not affect sabotage map
				__instance.Close();
				__instance.ShowNormalMap();
			}
		}

		// Sets the position & color of each player icon on the map
		var temp = Minimap_ShowMapPostfix.herePoints;
		foreach (var herePoint in temp)
		{
			try
			{ // try-catch to fix issues caused by player disconnections
				herePoint.sprite.gameObject.SetActive(false); // Initally make player icon invisible

				// Adds alive crewmates
				if (CheatToggles.mapCrew && !herePoint.player.Data.Role.IsImpostor)
				{
					if (!herePoint.player.Data.IsDead)
					{
						herePoint.sprite.gameObject.SetActive(true);
						if (CheatToggles.colorBasedMap)
						{
							color = herePoint.player.Data.Color; // Color-Based Icon
						}
						else
						{
							color = herePoint.player.Data.Role.TeamColor; // Role color based Icon
						}
					}
				}
				else if (CheatToggles.mapImps && herePoint.player.Data.Role.IsImpostor) // Adds alive impostors
				{
					if (!herePoint.player.Data.IsDead)
					{
						herePoint.sprite.gameObject.SetActive(true);
						if (CheatToggles.colorBasedMap)
						{
							color = herePoint.player.Data.Color;
						}
						else
						{
							color = herePoint.player.Data.Role.TeamColor;
						}
					}
				}

				// Any Role, dead
				if (CheatToggles.mapGhosts && herePoint.player.Data.IsDead)
				{
					herePoint.sprite.gameObject.SetActive(true);
					if (CheatToggles.colorBasedMap)
					{
						color = herePoint.player.Data.Color;
					}
					else
					{
						color = Palette.White;
					}
				}

				if (herePoint.sprite.gameObject.active)
				{
					// Set color of active icons
					herePoint.sprite.material.SetColor(PlayerMaterial.BackColor, color);
					herePoint.sprite.material.SetColor(PlayerMaterial.BodyColor, color);
					herePoint.sprite.material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);

					// Set position of active icons
					var vector = herePoint.player.transform.position;
					vector /= ShipStatus.Instance.MapScale;
					vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
					vector.z = -1f;
					herePoint.sprite.transform.localPosition = vector;
				}
			}
			catch
			{
				// Remove icons that are causing problems
				UnityEngine.Object.Destroy(herePoint.sprite.gameObject);
				herePointsToRemove.Add(herePoint);
			}
		}

		foreach (var herePoint in herePointsToRemove)
		{
			Minimap_ShowMapPostfix.herePoints.Remove(herePoint);
		}
	}
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
public static class Minimap_ClosePostfix
{
	// Postfix patch of MapBehaviour.Close to clean up map
	// Simply removes all player icons
	public static void Postfix(MapBehaviour __instance)
	{
		try
		{
			Minimap_ShowMapPostfix.herePoints.ForEach(x => UnityEngine.Object.Destroy(x.sprite.gameObject));
			Minimap_ShowMapPostfix.herePoints.Clear();
		}
		catch { }
	}
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
public static class Minimap_GetMapOptionsPostfix
{
	// Prefix patch of HudManager.Start to give minimap access to impostors too
	public static void Postfix(HudManager __instance)
	{
		__instance.MapButton.OnClick.RemoveAllListeners(); // Remove previous click action

		// Always open normal map when map button is clicked
		// To access sabotage map, sabotage button can be used
		__instance.MapButton.OnClick.AddListener((Action)(() =>
		{
			__instance.ToggleMapVisible(new MapOptions
			{
				Mode = MapOptions.Modes.Normal
			});
		}));
	}
}