using UnityEngine;

namespace MalumMenu;

public class RolesTab : ITab
{
    public string name => "Roles";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawImpostor();

        GUILayout.Space(15);

        DrawShapeshifter();

        GUILayout.Space(15);

        DrawCrewmate();

        GUILayout.Space(15);

        DrawTracker();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        DrawEngineer();

        GUILayout.Space(15);

        DrawScientist();

        GUILayout.Space(15);

        DrawDetective();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawGeneral()
    {
        CheatToggles.setFakeRole = GUILayout.Toggle(CheatToggles.setFakeRole, " Set Fake Role");
        CheatToggles.setFakeAlive = GUILayout.Toggle(CheatToggles.setFakeAlive, " Set Fake Alive");
    }

    private void DrawImpostor()
    {
        GUILayout.Label("Impostor", GUIStylePreset.TabSubtitle);

        CheatToggles.killReach = GUILayout.Toggle(CheatToggles.killReach, " Kill Reach");

        // CheatToggles.impostorTasks = GUILayout.Toggle(CheatToggles.impostorTasks, " Allow Tasks");
    }

    private void DrawShapeshifter()
    {
        GUILayout.Label("Shapeshifter", GUIStylePreset.TabSubtitle);

        CheatToggles.noShapeshiftAnim = GUILayout.Toggle(CheatToggles.noShapeshiftAnim, " No Ss Animation");

        CheatToggles.endlessSsDuration = GUILayout.Toggle(CheatToggles.endlessSsDuration, " Endless Ss Duration");
    }

    private void DrawCrewmate()
    {
        GUILayout.Label("Crewmate", GUIStylePreset.TabSubtitle);

        CheatToggles.showTasksMenu = GUILayout.Toggle(CheatToggles.showTasksMenu, " Show Tasks Menu");
    }

    private void DrawTracker()
    {
        GUILayout.Label("Tracker", GUIStylePreset.TabSubtitle);

        CheatToggles.endlessTracking = GUILayout.Toggle(CheatToggles.endlessTracking, " Endless Tracking");

        CheatToggles.noTrackingDelay = GUILayout.Toggle(CheatToggles.noTrackingDelay, " No Track Delay");

        CheatToggles.noTrackingCooldown = GUILayout.Toggle(CheatToggles.noTrackingCooldown, " No Track Cooldown");

        CheatToggles.trackReach = GUILayout.Toggle(CheatToggles.trackReach, " Track Reach");
    }

    private void DrawEngineer()
    {
        GUILayout.Label("Engineer", GUIStylePreset.TabSubtitle);

        CheatToggles.endlessVentTime = GUILayout.Toggle(CheatToggles.endlessVentTime, " Endless Vent Time");

        CheatToggles.noVentCooldown = GUILayout.Toggle(CheatToggles.noVentCooldown, " No Vent Cooldown");
    }

    private void DrawScientist()
    {
        GUILayout.Label("Scientist", GUIStylePreset.TabSubtitle);

        CheatToggles.endlessBattery = GUILayout.Toggle(CheatToggles.endlessBattery, " Endless Battery");

        CheatToggles.noVitalsCooldown = GUILayout.Toggle(CheatToggles.noVitalsCooldown, " No Vitals Cooldown");
    }

    private void DrawDetective()
    {
        GUILayout.Label("Detective", GUIStylePreset.TabSubtitle);

        CheatToggles.interrogateReach = GUILayout.Toggle(CheatToggles.interrogateReach, " Interrogate Reach");
    }
}
