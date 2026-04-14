using HarmonyLib;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(HudManager), "Update")]
public static class HudManager_Update
{
	public static void Postfix(HudManager __instance)
	{
		((Component)__instance.ShadowQuad).gameObject.SetActive(!MalumESP.fullBrightActive());
		if (Utils.chatUiActive())
		{
			((Component)__instance.Chat).gameObject.SetActive(true);
		}
		else
		{
			Utils.closeChat();
			((Component)__instance.Chat).gameObject.SetActive(false);
		}
		MalumCheats.useVentCheat(__instance);
		MalumESP.zoomOut(__instance);
		MalumESP.freecamCheat();
		if ((Object)(object)PlayerPickMenu.playerpickMenu != (Object)null && CheatToggles.shouldPPMClose())
		{
			((Minigame)PlayerPickMenu.playerpickMenu).Close();
		}
	}
}
