namespace MalumMenu
{
    public struct CheatToggles
    {
        //Player
        public static bool noClip;
        public static bool speedBoost;
        public static bool teleportPlayer;
        public static bool teleportCursor;
        public static bool reportBody;
        public static bool killPlayer;
        public static bool telekillPlayer;
        public static bool killAll;
        public static bool killAllCrew;
        public static bool killAllImps;

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
        public static bool seeDisguises;
        public static bool revealVotes;

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
        public static bool doorsSab;
        public static bool unfixableLights;
        public static bool commsSab;
        public static bool elecSab;
        public static bool reactorSab;
        public static bool oxygenSab;
        public static bool mushSab;

        //Vents
        public static bool useVents;
        public static bool walkVent;
        public static bool kickVents;

        //Host-Only
        //public static bool impostorHack;
        //public static bool godMode;
        //public static bool evilVote;
        //public static bool voteImmune;

        //Passive
        public static bool unlockFeatures = true;
        public static bool freeCosmetics = true;
        public static bool avoidBans = true;

        public static void DisablePPMCheats(string variableToKeep)
        {
            reportBody = variableToKeep != "reportBody" ? false : reportBody;
            killPlayer = variableToKeep != "killPlayer" ? false : killPlayer;
            telekillPlayer = variableToKeep != "telekillPlayer" ? false : telekillPlayer;
            spectate = variableToKeep != "spectate" ? false : spectate;
            changeRole = variableToKeep != "changeRole" ? false : changeRole;
            teleportPlayer = variableToKeep != "teleportPlayer" ? false : teleportPlayer;
        }

        public static bool shouldPPMClose(){
            return !changeRole && !reportBody && !telekillPlayer && !killPlayer && !spectate && !teleportPlayer;
        }
    }
}
