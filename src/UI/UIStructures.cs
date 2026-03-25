using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MalumMenu;

public struct SubmenuInfo
{
    public string name;
    public bool isExpanded;
    public List<ToggleInfo> toggles;

    public SubmenuInfo(string name, bool isExpanded, List<ToggleInfo> toggles)
    {
        this.name = name;
        this.isExpanded = isExpanded;
        this.toggles = toggles;
    }
}

public struct GroupInfo
{
    public string name;
    public bool isExpanded;
    public List<ToggleInfo> toggles;
    public List<SubmenuInfo> submenus;

    public GroupInfo(string name, bool isExpanded, List<ToggleInfo> toggles, List<SubmenuInfo> submenus)
    {
        this.name = name;
        this.isExpanded = isExpanded;
        this.toggles = toggles;
        this.submenus = submenus;
    }
}

public struct ToggleInfo
{
    public string label;
    public Func<bool> getState;
    public Action<bool> setState;

    public ToggleInfo(string label, Func<bool> getState, Action<bool> setState)
    {
        this.label = label;
        this.getState = getState;
        this.setState = setState;
    }
}

public static class KeybindManager
{
    private static Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();
    private static string waitingForToggle = null;
    private static Rect keybindWindow = new Rect(300, 200, 300, 150);
    private static readonly string profilePath = BepInEx.Paths.ConfigPath + "/MalumProfile.txt";

    public static void LoadKeybinds()
    {
        keybinds.Clear();
        
        if (!File.Exists(profilePath))
        {
            CreateDefaultProfile();
            return;
        }

        foreach (var line in File.ReadAllLines(profilePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split('=');
            if (parts.Length >= 3)
            {
                var toggleName = parts[0].Trim();
                var keyPart = parts[2].Trim();
                
                if (keyPart.StartsWith("KeyCode."))
                {
                    keyPart = keyPart.Substring("KeyCode.".Length);
                }
                
                if (Enum.TryParse<KeyCode>(keyPart, out var key))
                {
                    keybinds[toggleName] = key;
                }
            }
        }
    }

    private static void CreateDefaultProfile()
    {
        var defaultProfile = @"# MalumProfile

";
        Directory.CreateDirectory(Path.GetDirectoryName(profilePath));
        File.WriteAllText(profilePath, defaultProfile);
    }

    public static void SaveKeybinds()
    {
        var lines = new List<string>
        {
            "# MalumProfile",
            "# Format: ToggleName = True/False = KeyCode.KEY",
            "# - List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html",
            "# - Setting a keybind is optional. Use KeyCode.None to not set a keybind",
            "# - Multiple toggles may have the same key, but multiple keys per toggle are NOT supported",
            "# - Keybinds are only applied after loading this profile by pressing 'Load from Profile' in the Config menu",
            ""
        };

        foreach (var kvp in keybinds)
        {
            lines.Add($"{kvp.Key} = false = KeyCode.{kvp.Value}");
        }

        Directory.CreateDirectory(Path.GetDirectoryName(profilePath));
        File.WriteAllLines(profilePath, lines);
    }

    public static void SetKeybind(string toggleName, KeyCode key)
    {
        keybinds[toggleName] = key;
        SaveKeybinds();
    }

    public static KeyCode? GetKeybind(string toggleName)
    {
        if (keybinds.TryGetValue(toggleName, out var key))
        {
            return key;
        }
        return null;
    }

    public static void StartWaitingForToggle(string toggleName)
    {
        waitingForToggle = toggleName;
    }

    public static bool IsWaiting() => waitingForToggle != null;

    public static string GetWaitingToggle() => waitingForToggle;

    public static void StopWaiting()
    {
        waitingForToggle = null;
    }

    public static void CheckForKeybindInput()
    {
        if (waitingForToggle == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopWaiting();
            NotificationManager.Show($"Keybind cancelled");
            return;
        }

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (key == KeyCode.None || key == KeyCode.Mouse0 || key == KeyCode.Mouse1 || key == KeyCode.Mouse2 ||
                key == KeyCode.Mouse3 || key == KeyCode.Mouse4 || key == KeyCode.Mouse5 || key == KeyCode.Mouse6)
            {
                continue;
            }

            if (Input.GetKeyDown(key))
            {
                SetKeybind(waitingForToggle, key);
                StopWaiting();
                NotificationManager.Show($"Keybind set: {waitingForToggle} â†’ {key}");
                return;
            }
        }
    }

    public static void DrawKeybindWindow(List<GroupInfo> groups)
    {
        if (waitingForToggle == null) return;

        keybindWindow = GUI.Window(999, keybindWindow, (GUI.WindowFunction)KeybindWindowFunc, "Set Keybind");
    }

    private static void KeybindWindowFunc(int windowID)
    {
        GUILayout.BeginVertical();
        GUILayout.Label($"Press any key for: {waitingForToggle}");
        GUILayout.Label("Press ESC to cancel");

        GUILayout.Space(10f);

        if (GUILayout.Button("Cancel"))
        {
            StopWaiting();
        }

        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    public static void CheckAllKeybinds(List<GroupInfo> groups)
    {
        if (waitingForToggle != null)
        {
            CheckForKeybindInput();
            return;
        }

        foreach (var group in groups)
        {
            CheckToggleKeybinds(group.toggles);
            foreach (var submenu in group.submenus)
            {
                CheckToggleKeybinds(submenu.toggles);
            }
        }
    }

    private static void CheckToggleKeybinds(List<ToggleInfo> toggles)
    {
        foreach (var toggle in toggles)
        {
            var keybind = GetKeybind(toggle.label.Trim());
            if (keybind.HasValue)
            {
                if (Input.GetKeyDown(keybind.Value))
                {
                    bool currentState = toggle.getState();
                    toggle.setState(!currentState);
                }
            }
        }
    }
}

public static class NotificationManager
{
    private static List<Notification> notifications = new List<Notification>();
    private static Rect notificationArea = new Rect(Screen.width - 300, 10, 290, 500);

    public static void Show(string message, float duration = 2f)
    {
        notifications.Add(new Notification(message, duration));
    }

    public static void OnGUI()
    {
        for (int i = notifications.Count - 1; i >= 0; i--)
        {
            notifications[i].Update();
            if (notifications[i].IsExpired)
            {
                notifications.RemoveAt(i);
            }
        }

        if (notifications.Count == 0) return;

        var oldMatrix = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

        float yOffset = 10;
        for (int i = notifications.Count - 1; i >= 0; i--)
        {
            var notif = notifications[i];
            var rect = new Rect(Screen.width - 280, yOffset, 270, 25);
            
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            
            GUI.color = notif.Color;
            var textRect = new Rect(rect.x + 5, rect.y + 2, rect.width - 10, rect.height - 4);
            GUI.Label(textRect, notif.Message);
            GUI.color = Color.white;

            yOffset += 30;
        }

        GUI.matrix = oldMatrix;
    }

    private class Notification
    {
        public string Message { get; private set; }
        public float Duration { get; private set; }
        public float TimeRemaining { get; private set; }
        public bool IsExpired => TimeRemaining <= 0;
        public Color Color { get; private set; } = Color.white;

        public Notification(string message, float duration)
        {
            Message = message;
            Duration = duration;
            TimeRemaining = duration;
        }

        public void Update()
        {
            TimeRemaining -= Time.deltaTime;
        }
    }
}
