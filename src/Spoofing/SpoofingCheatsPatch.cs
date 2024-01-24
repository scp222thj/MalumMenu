using AmongUs.Data;
using AmongUs.Data.Player;
using HarmonyLib;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Update))]
public static class Spoofing_EOSManager_Update_Postfix
{
    public static string defaultFC = null;
    public static string defaultLevel = null;

    // Postfix patch of EOSManager.Update to spoof friend codes
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

            else if (MalumMenu.spoofFriendCode.Value != "" && MalumMenu.spoofFriendCode.Value != __instance.FriendCode)
            {
                __instance.FriendCode = MalumMenu.spoofFriendCode.Value; //Set custom friend code from config file
            }

            // Return to default friend code if both cheats are disabled
            else if (defaultFC != null)
            {
                __instance.FriendCode = defaultFC;
                defaultFC = null;
            }

            if (MalumMenu.spoofLevel.Value != "" && int.Parse(MalumMenu.spoofLevel.Value) > 0 && int.Parse(MalumMenu.spoofLevel.Value) != (int)DataManager.Player.Stats.Level)
            {
                if (defaultLevel == null)
                {
                    defaultLevel = $"{DataManager.Player.Stats.Level}"; // Saves the current level, in case the cheat is disabled
                }

                DataManager.Player.stats.level = (uint)(int.Parse(MalumMenu.spoofLevel.Value) - 1); // Sets the level to the spoofed level
                DataManager.Player.Save(); // Saves the level in AU's system
            }

            else if (defaultLevel != null)
            {
                DataManager.Player.stats.level = (uint)int.Parse(defaultLevel); // Returns to the default level
                DataManager.Player.Save();
                defaultLevel = null;
            }
        }
        catch { }
    }
}