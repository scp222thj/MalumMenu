using HarmonyLib;
using AmongUs.Data;

namespace MalumMenu;


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

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
public static class AmongUsClient_OnGameJoined
{
    /// <summary>
    /// Postfix patch of AmongUsClient.OnGameJoined to store the last joined game ID string
    /// </summary>
    public static string lastGameIdString = "";

    public static void Postfix(string gameIdString)
    {
        lastGameIdString = gameIdString;
    }
}
