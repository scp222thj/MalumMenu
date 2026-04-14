using AmongUs.Data;
using HarmonyLib;
using TMPro;

namespace FabMenu;

[HarmonyPatch(typeof(AmongUsClient), "Update")]
public static class AmongUsClient_Update
{
	public static void Postfix()
	{
		MalumSpoof.spoofLevel();
		if (DestroyableSingleton<EOSManager>.Instance.loginFlowFinished && FabMenu.guestMode.Value)
		{
			DataManager.Player.Account.LoginStatus = (AccountLoginStatus)1;
			if (string.IsNullOrWhiteSpace(DestroyableSingleton<EOSManager>.Instance.FriendCode))
			{
				string text = MalumSpoof.spoofFriendCode();
				EditAccountUsername editAccountUsername = DestroyableSingleton<EOSManager>.Instance.editAccountUsername;
				((TMP_Text)editAccountUsername.UsernameText).SetText(text, true);
				editAccountUsername.SaveUsername();
				DestroyableSingleton<EOSManager>.Instance.FriendCode = text;
			}
		}
	}
}
