using UnityEngine;

namespace MalumMenu;

public class ConsoleTab : ITab
{
    public string name => "Console";

    public void Draw()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    private void DrawGeneral()
    {
        CheatToggles.showConsole = GUILayout.Toggle(CheatToggles.showConsole, " Show Console");

        CheatToggles.logDeaths = GUILayout.Toggle(CheatToggles.logDeaths, " Log Deaths");

        CheatToggles.logShapeshifts = GUILayout.Toggle(CheatToggles.logShapeshifts, " Log Shapeshifts");

        CheatToggles.logVents        = GUILayout.Toggle(CheatToggles.logVents,        " Log Vents");

        CheatToggles.killSoundAlert  = GUILayout.Toggle(CheatToggles.killSoundAlert,  " Sound Alert on Kill");

        CheatToggles.sabotageAlert   = GUILayout.Toggle(CheatToggles.sabotageAlert,   " Sound Alert on Sabotage");

        CheatToggles.bodyIntelLogger = GUILayout.Toggle(CheatToggles.bodyIntelLogger, " Log Who Walked Past Bodies");

        CheatToggles.chatLogger      = GUILayout.Toggle(CheatToggles.chatLogger,      " Chat Logger (MalumChat.txt)");
    }
}
