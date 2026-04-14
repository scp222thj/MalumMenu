using HarmonyLib;
using InnerNet;
using TMPro;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(PingTracker), "Update")]
public static class PingTracker_Update
{
	public static void Postfix(PingTracker __instance)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.stealthMode)
		{
			((TMP_Text)__instance.text).alignment = (TextAlignmentOptions)257;
			return;
		}
		((TMP_Text)__instance.text).alignment = (TextAlignmentOptions)514;
		if (((InnerNetClient)AmongUsClient.Instance).IsGameStarted)
		{
			__instance.aspectPosition.DistanceFromEdge = new Vector3(-0.21f, 0.5f, 0f);
			((TMP_Text)__instance.text).text = Utils.getColoredPingText(((InnerNetClient)AmongUsClient.Instance).Ping);
		}
		else
		{
			((TMP_Text)__instance.text).text = Utils.getColoredPingText(((InnerNetClient)AmongUsClient.Instance).Ping);
		}
	}
}
