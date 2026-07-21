using System;
using BepInEx.Configuration;

namespace MalumMenu;

public static class CheatConfig
{
    public static ConfigFile Config { get; private set; }

    public static ConfigEntry<bool> FrameRateCapEnabled { get; private set; }
    public static ConfigEntry<int> MaxFrameRate { get; private set; }
    public static ConfigEntry<bool> CopyCodeOnLeave { get; private set; }
    public static ConfigEntry<bool> ShowPlayerInfo { get; private set; }
    public static ConfigEntry<bool> LowGfxMode { get; private set; }
    public static ConfigEntry<bool> ReduceMotion { get; private set; }

    public static bool MotionReduced
    {
        get
        {
            try
            {
                if (LowGfxMode != null && LowGfxMode.Value) return true;
                if (ReduceMotion != null && ReduceMotion.Value) return true;
            }
            catch { }
            return false;
        }
    }

    public static ConfigEntry<bool> EventLoggerEnabled { get; private set; }
    public static ConfigEntry<bool> ImmortalityEntry { get; private set; }

    public static bool Immortality
    {
        get => ImmortalityEntry != null && ImmortalityEntry.Value;
        set
        {
            if (ImmortalityEntry == null || ImmortalityEntry.Value == value) return;
            ImmortalityEntry.Value = value;
            try { Config?.Save(); } catch { }
        }
    }

    public static ConfigEntry<bool> AllowVenting { get; private set; }
    public static ConfigEntry<bool> ImpostorCanDoTasks { get; private set; }

    public static ConfigEntry<bool> EndlessVentTime { get; private set; }
    public static ConfigEntry<bool> NoVentCooldown { get; private set; }
    public static ConfigEntry<bool> EndlessShapeshiftDuration { get; private set; }
    public static ConfigEntry<bool> EndlessBattery { get; private set; }
    public static ConfigEntry<bool> NoVitalsCooldown { get; private set; }
    public static ConfigEntry<bool> EndlessTracking { get; private set; }
    public static ConfigEntry<bool> NoTrackingCooldown { get; private set; }

    public static void Initialize(ConfigFile config)
    {
        Config = config;

        FrameRateCapEnabled = config.Bind("Performance", "FrameRateCapEnabled", false, "Limit FPS locally");
        MaxFrameRate = config.Bind("Performance", "MaxFrameRate", 60, new ConfigDescription("Max FPS", new AcceptableValueRange<int>(15, 1000)));
        CopyCodeOnLeave = config.Bind("Lobby", "CopyCodeOnLeave", false, "Copy lobby code to clipboard on disconnect");
        ShowPlayerInfo = config.Bind("Lobby", "ShowPlayerInfo", false, "Show platform/level/ID next to player names");
        LowGfxMode = config.Bind("Performance", "LowGfxMode", false, "Disable visual effects");
        ReduceMotion = config.Bind("Performance", "ReduceMotion", false, "Snap animations instantly");
        EventLoggerEnabled = config.Bind("Features", "EventLoggerEnabled", false, "Log game events");
        ImmortalityEntry = config.Bind("Gameplay", "Immortality", false, "Fake-vent immortality");
        AllowVenting = config.Bind("Gameplay", "AllowVenting", false, "All roles can vent");
        ImpostorCanDoTasks = config.Bind("Gameplay", "ImpostorCanDoTasks", false, "Impostor can do crew tasks");

        EndlessVentTime = config.Bind("Roles", "EndlessVentTime", false, "Unlimited vent time (Engineer)");
        NoVentCooldown = config.Bind("Roles", "NoVentCooldown", false, "No vent cooldown (Engineer)");
        EndlessShapeshiftDuration = config.Bind("Roles", "EndlessShapeshiftDuration", false, "Unlimited shapeshift (Shapeshifter)");
        EndlessBattery = config.Bind("Roles", "EndlessBattery", false, "Unlimited battery (Scientist)");
        NoVitalsCooldown = config.Bind("Roles", "NoVitalsCooldown", false, "No vitals cooldown (Scientist)");
        EndlessTracking = config.Bind("Roles", "EndlessTracking", false, "Unlimited tracking (Tracker)");
        NoTrackingCooldown = config.Bind("Roles", "NoTrackingCooldown", false, "No tracking cooldown (Tracker)");

        FrameRateCapEnabled.SettingChanged += (_, __) => FrameRateCapService.ApplyIfNeeded(true);
        MaxFrameRate.SettingChanged += (_, __) => FrameRateCapService.ApplyIfNeeded(true);
        FrameRateCapService.ApplyIfNeeded(true);
    }
}
