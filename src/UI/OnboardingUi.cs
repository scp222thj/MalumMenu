using UnityEngine;
using System;

public class OnboardingUI : MonoBehaviour
{
    private bool isListeningForKey = false;
    private float windowWidth = 600;
    private float windowHeight = 550;
    private Rect windowRect;

    private GUIStyle titleStyle;
    private GUIStyle littleStyle;
    private GUIStyle labelStyle;
    private GUIStyle buttonStyle;
    private Color windowColor = Color.white;

    private void Start()
    {
        windowRect = new Rect((Screen.width - windowWidth) / 2, (Screen.height - windowHeight) / 2, windowWidth, windowHeight);
        windowColor = Color.white;
    }

    private void OnGUI()
    {
        // this check is technically not neccessary but safety first ig
        if (MalumMenu.MalumMenu.onboardingCompleted.Value) return;

        // All the styles
        titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, };
        littleStyle = new GUIStyle(GUI.skin.label) { fontSize = 14 };
        labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 20 };
         buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 18, fixedHeight = 35 };        

        GUI.Box(windowRect, "");

        var configHtmlColor = MalumMenu.MalumMenu.menuHtmlColor.Value;

        if (!ColorUtility.TryParseHtmlString(configHtmlColor, out var uiColor))
        {
            if (!configHtmlColor.StartsWith("#"))
            {
                if (ColorUtility.TryParseHtmlString("#" + configHtmlColor, out uiColor))
                {
                    GUI.backgroundColor = uiColor;
                }
            }
        }
        else
        {
            GUI.backgroundColor = uiColor;
        }

        windowRect = GUI.Window(99, windowRect, (GUI.WindowFunction)DrawWindow, "MalumMenu | Setup");
    }

    private void DrawWindow(int windowID)
    {
        GUILayout.BeginArea(new Rect(20, 40, windowWidth - 40, windowHeight - 60));

        GUILayout.Label("Welcome to MalumMenu!", titleStyle);

        // 1. keybind sections
        GUILayout.Label("1. Set Menu Keybind (Default: Delete)", labelStyle);
        // Checks if butten is pressed, if it is it will set the next keypress as Ui toggle
        string keyText = isListeningForKey ? "Press any key..." : MalumMenu.MalumMenu.menuKeybind.Value;
        if (GUILayout.Button(keyText, buttonStyle))
            isListeningForKey = true;

        if (isListeningForKey && Event.current.isKey && Event.current.keyCode != KeyCode.None)
        {
            MalumMenu.MalumMenu.menuKeybind.Value = Event.current.keyCode.ToString();
            isListeningForKey = false;
        }

        GUILayout.Space(20);

        // 2. Rgb color selection
        GUILayout.Label("2. Interface Color (RGB)", labelStyle);

        windowColor.r = DrawColorSlider("Red", windowColor.r);
        windowColor.g = DrawColorSlider("Green", windowColor.g);
        windowColor.b = DrawColorSlider("Blue", windowColor.b);

        //Save RGB to config as HEX
        MalumMenu.MalumMenu.menuHtmlColor.Value = "#" + ColorUtility.ToHtmlStringRGB(windowColor);

        if (GUILayout.Button("Reset to Default", GUILayout.Width(150)))
        {
            // It looks like white is the default, so i use it
            windowColor = Color.white;
        }

        GUILayout.Space(20);

        // 3. Settings
        GUILayout.Label("3. Do you want to load the Profile automatically?", labelStyle);
        MalumMenu.MalumMenu.autoLoadProfile.Value = GUILayout.Toggle(
            MalumMenu.MalumMenu.autoLoadProfile.Value,
            " Load Profile on Startup",
            new GUIStyle(GUI.skin.toggle) { fontSize = 18 }
        );

        GUILayout.FlexibleSpace();

        GUILayout.Label("The settings can be changed in the config", littleStyle);

        if (GUILayout.Button("Finish Setup", buttonStyle))
        { 
            MalumMenu.MalumMenu.onboardingCompleted.Value = true;
            this.enabled = false;
        }

        GUILayout.EndArea();
        GUI.DragWindow(new Rect(0, 0, 10000, 40));
    }

    // Method for the Sliders
    private float DrawColorSlider(string label, float value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(80));
        float newVal = GUILayout.HorizontalSlider(value, 0f, 1f);
        GUILayout.Label(((int)(newVal * 255)).ToString(), GUILayout.Width(30));
        GUILayout.EndHorizontal();
        return newVal;
    }
}
