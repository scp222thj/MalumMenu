using HarmonyLib;
using System.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
public static class MapBehaviour_ShowNormalMap
{
    public static void Postfix(MapBehaviour __instance)
    {
        MinimapHandler.sabotageMapActive = false;
        __instance.ColorControl.SetColor(Palette.Purple);
        __instance.DisableTrackerOverlays();
        MinimapHandler.RefreshHerePoints();
    }
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
public static class MapBehaviour_ShowSabotageMap
{
    public static void Postfix(MapBehaviour __instance)
    {
        MinimapHandler.sabotageMapActive = true;
        MinimapHandler.RefreshHerePoints();
    }
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
public static class MapBehaviour_FixedUpdate
{
    public static void Postfix(MapBehaviour __instance)
    {
        MinimapHandler.RefreshHerePoints();
    }
}

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Close))]
public static class MapBehaviour_Close
{
    public static void Postfix(MapBehaviour __instance)
    {
        try
        {
            MinimapHandler.sabotageMapActive = false;
            MinimapHandler.DestroyHerePoints();
        }
        catch { }
    }
}