using AmongUs.Data.Player;
using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.ProductUserId), MethodType.Getter)]
public static class SpoofPUID_EOSManager_ProductUserId_Getter_Postfix
{
    //Postfix patch of EOSManager.ProductUserId to spoof PUIDs
    public static void Postfix(ref string __result)
    {
        if (MalumMenu.spoofPuid.Value != ""){
            __result = MalumMenu.spoofPuid.Value; //Set custom PUID from config file
        }
    }
}

[HarmonyPatch(typeof(EOSManager), nameof(EOSManager.Update))]
public static class SpoofFriendCode_EOSManager_Update_Postfix
{
    public static string defaultFC = null;

    //Postfix patch of EOSManager.Update to spoof friend codes
    public static void Postfix(EOSManager __instance)
    {
        if (CheatToggles.spoofRandomFC){ //spoofRandomFC cheat logic
            if (defaultFC == null){
                defaultFC = __instance.FriendCode; //Save default friend code before randomizing it
            }

            //Generate and save a completly random friend code
            string username = DestroyableSingleton<AccountManager>.Instance.GetRandomName().ToLower();
            string discriminator = new System.Random().Next(1000, 10000).ToString();
            __instance.FriendCode = username + "#" + discriminator;

            return; //Do not use friend code spoofing from config if spoofRandomFC cheat is already going on
        }

        if (MalumMenu.spoofFriendCode.Value != "" && MalumMenu.spoofFriendCode.Value != __instance.FriendCode){ //friendCodeSpoofing from config cheat logic
            __instance.FriendCode = MalumMenu.spoofFriendCode.Value; //Set custom friend code from config file
            
            return;
        }

        //Return to default friend code if both cheats are disabled
        if (defaultFC != null){
            __instance.FriendCode = defaultFC;
            defaultFC = null;
        }
    }
}

[HarmonyPatch(typeof(PlayerCustomizationData), nameof(PlayerCustomizationData.Name), MethodType.Getter)]
public static class SpoofRandomName_PlayerCustomizationData_Name_Getter_Postfix
{
    //Postfix patch of PlayerCustomizationData.Name (Getter) to return a random name
    public static void Postfix(PlayerCustomizationData __instance, ref string __result)
    {        
        if (CheatToggles.spoofRandomName){ //Return the default, normal name if the cheat is disabled
            __result = DestroyableSingleton<AccountManager>.Instance.GetRandomName();
            return;
        }
    }
}