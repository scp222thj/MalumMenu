using Il2CppSystem;
using UnityEngine;
using System.Collections.Generic;

namespace MalumMenu;

public class ConsoleUI : MonoBehaviour
{
    public static int windowHeight = 380;
    public static int windowWidth = 600;
    private Rect _windowRect;

    private GUIStyle _logStyle;
    private static Vector2 _scrollPosition = Vector2.zero;
    private static List<string> _logEntries = new();
    private const int MaxLogEntries = 300;

    private void Start()
    {
        // Instantiate 2D area of ConsoleUI
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showConsole || !(MenuUI.isGUIActive || MalumMenu.menuKeepSubwindowsOpen.Value) || MalumMenu.isPanicked) return;

        _logStyle ??= new GUIStyle(GUI.skin.label)
        {
            fontSize = 15
        };

        UIHelpers.ApplyUIColor();

        _windowRect = GUI.Window((int)WindowId.ConsoleUI, _windowRect, (GUI.WindowFunction)ConsoleWindow, "Console");
    }

    private void ConsoleWindow(int windowID)
    {
        GUILayout.BeginVertical(GUI.skin.box);

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);

        foreach (var log in _logEntries)
        {
            GUILayout.Label(log, _logStyle);
        }

        GUILayout.EndScrollView();

        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Clear Log", GUILayout.Width(285)))
        {
            _logEntries.Clear();
        }

        if (GUILayout.Button("Copy Log to Clipboard"))
        {
            GUIUtility.systemCopyBuffer = String.Join("\n", _logEntries.ToArray());
        }

        GUILayout.EndHorizontal();

        GUI.DragWindow();
    }

    public static void Log(string message)
    {
        if (_logEntries.Count >= MaxLogEntries) // Limit the number of logs to keep memory usage in check
        {
            _logEntries.RemoveAt(0); // Remove the oldest log entry
        }

        var currentTime = DateTime.Now.ToString("HH:mm:ss");

        _logEntries.Add($"<b>[ {currentTime} ]  {message}</b>");

        // Scroll to the bottom
        _scrollPosition.y = float.MaxValue;
    }
}
