using UnityEngine;

namespace MalumMenu;

public class ConsoleTab : ITab
{
    public string name => "Console";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.EndVertical();
    }

    private void DrawGeneral()
    {
        CheatToggles.showConsole = GUILayout.Toggle(CheatToggles.showConsole, " Show Console");

        CheatToggles.logDeaths = GUILayout.Toggle(CheatToggles.logDeaths, " Log Deaths");

        CheatToggles.logShapeshifts = GUILayout.Toggle(CheatToggles.logShapeshifts, " Log Shapeshifts");

        CheatToggles.logVents = GUILayout.Toggle(CheatToggles.logVents, " Log Vents");

        CheatToggles.logRooms = GUILayout.Toggle(CheatToggles.logRooms, " Log Rooms");
        if (CheatToggles.logRooms)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            CheatToggles.logRoomsCrew = GUILayout.Toggle(CheatToggles.logRoomsCrew, " Log Crewmates");
            CheatToggles.logRoomsImps = GUILayout.Toggle(CheatToggles.logRoomsImps, " Log Impostors");
            CheatToggles.logRoomsGhosts = GUILayout.Toggle(CheatToggles.logRoomsGhosts, " Log Ghosts");
            CheatToggles.logRoomsTarget = GUILayout.Toggle(CheatToggles.logRoomsTarget, " Log Target");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}
