using UnityEngine;

namespace MalumMenu;

public class ESPTab : ITab
{
    public string name => "ESP";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawCamera();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        DrawTracers();

        GUILayout.Space(15);

        DrawMinimap();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawGeneral()
    {
        CheatToggles.seePlayerInfo = GUILayout.Toggle(CheatToggles.seePlayerInfo, " See Player Info");

        DrawMinimapToggle(ref CheatToggles.mapCrew, " Crewmates");

        DrawMinimapToggle(ref CheatToggles.mapImps, " Impostors");

        DrawMinimapToggle(ref CheatToggles.mapGhosts, " Ghosts");

        DrawMinimapToggle(ref CheatToggles.colorBasedMap, " Color-based");
    }

    private static void DrawMinimapToggle(ref bool toggleValue, string label)
    {
        var newValue = GUILayout.Toggle(toggleValue, label);
        if (newValue == toggleValue) return;

        toggleValue = newValue;
        MinimapHandler.RefreshHerePoints();

        CheatToggles.seeLobbyInfo = GUILayout.Toggle(CheatToggles.seeLobbyInfo, " See Lobby Info");
    }

    private void DrawCamera()
    {
        GUILayout.Label("Camera", GUIStylePreset.TabSubtitle);

        CheatToggles.zoomOut = GUILayout.Toggle(CheatToggles.zoomOut, " Zoom Out");

        CheatToggles.spectate = GUILayout.Toggle(CheatToggles.spectate, " Spectate");

        CheatToggles.freecam = GUILayout.Toggle(CheatToggles.freecam, " Freecam");
    }

    private void DrawTracers()
    {
        GUILayout.Label("Tracers", GUIStylePreset.TabSubtitle);

        CheatToggles.tracersCrew = GUILayout.Toggle(CheatToggles.tracersCrew, " Crewmates");

        CheatToggles.tracersImps = GUILayout.Toggle(CheatToggles.tracersImps, " Impostors");

        CheatToggles.tracersGhosts = GUILayout.Toggle(CheatToggles.tracersGhosts, " Ghosts");

        CheatToggles.tracersBodies = GUILayout.Toggle(CheatToggles.tracersBodies, " Dead Bodies");

        CheatToggles.colorBasedTracers = GUILayout.Toggle(CheatToggles.colorBasedTracers, " Color-based");

        CheatToggles.distanceBasedTracers = GUILayout.Toggle(CheatToggles.distanceBasedTracers, " Distance-based");
    }

    private void DrawMinimap()
    {
        GUILayout.Label("Minimap", GUIStylePreset.TabSubtitle);

        CheatToggles.mapSabotage = GUILayout.Toggle(CheatToggles.mapSabotage, " Show in Sabotage Map");

        CheatToggles.mapCrew = GUILayout.Toggle(CheatToggles.mapCrew, " Crewmates");

        CheatToggles.mapImps = GUILayout.Toggle(CheatToggles.mapImps, " Impostors");

        CheatToggles.mapGhosts = GUILayout.Toggle(CheatToggles.mapGhosts, " Ghosts");

    }
}
