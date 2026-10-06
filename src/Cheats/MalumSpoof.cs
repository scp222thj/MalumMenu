using AmongUs.Data;

namespace MalumMenu;
public static class MalumSpoof
{
    public static void SpoofLevel()
    {
        var player = DataManager.Player;
        if (player == null) return;

        // Parse Spoofing.Level config entry and turn it into a uint
        if (!string.IsNullOrEmpty(MalumMenu.spoofLevel.Value) &&
            uint.TryParse(MalumMenu.spoofLevel.Value, out uint parsedLevel) &&
            parsedLevel != player.Stats.Level)
        {

            // Store the spoofed level using DataManager
            player.stats.level = parsedLevel - 1;
            player.Save();
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
}
