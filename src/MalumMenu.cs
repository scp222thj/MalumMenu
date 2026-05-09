using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace MalumMenu;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class MalumMenu : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static MalumMenu Plugin;
    public new static ManualLogSource Log;

    public static MenuUI menuUI;
    public static ConsoleUI consoleUI;
    public static RolesUI rolesUI;
    public static OverloadUI overloadUI;
    public static DoorsUI doorsUI;
    public static TasksUI tasksUI;
    public static ProtectUI protectUI;
    public static KeybindListener keybindListener;
    public static TaskAutomationController taskAutomationController;

    public static string malumVersion = "3.1.0";
    public static List<string> supportedAU = new List<string> { "2026.3.31" };
    public static bool isPanicked = false;
    public static bool inStealthMode = false;

    public static ConfigEntry<string> menuKeybind;
    public static ConfigEntry<string> menuHtmlColor;
    public static ConfigEntry<bool> menuOpenOnMouse;
    public static ConfigEntry<bool> menuKeepSubwindowsOpen;
    public static ConfigEntry<string> spoofLevel;
    public static ConfigEntry<string> spoofPlatform;
    public static ConfigEntry<bool> spoofDeviceId;
    public static ConfigEntry<bool> noTelemetry;
    public static ConfigEntry<string> guestFriendCode;
    public static ConfigEntry<bool> guestMode;
    public static ConfigEntry<bool> autoLoadProfile;
    public static ConfigEntry<string> configEditor;
    public static ConfigEntry<float> autoTaskDefaultSeconds;
    public static ConfigEntry<float> autoDoorOpenDelaySeconds;
    public static ConfigEntry<float> sabotageCooldownReductionPercent;
    public static ConfigEntry<float> doorCooldownReductionPercent;
    public static ConfigEntry<float> killCooldownReductionPercent;
    public static ConfigEntry<float> minimapScale;
    public static ConfigEntry<float> minimapPosX;
    public static ConfigEntry<float> minimapPosY;
    public static ConfigEntry<int> adaptMaxStrength;
    public static ConfigEntry<float> adaptMaxCooldown;
    public static ConfigEntry<float> attackLogDelay;
    public static ConfigEntry<int> defaultStrength;
    public static ConfigEntry<float> defaultCooldown;
    public static ConfigEntry<int> killSwitchLvl;

    public override void Load()
    {
        Log = base.Log;
        Plugin = this;

        Log.LogInfo($"MalumMenu {malumVersion} starting");
        Log.LogInfo($"GameVersion={Application.version}, Unity={Application.unityVersion}, Platform={Application.platform}");

        try
        {
        // Loads config settings
        menuKeybind = Config.Bind("MalumMenu.GUI",
                                "Keybind",
                                "Delete",
                                "The keyboard key used to toggle the GUI on and off. List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html");

        menuHtmlColor = Config.Bind("MalumMenu.GUI",
                                "Color",
                                "",
                                "A custom color for your MalumMenu GUI. Supports html color codes");

        menuOpenOnMouse = Config.Bind("MalumMenu.GUI",
                                "OpenOnMouse",
                                false,
                                "When enabled, the MalumMenu GUI will always be opened at the current mouse position");

        menuKeepSubwindowsOpen = Config.Bind("MalumMenu.GUI",
                                "KeepSubwindowsOpen",
                                false,
                                "When enabled, closing the MalumMenu GUI will not automatically close its subwindows");

        autoLoadProfile = Config.Bind("MalumMenu.Profile",
                                "AutoLoadProfile",
                                false,
                                "When enabled, your saved keybind and toggle profile will be automatically loaded at game startup");

        configEditor = Config.Bind("MalumMenu.Config",
                                "ConfigEditor",
                                "notepad.exe",
                                "The program used to open the config file when using the Open Config toggle. Can be any executable, but using a text editor is recommended");

        autoTaskDefaultSeconds = Config.Bind("MalumMenu.Tasks",
                                "AutoTaskDefaultSeconds",
                                4f,
                                new ConfigDescription(
                                    "Default duration (in seconds) used by Auto-Complete On Open when no best time exists.",
                                    new AcceptableValueRange<float>(0.1f, 10f)
                                ));

        autoDoorOpenDelaySeconds = Config.Bind("MalumMenu.Ship",
                                "AutoDoorOpenDelaySeconds",
                                0.25f,
                                new ConfigDescription(
                                    "Delay (in seconds) before Auto-Open Doors triggers when using a door console.",
                                    new AcceptableValueRange<float>(0.1f, 4f)
                                ));

        sabotageCooldownReductionPercent = Config.Bind("MalumMenu.Ship",
                                "SabotageCooldownReductionPercent",
                                0f,
                                new ConfigDescription(
                                    "Reduces sabotage cooldowns by this percent (0% = no change, 100% = no cooldown).",
                                    new AcceptableValueRange<float>(0f, 100f)
                                ));

        doorCooldownReductionPercent = Config.Bind("MalumMenu.Ship",
                                "DoorCooldownReductionPercent",
                                0f,
                                new ConfigDescription(
                                    "Reduces door sabotage/door-close cooldowns by this percent (0% = no change, 100% = no cooldown).",
                                    new AcceptableValueRange<float>(0f, 100f)
                                ));

        killCooldownReductionPercent = Config.Bind("MalumMenu.Roles",
                                "KillCooldownReductionPercent",
                                0f,
                                new ConfigDescription(
                                    "Reduces kill cooldown by this percent of the lobby kill cooldown (0% = no change, 100% = no cooldown).",
                                    new AcceptableValueRange<float>(0f, 100f)
                                ));

        minimapScale = Config.Bind("MalumMenu.Minimap",
                                "AlwaysOnScale",
                                AlwaysOnMinimapController.scale,
                                new ConfigDescription(
                                    "Scale of the always-on minimap window.",
                                    new AcceptableValueRange<float>(0.15f, 0.75f)
                                ));

        minimapPosX = Config.Bind("MalumMenu.Minimap",
                                "AlwaysOnPosX",
                                AlwaysOnMinimapController.anchoredPosition.x,
                                "Anchored X position of the always-on minimap window.");

        minimapPosY = Config.Bind("MalumMenu.Minimap",
                                "AlwaysOnPosY",
                                AlwaysOnMinimapController.anchoredPosition.y,
                                "Anchored Y position of the always-on minimap window.");

        // GuestMode config settings are commented out as the cheats are broken in latest updates

        // guestMode = Config.Bind("MalumMenu.GuestMode",
        //                         "GuestMode",
        //                         false,
        //                         "When enabled, a new guest account will generate every time you start the game, allowing you to bypass account bans and PUID detection");

        // guestFriendCode = Config.Bind("MalumMenu.GuestMode",
        //                         "FriendName",
        //                         "",
        //                         "The username that will be used when setting a friend code for your guest account. IMPORTANT: Can only be used with GuestMode, needs to be ≤ 10 characters, and cannot include special characters/discriminator (#1234)");

        spoofLevel = Config.Bind("MalumMenu.Spoofing",
                                "Level",
                                "",
                                "A custom player level to display to others in online games to hide your actual platform. IMPORTANT: Custom levels can only be within 1 and 100001. Decimal numbers will not work");

        spoofPlatform = Config.Bind("MalumMenu.Spoofing",
                                "Platform",
                                "",
                                "A custom gaming platform to display to others in online lobbies to hide your actual platform. List of supported platforms: https://skeld.js.org/enums/_skeldjs_constant.Platform.html");

        spoofDeviceId = Config.Bind("MalumMenu.Privacy",
                                "HideDeviceId",
                                true,
                                "When enabled, it will hide your unique deviceId from Among Us, which could potentially help bypass hardware bans in the future");

        noTelemetry = Config.Bind("MalumMenu.Privacy",
                                "NoTelemetry",
                                true,
                                "When enabled, it will stop Among Us from collecting analytics of your games and sending them to Innersloth using Unity Analytics");

        adaptMaxStrength = Config.Bind("MalumMenu.Overload",
                                "AdaptMaxStrength",
                                500,
                                new ConfigDescription(
                                    "Maximum total number of RPCs sent during one overload cycle in AutoAdapt mode. Automatically divided between targets and reduced based on ping. IMPORTANT: Only goes from 1 to 1000 RPCs",
                                    new AcceptableValueRange<int>(1, 1000)
                                ));

        adaptMaxCooldown = Config.Bind("MalumMenu.Overload",
                                "AdaptMaxCooldown",
                                1f,
                                new ConfigDescription(
                                    "Maximum time (in seconds) for one full overload cycle to complete in AutoAdapt mode. Automatically distributed across targets (more targets = shorter delay per target). IMPORTANT: Only goes from 0s to 10s",
                                    new AcceptableValueRange<float>(0f, 10f)
                                ));

        attackLogDelay = Config.Bind("MalumMenu.Overload",
                                "AttackLogDelay",
                                2f,
                                "Minimum time (in seconds) between attack logs in normal (non-verbose) mode");

        defaultStrength = Config.Bind("MalumMenu.Overload",
                                "DefaultStrength",
                                500,
                                new ConfigDescription(
                                    "Default number of malformed RPCs sent to each target during an overload cycle. Overridden if AutoAdapt mode is enabled. IMPORTANT: Only goes from 1 to 1000 RPCs",
                                    new AcceptableValueRange<int>(1, 1000)
                                ));

        defaultCooldown = Config.Bind("MalumMenu.Overload",
                                "DefaultCooldown",
                                1f,
                                new ConfigDescription(
                                    "Default cooldown (in seconds) between each target during an overload cycle. Overridden if AutoAdapt mode is enabled. IMPORTANT: Only goes from 0s to 10s",
                                    new AcceptableValueRange<float>(0f, 10f)
                                ));

        killSwitchLvl = Config.Bind("MalumMenu.Overload",
                                "DefaultKillSwitchLevel",
                                1,
                                new ConfigDescription(
                                    "Default level used by kill switch. Each level adds 500 ms to the max allowed ping before overload stops. Helps avoid lagging / disconnects. IMPORTANT: Only goes from level 1 (500 ms) to 6 (3000 ms)",
                                    new AcceptableValueRange<int>(1, 6)
                                ));

        // Enabled by default
        CheatToggles.unlockFeatures = true;
        CheatToggles.freeCosmetics = true;
        CheatToggles.avoidPenalties = true;

        // Enabled by default
        CheatToggles.olAutoAdapt = true;
        CheatToggles.olKillSwitch = true;
        CheatToggles.olAutoStop = true;
        CheatToggles.olAutoClear = true;
        CheatToggles.olLogStartStop = true;
        CheatToggles.olLogAttack = true;
        CheatToggles.olLogAddRemove = true;
        CheatToggles.olLogDisconnect = true;

        try
        {
            Harmony.PatchAll();
        }
        catch (Exception ex)
        {
            Log.LogError($"Harmony.PatchAll failed: {ex}");
        }

        try
        {
            var patched = 0;
            foreach (var _ in Harmony.GetPatchedMethods())
            {
                patched++;
            }
            Log.LogInfo($"Harmony patched methods: {patched}");
        }
        catch (Exception ex)
        {
            Log.LogWarning($"Unable to enumerate patched methods: {ex.GetType().Name}");
        }

        try
        {
            Log.LogInfo($"AntiCrash: {AntiCrashLimiter.GetDiagnosticsSummary()}");
        }
        catch (Exception ex)
        {
            Log.LogWarning($"AntiCrash diagnostics failed: {ex.GetType().Name}");
        }

        try
        {
            TaskTimeStore.Load();
            Log.LogInfo("TaskTimeStore loaded");
        }
        catch (Exception ex)
        {
            Log.LogWarning($"TaskTimeStore load failed: {ex.GetType().Name}");
        }

        // UI
        menuUI = AddComponent<MenuUI>();
        consoleUI = AddComponent<ConsoleUI>();
        overloadUI = AddComponent<OverloadUI>();
        doorsUI = AddComponent<DoorsUI>();
        tasksUI = AddComponent<TasksUI>();
        protectUI = AddComponent<ProtectUI>();
        // rolesUI = AddComponent<RolesUI>();

        // Components
        keybindListener = AddComponent<KeybindListener>();
        taskAutomationController = AddComponent<TaskAutomationController>();
        AddComponent<AlwaysOnMinimapController>();
        AddComponent<TrailRecorderController>();

        AlwaysOnMinimapController.scale = minimapScale.Value;
        AlwaysOnMinimapController.anchoredPosition = new Vector2(minimapPosX.Value, minimapPosY.Value);

        // Disables Telemetry (haven't fully tested if it works, but according to Unity docs it should)
        if (noTelemetry.Value)
        {
            Analytics.enabled = false;
            Analytics.deviceStatsEnabled = false;
            PerformanceReporting.enabled = false;
        }

        // Load profile on start
        if (autoLoadProfile.Value)
        {
            Log.LogInfo("AutoLoadProfile enabled: loading profile");
            CheatToggles.LoadTogglesFromProfile();
        }

        SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>) ((scene, _) =>
        {
            if (scene.name == "MainMenu" && !(inStealthMode || isPanicked))
            {
                // Warns about unsupported AU versions
                if (!supportedAU.Contains(Application.version))
                {
                    Utils.ShowPopup("\nThis version of MalumMenu and this version of Among Us are incompatible\n\nInstall the right version to avoid problems");
                }
            }
        }));
        }
        catch (Exception ex)
        {
            Log.LogError($"MalumMenu failed during Load(): {ex}");
        }

        Log.LogInfo("MalumMenu Load() complete");
    }
}
