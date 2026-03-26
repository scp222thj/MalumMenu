using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AmongUs.GameOptions;
using UnityEngine;

namespace MalumMenu;

public struct CheatToggles
{
    // Player
    public static bool noClip;
    public static bool speedBoost;
    public static bool teleportPlayer;
    public static bool teleportCursor;
    public static bool reportBody;
    public static bool ejectPlayer;
    public static bool killPlayer;
    public static bool telekillPlayer;
    public static bool killAll;
    public static bool killAllCrew;
    public static bool killAllImps;
    public static bool fakeRevive;
    public static bool invertControls;
    public static bool moonWalk;

    // Roles
    public static bool changeRole;
    public static bool zeroKillCd;
    public static bool showTasksMenu;
    public static bool completeMyTasks;
    public static bool impostorTasks;
    public static bool killReach;
    public static bool killAnyone;
    public static bool endlessSsDuration;
    public static bool endlessBattery;
    public static bool endlessTracking;
    public static bool noTrackingCooldown;
    public static bool noTrackingDelay;
    public static bool trackReach;
    public static bool interrogateReach;
    public static bool noVitalsCooldown;
    public static bool noVentCooldown;
    public static bool endlessVentTime;
    public static bool endlessVanish;
    public static bool killVanished;
    public static bool noVanishAnim;
    public static bool noShapeshiftAnim;

    // ESP
    public static bool fullBright;
    public static bool seeGhosts;
    public static bool seeRoles;
    public static bool showPlayerInfo;
    public static bool seeDisguises;
    public static bool taskArrows;
    public static bool revealVotes;
    public static bool showLobbyInfo;

    // Camera
    public static bool spectate;
    public static bool zoomOut;
    public static bool freecam;

    // Minimap
    public static bool mapCrew;
    public static bool mapImps;
    public static bool mapGhosts;
    public static bool colorBasedMap;

    // Tracers
    public static bool tracersImps;
    public static bool tracersCrew;
    public static bool tracersGhosts;
    public static bool tracersBodies;
    public static bool colorBasedTracers;
    public static bool distanceBasedTracers;

    // Chat
    public static bool alwaysChat;
    public static bool unlockCharacters;
    public static bool bypassUrlBlock;
    public static bool longerMessages;
    public static bool unlockClipboard;
    public static bool lowerRateLimits;

    // Console
    public static bool showConsole;
    public static bool logDeaths;
    public static bool logShapeshifts;
    public static bool logVents;

    // Ship
    public static bool closeMeeting;
    public static bool sabotageMap;
    public static bool openAllDoors;
    public static bool closeAllDoors;
    public static bool spamOpenAllDoors;
    public static bool spamCloseAllDoors;
    public static bool autoOpenDoorsOnUse;
    public static bool unfixableLights;
    public static bool commsSab;
    public static bool elecSab;
    public static bool reactorSab;
    public static bool oxygenSab;
    public static bool mushSab;
    public static bool mushSpore;
    public static bool showDoorsMenu;

    // Vents
    public static bool useVents;
    public static bool walkVent;
    public static bool kickVents;

    // Host-Only
    // public static bool impostorHack;
    // public static bool godMode;
    // public static bool evilVote;
    public static bool voteImmune;
    public static bool forceRole;
    public static RoleTypes? forcedRole;
    public static bool showRolesMenu;
    public static bool skipMeeting;
    public static bool callMeeting;
    public static bool forceStartGame;
    public static bool noGameEnd;
    public static bool showProtectMenu;
    public static bool noOptionsLimits;

    // Passive
    public static bool unlockFeatures;
    public static bool freeCosmetics;
    public static bool avoidBans;
    public static bool copyLobbyCodeOnDisconnect;
    public static bool spoofAprilFoolsDate;
    public static bool stealthMode;
    public static bool panic;

    // Animations
    public static bool animShields;
    public static bool animAsteroids;
    public static bool animEmptyGarbage;
    public static bool animScan;
    public static bool animCamsInUse;
    public static bool animPet;

    // Config
    public static bool reloadConfig;
    public static bool rgbMode;

    public static readonly Dictionary<string, KeyCode> Keybinds = new();

    private static readonly Dictionary<string, FieldInfo> ToggleFields = new();

    private static readonly string LegacyProfilePath = Path.Combine(BepInEx.Paths.ConfigPath, "MalumProfile.txt");

    private static ManualLogSource Log => MalumMenu.Log;

    static CheatToggles()
    {
        var fields = typeof(CheatToggles).GetFields(BindingFlags.Static | BindingFlags.Public);

        foreach (var field in fields)
        {
            if (field.FieldType != typeof(bool)) continue;

            ToggleFields[field.Name] = field;
            Keybinds[field.Name] = KeyCode.None;
        }
    }

    public static void DisablePPMCheats(string variableToKeep)
    {
        ejectPlayer = variableToKeep == "ejectPlayer" && ejectPlayer;
        reportBody = variableToKeep == "reportBody" && reportBody;
        killPlayer = variableToKeep == "killPlayer" && killPlayer;
        telekillPlayer = variableToKeep == "telekillPlayer" && telekillPlayer;
        spectate = variableToKeep == "spectate" && spectate;
        changeRole = variableToKeep == "changeRole" && changeRole;
        forceRole = variableToKeep == "forceRole" && forceRole;
        teleportPlayer = variableToKeep == "teleportPlayer" && teleportPlayer;
    }

    public static bool ShouldPPMClose()
    {
        return !changeRole && !forceRole && !ejectPlayer && !reportBody && !telekillPlayer && !killPlayer && !spectate && !teleportPlayer;
    }

    // Disables all cheat toggles by setting all to false using the cached ToggleFields
    public static void DisableAll()
    {
        foreach (var field in ToggleFields.Values)
        {
            field.SetValue(null, false);
        }
    }

    public static void SaveTogglesToProfile()
    {
        ProfileManager.SaveCurrentProfile();
    }

    public static void LoadTogglesFromProfile()
    {
        ProfileManager.LoadCurrentProfile();
    }

    public static void MigrateLegacyProfile()
    {
        if (!File.Exists(LegacyProfilePath)) return;

        if (ProfileManager.ProfileExists("Default")) return;

        try
        {
            var defaultProfilePath = Path.Combine(ProfileManager.ProfilesDirectory, "Default.txt");
            File.Copy(LegacyProfilePath, defaultProfilePath, true);

            var lines = new List<string>(File.ReadAllLines(defaultProfilePath));
            if (lines.Count > 0 && lines[0].StartsWith("#"))
            {
                lines[0] = $"# MalumMenu Profile: Default";
                lines.Insert(1, $"# Migrated from legacy profile on {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                File.WriteAllLines(defaultProfilePath, lines);
            }
        }
        catch (Exception ex)
        {
            Log.LogError($"Failed to migrate legacy profile: {ex.Message}");
        }
    }

    public class KeybindListener : MonoBehaviour
    {
        public MalumMenu Plugin { get; internal set; }

        public void Update()
        {
            if (MalumMenu.isPanicked) return;

            // Keybinds aren't triggered from typing in the chat
            if (HudManager.InstanceExists && HudManager.Instance.Chat && HudManager.Instance.Chat.IsOpenOrOpening) return;

            if (reloadConfig)
            {
                Plugin.Config.Reload();
                MalumMenu.Log.LogInfo("Configuration reloaded");

                reloadConfig = false;
            }

            // Check each keybind to see if the user pressed it and toggle the corresponding cheat
            foreach (var (name, key) in Keybinds)
            {
                if (key == KeyCode.None) continue;
                if (!Input.GetKeyDown(key)) continue;

                if (!ToggleFields.TryGetValue(name, out var field)) continue;

                var current = (bool)field.GetValue(null);
                field.SetValue(null, !current);
            }
        }
    }
}
