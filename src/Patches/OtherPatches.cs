using HarmonyLib;
using AmongUs.Data;
using AmongUs.Data.Player;
using AmongUs.GameOptions;
using UnityEngine;
using System;
using System.Linq;
using System.Security.Cryptography;
using InnerNet;

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

[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.UpdateCharCount))]
public static class FreeChatInputField_UpdateCharCount
{
    // Postfix patch of FreeChatInputField.UpdateCharCount to change how charCountText displays
    public static void Postfix(FreeChatInputField __instance)
    {
        if (!CheatToggles.chatJailbreak){
            return; // Only works if CheatToggles.chatJailbreak is enabled
        }

        // Update charCountText to account for longer characterLimit

        int length = __instance.textArea.text.Length;
        __instance.charCountText.SetText($"{length}/{__instance.textArea.characterLimit}");

        if (length < 90){ // Under 75%

            __instance.charCountText.color = Color.black;

        }else if (length < 119){ // Under 100%

            __instance.charCountText.color = new Color(1f, 1f, 0f, 1f);

        }else{ // Over or equal to 100%

            __instance.charCountText.color = Color.red;

        }
    }
}

[HarmonyPatch(typeof(SystemInfo), nameof(SystemInfo.deviceUniqueIdentifier), MethodType.Getter)]
public static class SystemInfo_deviceUniqueIdentifier_Getter
{
    // Postfix patch of SystemInfo.deviceUniqueIdentifier Getter method
    // Made to hide the user's real unique deviceId by generating a random fake one
    public static void Postfix(ref string __result)
    {
        if (!MalumMenu.spoofDeviceId.Value) return;
        var bytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        __result = BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Update))]
public static class AmongUsClient_Update
{
    public static void Postfix()
    {
        MalumSpoof.spoofLevel();

        // Code to treat temp accounts the same as full accounts, including access to friend codes
        if (!EOSManager.Instance.loginFlowFinished || !MalumMenu.guestMode.Value) return;
        DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;

        if (!string.IsNullOrWhiteSpace(EOSManager.Instance.FriendCode)) return;
        var friendCode = MalumSpoof.spoofFriendCode();
        var editUsername = EOSManager.Instance.editAccountUsername;
        editUsername.UsernameText.SetText(friendCode);
        editUsername.SaveUsername();
        EOSManager.Instance.FriendCode = friendCode;
    }
}

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShower_Start
{
    /// <summary>
    /// Postfix patch of VersionShower.Start to show MalumMenu version
    /// </summary>
    /// <param name="__instance">The <c>VersionShower</c> instance.</param>
    public static void Postfix(VersionShower __instance)
    {
        if (CheatToggles.stealthMode) return;

        if (MalumMenu.supportedAU.Contains(Application.version)) // Checks if Among Us version is supported
        {
            __instance.text.text =  $"MalumMenu v{MalumMenu.malumVersion} (v{Application.version})"; // Supported
        }
        else
        {
            __instance.text.text =  $"MalumMenu v{MalumMenu.malumVersion} (<color=red>v{Application.version}</color>)"; // Unsupported
        }
    }
}

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
    /// <summary>
    /// Postfix patch of PingTracker.Update to show MalumMenu author and colored ping text
    /// </summary>
    /// <param name="__instance">The <c>PingTracker</c> instance.</param>
    public static void Postfix(PingTracker __instance)
    {
        if (CheatToggles.stealthMode)
        {
            __instance.text.alignment = TMPro.TextAlignmentOptions.TopLeft;
            return;
        }

        __instance.text.alignment = TMPro.TextAlignmentOptions.Center;

        if (AmongUsClient.Instance.IsGameStarted){

            __instance.aspectPosition.DistanceFromEdge = new Vector3(-0.21f, 0.50f, 0f);

            __instance.text.text = $"MalumMenu by scp222thj & Astral ~ {Utils.getColoredPingText(AmongUsClient.Instance.Ping)}";

            return;
        }

        __instance.text.text = $"MalumMenu by scp222thj & Astral\n{Utils.getColoredPingText(AmongUsClient.Instance.Ping)}";

    }
}

[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
public static class HatManager_Initialize
{
    public static void Postfix(HatManager __instance){

        CosmeticsUnlocker.unlockCosmetics(__instance);

    }
}

[HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.BanMinutesLeft), MethodType.Getter)]
public static class PlayerBanData_BanMinutesLeft_Getter
{
    /// <summary>
    /// Postfix patch of PlayerBanData.BanMinutesLeft Getter method to remove disconnect penalty
    /// </summary>
    /// <param name="__instance">The <c>PlayerBanData</c> instance.</param>
    /// <param name="__result">Original return value of <c>BanMinutesLeft</c>.</param>
    public static void Postfix(PlayerBanData __instance, ref int __result)
    {
        if (!CheatToggles.avoidBans) return;
        __instance.BanPoints = 0f; // Removes all BanPoints
        __result = 0; // Removes all BanMinutes
    }
}

[HarmonyPatch(typeof(FullAccount), nameof(FullAccount.CanSetCustomName))]
public static class FullAccount_CanSetCustomName
{
    /// <summary>
    /// Prefix patch of FullAccount.CanSetCustomName to allow the usage of custom names
    /// </summary>
    /// <param name="canSetName">Whether the button to edit the name should be visible and enabled.</param>
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

[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
public static class InnerNetClient_JoinGame
{
    // Prefix patch of InnerNetClient.JoinGame to allow online games
    public static void Prefix()
    {
        if (CheatToggles.unlockFeatures){
            DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;
        }
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.CheckTaskCompletion))]
public static class GameManager_CheckTaskCompletion
{
    /// <summary>
    /// Prefix patch of GameManager.CheckTaskCompletion to prevent the game from ending
    /// </summary>
    /// <param name="__result">Original return value of <c>CheckTaskCompletion</c>.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(ref bool __result)
    {
        if (!CheatToggles.noGameEnd) return true;
        __result = false;
        return false;
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

// https://github.com/g0aty/SickoMenu/blob/main/hooks/PlainDoor.cpp
[HarmonyPatch(typeof(DoorBreakerGame), nameof(DoorBreakerGame.Start))]
public static class DoorBreakerGame_Start
{
    /// <summary>
    /// Prefix patch of DoorBreakerGame.Start to automatically open a door when the player interacts with it
    /// </summary>
    /// <param name="__instance">The <c>DoorBreakerGame</c> instance.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(DoorBreakerGame __instance)
    {
        if (!CheatToggles.autoOpenDoorsOnUse) return true;

        DoorsHandler.OpenDoor(__instance.MyDoor);
        __instance.MyDoor.SetDoorway(true);
        __instance.Close();
        return false;
    }
}

[HarmonyPatch(typeof(DoorCardSwipeGame), nameof(DoorCardSwipeGame.Begin))]
public static class DoorCardSwipeGame_Begin
{
    /// <summary>
    /// Prefix patch of DoorCardSwipeGame.Begin to automatically open a door when the player interacts with it
    /// </summary>
    /// <param name="__instance">The <c>DoorCardSwipeGame</c> instance.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(DoorCardSwipeGame __instance)
    {
        if (!CheatToggles.autoOpenDoorsOnUse) return true;

        DoorsHandler.OpenDoor(__instance.MyDoor);
        __instance.MyDoor.SetDoorway(true);
        __instance.Close();
        return false;
    }
}

[HarmonyPatch(typeof(MushroomDoorSabotageMinigame), nameof(MushroomDoorSabotageMinigame.Begin))]
public static class MushroomDoorSabotageMinigame_Begin
{
    /// <summary>
    /// Prefix patch of MushroomDoorSabotageMinigame.Begin to automatically open a door when the player interacts with it
    /// </summary>
    /// <param name="__instance">The <c>MushroomDoorSabotageMinigame</c> instance.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(MushroomDoorSabotageMinigame __instance)
    {
        if (!CheatToggles.autoOpenDoorsOnUse) return true;

        __instance.FixDoorAndCloseMinigame();
        return false;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
public static class Vent_CanUse
{
    // Prefix patch of Vent.CanUse to allow venting for cheaters
    // Basically does what the original method did with the required modifications
    public static void Postfix(Vent __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
    {
        if (!PlayerControl.LocalPlayer || !PlayerControl.LocalPlayer.Data) return;
        if (PlayerControl.LocalPlayer.Data.Role.CanVent || PlayerControl.LocalPlayer.Data.IsDead) return;
        if (!CheatToggles.useVents) return;
        var @object = pc.Object;

        var center = @object.Collider.bounds.center;
        var position = __instance.transform.position;
        var num = Vector2.Distance(center, position);

        // Allow usage of vents unless the vent is too far or there are objects blocking the player's path
        canUse = num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false);
        couldUse = true;
        __result = num;
    }
}

[HarmonyPatch(typeof(AmongUsDateTime), nameof(AmongUsDateTime.UtcNow), MethodType.Getter)]
public static class AmongUsDateTime_UtcNow
{
    /// <summary>
    /// Prefix patch of AmongUsDateTime.UtcNow to spoof the date to April 2nd, 7:01 AM UTC.
    /// This sets the map to dlekS ehT (The Skeld but flipped) when hosting a lobby.
    /// </summary>
    /// <param name="__result">Original return value of <c>UtcNow</c>.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(ref Il2CppSystem.DateTime __result)
    {
        if (!CheatToggles.spoofAprilFoolsDate) return true;

        var managedDate = new DateTime(DateTime.UtcNow.Year, 4, 2, 7, 1, 0, DateTimeKind.Utc);
        __result = new Il2CppSystem.DateTime(managedDate.Ticks);
        return false;
    }
}

// https://github.com/g0aty/SickoMenu/blob/main/hooks/LobbyBehaviour.cpp
[HarmonyPatch(typeof(GameContainer), nameof(GameContainer.SetupGameInfo))]
public static class GameContainer_SetupGameInfo
{
    /// <summary>
    /// Postfix patch of GameContainer.SetupGameInfo to show more information when finding a game:
    /// host name (e.g. Astral), lobby code (e.g. KLHCEG), host platform (e.g. Epic), and lobby age in minutes (e.g. 4:20)
    /// </summary>
    /// <param name="__instance">The <c>GameContainer</c> instance.</param>
    public static void Postfix(GameContainer __instance)
    {
        if (!CheatToggles.moreLobbyInfo) return;

        // The Crewmate icon gets aligned properly with this
        const string separator = "<#0000>000000000000000</color>";

        var trueHostName = __instance.gameListing.TrueHostName;
        var age = __instance.gameListing.Age;
        var lobbyTime = $"Age: {age / 60}:{(age % 60 < 10 ? "0" : "")}{age % 60}";
        var platform = Utils.PlatformTypeToString(__instance.gameListing.Platform);

        // Set the text of the capacity field to include the new information
        __instance.capacity.text = $"<size=40%>{separator}\n{trueHostName}\n{__instance.capacity.text}\n" +
                                   $"<#fb0>{GameCode.IntToGameName(__instance.gameListing.GameId)}</color>\n" +
                                   $"<#b0f>{platform}</color>\n{lobbyTime}\n{separator}</size>";
    }
}

[HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.AddVote))]
public static class VoteBanSystem_AddVote
{
    /// <summary>
    /// Prefix patch of VoteBanSystem.AddVote to instantly kick players when host votes to kick them
    /// </summary>
    /// <param name="__instance">The <c>VoteBanSystem</c> instance.</param>
    /// <param name="srcClient">The client ID of the player who is voting.</param>
    /// <param name="clientId">The client ID of the player being voted to kick.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(VoteBanSystem __instance, int srcClient, int clientId)
    {
        if (!Utils.isHost) return true;

        if (AmongUsClient.Instance.ClientId == srcClient)
        {
            AmongUsClient.Instance.KickPlayer(clientId, false);
        }
        return false;
    }
}

[HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.CmdAddVote))]
public static class VoteBanSystem_CmdAddVote
{
    /// <summary>
    /// Prefix patch of VoteBanSystem.CmdAddVote to prevent AddVoteBan RPC from being sent when host votes to kick a player
    /// </summary>
    /// <param name="clientIdToVoteBan">The client ID of the player being voted to kick.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(int clientIdToVoteBan)
    {
        return !Utils.isHost;
    }
}

[HarmonyPatch(typeof(BanMenu), nameof(BanMenu.SetVisible))]
public static class BanMenu_SetVisible
{
    /// <summary>
    /// Prefix patch of BanMenu.SetVisible to always show kick and ban buttons as host
    /// </summary>
    /// <param name="__instance">The <c>BanMenu</c> instance.</param>
    /// <param name="show">Whether to show the button that opens the ban menu.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(BanMenu __instance, bool show)
    {
        if (!Utils.isHost) return true;

        show &= PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data != null;
        __instance.BanButton.gameObject.SetActive(true);
        __instance.KickButton.gameObject.SetActive(true);
        __instance.MenuButton.gameObject.SetActive(show);
        return false;
    }
}

[HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
public static class IGameOptionsExtensions_GetAdjustedNumImpostors
{
    /// <summary>
    /// Prefix patch of IGameOptionsExtensions.GetAdjustedNumImpostors to remove impostor limits.
    /// </summary>
    /// <param name="__instance">The <c>IGameOptions</c> instance.</param>
    /// <param name="playerCount">The current player count. Unused even in the original method.</param>
    /// <param name="__result">Original return value of <c>GetAdjustedNumImpostors</c>.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(IGameOptions __instance, int playerCount, ref int __result)
    {
        if (!CheatToggles.noOptionsLimits) return true;
        __result = GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;
        return false;
    }
}
