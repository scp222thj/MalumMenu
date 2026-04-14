using System.Collections.Generic;
using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(TextBoxTMP), "IsCharAllowed")]
public static class TextBoxTMP_IsCharAllowed
{
	public static bool Prefix(TextBoxTMP __instance, char i, ref bool __result)
	{
		if (!CheatToggles.chatJailbreak)
		{
			return true;
		}
		if (new HashSet<char> { '\b', '\r' }.Contains(i))
		{
			__result = false;
			return false;
		}
		__result = true;
		return false;
	}
}
