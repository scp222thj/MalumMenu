using AmongUs.Data;

namespace MalumMenu;

public static class MalumSpoof
{
    public static uint parsedLevel;

    public static void spoofLevel()
    {
        string levelInput = MalumMenu.spoofLevel.Value;

        if (!string.IsNullOrWhiteSpace(levelInput) &&
            uint.TryParse(levelInput, out parsedLevel) &&
            parsedLevel != DataManager.Player.Stats.Level)
        {
            // Set spoofed level (internally stores +1)
            DataManager.Player.stats.level = parsedLevel - 1;
            DataManager.Player.Save();
        }
    }

    public static string spoofFriendCode()
    {
        string friendCode = MalumMenu.guestFriendCode.Value;

        if (string.IsNullOrWhiteSpace(friendCode))
        {
            friendCode = DestroyableSingleton<AccountManager>.Instance?.GetRandomName() ?? "Guest";
        }

        return friendCode;
    }

    public static void spoofPlatform(PlatformSpecificData platformSpecificData)
    {
        if (platformSpecificData == null) return;

        Platforms? platformType;

        if (Utils.stringToPlatformType(MalumMenu.spoofPlatform.Value, out platformType))
        {
            platformSpecificData.Platform = platformType.Value;
        }
    }
}
