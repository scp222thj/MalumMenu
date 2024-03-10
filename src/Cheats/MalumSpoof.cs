using AmongUs.Data;

namespace MalumMenu;
public static class MalumSpoof
{
    public static uint parsedLevel;

    public static void spoofLevel()
    {
        // Parse Spoofing.Level config entry and turn it into a uint
        if (!string.IsNullOrEmpty(MalumMenu.spoofLevel.Value) && 
            uint.TryParse(MalumMenu.spoofLevel.Value, out parsedLevel) &&
            parsedLevel != DataManager.Player.Stats.Level)
        {

            // Temporarily save the spoofed level using DataManager
            DataManager.Player.stats.level = parsedLevel - 1;
            DataManager.Player.Save();
        }
    }

    public static string spoofFriendCode()
    {
        string friendCode = MalumMenu.guestFriendCode.Value;
        if (string.IsNullOrWhiteSpace(friendCode))
        {
            friendCode = DestroyableSingleton<AccountManager>.Instance.GetRandomName();
        }
        return friendCode;
    }

    public static void spoofPlatform(PlatformSpecificData platformSpecificData)
    {
        Platforms? platformType;

        // Parse Spoofing.Platform config entry and save it as the spoofed platform type
        if (Utils.stringToPlatformType(MalumMenu.spoofPlatform.Value, out platformType))
        {
            platformSpecificData.Platform = (Platforms)platformType;
        }
    }
}