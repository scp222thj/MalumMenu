using AmongUs.Data;
using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Update))]
public static class Spoofing_EOSManager_Update_Postfix
{
    public static string defaultFC = null;
    public static uint parsedValue;

    //Postfix patch of EOSManager.Update to spoof some player attributes
    public static void Postfix(EOSManager __instance)
    {
        try{
            if (CheatToggles.spoofRandomFC){ //spoofRandomFC cheat logic
                if (defaultFC == null){
                    defaultFC = __instance.FriendCode; //Save default friend code before randomizing it
                }

                //Generate and save a completly random friend code
                string username = DestroyableSingleton<AccountManager>.Instance.GetRandomName().ToLower();
                string discriminator = new System.Random().Next(1000, 10000).ToString();
                __instance.FriendCode = username + "#" + discriminator;
            }

            else if (!string.IsNullOrEmpty(MalumMenu.spoofFriendCode.Value) && MalumMenu.spoofFriendCode.Value != __instance.FriendCode){ //friendCodeSpoofing from config cheat logic
                __instance.FriendCode = MalumMenu.spoofFriendCode.Value; //Set custom friend code from config file
            }

            //Return to default friend code if both cheats are disabled
            else if (defaultFC != null){
                __instance.FriendCode = defaultFC;
                defaultFC = null;
            }

            if (!string.IsNullOrEmpty(MalumMenu.spoofLevel.Value) && 
                uint.TryParse(MalumMenu.spoofLevel.Value, out parsedValue) &&
                parsedValue != DataManager.Player.Stats.Level)
            {
                DataManager.Player.stats.level = parsedValue - 1;
                DataManager.Player.Save();
            }
        }
        catch{}
    }
}

[HarmonyPatch(typeof(PlatformSpecificData), nameof(PlatformSpecificData.Serialize))]
public static class Spoofing_Constants_GetPlatformType_Postfix
{
    //Prefix patch of Constants.GetPlatformType to spoof the user's platform type
    public static bool Prefix(PlatformSpecificData __instance)
    {

        Platforms? platformType;

        if (Utils.stringToPlatformType(MalumMenu.spoofPlatform.Value, out platformType))
        {
            __instance.Platform = (Platforms)platformType;
        }

        return true;
    }
}