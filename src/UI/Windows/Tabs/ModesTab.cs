using MalumMenu.UI.Elements;
using MalumMenu.UI.Utilities.Structures;
using UnityEngine;

namespace MalumMenu.UI.Windows.Tabs;

public class ModesTab : ITab
{
    public string name => "Modes";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.EndVertical();
    }

    private void DrawGeneral()
    {
        CheatToggles.rgbMode = GUILayout.Toggle(CheatToggles.rgbMode, " RGB Mode");

        CheatToggles.stealthMode = GUILayout.Toggle(CheatToggles.stealthMode, " Stealth Mode");

        CheatToggles.panicMode = GUILayout.Toggle(CheatToggles.panicMode, " Panic Mode");
    }
}
