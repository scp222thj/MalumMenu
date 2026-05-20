using UnityEngine;

namespace MalumMenu;

public class ConfigTab : ITab
{
    public string name => "Config";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawResize();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawGeneral()
    {
        CheatToggles.openConfig = GUILayout.Toggle(CheatToggles.openConfig, " Open Config");

        CheatToggles.reloadConfig = GUILayout.Toggle(CheatToggles.reloadConfig, " Reload Config");

        CheatToggles.saveProfile = GUILayout.Toggle(CheatToggles.saveProfile, " Save to Profile");

        CheatToggles.loadProfile = GUILayout.Toggle(CheatToggles.loadProfile, " Load from Profile");
    }

    private void DrawResize()
    {
        GUILayout.Space(15);
        GUILayout.Label($"Menu Height: {MenuUI.windowHeight}");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-", GUIStylePreset.NormalButton, GUILayout.Width(40))) MenuUI.windowHeight = System.Math.Max(350, MenuUI.windowHeight - 50);
        if (GUILayout.Button("+", GUIStylePreset.NormalButton, GUILayout.Width(40))) MenuUI.windowHeight = System.Math.Min(900, MenuUI.windowHeight + 50);
        GUILayout.EndHorizontal();
    }
}
