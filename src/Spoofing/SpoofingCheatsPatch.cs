using AmongUs.Data;
using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Update))]
public static class Spoofing_EOSManager_Update_Postfix
{
    public static string defaultFC = null;
    public static uint? defaultLevel = null;
    public static uint parsedValue;

    // Postfix patch of EOSManager.Update to spoof player attributes
    public static void Postfix(EOSManager __instance)
    {
        try
        {
            if (CheatToggles.spoofRandomFC)
            {
                if (defaultFC == null)
                {
                    defaultFC = __instance.FriendCode; //Save default friend code before randomizing it, so that the cheat can be disabled
                }

                // Generate and save a completly random friend code, based on AU's official generation
                string username = DestroyableSingleton<AccountManager>.Instance.GetRandomName().ToLower();
                string discriminator = new System.Random().Next(1000, 10000).ToString();
                __instance.FriendCode = username + "#" + discriminator;
            }
            else if (!string.IsNullOrEmpty(MalumMenu.spoofFriendCode.Value) && MalumMenu.spoofFriendCode.Value != __instance.FriendCode)
            {
                __instance.FriendCode = MalumMenu.spoofFriendCode.Value; // Set custom friend code from config file
            }
            else if (defaultFC != null) // Return to default friend code if both cheats are disabled
            {
                __instance.FriendCode = defaultFC;
                defaultFC = null;
            }

            // Ensure the level to set is a real level
            if (!string.IsNullOrEmpty(MalumMenu.spoofLevel.Value) &&
                uint.TryParse(MalumMenu.spoofLevel.Value, out parsedValue) &&
                parsedValue != DataManager.Player.Stats.Level)
            {
                if (defaultLevel == null)
                {
                    defaultLevel = DataManager.Player.Stats.Level; // Set the default value in case the cheat is disabled
                }

                DataManager.Player.stats.level = parsedValue - 1;
                DataManager.Player.Save();
            }
            else if (defaultLevel != null) // Reset the level to the player's real value
            {
                DataManager.Player.stats.level = (uint)defaultLevel;
                DataManager.Player.Save();
                defaultLevel = null;
            }
        }
        catch { }
    }
}