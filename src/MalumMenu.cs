using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;
using UniverseLib.Config;
using UniverseLib.UI;

namespace MalumMenu;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class MalumMenu : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static string malumVersion = "2.2.0";
    public static List<string> supportedAU = new List<string> { "2023.11.28", "2024.3.5" };
    public static UIBase malumUi { get; private set; }
    public static MalumPanelManager malumPanelManager;
    public static BepInEx.Logging.ManualLogSource Logger;
    public static Dictionary<string, Action<bool>> allTogglesPreloaded;
    // public static ConsoleUI consoleUI;
    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> menuHtmlColor;
    public static ConfigEntry<string> spoofLevel;
    public static ConfigEntry<string> spoofPlatform;
    public static ConfigEntry<bool> spoofDeviceId;
    public static ConfigEntry<string> guestFriendCode;
    public static ConfigEntry<bool> guestMode;
    public static ConfigEntry<bool> noTelemetry;
    public static string ErrorText = "";

    public override void Load()
    {
        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
        {
            if (scene.name == "SplashIntro")
            {
                SplashManager splashManager = GameObject.FindAnyObjectByType<SplashManager>();
                SplashErrorPopup splashError = splashManager.errorPopup;
                if (string.IsNullOrEmpty(ErrorText)) return;
                if (splashManager != null) splashManager.enabled = false;
                if (splashError != null) splashError.Show("MalumMenu", ErrorText);
                Harmony.UnpatchAll();
            }
            if (scene.name == "MainMenu")
            {
                ModManager.Instance.ShowModStamp(); // Required by InnerSloth Modding Policy

                //Warn about unsupported AU versions
                if (!supportedAU.Contains(Application.version))
                {
                    Utils.showPopup("\nThis version of MalumMenu and this version of Among Us are incompatible\n\nInstall the right version to avoid problems");
                }
            }
        }));

        //Add Logger
        Logger = BepInEx.Logging.Logger.CreateLogSource("MalumMenu");

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
                                "When enabled it will hide your unique deviceId from Among Us, which could help bypass hardware bans in the future");

        noTelemetry = Config.Bind("MalumMenu.Privacy",
                                "NoTelemetry",
                                true,
                                "When enabled it will stop Among Us from collecting analytics of your games and sending them Innersloth using Unity Analytics");


        try
        {
        allTogglesPreloaded = CheatToggles.AllTogglesFast();
        LogHandler("Preloaded toggles", LogType.Log);
        }
        catch 
        {
        LogHandler("Failed to load toggles", LogType.Exception);
        ErrorText = "Failed to load toggles!";
        return;
        }

        Harmony.PatchAll();

        float startupDelay = 1f;
        UniverseLibConfig config = new()
        {
            Disable_EventSystem_Override = false,
            Force_Unlock_Mouse = true,
            Unhollowed_Modules_Folder = null
        };

        UniverseLib.Universe.Init(startupDelay, OnInitialized, LogHandler, config);
        
        //menuUI = AddComponent<MenuUI>();
        // consoleUI = AddComponent<ConsoleUI>();

        if (noTelemetry.Value){

            Analytics.enabled = false;
            Analytics.deviceStatsEnabled = false;
            PerformanceReporting.enabled = false;

        }
    }


    void OnInitialized() 
    {
        malumPanelManager = new MalumPanelManager();
        malumUi = UniversalUI.RegisterUI("scp222thj.malummenu.malumui", malumPanelManager.Update);
        MalumPanel malumPanel = new(malumUi);
        malumPanelManager.RegisterPanel(malumPanel);
    }

    public static void LogHandler(string message, LogType type)
    {
        switch (type)
        {
            case LogType.Log:
            Logger.LogMessage(message);
            break;
            case LogType.Error:
            Logger.LogError(message);
            break;
            case LogType.Assert:
            Logger.LogInfo(message);
            break;
            case LogType.Warning:
            Logger.LogWarning(message);
            break;
            case LogType.Exception:
            Logger.LogFatal(message);
            break;
        }
    }
}

