using HarmonyLib;
using TMPro;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(DisconnectPopup), "DoShow")]
public static class DisconnectPopup_DoShow
{
	public static void Postfix(DisconnectPopup __instance)
	{
		if (CheatToggles.copyLobbyCodeOnDisconnect)
		{
			GUIUtility.systemCopyBuffer = AmongUsClient_OnGameJoined.lastGameIdString;
			__instance.SetText(((TMP_Text)__instance._textArea).text + "\n\nLobby code has been copied to the clipboard.");
		}
	}
}
