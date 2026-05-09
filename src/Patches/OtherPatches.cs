using HarmonyLib;
using AmongUs.Data;
using AmongUs.Data.Player;
using AmongUs.GameOptions;
using UnityEngine;
using System;
using System.Security.Cryptography;
using InnerNet;
using System.Collections.Generic;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Reflection;

namespace MalumMenu;

[HarmonyPatch(typeof(Constants), nameof(Constants.GetPlatformData))]
public static class Constants_GetPlatformData
{
    // Postfix patch of Constants.GetPlatformData to spoof the user's platform type
    public static void Postfix(ref PlatformSpecificData __result)
    {
        if (Utils.StringToPlatformType(MalumMenu.spoofPlatform.Value, out Platforms? platformType))
        {
            __result = new PlatformSpecificData
            {
                Platform = (Platforms)platformType,
                PlatformName = Constants.GetPlatformName()
            };
        }
    }
}

[HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), new[] { typeof(PlayerControl), typeof(DisconnectReasons) })]
public static class GameData_HandleDisconnect
{
    public static HashSet<int> disconnectQueue = new();

    // Prefix patch of GameData.HandleDisconnect to keep track of successful overloads
    public static void Prefix(PlayerControl player)
    {
        if (!CheatToggles.runOverload) return;

        NetworkedPlayerInfo playerData = player?.Data;
        if (playerData == null) return;

        bool isTarget = OverloadUI.currentTargets.Contains(playerData);

        if (isTarget) disconnectQueue.Add(playerData.ClientId);
    }

    // Postfix patch of GameData.HandleDisconnect to keep track of successful overloads
    // (Avoids race-condition double counting)
    public static void Postfix(PlayerControl player)
    {
        if (!CheatToggles.runOverload) return;

        NetworkedPlayerInfo playerData = player?.Data;
        if (playerData == null) return;

        int clientId = player.Data.ClientId;

        if (disconnectQueue.Contains(clientId))
        {
            OverloadUI.numSuccesses++;

            if (CheatToggles.olLogDisconnect)
            {
                int total = OverloadUI.currentTargets.Count // Targets still connected
                            + OverloadUI.numSuccesses // Targets already crashed
                            - disconnectQueue.Count; // Pending disconnect logs (Avoids race-condition double counting)

                string colorStr = ColorUtility.ToHtmlStringRGB(Color.green);

                OverloadUI.LogConsole($"> <b><color=#{colorStr}>!! {playerData.DefaultOutfit.PlayerName} (ID : {playerData.ClientId}) Disconnected !! - [{OverloadUI.numSuccesses}/{total}]</color></b>");
            }

            disconnectQueue.Remove(clientId);
        }
    }
}

[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.UpdateCharCount))]
public static class FreeChatInputField_UpdateCharCount
{
    // Postfix patch of FreeChatInputField.UpdateCharCount to change how charCountText displays
    public static void Postfix(FreeChatInputField __instance)
    {
        // Only works if CheatToggles.longerMsgs is enabled
        if (!CheatToggles.longerMessages) return;

        // Update charCountText to account for longer characterLimit
        int length = __instance.textArea.text.Length;
        __instance.charCountText.SetText($"{length}/{__instance.textArea.characterLimit}");

        if (length < 90) // Under 75%
        {
            __instance.charCountText.color = Color.black;
        }
        else if (length < 120) // Under 100%
        {
            __instance.charCountText.color = new Color(1f, 1f, 0f, 1f);
        }
        else // Over or equal to 100%
        {
            __instance.charCountText.color = Color.red;
        }
    }
}

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class ChatBubble_SetName
{
    public static void Postfix(ChatBubble __instance)
    {
        MalumESP.ChatNametags(__instance);
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

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
public static class VersionShower_Start
{
    // Postfix patch of VersionShower.Start to show MalumMenu version
    public static void Postfix(VersionShower __instance)
    {
        if (MalumMenu.inStealthMode || MalumMenu.isPanicked) return;

        if (MalumMenu.supportedAU.Contains(Application.version)) // Checks if Among Us version is supported
        {
            __instance.text.text = $"MalumMenu v{MalumMenu.malumVersion} (v{Application.version})"; // Supported
        }
        else
        {
            __instance.text.text = $"MalumMenu v{MalumMenu.malumVersion} (<color=red>v{Application.version}</color>)"; // Unsupported
        }
    }
}

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
    // Postfix patch of PingTracker.Update to show MalumMenu authors and colored ping text
    public static void Postfix(PingTracker __instance)
    {
        if (MalumMenu.inStealthMode)
        {
            __instance.text.alignment = TMPro.TextAlignmentOptions.TopLeft;

            return;
        }

        __instance.text.alignment = TMPro.TextAlignmentOptions.Center;

        int ping = Utils.GetPing();
        string pingText = Utils.GetColoredPingText($"PING: {ping} ms", ping);

        if (AmongUsClient.Instance.IsGameStarted)
        {
            __instance.aspectPosition.DistanceFromEdge = new Vector3(-0.21f, 0.50f, 0f);

            __instance.text.text = $"MalumMenu by scp222thj & Astral ~ {pingText}";

            return;
        }

        __instance.text.text = $"MalumMenu by scp222thj & Astral\n{pingText}";

    }
}

[HarmonyPatch(typeof(DisconnectPopup), nameof(DisconnectPopup.DoShow))]
public static class DisconnectPopup_DoShow
{
    // Postfix patch of DisconnectPopup.DoShow to copy lobby code to clipboard on disconnect
    public static void Postfix(DisconnectPopup __instance)
    {
        if (!CheatToggles.copyLobbyCodeOnDisconnect) return;

        GUIUtility.systemCopyBuffer = AmongUsClient_OnGameJoined.lastGameIdString;

        __instance.SetText(__instance._textArea.text + "\n\n<size=60%>Lobby code has been copied to the clipboard</size>");
    }
}

[HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.BanMinutesLeft), MethodType.Getter)]
public static class PlayerBanData_BanMinutesLeft_Getter
{
    // Postfix patch of PlayerBanData.BanMinutesLeft Getter method to remove disconnect penalty
    public static void Postfix(PlayerBanData __instance, ref int __result)
    {
        if (!CheatToggles.avoidPenalties) return;

        __instance.BanPoints = 0f; // Removes all BanPoints
        __result = 0; // Removes all BanMinutes
    }
}

[HarmonyPatch(typeof(FullAccount), nameof(FullAccount.CanSetCustomName))]
public static class FullAccount_CanSetCustomName
{
    // Prefix patch of FullAccount.CanSetCustomName to allow the usage of custom names
    public static void Prefix(ref bool canSetName)
    {
        if (CheatToggles.unlockFeatures)
        {
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
        if (CheatToggles.unlockFeatures)
        {
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
        if (CheatToggles.unlockFeatures)
        {
            DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;
        }
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.CheckTaskCompletion))]
public static class GameManager_CheckTaskCompletion
{
    // Prefix patch of GameManager.CheckTaskCompletion to prevent a running game from ending
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
        MalumESP.SporeCloudVision(__instance);
    }
}

// Found here: https://github.com/g0aty/SickoMenu/blob/main/hooks/PlainDoor.cpp
[HarmonyPatch(typeof(DoorBreakerGame), nameof(DoorBreakerGame.Start))]
public static class DoorBreakerGame_Start
{
    // Prefix patch of DoorBreakerGame.Start to automatically open a door when the player interacts with it
    public static bool Prefix(DoorBreakerGame __instance)
    {
        if (!CheatToggles.autoOpenDoorsOnUse) return true;

        var delay = MalumMenu.autoDoorOpenDelaySeconds.Value;
        if (delay <= 0.1f)
        {
            DoorsHandler.OpenDoor(__instance.MyDoor);
            __instance.MyDoor.SetDoorway(true);
            __instance.Close();
            return false;
        }

        __instance.StartCoroutine(DelayedDoorOpen(__instance.MyDoor, delay, __instance).WrapToIl2Cpp());

        return false;
    }

    private static IEnumerator DelayedDoorOpen(OpenableDoor door, float delay, Minigame minigame)
    {
        yield return new WaitForSeconds(delay);
        if (door != null)
        {
            DoorsHandler.OpenDoor(door);
            door.SetDoorway(true);
        }

        if (minigame != null)
        {
            minigame.Close();
        }
    }
}

// Found here: https://github.com/g0aty/SickoMenu/blob/main/hooks/PlainDoor.cpp
[HarmonyPatch(typeof(DoorCardSwipeGame), nameof(DoorCardSwipeGame.Begin))]
public static class DoorCardSwipeGame_Begin
{
    // Prefix patch of DoorCardSwipeGame.Begin to automatically open a door when the player interacts with it
    public static bool Prefix(DoorCardSwipeGame __instance)
    {
        if (!CheatToggles.autoOpenDoorsOnUse) return true;

        var delay = MalumMenu.autoDoorOpenDelaySeconds.Value;
        if (delay <= 0.1f)
        {
            DoorsHandler.OpenDoor(__instance.MyDoor);
            __instance.MyDoor.SetDoorway(true);
            __instance.Close();
            return false;
        }

        __instance.StartCoroutine(DelayedDoorOpen(__instance.MyDoor, delay, __instance).WrapToIl2Cpp());

        return false;
    }

    private static IEnumerator DelayedDoorOpen(OpenableDoor door, float delay, Minigame minigame)
    {
        yield return new WaitForSeconds(delay);
        if (door != null)
        {
            DoorsHandler.OpenDoor(door);
            door.SetDoorway(true);
        }

        if (minigame != null)
        {
            minigame.Close();
        }
    }
}

// Found here: https://github.com/g0aty/SickoMenu/blob/main/hooks/PlainDoor.cpp
[HarmonyPatch(typeof(MushroomDoorSabotageMinigame), nameof(MushroomDoorSabotageMinigame.Begin))]
public static class MushroomDoorSabotageMinigame_Begin
{
    // Prefix patch of MushroomDoorSabotageMinigame.Begin to automatically open a door when the player interacts with it
    public static bool Prefix(MushroomDoorSabotageMinigame __instance)
    {
        if (!CheatToggles.autoOpenDoorsOnUse) return true;

        var delay = MalumMenu.autoDoorOpenDelaySeconds.Value;
        if (delay <= 0.1f)
        {
            __instance.FixDoorAndCloseMinigame();
            return false;
        }

        __instance.StartCoroutine(DelayedFix(__instance, delay).WrapToIl2Cpp());

        return false;
    }

    private static IEnumerator DelayedFix(MushroomDoorSabotageMinigame minigame, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (minigame != null)
        {
            minigame.FixDoorAndCloseMinigame();
        }
    }
}

[HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
public static class Console_CanUse
{
    public static void Prefix(Console __instance, ref bool __state)
    {
        if (__instance == null) return;
        __state = __instance.AllowImpostor;
        if (!CheatToggles.impostorTasks) return;
        __instance.AllowImpostor = true;
    }

    public static void Postfix(Console __instance, ref bool __state)
    {
        if (__instance == null) return;
        __instance.AllowImpostor = __state;
    }
}

[HarmonyPatch(typeof(IntroCutscene), "CoBegin")]
public static class IntroCutscene_CoBegin
{
    // Prefix patch of IntroCutscene.CoBegin to force the LocalPlayer's role to a specified role
    public static void Prefix()
    {
        if (!Utils.isHost) return;
        if (!PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data == null) return;

        if (CheatToggles.forcedRole.HasValue)
        {
            ApplyForcedRole(CheatToggles.forcedRole.Value);
            return;
        }

        if (CheatToggles.hostAlwaysImpostor)
        {
            EnsureHostIsImpostor();
        }
    }

    public static void ApplyForcedRole(RoleTypes forcedRole)
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (!localPlayer || localPlayer.Data == null) return;

        var originalRole = localPlayer.Data.RoleType;
        if (originalRole == forcedRole) return;

        PlayerControl swapTarget = null;
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player || player.Data == null) continue;
            if (player.PlayerId == localPlayer.PlayerId) continue;
            if (player.Data.RoleType != forcedRole) continue;
            swapTarget = player;
            break;
        }

        DestroyableSingleton<RoleManager>.Instance.SetRole(localPlayer, forcedRole);

        if (swapTarget != null)
        {
            DestroyableSingleton<RoleManager>.Instance.SetRole(swapTarget, originalRole);
        }
    }

    public static void EnsureHostIsImpostor()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (!localPlayer || localPlayer.Data == null) return;
        if (localPlayer.Data.Role != null && localPlayer.Data.Role.IsImpostor) return;

        PlayerControl swapTarget = null;
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player || player.Data == null) continue;
            if (player.PlayerId == localPlayer.PlayerId) continue;
            if (player.Data.Role == null) continue;
            if (!player.Data.Role.IsImpostor) continue;
            swapTarget = player;
            break;
        }

        if (swapTarget == null) return;

        var hostRole = localPlayer.Data.RoleType;
        var impostorRole = swapTarget.Data.RoleType;

        DestroyableSingleton<RoleManager>.Instance.SetRole(localPlayer, impostorRole);
        DestroyableSingleton<RoleManager>.Instance.SetRole(swapTarget, hostRole);
    }
}

[HarmonyPatch]
public static class IntroCutscene_BeginTeam_EnsureHostImpostor
{
    private static bool _ran;

    public static IEnumerable<MethodBase> TargetMethods()
    {
        var methods = AccessTools.GetDeclaredMethods(typeof(IntroCutscene));
        foreach (var m in methods)
        {
            if (m == null) continue;
            if (m.Name != "BeginCrewmate" && m.Name != "BeginImpostor") continue;
            yield return m;
        }
    }

    public static void Prefix()
    {
        _ran = false;
    }

    public static void Postfix()
    {
        if (_ran) return;
        _ran = true;

        if (!Utils.isHost) return;
        if (!CheatToggles.hostAlwaysImpostor) return;

        try
        {
            IntroCutscene_CoBegin.EnsureHostIsImpostor();
        }
        catch
        {
        }
    }
}

// Found here: https://github.com/g0aty/SickoMenu/blob/main/hooks/LobbyBehaviour.cpp
[HarmonyPatch(typeof(GameContainer), nameof(GameContainer.SetupGameInfo))]
public static class GameContainer_SetupGameInfo
{
    // Postfix patch of GameContainer.SetupGameInfo to show more information when finding a game:
    // host name (e.g. Astral), lobby code (e.g. KLHCEG), host platform (e.g. Epic), and lobby age in minutes (e.g. 4:20)
    public static void Postfix(GameContainer __instance)
    {
        if (!CheatToggles.seeLobbyInfo) return;

        // The Crewmate icon gets aligned properly with this
        const string separator = "<#0000>000000000000000</color>";

        var trueHostName = __instance.gameListing.TrueHostName;

        var age = __instance.gameListing.Age;
        var lobbyTime = $"Age: {age / 60}:{(age % 60 < 10 ? "0" : "")}{age % 60}";

        var platform = Utils.PlatformTypeToString(__instance.gameListing.Platform);

        // Sets the text of the capacity field to include the new information
        __instance.capacity.text = $"<size=40%>{separator}\n{trueHostName}\n{__instance.capacity.text}\n" +
                                   $"<#fb0>{GameCode.IntToGameName(__instance.gameListing.GameId)}</color>\n" +
                                   $"<#b0f>{platform}</color>\n{lobbyTime}\n{separator}</size>";
    }
}

[HarmonyPatch(typeof(BanMenu), nameof(BanMenu.SetVisible))]
public static class BanMenu_SetVisible
{
    // Prefix patch of BanMenu.SetVisible to always show kick and ban buttons as host
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
    // Prefix patch of IGameOptionsExtensions.GetAdjustedNumImpostors to remove impostor limits
    public static bool Prefix(IGameOptions __instance, ref int __result)
    {
        if (!CheatToggles.noOptionsLimits) return true;

        __result = GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;

        return false;
    }
}

[HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.GetPurchase))]
public static class PlayerPurchasesData_GetPurchase
{
    // Postfix patch of PlayerPurchasesData.GetPurchase to unlock all cosmetics
    public static void Postfix(ref bool __result)
    {
        if (!CheatToggles.freeCosmetics) return;

        __result = true;
    }
}
