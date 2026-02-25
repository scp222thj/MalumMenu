using AmongUs.Data;

namespace MalumMenu;
public static class MalumSpoof
{
    public static void SpoofLevel()
    {
        // Parse Spoofing.Level config entry and turn it into a uint
        if (!string.IsNullOrEmpty(MalumMenu.spoofLevel.Value) &&
            uint.TryParse(MalumMenu.spoofLevel.Value, out uint parsedLevel) &&
            parsedLevel != DataManager.Player.Stats.Level)
        {

            // Store the spoofed level using DataManager
            DataManager.Player.stats.level = parsedLevel - 1;
            DataManager.Player.Save();
        }
    }

    public static string SpoofFriendCode()
    {
        string friendCode = MalumMenu.guestFriendCode.Value;
        if (string.IsNullOrWhiteSpace(friendCode))
        {
            friendCode = DestroyableSingleton<AccountManager>.Instance.GetRandomName();
        }
        return friendCode;
    }

    public static void SpoofPlatform(PlatformSpecificData platformSpecificData)
    {
        // Parse Spoofing.Platform config entry and save it as the spoofed platform type
        if (Utils.StringToPlatformType(MalumMenu.spoofPlatform.Value, out Platforms? platformType))
        {
            platformSpecificData.Platform = (Platforms)platformType;
        }
    }
}