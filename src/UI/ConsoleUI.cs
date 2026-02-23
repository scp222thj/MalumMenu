using Il2CppSystem;
using UnityEngine;
using System.Collections.Generic;

namespace MalumMenu;

public class ConsoleUI : MonoBehaviour
{
    private static Vector2 _consoleScrollPosition = Vector2.zero;
    private Vector2 _lobbyInfoScrollPosition;
    private int activeTab = 0;
    private static List<string> _logEntries = new();
    private const int MaxLogEntries = 300;
    private Rect _windowRect = new(320, 10, 500, 300);
    private GUIStyle _logStyle;

    public static void Log(string message)
    {
        if (_logEntries.Count >= MaxLogEntries) // Limit the number of logs to keep memory usage in check
        {
            _logEntries.RemoveAt(0); // Remove the oldest log entry
        }

        _logEntries.Add(message);

        // Scroll to the bottom
        _consoleScrollPosition.y = float.MaxValue;
    }

    private void OnGUI()
    {
        if (!CheatToggles.showConsole) return;

        _logStyle ??= new GUIStyle(GUI.skin.label)
        {
            fontSize = 16
        };

        if(ColorUtility.TryParseHtmlString(MalumMenu.menuHtmlColor.Value, out var configUIColor))
        {
            GUI.backgroundColor = configUIColor;
        }

        _windowRect = GUI.Window(1, _windowRect, (GUI.WindowFunction)ConsoleWindow, "MalumConsole");
    }

    private void ConsoleWindow(int windowID)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(activeTab == 0, "Console", GUI.skin.button, GUILayout.Height(28)))
            activeTab = 0;

        if (GUILayout.Toggle(activeTab == 1, "Lobby Info", GUI.skin.button, GUILayout.Height(28)))
            activeTab = 1;
        GUILayout.EndHorizontal();

        if (activeTab == 0)
            DrawConsoleTab();
        else if (activeTab == 1)
            DrawLobbyInfoTab();

        GUILayout.EndVertical();

        GUI.DragWindow();
    }

    private void DrawConsoleTab()
    {
        _consoleScrollPosition = GUILayout.BeginScrollView(_consoleScrollPosition, false, true);

        foreach (var log in _logEntries)
            GUILayout.Label(log, _logStyle);

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Log", GUILayout.Width(235)))
            _logEntries.Clear();

        if (GUILayout.Button("Copy Log to Clipboard"))
            GUIUtility.systemCopyBuffer = string.Join("\n", _logEntries);

        GUILayout.EndHorizontal();
    }

    private void DrawLobbyInfoTab()
    {
        _lobbyInfoScrollPosition = GUILayout.BeginScrollView(_lobbyInfoScrollPosition, false, true);

        MalumMenu.LobbyInfoUI.DrawLobbyInfo();

        GUILayout.EndScrollView();
    }
}