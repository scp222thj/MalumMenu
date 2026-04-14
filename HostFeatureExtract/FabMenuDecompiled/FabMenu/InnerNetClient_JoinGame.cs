using AmongUs.Data;
using HarmonyLib;
using InnerNet;

namespace FabMenu;

[HarmonyPatch(typeof(InnerNetClient), "JoinGame")]
public static class InnerNetClient_JoinGame
{
	public static void Prefix()
	{
		if (CheatToggles.unlockFeatures)
		{
			DataManager.Player.Account.LoginStatus = (AccountLoginStatus)1;
		}
	}
}
