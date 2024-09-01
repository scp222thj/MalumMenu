using System;
using System.Threading;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;

namespace MalumMenu
{
    public static class SafeSettings
    {        
        private static Timer updateTimer;
        
        // Roles
        public static ConfigEntry<bool> zeroKillCd;
        public static ConfigEntry<bool> killReach;
        public static ConfigEntry<bool> killAnyone;
        public static ConfigEntry<bool> endlessSsDuration;
        public static ConfigEntry<bool> endlessBattery;
        public static ConfigEntry<bool> endlessTracking;
        public static ConfigEntry<bool> noTrackingCooldown;
        public static ConfigEntry<bool> noTrackingDelay;
        public static ConfigEntry<bool> noVitalsCooldown;
        public static ConfigEntry<bool> noVentCooldown;
        public static ConfigEntry<bool> endlessVentTime;
        public static ConfigEntry<bool> endlessVanish;
        public static ConfigEntry<bool> killVanished;
        public static ConfigEntry<bool> noVanishAnim;
        public static ConfigEntry<bool> noShapeshiftAnim;

        // ESP
        public static ConfigEntry<bool> fullBright;
        public static ConfigEntry<bool> alwaysChat;
        public static ConfigEntry<bool> seeGhosts;
        public static ConfigEntry<bool> seeRoles;
        public static ConfigEntry<bool> seeDisguises;
        public static ConfigEntry<bool> revealVotes;

        // Minimap
        public static ConfigEntry<bool> mapCrew;
        public static ConfigEntry<bool> mapImps;
        public static ConfigEntry<bool> mapGhosts;
        public static ConfigEntry<bool> colorBasedMap;

        // Tracers
        public static ConfigEntry<bool> tracersImps;
        public static ConfigEntry<bool> tracersCrew;
        public static ConfigEntry<bool> tracersGhosts;
        public static ConfigEntry<bool> tracersBodies;
        public static ConfigEntry<bool> colorBasedTracers;
        public static ConfigEntry<bool> distanceBasedTracers;

        // Vents
        public static ConfigEntry<bool> useVents;
        public static ConfigEntry<bool> walkVent;

        public static void LoadSettings(BasePlugin plugin)
        {
            //Load config 

            // Added z. to put all the settings at the bottom of the config file to make the file easier to read
            
            // Roles
            zeroKillCd = plugin.Config.Bind("z.Roles", "ZeroKillCd", false);
            killReach = plugin.Config.Bind("z.Roles", "KillReach", false);
            killAnyone = plugin.Config.Bind("z.Roles", "KillAnyone", false);
            endlessSsDuration = plugin.Config.Bind("z.Roles", "EndlessSsDuration", false);
            endlessBattery = plugin.Config.Bind("z.Roles", "EndlessBattery", false);
            endlessTracking = plugin.Config.Bind("z.Roles", "EndlessTracking", false);
            noTrackingCooldown = plugin.Config.Bind("z.Roles", "NoTrackingCooldown", false);
            noTrackingDelay = plugin.Config.Bind("z.Roles", "NoTrackingDelay", false);
            noVitalsCooldown = plugin.Config.Bind("z.Roles", "NoVitalsCooldown", false);
            noVentCooldown = plugin.Config.Bind("z.Roles", "NoVentCooldown", false);
            endlessVentTime = plugin.Config.Bind("z.Roles", "EndlessVentTime", false);
            endlessVanish = plugin.Config.Bind("z.Roles", "EndlessVanish", false);
            killVanished = plugin.Config.Bind("z.Roles", "KillVanished", false);
            noVanishAnim = plugin.Config.Bind("z.Roles", "NoVanishAnim", false);
            noShapeshiftAnim = plugin.Config.Bind("z.Roles", "NoShapeshiftAnim", false);

            // ESP
            fullBright = plugin.Config.Bind("z.ESP", "FullBright", false);
            alwaysChat = plugin.Config.Bind("z.ESP", "AlwaysChat", false);
            seeGhosts = plugin.Config.Bind("z.ESP", "SeeGhosts", false);
            seeRoles = plugin.Config.Bind("z.ESP", "SeeRoles", false);
            seeDisguises = plugin.Config.Bind("z.ESP", "SeeDisguises", false);
            revealVotes = plugin.Config.Bind("z.ESP", "RevealVotes", false);

            // Minimap
            mapCrew = plugin.Config.Bind("z.Minimap", "MapCrew", false);
            mapImps = plugin.Config.Bind("z.Minimap", "MapImps", false);
            mapGhosts = plugin.Config.Bind("z.Minimap", "MapGhosts", false);
            colorBasedMap = plugin.Config.Bind("z.Minimap", "ColorBasedMap", false);

            // Tracers
            tracersImps = plugin.Config.Bind("z.Tracers", "TracersImps", false);
            tracersCrew = plugin.Config.Bind("z.Tracers", "TracersCrew", false);
            tracersGhosts = plugin.Config.Bind("z.Tracers", "TracersGhosts", false);
            tracersBodies = plugin.Config.Bind("z.Tracers", "TracersBodies", false);
            colorBasedTracers = plugin.Config.Bind("z.Tracers", "ColorBasedTracers", false);
            distanceBasedTracers = plugin.Config.Bind("z.Tracers", "DistanceBasedTracers", false);

            // Vents
            useVents = plugin.Config.Bind("z.Vents", "UseVents", false);
            walkVent = plugin.Config.Bind("z.Vents", "WalkVent", false);

            LoadCheatToggles();
        }

        public static void LoadCheatToggles()
        {

            // Roles
            CheatToggles.zeroKillCd = zeroKillCd.Value;
            CheatToggles.killReach = killReach.Value;
            CheatToggles.killAnyone = killAnyone.Value;
            CheatToggles.endlessSsDuration = endlessSsDuration.Value;
            CheatToggles.endlessBattery = endlessBattery.Value;
            CheatToggles.endlessTracking = endlessTracking.Value;
            CheatToggles.noTrackingCooldown = noTrackingCooldown.Value;
            CheatToggles.noTrackingDelay = noTrackingDelay.Value;
            CheatToggles.noVitalsCooldown = noVitalsCooldown.Value;
            CheatToggles.noVentCooldown = noVentCooldown.Value;
            CheatToggles.endlessVentTime = endlessVentTime.Value;
            CheatToggles.endlessVanish = endlessVanish.Value;
            CheatToggles.killVanished = killVanished.Value;
            CheatToggles.noVanishAnim = noVanishAnim.Value;
            CheatToggles.noShapeshiftAnim = noShapeshiftAnim.Value;

            // ESP
            CheatToggles.fullBright = fullBright.Value;
            CheatToggles.alwaysChat = alwaysChat.Value;
            CheatToggles.seeGhosts = seeGhosts.Value;
            CheatToggles.seeRoles = seeRoles.Value;
            CheatToggles.seeDisguises = seeDisguises.Value;
            CheatToggles.revealVotes = revealVotes.Value;

            // Minimap
            CheatToggles.mapCrew = mapCrew.Value;
            CheatToggles.mapImps = mapImps.Value;
            CheatToggles.mapGhosts = mapGhosts.Value;
            CheatToggles.colorBasedMap = colorBasedMap.Value;

            // Tracers
            CheatToggles.tracersImps = tracersImps.Value;
            CheatToggles.tracersCrew = tracersCrew.Value;
            CheatToggles.tracersGhosts = tracersGhosts.Value;
            CheatToggles.tracersBodies = tracersBodies.Value;
            CheatToggles.colorBasedTracers = colorBasedTracers.Value;
            CheatToggles.distanceBasedTracers = distanceBasedTracers.Value;

            // Vents
            CheatToggles.useVents = useVents.Value;
            CheatToggles.walkVent = walkVent.Value;
            
            // Call UpdateTimer to start the Timer after loading the cheat toggles to avoid problems
            UpdateTimer();
        }
        public static void UpdateTimer()
        {
            // Timer to update config from cheat toggles every 60 Seconds
            updateTimer = new Timer(_ => UpdateConfigFromCheatToggles(), null, 0, 60000);
            
        }

        public static void UpdateConfigFromCheatToggles()
        {
            // Funktion to write cheat toggle bools to config

            // Roles
            zeroKillCd.Value = CheatToggles.zeroKillCd;
            killReach.Value = CheatToggles.killReach;
            killAnyone.Value = CheatToggles.killAnyone;
            endlessSsDuration.Value = CheatToggles.endlessSsDuration;
            endlessBattery.Value = CheatToggles.endlessBattery;
            endlessTracking.Value = CheatToggles.endlessTracking;
            noTrackingCooldown.Value = CheatToggles.noTrackingCooldown;
            noTrackingDelay.Value = CheatToggles.noTrackingDelay;
            noVitalsCooldown.Value = CheatToggles.noVitalsCooldown;
            noVentCooldown.Value = CheatToggles.noVentCooldown;
            endlessVentTime.Value = CheatToggles.endlessVentTime;
            endlessVanish.Value = CheatToggles.endlessVanish;
            killVanished.Value = CheatToggles.killVanished;
            noVanishAnim.Value = CheatToggles.noVanishAnim;
            noShapeshiftAnim.Value = CheatToggles.noShapeshiftAnim;

            // ESP
            fullBright.Value = CheatToggles.fullBright;
            alwaysChat.Value = CheatToggles.alwaysChat;
            seeGhosts.Value = CheatToggles.seeGhosts;
            seeRoles.Value = CheatToggles.seeRoles;
            seeDisguises.Value = CheatToggles.seeDisguises;
            revealVotes.Value = CheatToggles.revealVotes;

            // Minimap
            mapCrew.Value = CheatToggles.mapCrew;
            mapImps.Value = CheatToggles.mapImps;
            mapGhosts.Value = CheatToggles.mapGhosts;
            colorBasedMap.Value = CheatToggles.colorBasedMap;

            // Tracers
            tracersImps.Value = CheatToggles.tracersImps;
            tracersCrew.Value = CheatToggles.tracersCrew;
            tracersGhosts.Value = CheatToggles.tracersGhosts;
            tracersBodies.Value = CheatToggles.tracersBodies;
            colorBasedTracers.Value = CheatToggles.colorBasedTracers;
            distanceBasedTracers.Value = CheatToggles.distanceBasedTracers;

            // Vents
            useVents.Value = CheatToggles.useVents;
            walkVent.Value = CheatToggles.walkVent;
        }

    }
}
