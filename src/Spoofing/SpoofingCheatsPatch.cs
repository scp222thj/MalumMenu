using AmongUs.Data;
using AmongUs.Data.Player;
using HarmonyLib;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Update))]
public static class Spoofing_EOSManager_Update_Postfix
{
    public static string defaultFC = null;
    public static int defaultLevel = -1;

    //Postfix patch of EOSManager.Update to spoof friend codes
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

            else if (MalumMenu.spoofFriendCode.Value != "" && MalumMenu.spoofFriendCode.Value != __instance.FriendCode){ //friendCodeSpoofing from config cheat logic
                __instance.FriendCode = MalumMenu.spoofFriendCode.Value; //Set custom friend code from config file
            }

            //Return to default friend code if both cheats are disabled
            else if (defaultFC != null){
                __instance.FriendCode = defaultFC;
                defaultFC = null;
            }

            if (CheatToggles.spoofLevel && MalumMenu.spoofLevel.Value.ToString() != "" && MalumMenu.spoofLevel.Value > 0)
            {
                if (defaultLevel == -1)
                {
                    defaultLevel = (int)DataManager.Player.Stats.Level;
                }
                DataManager.Player.stats.level = (uint)MalumMenu.spoofLevel.Value - 1;
                DataManager.Player.Save();
            }
            else if (defaultLevel > 0)
            {
                DataManager.Player.stats.level = (uint)defaultLevel;
                DataManager.Player.Save();
                defaultLevel = -1;
            }
        }
        catch{}
    }
}