using UnityEngine;

namespace MalumMenu;

public class TasksTab : ITab
{
    public string name => "Tasks";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawAutomation();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        DrawTimings();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawGeneral()
    {
        GUILayout.Label("General", GUIStylePreset.TabSubtitle);

        CheatToggles.showTasksMenu = GUILayout.Toggle(CheatToggles.showTasksMenu, " Show Tasks Menu");

        CheatToggles.completeMyTasks = GUILayout.Toggle(CheatToggles.completeMyTasks, " Complete My Tasks (Instant)");
    }

    private void DrawAutomation()
    {
        GUILayout.Label("Automation", GUIStylePreset.TabSubtitle);

        CheatToggles.autoTaskOnOpen = GUILayout.Toggle(CheatToggles.autoTaskOnOpen, " Auto-Complete On Open");

        CheatToggles.autoTaskUseBestTime = GUILayout.Toggle(CheatToggles.autoTaskUseBestTime, " Use Best Recorded Time");
    }

    private void DrawTimings()
    {
        GUILayout.Label("Timings", GUIStylePreset.TabSubtitle);

        CheatToggles.recordTaskTimes = GUILayout.Toggle(CheatToggles.recordTaskTimes, " Record Task Times");

        var seconds = MalumMenu.autoTaskDefaultSeconds.Value;
        GUILayout.Label($" Default Auto Time: {seconds:0.00}s");
        var newSeconds = GUILayout.HorizontalSlider(seconds, 0.1f, 10f);
        if (Mathf.Abs(newSeconds - seconds) > 0.0001f)
        {
            MalumMenu.autoTaskDefaultSeconds.Value = newSeconds;
            MalumMenu.Plugin.Config.Save();
        }

        if (GUILayout.Button("Clear Recorded Times", GUILayout.Height(30)))
        {
            TaskTimeStore.Clear();
        }
    }
}
