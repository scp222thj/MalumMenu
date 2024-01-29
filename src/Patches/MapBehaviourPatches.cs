using HarmonyLib;
using System.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowNormalMap))]
public static class MapBehaviour_ShowNormalMap
{
    //Postfix patch of MapBehaviour.ShowNormalMap if CheatSettings.miniMap is enabled
    //Creates players icons for each player on the ship
    public static void Postfix(MapBehaviour __instance)
    {
        MinimapHandler.minimapActive = MinimapHandler.isCheatEnabled();
        if (!MinimapHandler.minimapActive) {
            return;
        }

        __instance.ColorControl.SetColor(Palette.Purple); //Custom map color 😎

        //Destroy old player icons (herePoints)
        try
            {
                MinimapHandler.herePoints.ForEach(x => UnityEngine.Object.Destroy(x.sprite.gameObject));
                MinimapHandler.herePoints.Clear();
            }
        catch { }

        //& create new ones for each player
        var temp = new List<HerePoint>();
        foreach (var t in PlayerControl.AllPlayerControls)
        {
            if (!t.AmOwner){ //LocalPlayer is treated normally

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
    //Postfix patch of MapBehaviour.FixedUpdate if CheatSettings.miniMap is enabled
    //Sets each player icon's color and position on the map based on their respective player
    public static void Postfix(MapBehaviour __instance)
    {
        //Resets map if CheatSettings.miniMap changes value
        if (MinimapHandler.isCheatEnabled() != MinimapHandler.minimapActive){
            if (!__instance.infectedOverlay.gameObject.active){ //Do not affect sabotage map
                __instance.Close();
                __instance.ShowNormalMap();
            }
        }

        //Sets the position & color of each player icon on the map
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
    //Postfix patch of MapBehaviour.Close to clean up map
    //Simply removes all player icons
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