using HarmonyLib;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(BanMenu), "SetVisible")]
public static class BanMenu_SetVisible
{
	public static bool Prefix(BanMenu __instance, bool show)
	{
		((Component)__instance).gameObject.SetActive(true);
		((Component)__instance.KickButton).gameObject.SetActive(true);
		((Component)__instance.MenuButton).gameObject.SetActive(true);
		if (Utils.isHost)
		{
			((Component)__instance.BanButton).gameObject.SetActive(true);
		}
		else
		{
			((Component)__instance.BanButton).gameObject.SetActive(false);
		}
		return false;
	}
}
