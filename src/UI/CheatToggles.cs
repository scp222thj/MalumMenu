namespace MalumMenu
{
    public struct CheatToggles
    {
        //Player
        public static bool noClip;
        public static bool speedBoost;

        //Roles
        public static bool changeRole;
        public static bool zeroKillCd;
        public static bool completeMyTasks;
        public static bool completeAllTasks;
        public static bool killReach;
        public static bool killAnyone;
        public static bool impostorTasks;
        public static bool endlessSsDuration;
        public static bool endlessBattery;
        public static bool noSsCooldown;
        public static bool noVitalsCooldown;
        public static bool noVentCooldown;
        public static bool endlessVentTime;
        public static bool noShapeshiftAnim;

        //ESP
        public static bool fullBright;
        public static bool seeTaskCount;
        public static bool seeKillCd;
        public static bool seeDisguises;
        public static bool seeGhosts;
        public static bool seeRoles;
        public static bool ventVision;
        public static bool revealVotes;

        //RPC Exploit
        public static bool kickPlayer;
        public static bool allMedScan;
        public static bool reportBody;
        public static bool allReportBody;
        public static bool murderPlayer;
        public static bool murderAll;
        public static bool shapeshiftAll;
        public static bool revertShapeshifters;
        public static bool shapeshiftCheat;

        //Teleport
        public static bool teleportMeCursor;
        public static bool teleportMePlayer;
        public static bool teleportAllPlayer;
        public static bool teleportAllCursor;

        //Appearance
        public static bool resetAppearance;
        public static bool setName;
        public static bool setNameAll;
        public static bool unlockColors;
        public static bool mimicOutfit;
        public static bool mimicAllOutfits;

        //Spoofing
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
            mimicAllOutfits = variableToKeep != "mimicAllOutfits" ? false : mimicAllOutfits;
            copyPlayerFC = variableToKeep != "copyPlayerFC" ? false : copyPlayerFC;
            chatMimic = variableToKeep != "chatMimic" ? false : chatMimic;
            reportBody = variableToKeep != "reportBody" ? false : reportBody;
            teleportMePlayer = variableToKeep != "teleportMePlayer" ? false : teleportMePlayer;
            teleportAllPlayer = variableToKeep != "teleportAllPlayer" ? false : teleportAllPlayer;
            mimicOutfit = variableToKeep != "mimicOutfit" ? false : mimicOutfit;
            murderPlayer = variableToKeep != "murderPlayer" ? false : murderPlayer;
            kickPlayer = variableToKeep != "kickPlayer" ? false : kickPlayer;
            spectate = variableToKeep != "spectate" ? false : spectate;
            shapeshiftAll = variableToKeep != "shapeshiftAll" ? false : shapeshiftAll;
            shapeshiftCheat = variableToKeep != "shapeshiftCheat" ? false : shapeshiftCheat;
            changeRole = variableToKeep != "changeRole" ? false : changeRole;
        }

        public static bool shouldPPMClose(){
            return !mimicAllOutfits && !changeRole && !shapeshiftAll && !shapeshiftCheat && !copyPlayerFC && !chatMimic && !reportBody && !teleportMePlayer && !teleportAllPlayer && !mimicOutfit && !murderPlayer && !kickPlayer && !spectate;
        }
    }
}
