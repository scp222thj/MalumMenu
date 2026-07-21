using UnityEngine;

namespace MalumMenu;

public class ConsoleTab : ITab
{
    public string name => "Console";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();
        GUILayout.Space(10);
        DrawEventLogger();

        GUILayout.EndVertical();
    }

    private void DrawGeneral()
    {
        CheatToggles.showConsole = GUILayout.Toggle(CheatToggles.showConsole, " Show Console");

        CheatToggles.logDeaths = GUILayout.Toggle(CheatToggles.logDeaths, " Log Deaths");

        CheatToggles.logShapeshifts = GUILayout.Toggle(CheatToggles.logShapeshifts, " Log Shapeshifts");

        CheatToggles.logVents = GUILayout.Toggle(CheatToggles.logVents, " Log Vents");
    }

    private void DrawEventLogger()
    {
        GUILayout.Label("Event Logger", GUIStylePreset.TabSubtitle);

        bool newEnabled = GUILayout.Toggle(EventLogger.IsEnabled, " Enable Event Logger");
        if (newEnabled != EventLogger.IsEnabled)
        {
            EventLogger.IsEnabled = newEnabled;
        }

        if (EventLogger.IsEnabled)
        {
            bool newShowKills = GUILayout.Toggle(EventLogger.ShowKills, " Show Kills");
            if (newShowKills != EventLogger.ShowKills) EventLogger.ShowKills = newShowKills;

            bool newShowTasks = GUILayout.Toggle(EventLogger.ShowTasks, " Show Tasks");
            if (newShowTasks != EventLogger.ShowTasks) EventLogger.ShowTasks = newShowTasks;

            bool newShowVents = GUILayout.Toggle(EventLogger.ShowVents, " Show Vents");
            if (newShowVents != EventLogger.ShowVents) EventLogger.ShowVents = newShowVents;

            bool newShowSabotages = GUILayout.Toggle(EventLogger.ShowSabotages, " Show Sabotages");
            if (newShowSabotages != EventLogger.ShowSabotages) EventLogger.ShowSabotages = newShowSabotages;

            bool newShowReports = GUILayout.Toggle(EventLogger.ShowReports, " Show Reports");
            if (newShowReports != EventLogger.ShowReports) EventLogger.ShowReports = newShowReports;

            bool newShowVotes = GUILayout.Toggle(EventLogger.ShowVotes, " Show Votes");
            if (newShowVotes != EventLogger.ShowVotes) EventLogger.ShowVotes = newShowVotes;

            GUILayout.Label($"Events: {EventLogger.Events.Count}/{EventLogger.MaxEvents}");
        }
    }
}
