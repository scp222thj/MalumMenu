using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;

namespace MalumMenu;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class MalumMenu : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static string malumVersion = "2.4.2";
    public static List<string> supportedAU = new List<string> { "2024.9.4" };
    public static MenuUI menuUI;
    // public static ConsoleUI consoleUI;
    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> menuHtmlColor;
    public static ConfigEntry<string> spoofLevel;
    public static ConfigEntry<string> spoofPlatform;
    public static ConfigEntry<bool> spoofDeviceId;
    public static ConfigEntry<string> guestFriendCode;
    public static ConfigEntry<bool> guestMode;
    public static ConfigEntry<bool> noTelemetry;

    public override void Load()
    {

        //Load config settings
        menuKeybind = Config.Bind("MalumMenu.GUI",
                                "Keybind",
                                "Delete",
                                "The keyboard key used to toggle the GUI on and off. List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html");

        menuHtmlColor = Config.Bind("MalumMenu.GUI",
                                "Color",
                                "",
                                "A custom color for your MalumMenu GUI. Supports html color codes");

        guestMode = Config.Bind("MalumMenu.GuestMode",
                                "GuestMode",
                                false,
                                "When enabled, a new guest account will generate every time you start the game, allowing you to bypass account bans and PUID detection");
        
        guestFriendCode = Config.Bind("MalumMenu.GuestMode",
                                "FriendName",
                                "",
                                "The username that will be used when setting a friend code for your guest account. IMPORTANT: Can only be used with GuestMode, needs to be ≤ 10 characters, and cannot include special characters/discriminator (#1234)");
        
        spoofLevel = Config.Bind("MalumMenu.Spoofing",
                                "Level",
                                "",
                                "A custom player level to display to others in online games to hide your actual platform. IMPORTANT: Custom levels can only be within 0 and 4294967295. Decimal numbers will not work");
        
        spoofPlatform = Config.Bind("MalumMenu.Spoofing",
                                "Platform",
                                "",
                                "A custom gaming platform to display to others in online lobbies to hide your actual platform. List of supported platforms: https://skeld.js.org/enums/constant.Platform.html");

        spoofDeviceId = Config.Bind("MalumMenu.Privacy",
                                "HideDeviceId",
                                true,
                                "When enabled it will hide your unique deviceId from Among Us, which could potentially help bypass hardware bans in the future");

        noTelemetry = Config.Bind("MalumMenu.Privacy",
                                "NoTelemetry",
                                true,
                                "When enabled it will stop Among Us from collecting analytics of your games and sending them to Innersloth using Unity Analytics");



        Harmony.PatchAll();
        
        menuUI = AddComponent<MenuUI>();
        // consoleUI = AddComponent<ConsoleUI>();

        // Disable Telemetry (haven't fully tested if it works, but according to Unity docs it should)
        if (noTelemetry.Value){

            Analytics.enabled = false;
            Analytics.deviceStatsEnabled = false;
            PerformanceReporting.enabled = false;

        }

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

