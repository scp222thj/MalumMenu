using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;

namespace MalumMenu;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class MalumMenu : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static string malumVersion = "2.2.0";
    public static List<string> supportedAU = new List<string> { "2023.11.28", "2024.3.5" };
    public static MenuUI menuUI;
    // public static ConsoleUI consoleUI;
    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> menuHtmlColor;
    public static ConfigEntry<string> spoofLevel;
    public static ConfigEntry<string> spoofPlatform;

    public override void Load()
    {

        //Load config settings
        menuKeybind = Config.Bind("MalumMenu",
                                "GUIKeybind",
                                "Delete",
                                "The keyboard key used to toggle the GUI on and off. List of supported keycodes: https://docs.unity3d.com/ScriptReference/KeyCode.html");

        menuHtmlColor = Config.Bind("MalumMenu",
                                "GUIColor",
                                "",
                                "A custom color for your MalumMenu GUI. Supports html color codes");

        spoofLevel = Config.Bind("MalumMenu.Spoofing",
                                "Level",
                                "",
                                "A spoofed level that will be showed instead of your normal level. IMPORTANT: Custom levels can only be within 0 and 4294967295. Decimal numbers will not work");
        
        spoofPlatform = Config.Bind("MalumMenu.Spoofing",
                                "Platform",
                                "",
                                "A spoofed platform type that will be showed instead of your actual platform. List of supported platforms: https://skeld.js.org/enums/constant.Platform.html");

        Harmony.PatchAll();
        
        menuUI = AddComponent<MenuUI>();
        // consoleUI = AddComponent<ConsoleUI>();

        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((scene, _) =>
        {
            if (scene.name == "MainMenu")
            {
                ModManager.Instance.ShowModStamp(); // Required by InnerSloth Modding Policy

                //Warn about unsupported AU versions
                if (!supportedAU.Contains(Application.version)){
                    Utils.showPopup("\nThis version of MalumMenu and this version of Among Us are incompatible\n\nInstall the right version to avoid problems");
                }
            }
        }));
    }
}

