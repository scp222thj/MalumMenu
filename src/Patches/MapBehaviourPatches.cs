using HarmonyLib;
using System.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
public static class MapBehaviour_ShowNormalMap
{
    /// <summary>
    /// Postfix patch of MapBehaviour.ShowNormalMap to spawn herePoint icons for each player
    /// </summary>
    /// <param name="__instance">The <c>MapBehaviour</c> instance.</param>
    public static void Postfix(MapBehaviour __instance)
    {
        MinimapHandler.minimapActive = MinimapHandler.isCheatEnabled();

        if (!MinimapHandler.minimapActive) {
            return; // Only runs if miniMap Cheat is enabled
        }

        __instance.ColorControl.SetColor(Palette.Purple); // Custom map color 😎

        __instance.DisableTrackerOverlays();

        // Destroy old player icons (herePoints)
        try
        {
            MinimapHandler.herePoints.ForEach(x => UnityEngine.Object.Destroy(x.sprite.gameObject));
            MinimapHandler.herePoints.Clear();
        }
        catch { }

        // & create new ones for each player
        var temp = new List<HerePoint>();
        foreach (var t in PlayerControl.AllPlayerControls)
        {
            if (!t.AmOwner){ // LocalPlayer is always treated normally

                var herePoint = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);

                temp.Add(new HerePoint(t, herePoint));
            }
        }
        MinimapHandler.herePoints = temp;

    }
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
public static class MapBehaviour_FixedUpdate
{
    /// <summary>
    /// Postfix patch of MapBehaviour.FixedUpdate to update each herePoint icon's color and position on the map based on their respective player
    /// </summary>
    /// <param name="__instance">The <c>MapBehaviour</c> instance.</param>
    public static void Postfix(MapBehaviour __instance)
    {
        // Reset map if miniMap cheat is disabled
        if (MinimapHandler.isCheatEnabled() != MinimapHandler.minimapActive){
            if (!__instance.infectedOverlay.gameObject.active){ // Do not affect sabotage map
                __instance.Close();
                __instance.ShowNormalMap();
            }
        }

        // Properly handles each herePoint icon on the map
        var temp = MinimapHandler.herePoints;
        foreach (var herePoint in temp)
        {
            MinimapHandler.handleHerePoint(herePoint);
        }

        foreach (var herePoint in MinimapHandler.herePointsToRemove)
        {
            MinimapHandler.herePoints.Remove(herePoint);
        }

    }
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
public static class MapBehaviour_Close
{
    /// <summary>
    /// Postfix patch of MapBehaviour.Close to clean up all herePoint icons
    /// </summary>
    /// <param name="__instance">The <c>MapBehaviour</c> instance.</param>
    public static void Postfix(MapBehaviour __instance)
    {
        try
        {
            MinimapHandler.herePoints.ForEach(x => UnityEngine.Object.Destroy(x.sprite.gameObject));
            MinimapHandler.herePoints.Clear();
        }

        catch { }
    }
}
