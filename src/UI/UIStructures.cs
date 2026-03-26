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
    private static string waitingForToggle = null;
    private static Rect keybindWindow = new Rect(300, 200, 300, 150);

    public static void LoadKeybinds()
    {
        // Keybinds are now loaded along with toggle states from profiles
        // This method is kept for backward compatibility but keybinds are managed by ProfileManager
    }

    public static void SetKeybind(string toggleName, KeyCode key)
    {
        // Update the main Keybinds dictionary in CheatToggles
        CheatToggles.Keybinds[toggleName] = key;
        // Keybinds are now saved as part of the profile in ProfileManager
    }

    public static KeyCode? GetKeybind(string toggleName)
    {
        if (CheatToggles.Keybinds.TryGetValue(toggleName, out var key))
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
                NotificationManager.Show($"Keybind set: {waitingForToggle} → {key}");
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

        float yOffset = 10;
        for (int i = notifications.Count - 1; i >= 0; i--)
        {
            var notif = notifications[i];
            var rect = new Rect(Screen.width - 280, yOffset, 270, 25);

            // Draw notification background using box instead of DrawTexture
            GUI.Box(rect, "");

            var textRect = new Rect(rect.x + 5, rect.y + 2, rect.width - 10, rect.height - 4);
            GUI.Label(textRect, notif.Message);

            yOffset += 30;
        }
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
