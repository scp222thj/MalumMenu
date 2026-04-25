using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace MalumMenu;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class MalumMenu : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public static MalumMenu Plugin;
    public static new ManualLogSource Log;

    public static UIManager UiManager;
    public static PluginSettings Settings;

    public static KeybindListener KeyBindListener;

    public static string MalumVersion = "3.0.2";

    public static readonly HashSet<string> SupportedAu = ["2026.2.24", "2026.3.17", "2026.3.31"];

    public static bool IsPanicked = false;
    public static bool InStealthMode = false;

    public override void Load()
    {
        Log = base.Log;
        Plugin = this;

        InitializeManagers();
        BindSettings();
        ApplyDefaults();
        InitializePatches();
        InitializeUI();
        InitializeComponents();
        ConfigureTelemetry();
        LoadProfileOnStart();
        RegisterSceneEvents();

        Log.LogInfo($"Loaded MalumMenu v{MalumVersion}");
    }

    private void InitializeManagers()
    {
        UiManager = new UIManager();
    }

    private void BindSettings()
    {
        // Load config settings
        Settings = new PluginSettings();
        Settings.Bind(Config);
    }

    private void ApplyDefaults()
    {
        // Passives are enabled by default
        CheatToggles.unlockFeatures =
            CheatToggles.freeCosmetics =
            CheatToggles.avoidPenalties =
                true;
    }

    private void InitializePatches()
    {
        Harmony.PatchAll();
    }

    private void InitializeUI()
    {
        UiManager.Initialize(this);
    }

    private void InitializeComponents()
    {
        KeyBindListener = AddComponent<KeybindListener>();
    }

    private void ConfigureTelemetry()
    {
        // Disables Telemetry (haven't fully tested if it works, but according to Unity docs it should)
        if (Settings.NoTelemetry.Value)
        {
            Analytics.enabled = false;
            Analytics.deviceStatsEnabled = false;
            PerformanceReporting.enabled = false;
        }
    }

    private void LoadProfileOnStart()
    {
        // Load profile on start
        if (Settings.AutoLoadProfile.Value)
        {
            CheatToggles.LoadTogglesFromProfile();
        }
    }

    private void RegisterSceneEvents()
    {
        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)OnSceneLoaded);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode _mode)
    {
        if (scene.name == "MainMenu" && !(InStealthMode || IsPanicked))
        {
            // Warns about unsupported AU versions
            if (!SupportedAu.Contains(Application.version))
            {
                Utils.ShowPopup(
                    "\nThis version of MalumMenu and this version of Among Us are incompatible\n\nInstall the right version to avoid problems"
                );
            }
        }
    }
}
