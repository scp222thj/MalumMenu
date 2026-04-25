using AmongUs.Data;

namespace MalumMenu;
public static class MalumSpoof
{
    public static void SpoofLevel()
    {
        // Parse Spoofing.Level config entry and turn it into a uint
        if (!string.IsNullOrEmpty(MalumMenu.Settings.SpoofLevel.Value) &&
            uint.TryParse(MalumMenu.Settings.SpoofLevel.Value, out uint parsedLevel) &&
            parsedLevel != DataManager.Player.Stats.Level)
        {

            // Store the spoofed level using DataManager
            DataManager.Player.stats.level = parsedLevel - 1;
            DataManager.Player.Save();
        }
    }

    public static string SpoofFriendCode()
    {
        string friendCode = MalumMenu.Settings.GuestFriendCode.Value;
        if (string.IsNullOrWhiteSpace(friendCode))
        {
            friendCode = DestroyableSingleton<AccountManager>.Instance.GetRandomName();
        }
        return friendCode;
    }
}
