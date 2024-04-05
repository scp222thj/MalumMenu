using HarmonyLib;
using AmongUs.Data;
using UnityEngine;
using System;
using System.Security.Cryptography;

namespace MalumMenu;

[HarmonyPatch(typeof(PlatformSpecificData), nameof(PlatformSpecificData.Serialize))]
public static class PlatformSpecificData_Serialize
{
    // Prefix patch of Constants.GetPlatformType to spoof the user's platform type
    public static void Prefix(PlatformSpecificData __instance)
    {

        MalumSpoof.spoofPlatform(__instance);

    }
}

[HarmonyPatch(typeof(SystemInfo), nameof(SystemInfo.deviceUniqueIdentifier), MethodType.Getter)]
public static class SystemInfo_deviceUniqueIdentifier_Getter
{
    // Postfix patch of SystemInfo.deviceUniqueIdentifier Getter method 
    // to hide the user's real unique deviceId by generating a random fake one
    public static void Postfix(ref string __result)
    {
        if (MalumMenu.spoofDeviceId.Value){

            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            __result = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        }
        
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Update))]
public static class AmongUsClient_Update
{
    public static void Postfix()
    {
        MalumSpoof.spoofLevel();

        if (EOSManager.Instance.loginFlowFinished && MalumMenu.guestMode.Value){

            DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;

            if (string.IsNullOrWhiteSpace(EOSManager.Instance.FriendCode))
            {
                string friendCode = MalumSpoof.spoofFriendCode();
                EditAccountUsername editUsername = EOSManager.Instance.editAccountUsername;
                editUsername.UsernameText.SetText(friendCode);
                editUsername.SaveUsername();
                EOSManager.Instance.FriendCode = friendCode;
            }

        }
    }
}

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShower_Start
{
    // Postfix patch of VersionShower.Start to show MalumMenu version
    public static void Postfix(VersionShower __instance)
    {
        if (MalumMenu.supportedAU.Contains(Application.version)){ // 检测AU版本

            __instance.text.text =  $"MalumMenu-Yu v{MalumMenu.malumVersion} (v{Application.version})"; // 支持
        
        }else{

            __instance.text.text =  $"MalumMenu-Yu v{MalumMenu.malumVersion} (<color=red>v{Application.version}</color>)\n使用了不被支持的AmongUs版本"; //不支持
        
        }

    }
}

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
    // Postfix patch of PingTracker.Update to show game metrics
    public static void Postfix(PingTracker __instance)
    {
        __instance.text.alignment = TMPro.TextAlignmentOptions.TopRight;
        
        __instance.text.text = $"- MalumMenu by scp222thj\n- MalumMenu-Yu by Yu" + // Mod info
                                Utils.getColoredPingText(AmongUsClient.Instance.Ping); // Colored Ping
#if DEBUG
        __instance.text.text += $"\n<color=red>Debug</color>";
#endif

        // Position adjustments
        var offset_x = 1.2f;
        if (HudManager.InstanceExists && HudManager._instance.Chat.chatButton.active) offset_x += 0.8f;
        if (FriendsListManager.InstanceExists && FriendsListManager._instance.FriendsListButton.Button.active) offset_x += 0.8f;
        __instance.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(offset_x, 0f, 0f);
        
    }
}

[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
public static class HatManager_Initialize
{
    public static void Postfix(HatManager __instance){

        CosmeticsUnlocker.unlockCosmetics(__instance);
        
    }
}

[HarmonyPatch(typeof(StatsManager), nameof(StatsManager.BanMinutesLeft), MethodType.Getter)]
public static class StatsManager_BanMinutesLeft
{
    // Prefix patch of Getter method for StatsManager.BanMinutesLeft to remove disconnect penalty
    public static void Postfix(StatsManager __instance, ref int __result)
    {
        if (CheatToggles.avoidBans){
            __instance.BanPoints = 0f; // Removes all BanPoints
            __result = 0; // Removes all BanMinutes
        }
    }
}

[HarmonyPatch(typeof(FullAccount), nameof(FullAccount.CanSetCustomName))]
public static class FullAccount_CanSetCustomName
{
    // Prefix patch of FullAccount.CanSetCustomName to allow the usage of custom names
    public static void Prefix(ref bool canSetName)
    {
        if (CheatToggles.unlockFeatures){ 
            canSetName = true;
        }
    }
}

[HarmonyPatch(typeof(AccountManager), nameof(AccountManager.CanPlayOnline))]
public static class AccountManager_CanPlayOnline
{
    // Prefix patch of AccountManager.CanPlayOnline to allow online games
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures){
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(InnerNet.InnerNetClient), nameof(InnerNet.InnerNetClient.JoinGame))]
public static class InnerNet_InnerNetClient_JoinGame
{
    // Prefix patch of InnerNet.InnerNetClient.JoinGame to allow online games
    public static void Prefix()
    {
        if (CheatToggles.unlockFeatures){
            DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;
        }
    }
}

[HarmonyPatch(typeof(Mushroom), nameof(Mushroom.FixedUpdate))]
public static class Mushroom_FixedUpdate
{
    public static void Postfix(Mushroom __instance)
    {
        MalumESP.sporeCloudVision(__instance);
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
public static class Vent_CanUse
{
    // Prefix patch of Vent.CanUse to allow venting for cheaters
    // Basically does what the original method did with the required modifications
    public static void Postfix(Vent __instance, GameData.PlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
    {
        if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead){
            if (CheatToggles.useVents){
                float num = float.MaxValue;
                PlayerControl @object = pc.Object;

                Vector3 center = @object.Collider.bounds.center;
		        Vector3 position = __instance.transform.position;
		        num = Vector2.Distance(center, position);
            
                // Allow usage of vents unless the vent is too far or there are objects blocking the player's path
                canUse = num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false);
                couldUse = true;
                __result = num;
            }    
        }
    }
}