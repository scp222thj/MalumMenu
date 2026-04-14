using HarmonyLib;
using TMPro;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(VersionShower), "Start")]
public static class VersionShower_Start
{
	public static void Postfix(VersionShower __instance)
	{
		if (!CheatToggles.stealthMode)
		{
			if (FabMenu.supportedAU.Contains(Application.version))
			{
				((TMP_Text)__instance.text).text = $"zorobruh v{FabMenu.malumVersion} (v{Application.version})";
			}
			else
			{
				((TMP_Text)__instance.text).text = $"zorobruh v{FabMenu.malumVersion} (<color=red>v{Application.version}</color>)";
			}
		}
	}
}
