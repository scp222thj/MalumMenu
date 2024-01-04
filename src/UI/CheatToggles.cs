namespace MalumMenu
{
    public struct CheatToggles
    {
        //Player
        public static bool noClip;
        public static bool speedBoost;
        public static bool noCooldowns_scientist;
        public static bool noCooldowns_engineer;
        public static bool noCooldowns_shapeshifter;
        public static bool teleportCursor;
        public static bool teleportPlayer;

        //ESP
        public static bool fullBright;
        public static bool seeGhosts;
        public static bool seeRoles;
        public static bool ventVision;
        public static bool revealVotes;

        //RPC Exploit
        public static bool kickPlayer;
        public static bool reportBody;
        public static bool murderPlayer;
        public static bool murderAll;

        //Appearance
        public static bool resetOutfit;
        public static bool shuffleAllOutfits;
        public static bool shuffleOutfit;
        public static bool unlockColors;
        public static bool mimicOutfit;
        public static bool mimicAllOutfits;

        //Spoofing
        public static bool copyPlayerPUID;
        public static bool spoofRandomName;
        public static bool spoofRandomFC;
        public static bool copyPlayerFC;

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

        //Chat
        public static bool alwaysChat;
        public static bool chatJailbreak;
        public static bool spamChat;
        public static bool chatMimic;

        //Host-Only
        public static bool impostorHack;
        public static bool godMode;
        public static bool evilVote;
        public static bool voteImmune;

        //Passive
        public static bool unlockFeatures = true;
        public static bool freeCosmetics = true;
        public static bool avoidBans = true;

        public static void DisablePPMCheats(){
            mimicAllOutfits = false;
            copyPlayerFC = false;
            chatMimic = false;
            copyPlayerPUID = false;
            reportBody = false;
            teleportPlayer = false;
            mimicOutfit = false;
            murderPlayer = false;
            kickPlayer = false;
            spectate = false;
        }

        public static bool shouldPPMClose(){
            return !mimicAllOutfits && !copyPlayerFC && !chatMimic && !copyPlayerPUID && !reportBody && !teleportPlayer && !mimicOutfit && !murderPlayer && !kickPlayer && !spectate;
        }
    }
}
