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

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawGeneral()
    {
        CheatToggles.seePlayerInfo = GUILayout.Toggle(CheatToggles.seePlayerInfo, " See Player Info");

        CheatToggles.seeRoles = GUILayout.Toggle(CheatToggles.seeRoles, " See Roles");

        CheatToggles.seeGhosts = GUILayout.Toggle(CheatToggles.seeGhosts, " See Ghosts");

        CheatToggles.noShadows = GUILayout.Toggle(CheatToggles.noShadows, " No Shadows");

        CheatToggles.taskArrows = GUILayout.Toggle(CheatToggles.taskArrows, " Task Arrows");

        CheatToggles.revealVotes = GUILayout.Toggle(CheatToggles.revealVotes, " Reveal Votes");

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

}
