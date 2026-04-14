using HarmonyLib;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(TextBoxTMP), "Update")]
public static class TextBoxTMP_Update
{
	public static void Postfix(TextBoxTMP __instance)
	{
		if (CheatToggles.chatJailbreak && __instance.hasFocus && (Input.GetKey((KeyCode)306) || Input.GetKey((KeyCode)305)))
		{
			if (Input.GetKeyDown((KeyCode)99))
			{
				GUIUtility.systemCopyBuffer = __instance.text;
			}
			if (Input.GetKeyDown((KeyCode)118))
			{
				__instance.SetText(__instance.text + GUIUtility.systemCopyBuffer, "");
			}
			if (Input.GetKeyDown((KeyCode)120))
			{
				GUIUtility.systemCopyBuffer = __instance.text;
				__instance.SetText("", "");
			}
		}
	}
}
