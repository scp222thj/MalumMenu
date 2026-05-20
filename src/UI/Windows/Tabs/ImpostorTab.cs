using UnityEngine;

namespace MalumMenu;

public class ImpostorTab : ITab
{
    public string name => "Impostor";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawKill();

        GUILayout.Space(15);

        DrawMassKill();

        GUILayout.Space(15);

        DrawSmartKill();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        DrawKillMods();

        GUILayout.Space(15);

        DrawShapeshifter();

        GUILayout.Space(15);

        DrawPhantom();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawKill()
    {
        GUILayout.Label("Kill", GUIStylePreset.TabSubtitle);

        CheatToggles.killPlayer = GUILayout.Toggle(CheatToggles.killPlayer, " Kill Player");

        CheatToggles.telekillPlayer = GUILayout.Toggle(CheatToggles.telekillPlayer, " Telekill Player");

        CheatToggles.killAnyone = GUILayout.Toggle(CheatToggles.killAnyone, " Kill Anyone");

        CheatToggles.killVanished = GUILayout.Toggle(CheatToggles.killVanished, " Kill While Vanished");
    }

    private void DrawMassKill()
    {
        GUILayout.Label("Mass Kill", GUIStylePreset.TabSubtitle);

        CheatToggles.killAllCrew = GUILayout.Toggle(CheatToggles.killAllCrew, " Kill All Crewmates");

        CheatToggles.killAllImps = GUILayout.Toggle(CheatToggles.killAllImps, " Kill All Impostors");

        CheatToggles.killAll = GUILayout.Toggle(CheatToggles.killAll, " Kill Everyone");
    }

    private void DrawSmartKill()
    {
        GUILayout.Label("Smart Kill", GUIStylePreset.TabSubtitle);

        var (canExecute, statusText) = MalumCheats.SmartKillStatus();

        var prevColor = GUI.contentColor;
        GUI.contentColor = canExecute ? Color.green : new Color(1f, 0.4f, 0.4f);
        GUILayout.Label(statusText);
        GUI.contentColor = prevColor;

        GUI.enabled = canExecute;
        if (GUILayout.Button("Close Door + Kill + Vent", GUIStylePreset.NormalButton))
            MalumCheats.SmartKillCombo();
        GUI.enabled = true;
    }

    private void DrawKillMods()
    {
        GUILayout.Label("Kill Mods", GUIStylePreset.TabSubtitle);

        CheatToggles.killReach = GUILayout.Toggle(CheatToggles.killReach, " Infinite Kill Range");
    }

    private void DrawShapeshifter()
    {
        GUILayout.Label("Shapeshifter", GUIStylePreset.TabSubtitle);

        CheatToggles.noShapeshiftAnim = GUILayout.Toggle(CheatToggles.noShapeshiftAnim, " No Shapeshift Animation");

        CheatToggles.endlessSsDuration = GUILayout.Toggle(CheatToggles.endlessSsDuration, " Endless Shapeshift Duration");
    }

    private void DrawPhantom()
    {
        GUILayout.Label("Phantom / Viper", GUIStylePreset.TabSubtitle);

        CheatToggles.endlessVanish = GUILayout.Toggle(CheatToggles.endlessVanish, " Endless Vanish");

        CheatToggles.noVanishAnim = GUILayout.Toggle(CheatToggles.noVanishAnim, " No Vanish Animation");
    }
}
