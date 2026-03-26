using System;
using UnityEngine;

namespace MalumMenu;

public static class LevelSpoofUI
{
    private static Rect _windowRect = new Rect(400, 200, 450, 170);
    private static bool _showMenu = false;
    private static uint _currentLevel = 0;
    private static float _sliderValue = 0f;
    private static bool _isDragging = false;
    
    private const uint MAX_LEVEL = 100000;
    private const uint MIN_LEVEL = 1;

    public static void ToggleMenu()
    {
        _showMenu = !_showMenu;
        if (_showMenu)
        {
            _currentLevel = GetCurrentLevel();
            _sliderValue = LevelToSlider(_currentLevel);
        }
    }

    private static uint GetCurrentLevel()
    {
        if (AmongUsClient.Instance != null && PlayerControl.LocalPlayer != null)
        {
            return PlayerControl.LocalPlayer.Data.PlayerLevel;
        }
        return 0;
    }

    private static float LevelToSlider(uint level)
    {
        if (level <= MIN_LEVEL) return 0f;
        if (level >= MAX_LEVEL) return 100f;
        return Mathf.Log10(level) / Mathf.Log10(MAX_LEVEL) * 100f;
    }

    private static uint SliderToLevel(float slider)
    {
        if (slider <= 0f) return MIN_LEVEL;
        if (slider >= 100f) return MAX_LEVEL;
        uint level = (uint)Mathf.Round(Mathf.Pow(10f, slider / 100f * Mathf.Log10(MAX_LEVEL)));
        if (level > 1) level -= 1;
        return level;
    }

    public static void OnGUI()
    {
        if (!_showMenu) return;

        UIHelpers.ApplyUIColor();
        _windowRect = GUI.Window(100, _windowRect, (GUI.WindowFunction)LevelSpoofWindow, "★ Level Spoof ★");
    }

    private static void LevelSpoofWindow(int windowID)
    {
        GUILayout.BeginVertical();

        // Level display - centered
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Level: " + _currentLevel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);

        // Slider row - labels and slider perfectly aligned
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        GUILayout.Label("1", labelStyle, GUILayout.Width(30f), GUILayout.Height(20f));
        
        // Check if user is dragging the slider
        Rect sliderRect = GUILayoutUtility.GetRect(300f, 20f);
        float newSliderValue = GUI.HorizontalSlider(sliderRect, _sliderValue, 0f, 100f);
        
        // Detect if slider is being dragged
        if (Event.current.type == EventType.MouseDown && sliderRect.Contains(Event.current.mousePosition))
        {
            _isDragging = true;
        }
        if (Event.current.type == EventType.MouseUp)
        {
            _isDragging = false;
        }
        
        GUILayout.Label("100K", labelStyle, GUILayout.Width(35f), GUILayout.Height(20f));
        
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Update level when slider changes (only when not dragging to prevent feedback loop)
        if (!_isDragging && newSliderValue != _sliderValue)
        {
            _sliderValue = newSliderValue;
            uint newLevel = SliderToLevel(_sliderValue);
            SetLevel(newLevel);
        }
        else if (_isDragging)
        {
            _sliderValue = newSliderValue;
        }

        GUILayout.Space(10f);

        // Preset buttons - centered
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("1", GUILayout.Width(40f), GUILayout.Height(20f))) SetLevel(1);
        if (GUILayout.Button("100", GUILayout.Width(40f), GUILayout.Height(20f))) SetLevel(100);
        if (GUILayout.Button("1K", GUILayout.Width(40f), GUILayout.Height(20f))) SetLevel(1000);
        if (GUILayout.Button("10K", GUILayout.Width(40f), GUILayout.Height(20f))) SetLevel(10000);
        if (GUILayout.Button("50K", GUILayout.Width(40f), GUILayout.Height(20f))) SetLevel(50000);
        if (GUILayout.Button("100K", GUILayout.Width(40f), GUILayout.Height(20f))) SetLevel(100000);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5f);

        // Bottom buttons - centered
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reset", GUILayout.Width(70f), GUILayout.Height(20f))) ResetLevel();
        GUILayout.Space(20f);
        if (GUILayout.Button("Close", GUILayout.Width(50f), GUILayout.Height(20f))) _showMenu = false;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    private static void SetLevel(uint level)
    {
        if (level < MIN_LEVEL) level = MIN_LEVEL;
        if (level > MAX_LEVEL) level = MAX_LEVEL;
        
        _currentLevel = level;
        // Don't update slider value here to prevent feedback loop
        MalumMenu.spoofLevel.Value = level.ToString();
        SpoofLevel(level);
    }

    private static void ResetLevel()
    {
        MalumMenu.spoofLevel.Value = "";
        _currentLevel = GetCurrentLevel();
        _sliderValue = LevelToSlider(_currentLevel);
        SpoofLevel(_currentLevel);
    }

    private static void SpoofLevel(uint level)
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
