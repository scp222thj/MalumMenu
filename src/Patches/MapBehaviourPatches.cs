using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
public static class MapBehaviour_ShowNormalMap
{
    // Postfix patch to spawn herePoint icons for each non-local player
    public static void Postfix(MapBehaviour __instance)
    {
        MinimapHandler.minimapActive = MinimapHandler.isCheatEnabled();
        if (!MinimapHandler.minimapActive) return;

        __instance.ColorControl.SetColor(Palette.Purple); // Custom map color 😎
        __instance.DisableTrackerOverlays();

        // Destroy old herePoints
        foreach (var point in MinimapHandler.herePoints)
        {
            if (point?.sprite?.gameObject != null)
                Object.Destroy(point.sprite.gameObject);
        }
        MinimapHandler.herePoints.Clear();

        // Create new herePoints
        var newPoints = new List<HerePoint>();
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.AmOwner)
            {
                var icon = Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
                newPoints.Add(new HerePoint(player, icon));
            }
        }
        MinimapHandler.herePoints = newPoints;
    }
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
public static class MapBehaviour_FixedUpdate
{
    // Updates each herePoint icon's position and color per player state
    public static void Postfix(MapBehaviour __instance)
    {
        if (MinimapHandler.isCheatEnabled() != MinimapHandler.minimapActive)
        {
            if (!__instance.infectedOverlay.gameObject.activeSelf)
            {
                __instance.Close();
                __instance.ShowNormalMap();
                return;
            }
        }

        foreach (var point in MinimapHandler.herePoints)
        {
            MinimapHandler.handleHerePoint(point);
        }

        foreach (var pointToRemove in MinimapHandler.herePointsToRemove)
        {
            MinimapHandler.herePoints.Remove(pointToRemove);
        }

        MinimapHandler.herePointsToRemove.Clear();
    }
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
public static class MapBehaviour_Close
{
    // Clean up icons when map closes
    public static void Postfix(MapBehaviour __instance)
    {
        foreach (var point in MinimapHandler.herePoints)
        {
            if (point?.sprite?.gameObject != null)
                Object.Destroy(point.sprite.gameObject);
        }
        MinimapHandler.herePoints.Clear();
    }
}
