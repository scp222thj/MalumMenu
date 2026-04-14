using HarmonyLib;
using InnerNet;
using TMPro;

namespace FabMenu;

[HarmonyPatch(typeof(GameContainer), "SetupGameInfo")]
public static class GameContainer_SetupGameInfo
{
	public static void Postfix(GameContainer __instance)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.moreLobbyInfo)
		{
			string trueHostName = __instance.gameListing.TrueHostName;
			int age = __instance.gameListing.Age;
			string value = $"Age: {age / 60}:{((age % 60 < 10) ? "0" : "")}{age % 60}";
			string value2 = Utils.PlatformTypeToString(__instance.gameListing.Platform);
			((TMP_Text)__instance.capacity).text = $"<size=40%>{"<#0000>000000000000000</color>"}\n{trueHostName}\n{((TMP_Text)__instance.capacity).text}\n<#fb0>{GameCode.IntToGameName(__instance.gameListing.GameId)}</color>\n<#b0f>{value2}</color>\n{value}\n{"<#0000>000000000000000</color>"}</size>";
		}
	}
}
