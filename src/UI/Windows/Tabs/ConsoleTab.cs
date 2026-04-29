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

        CheatToggles.logGameState  = GUILayout.Toggle(CheatToggles.logGameState, " Log Game State");
    }
}
