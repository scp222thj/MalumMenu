using HarmonyLib;
using System.Collections.Generic;
namespace MalumMenu;
[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
public static class MapBehaviour_ShowNormalMap
{
    // Postfix patch of MapBehaviour.ShowNormalMap to spawn herePoint icons for each player
    public static void Postfix(MapBehaviour __instance)
    {
        MinimapHandler.minimapActive = MinimapHandler.IsCheatEnabled();
        if (!MinimapHandler.minimapActive)
        {
            return; // Only runs if miniMap Cheat is enabled
        }
        __instance.ColorControl.SetColor(Palette.Purple); // Custom map color
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
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.AmOwner) // LocalPlayer is always treated normally
            {
                var herePoint = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
                temp.Add(new HerePoint(player, herePoint));
            }
        }
        MinimapHandler.herePoints = temp;
    }
}
[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
public static class MapBehaviour_FixedUpdate
{
    // Postfix patch of MapBehaviour.FixedUpdate to update each herePoint icon's color and position on the map based on their respective player
    public static void Postfix(MapBehaviour __instance)
    {
        bool cheatEnabled = MinimapHandler.IsCheatEnabled();

        // Spawn herePoints if they don't exist and the cheat is enabled (handles both normal and sabotage maps)
        if (cheatEnabled && MinimapHandler.herePoints.Count == 0)
        {
            var temp = new List<HerePoint>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.AmOwner)
                {
                    var herePoint = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
                    temp.Add(new HerePoint(player, herePoint));
                }
            }
            MinimapHandler.herePoints = temp;
        }

        // Clean up herePoints if cheat got disabled
        if (!cheatEnabled && MinimapHandler.herePoints.Count > 0)
        {
            try
            {
                MinimapHandler.herePoints.ForEach(x => UnityEngine.Object.Destroy(x.sprite.gameObject));
                MinimapHandler.herePoints.Clear();
            }
            catch { }
        }

        MinimapHandler.minimapActive = cheatEnabled;

        // Properly handles each herePoint icon on the map
        if (cheatEnabled)
        {
            var herePoints = MinimapHandler.herePoints;
            foreach (var herePoint in herePoints)
            {
                MinimapHandler.HandleHerePoint(herePoint);
            }

            foreach (var herePoint in MinimapHandler.herePointsToRemove)
            {
                MinimapHandler.herePoints.Remove(herePoint);
            }
        }
    }
}
[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
public static class MapBehaviour_Close
{
    // Postfix patch of MapBehaviour.Close to clean up all herePoint icons
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