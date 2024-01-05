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
        public static bool shapeshiftAll;
        public static bool revertShapeshifters;
        public static bool shapeshiftCheat;

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

        public static void DisablePPMCheats(string variableToKeep)
        {
            mimicAllOutfits = variableToKeep != "mimicAllOutfits" ? false : mimicAllOutfits;
            copyPlayerFC = variableToKeep != "copyPlayerFC" ? false : copyPlayerFC;
            chatMimic = variableToKeep != "chatMimic" ? false : chatMimic;
            copyPlayerPUID = variableToKeep != "copyPlayerPUID" ? false : copyPlayerPUID;
            reportBody = variableToKeep != "reportBody" ? false : reportBody;
            teleportPlayer = variableToKeep != "teleportPlayer" ? false : teleportPlayer;
            mimicOutfit = variableToKeep != "mimicOutfit" ? false : mimicOutfit;
            murderPlayer = variableToKeep != "murderPlayer" ? false : murderPlayer;
            kickPlayer = variableToKeep != "kickPlayer" ? false : kickPlayer;
            spectate = variableToKeep != "spectate" ? false : spectate;
            shapeshiftAll = variableToKeep != "shapeshiftAll" ? false : shapeshiftAll;
            shapeshiftCheat = variableToKeep != "shapeshiftCheat" ? false : shapeshiftCheat;
        }

        public static bool shouldPPMClose(){
            return !mimicAllOutfits && !shapeshiftAll && !shapeshiftCheat && !copyPlayerFC && !chatMimic && !copyPlayerPUID && !reportBody && !teleportPlayer && !mimicOutfit && !murderPlayer && !kickPlayer && !spectate;
        }
    }
}
