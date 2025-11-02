using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MalumMenu;

public struct CheatToggles
{
    //Player
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
    public static bool revive;
    public static bool protectPlayer;
    public static bool invertControls;

    //Roles
    public static bool changeRole;
    public static bool zeroKillCd;
    public static bool completeMyTasks;
    public static bool killReach;
    public static bool killAnyone;
    public static bool endlessSsDuration;
    public static bool endlessBattery;
    public static bool endlessTracking;
    public static bool noTrackingCooldown;
    public static bool noTrackingDelay;
    public static bool noVitalsCooldown;
    public static bool noVentCooldown;
    public static bool endlessVentTime;
    public static bool endlessVanish;
    public static bool killVanished;
    public static bool noVanishAnim;
    public static bool noShapeshiftAnim;

    //ESP
    public static bool fullBright;
    public static bool seeGhosts;
    public static bool seeRoles;
    public static bool showPlayerInfo;
    public static bool seeDisguises;
    public static bool revealVotes;
    public static bool moreLobbyInfo;

    //Camera
    public static bool spectate;
    public static bool zoomOut;
    public static bool freecam;

    //Minimap
    public static bool mapCrew;
    public static bool mapImps;
    public static bool mapGhosts;
    public static bool colorBasedMap;

    //Tracers
    public static bool tracersImps;
    public static bool tracersCrew;
    public static bool tracersGhosts;
    public static bool tracersBodies;
    public static bool colorBasedTracers;
    public static bool distanceBasedTracers;

    //Chat
    public static bool alwaysChat;
    public static bool chatJailbreak;

    //Ship
    public static bool closeMeeting;
    public static bool sabotageMap;
    public static bool doorsSab;
    public static bool unfixableLights;
    public static bool commsSab;
    public static bool elecSab;
    public static bool reactorSab;
    public static bool oxygenSab;
    public static bool mushSab;
    public static bool mushSpore;

    //Vents
    public static bool useVents;
    public static bool walkVent;
    public static bool kickVents;

    //Host-Only
    //public static bool impostorHack;
    //public static bool godMode;
    //public static bool evilVote;
    //public static bool voteImmune;
    public static bool skipMeeting;
    public static bool forceStartGame;
    public static bool noGameEnd;
    public static bool noOptionsLimits;

    //Passive
    public static bool unlockFeatures;
    public static bool freeCosmetics;
    public static bool avoidBans;
    public static bool spoofAprilFoolsDate;
    public static bool isPanicked;

    // Animations
    public static bool animShields;
    public static bool animAsteroids;
    public static bool animEmptyGarbage;
    public static bool animScan;
    public static bool animCamsInUse;

    //Config
    public static bool reloadConfig;
    public static bool RGBMode;

    public static void DisablePPMCheats(string variableToKeep)
    {
        ejectPlayer = variableToKeep == "ejectPlayer" && ejectPlayer;
        reportBody = variableToKeep == "reportBody" && reportBody;
        killPlayer = variableToKeep == "killPlayer" && killPlayer;
        telekillPlayer = variableToKeep == "telekillPlayer" && telekillPlayer;
        spectate = variableToKeep == "spectate" && spectate;
        changeRole = variableToKeep == "changeRole" && changeRole;
        teleportPlayer = variableToKeep == "teleportPlayer" && teleportPlayer;
        protectPlayer = variableToKeep == "protectPlayer" && protectPlayer;
    }

    public static bool shouldPPMClose(){
        return !changeRole && !ejectPlayer && !reportBody && !telekillPlayer && !killPlayer && !spectate && !teleportPlayer && !protectPlayer;
    }

    /// <summary>
    /// Disables all cheat toggles by setting all boolean fields to false using reflection.
    /// </summary>
    public static void DisableAll()
    {
        var fields = typeof(CheatToggles).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(bool))
            {
                field.SetValue(null, false);
            }
        }
    }

    /// <summary>
    /// Saves all cheat toggles to a file named "MalumProfile.txt" in the BepInEx config directory.
    /// Each line in the file contains a toggle name and its value in the format "ToggleName=true/false".
    /// </summary>
    public static void SaveTogglesToProfile()
    {
        var fields = typeof(CheatToggles).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        using var writer = new StreamWriter(Path.Combine(BepInEx.Paths.ConfigPath, "MalumProfile.txt"));
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(bool))
            {
                writer.WriteLine($"{field.Name}={field.GetValue(null)}");
            }
        }
    }

    /// <summary>
    /// Loads cheat toggles from a file named "MalumProfile.txt" in the BepInEx config directory.
    /// Each line in the file contains a toggle name and its value in the format "ToggleName=true/false".
    /// </summary>
    public static void LoadTogglesFromProfile()
    {
        var fields = typeof(CheatToggles).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        var fieldDict = new Dictionary<string, System.Reflection.FieldInfo>();
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(bool))
            {
                fieldDict[field.Name] = field;
            }
        }

        var filePath = Path.Combine(BepInEx.Paths.ConfigPath, "MalumProfile.txt");
        if (!File.Exists(filePath)) return;

        using var reader = new StreamReader(filePath);
        while (reader.ReadLine() is { } line)
        {
            var parts = line.Split("=");
            if (parts.Length == 2 && fieldDict.ContainsKey(parts[0]) && bool.TryParse(parts[1], out var value))
            {
                fieldDict[parts[0]].SetValue(null, value);
            }
        }
    }

    public class KeybindListener : MonoBehaviour
    {
        public MalumMenu Plugin { get; internal set; }

        public void Update()
        {
            if (reloadConfig)
            {
                Plugin.Config.Reload();
                Plugin.Log.LogInfo("Plugin config reloaded.");
                reloadConfig = false;
            }
        }
    }
}
