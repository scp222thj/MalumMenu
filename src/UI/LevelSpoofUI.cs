using System;
using UnityEngine;

namespace MalumMenu;

public class LevelSpoofUI : MonoBehaviour
{
    private Rect _windowRect = new Rect(400, 200, 400, 250);
    private string _levelInput = "";
    private bool _showMenu = false;
    private string _currentLevel = "";
    private string _message = "";
    private Color _messageColor = Color.white;

    public static LevelSpoofUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ToggleMenu()
    {
        _showMenu = !_showMenu;
        if (_showMenu)
        {
            _levelInput = MalumMenu.spoofLevel.Value;
            _currentLevel = GetCurrentLevel();
            _message = "";
        }
    }

    private string GetCurrentLevel()
    {
        if (AmongUsClient.Instance != null && PlayerControl.LocalPlayer != null)
        {
            return PlayerControl.LocalPlayer.Data.PlayerLevel.ToString();
        }
        return "N/A";
    }

    private void OnGUI()
    {
        if (!_showMenu) return;

        UIHelpers.ApplyUIColor();
        _windowRect = GUI.Window(100, _windowRect, (GUI.WindowFunction)LevelSpoofWindow, "★ Level Spoof ★");
    }

    private void LevelSpoofWindow(int windowID)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Current Level:", GUILayout.Width(100f));
        GUILayout.Label(_currentLevel, GUILayout.Width(50f));
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);

        GUIStyle warningStyle = new GUIStyle(GUI.skin.label)
        {
            wordWrap = true,
            fontSize = 10
        };
        warningStyle.normal.textColor = Color.yellow;
        GUILayout.Label("IMPORTANT: Custom levels can only be within 0 and 4294967295. Decimal numbers will not work", warningStyle);

        GUILayout.Space(10f);

        GUILayout.BeginHorizontal();
        GUILayout.Label("New Level:", GUILayout.Width(100f));
        _levelInput = GUILayout.TextField(_levelInput, GUILayout.Width(200f));
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);

        if (!string.IsNullOrEmpty(_message))
        {
            GUIStyle messageStyle = new GUIStyle(GUI.skin.label) { fontSize = 11 };
            messageStyle.normal.textColor = _messageColor;
            GUILayout.Label(_message, messageStyle);
            GUILayout.Space(5f);
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply", GUILayout.Width(100f)))
        {
            ApplyLevel();
        }
        if (GUILayout.Button("Reset", GUILayout.Width(100f)))
        {
            ResetLevel();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5f);
        if (GUILayout.Button("Close", GUILayout.Width(80f)))
        {
            _showMenu = false;
        }

        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    private void ApplyLevel()
    {
        if (uint.TryParse(_levelInput, out uint level))
        {
            MalumMenu.spoofLevel.Value = _levelInput;
            SpoofLevel(level);
            _message = $"✓ Level set to {level}";
            _messageColor = Color.green;
            _currentLevel = level.ToString();
        }
        else
        {
            _message = "✗ Invalid level. Enter a whole number (0-4294967295)";
            _messageColor = Color.red;
        }
    }

    private void ResetLevel()
    {
        MalumMenu.spoofLevel.Value = "";
        _levelInput = "";
        _message = "✓ Level spoof reset";
        _messageColor = Color.green;
        _currentLevel = GetCurrentLevel();
    }

    private void SpoofLevel(uint level)
    {
        if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data != null)
        {
            PlayerControl.LocalPlayer.Data.PlayerLevel = level;
            if (AmongUsClient.Instance != null)
            {
                PlayerControl.LocalPlayer.RpcSetLevel(level);
            }
        }
    }
}
