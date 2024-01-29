using AmongUs.Data;

namespace MalumMenu;
public static class SpoofingHandler
{
    public static string defaultFC = null;
    public static uint parsedValue;

    public static void spoofFriendCode(EOSManager eosManager)
    {
        try{
            if (CheatToggles.spoofRandomFC){ //spoofRandomFC cheat logic
                if (defaultFC == null){
                    defaultFC = eosManager.FriendCode; //Save default friend code before randomizing it
                }

                //Generate and save a completly random friend code
                string username = DestroyableSingleton<AccountManager>.Instance.GetRandomName().ToLower();
                string discriminator = new System.Random().Next(1000, 10000).ToString();
                eosManager.FriendCode = username + "#" + discriminator;
            }

            else if (!string.IsNullOrEmpty(MalumMenu.spoofFriendCode.Value) && MalumMenu.spoofFriendCode.Value != eosManager.FriendCode){ //friendCodeSpoofing from config cheat logic
                eosManager.FriendCode = MalumMenu.spoofFriendCode.Value; //Set custom friend code from config file
            }

            //Return to default friend code if both cheats are disabled
            else if (defaultFC != null){
                eosManager.FriendCode = defaultFC;
                defaultFC = null;
            }
        }catch{}
    }

    public static void spoofLevel(EOSManager eosManager)
    {
        if (!string.IsNullOrEmpty(MalumMenu.spoofLevel.Value) && 
            uint.TryParse(MalumMenu.spoofLevel.Value, out parsedValue) &&
            parsedValue != DataManager.Player.Stats.Level)
        {
            DataManager.Player.stats.level = parsedValue - 1;
            DataManager.Player.Save();
        }
    }

    public static bool spoofPlatform(PlatformSpecificData platformSpecificData)
    {
        Platforms? platformType;

        if (Utils.stringToPlatformType(MalumMenu.spoofPlatform.Value, out platformType))
        {
            platformSpecificData.Platform = (Platforms)platformType;
        }

        return true;
    }
}