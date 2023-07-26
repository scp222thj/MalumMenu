namespace MalumMenu
{
    public struct CheatSettings
    {
        //Player
        public static bool noClip;
        public static bool impostorHack;
        public static bool noCooldowns;
        public static bool teleport;
        public static bool speedBoost;

        //ESP
        public static bool zoomOut;
        public static bool fullBright;
        public static bool seeGhosts;
        public static bool seeRoles;

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

        //Sabotage
        public static bool fullLockdown;
        public static bool blackOut;
        public static bool commsSab;
        public static bool elecSab;
        public static bool reactorSab;
        public static bool oxygenSab;


        //Meetings
        public static bool closeMeeting;
        public static bool callMeeting;
        public static bool revealVotes;

        //Vents
        public static bool useVents;
        public static bool walkVent;
        public static bool kickVents;
        public static bool ventVision;


        //Host-Only
        public static bool godMode;
        public static bool evilVote;
        public static bool voteImmune;

        //Passive
        public static bool alwaysChat = true;
        public static bool unlockFeatures = true;
        public static bool freeCosmetics = true;
        public static bool avoidBans = true;
    }
}
