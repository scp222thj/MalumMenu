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
        public static bool murderPlayer;
        public static bool murderAll;

        //Roles
        public static bool changeRole;
        public static bool zeroKillCd;
        public static bool completeMyTasks;
        public static bool killReach;
        public static bool killAnyone;
        public static bool endlessSsDuration;
        public static bool endlessBattery;
        public static bool noVitalsCooldown;
        public static bool noVentCooldown;
        public static bool endlessVentTime;
        public static bool noShapeshiftAnim;

        //ESP
        public static bool fullBright;
        public static bool alwaysChat;
        public static bool seeTaskCount;
        public static bool seeKillCd;
        public static bool seeDisguises;
        public static bool seeGhosts;
        public static bool seeRoles;
        public static bool ventVision;
        public static bool revealVotes;

        //Spoofing
        public static bool spoofRandomFC;

        //Camera
        public static bool spectate;
        public static bool zoomOut;
        public static bool freeCam;

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
            murderPlayer = variableToKeep != "murderPlayer" ? false : murderPlayer;
            spectate = variableToKeep != "spectate" ? false : spectate;
            changeRole = variableToKeep != "changeRole" ? false : changeRole;
            teleportPlayer = variableToKeep != "teleportPlayer" ? false : teleportPlayer;
        }

        public static bool shouldPPMClose(){
            return !changeRole && !reportBody && !murderPlayer && !spectate && !teleportPlayer;
        }
    }
}
