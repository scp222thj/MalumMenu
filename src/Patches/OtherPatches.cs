using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlatformSpecificData), nameof(PlatformSpecificData.Serialize))]
public static class PlatformSpecificData_Serialize
{
    //Prefix patch of Constants.GetPlatformType to spoof the user's platform type
    public static bool Prefix(PlatformSpecificData __instance)
    {

        return SpoofingHandler.spoofPlatform(__instance);

    }
}

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShower_Start
{
    //Postfix patch of VersionShower.Start to show MalumMenu version
    public static void Postfix(VersionShower __instance)
    {
        if (MalumMenu.supportedAU.Contains(Application.version)){ // Checks if Among Us version is supported

            __instance.text.text =  $"MalumMenu v{MalumMenu.malumVersion} (v{Application.version})"; // Supported
        
        }else{

            __instance.text.text =  $"MalumMenu v{MalumMenu.malumVersion} (<color=red>v{Application.version}</color>)"; //Unsupported
        
        }
    }
}

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
    //Postfix patch of PingTracker.Update to show game metrics
    public static void Postfix(PingTracker __instance)
    {
        __instance.text.alignment = TMPro.TextAlignmentOptions.TopRight;
        
        __instance.text.text = $"MalumMenu by scp222thj" + //ModInfo
                                Utils.getColoredPingText(AmongUsClient.Instance.Ping); //Colored Ping

        //Position adjustments
        var offset_x = 1.2f;
        if (HudManager.InstanceExists && HudManager._instance.Chat.chatButton.active) offset_x += 0.8f;
        if (FriendsListManager.InstanceExists && FriendsListManager._instance.FriendsListButton.Button.active) offset_x += 0.8f;
        __instance.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(offset_x, 0f, 0f);
        
    }
}

[HarmonyPatch(typeof(BackendEndpoints), nameof(BackendEndpoints.Announcements), MethodType.Getter)]
public static class BackendEndpoints_Announcements_Getter
{
    //Prefix patch of Getter method for BackendEndpoints.Announcements for custom announcements
    public static bool Prefix(ref string __result)
    {
        __result = "https://scp222thj.dev/malumnews"; //MalumNews webserver
        return false;
    }
}

[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
public static class HatManager_Initialize
{
    //Postfix patch of HatManager.Initialize to loop through all cosmetics and set them as free
    public static void Postfix(HatManager __instance){

        CosmeticsUnlocker.unlockCosmetics(__instance);
        
    }
}

[HarmonyPatch(typeof(StatsManager), nameof(StatsManager.BanMinutesLeft), MethodType.Getter)]
public static class StatsManager_BanMinutesLeft
{
    //Prefix patch of Getter method for StatsManager.BanMinutesLeft to remove disconnect penalty
    public static void Postfix(StatsManager __instance, ref int __result)
    {
        if (CheatToggles.avoidBans){
            __instance.BanPoints = 0f; //Removes all BanPoints
            __result = 0; //Removes all BanMinutes
        }
    }
}

[HarmonyPatch(typeof(FullAccount), nameof(FullAccount.CanSetCustomName))]
public static class FullAccount_CanSetCustomName
{
    //Prefix patch of FullAccount.CanSetCustomName to allow the usage of custom names
    public static void Prefix(ref bool canSetName)
    {
        if (CheatToggles.unlockFeatures){ //Only works if CheatSettings.unlockFeatures is enabled
            canSetName = true;
        }
    }
}

[HarmonyPatch(typeof(AccountManager), nameof(AccountManager.CanPlayOnline))]
public static class AccountManager_CanPlayOnline
{
    //Prefix patch of AccountManager.CanPlayOnline to allow online games
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures){ //Only works if CheatSettings.unlockFeatures is enabled
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(InnerNet.InnerNetClient), nameof(InnerNet.InnerNetClient.JoinGame))]
public static class InnerNet_InnerNetClient_JoinGame
{
    //Prefix patch of InnerNet.InnerNetClient.JoinGame to allow online games
    public static void Prefix()
    {
        if (CheatToggles.unlockFeatures){ //Only works if CheatSettings.unlockFeatures is enabled
            AmongUs.Data.DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;
        }
    }
}

[HarmonyPatch(typeof(Mushroom), nameof(Mushroom.FixedUpdate))]
public static class SporeVision_Mushroom_FixedUpdate_Postfix
{
    //Postfix patch of Mushroom.FixedUpdate that slightly moves spore clouds on the Z axis when sporeVision is enabled
    //This allows sporeVision users to see players even when they are inside a spore cloud
    public static void Postfix(Mushroom __instance)
    {
        EspHandler.sporeCloudVision(__instance);
    }
}