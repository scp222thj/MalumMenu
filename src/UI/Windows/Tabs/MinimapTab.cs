using UnityEngine;

namespace MalumMenu;

public class MinimapTab : ITab
{
    public string name => "Minimap";

    public void Draw()
    {
        GUILayout.Label("Window", GUIStylePreset.TabSubtitle);

        CheatToggles.minimapAlwaysOn = GUILayout.Toggle(CheatToggles.minimapAlwaysOn, " Always-On Minimap");
        if (CheatToggles.minimapAlwaysOn)
        {
            var scale = AlwaysOnMinimapController.scale;
            GUILayout.Label($" Scale: {scale:0.00}");
            AlwaysOnMinimapController.scale = GUILayout.HorizontalSlider(scale, 0.15f, 0.75f);

            GUILayout.Label(" Drag the minimap to move it (while menu is open).");
        }

        GUILayout.Space(10);

        GUILayout.Label("Players", GUIStylePreset.TabSubtitle);

        CheatToggles.mapCrew = GUILayout.Toggle(CheatToggles.mapCrew, " Crewmates");
        CheatToggles.mapImps = GUILayout.Toggle(CheatToggles.mapImps, " Impostors");
        CheatToggles.mapGhosts = GUILayout.Toggle(CheatToggles.mapGhosts, " Ghosts");
        CheatToggles.colorBasedMap = GUILayout.Toggle(CheatToggles.colorBasedMap, " Color-based");

        GUILayout.Space(10);

        GUILayout.Label("Extras", GUIStylePreset.TabSubtitle);

        CheatToggles.mapImpsHighlight = GUILayout.Toggle(CheatToggles.mapImpsHighlight, " Highlight Impostors (Red)");

        CheatToggles.mapTrails = GUILayout.Toggle(CheatToggles.mapTrails, " Player Trails");
        if (CheatToggles.mapTrails)
        {
            var seconds = MinimapHandler.trailSeconds;
            GUILayout.Label($" Trail Duration: {seconds:0}s");
            var newSeconds = GUILayout.HorizontalSlider(seconds, 5f, 60f);
            if (Mathf.Abs(newSeconds - seconds) > 0.001f)
            {
                MinimapHandler.trailSeconds = newSeconds;
            }
        }
    }
}
