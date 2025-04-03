using HarmonyLib;
using AmongUs.Data;
using AmongUs.Data.Player;
using UnityEngine;
using System;
using System.Security.Cryptography;

namespace MalumMenu;

[HarmonyPatch(typeof(PlatformSpecificData), nameof(PlatformSpecificData.Serialize))]
public static class PlatformSpecificData_Serialize
{
    public static void Prefix(PlatformSpecificData __instance)
    {
        MalumSpoof.spoofPlatform(__instance);
    }
}

[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.UpdateCharCount))]
public static class FreeChatInputField_UpdateCharCount
{
    public static void Postfix(FreeChatInputField __instance)
    {
        if (!CheatToggles.chatJailbreak) return;

        int length = __instance.textArea.text.Length;
        __instance.charCountText.SetText($"{length}/{__instance.textArea.characterLimit}");

        if (length < 90)
        {
            __instance.charCountText.color = Color.black;
        }
        else if (length < 119)
        {
            __instance.charCountText.color = new Color(1f, 1f, 0f, 1f);
        }
        else
        {
            __instance.charCountText.color = Color.red;
        }
    }
}

[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.OnFieldChanged))]
public static class FreeChatInputField_Update_LogKeypresses
{
    public static void Postfix()
    {
        if (Event.current != null && Event.current.isKey)
        {
            char ch = Event.current.character;
            if (!char.IsControl(ch))
                Debug.Log($"[MalumMenu Debug] Typed char: {ch} (Unicode: U+{((int)ch):X4})");
        }
    }
}

[HarmonyPatch(typeof(SystemInfo), nameof(SystemInfo.deviceUniqueIdentifier), MethodType.Getter)]
public static class SystemInfo_deviceUniqueIdentifier_Getter
{
    public static void Postfix(ref string __result)
    {
        if (!MalumMenu.spoofDeviceId.Value) return;

        byte[] bytes = new byte[16];
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

        if (EOSManager.Instance.loginFlowFinished && MalumMenu.guestMode.Value)
        {
            DataManager.Player.Account.LoginStatus = EOSManager.AccountLoginStatus.LoggedIn;

            if (string.IsNullOrWhiteSpace(EOSManager.Instance.FriendCode))
            {
                string friendCode = MalumSpoof.spoofFriendCode();
                var editUsername = EOSManager.Instance.editAccountUsername;
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
    public static void Postfix(VersionShower __instance)
    {
        string version = $"MalumMenu v{MalumMenu.malumVersion}";

        if (MalumMenu.supportedAU.Contains(Application.version))
        {
            __instance.text.text = $"{version} (v{Application.version})";
        }
        else
        {
            __instance.text.text = $"{version} (<color=red>v{Application.version}</color>)";
        }
    }
}

[HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
public static class PingTracker_Update
{
    public static void Postfix(PingTracker __instance)
    {
        __instance.text.alignment = TMPro.TextAlignmentOptions.Center;

        if (AmongUsClient.Instance.IsGameStarted)
        {
            __instance.aspectPosition.DistanceFromEdge = new Vector3(-0.21f, 0.50f, 0f);
            __instance.text.text = $"MalumMenu by scp222thj ~ {Utils.getColoredPingText(AmongUsClient.Instance.Ping)}";
        }
        else
        {
            __instance.text.text = $"MalumMenu by scp222thj\n{Utils.getColoredPingText(AmongUsClient.Instance.Ping)}";
        }
    }
}

[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
public static class HatManager_Initialize
{
    public static void Postfix(HatManager __instance)
    {
        CosmeticsUnlocker.unlockCosmetics(__instance);
    }
}

[HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.BanMinutesLeft), MethodType.Getter)]
public static class PlayerBanData_BanMinutesLeft_Getter
{
    public static void Postfix(PlayerBanData __instance, ref int __result)
    {
        if (!CheatToggles.avoidBans) return;

        __instance.BanPoints = 0f;
        __result = 0;
    }
}

[HarmonyPatch(typeof(FullAccount), nameof(FullAccount.CanSetCustomName))]
public static class FullAccount_CanSetCustomName
{
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
    public static void Postfix(ref bool __result)
    {
        if (CheatToggles.unlockFeatures)
        {
            __result = true;
        }
    }
}

[HarmonyPatch(typeof(InnerNet.InnerNetClient), nameof(InnerNet.InnerNetClient.JoinGame))]
public static class InnerNet_InnerNetClient_JoinGame
{
    public static void Prefix()
    {
        if (CheatToggles.unlockFeatures)
        {
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
    public static void Postfix(Vent __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
    {
        if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead && CheatToggles.useVents)
        {
            var player = pc.Object;
            var center = player.Collider.bounds.center;
            var targetPos = __instance.transform.position;

            float dist = Vector2.Distance(center, targetPos);
            bool unblocked = !PhysicsHelpers.AnythingBetween(player.Collider, center, targetPos, Constants.ShipOnlyMask, false);

            canUse = dist <= __instance.UsableDistance && unblocked;
            couldUse = true;
            __result = dist;
        }
    }
}
