using HarmonyLib;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
public static class HudManager_Start
{
	// Postfix patch of HudManager.Start to give minimap access to impostors too
	public static void Postfix(HudManager __instance)
	{
		return;
		if (__instance == null) return;
		if (__instance.MapButton == null) return;
		if (__instance.MapButton.OnClick == null) return;

		__instance.MapButton.OnClick.RemoveAllListeners(); // Remove previous OnClick action

		// Always open normal map when map button is clicked
		// To access sabotage map, sabotage button can be used
		__instance.MapButton.OnClick.AddListener((Action) (() =>
        {
			if (__instance == null) return;
			__instance.ToggleMapVisible(new MapOptions
			{
				Mode = MapOptions.Modes.Normal
			});

		}));
	}
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class HudManager_Update
{
	public static void Postfix(HudManager __instance)
    {
		if (__instance == null) return;

		if (__instance.ShadowQuad != null && __instance.ShadowQuad.gameObject != null)
		{
			__instance.ShadowQuad.gameObject.SetActive(!MalumESP.IsFullbrightActive()); // Fullbright
		}

		if (Utils.IsChatUiActive()) // AlwaysChat
		{
			if (__instance.Chat != null && __instance.Chat.gameObject != null)
			{
				__instance.Chat.gameObject.SetActive(true);
			}
		}
		else
		{
			Utils.CloseChat();
			if (__instance.Chat != null && __instance.Chat.gameObject != null)
			{
				__instance.Chat.gameObject.SetActive(false);
			}
		}

		MalumCheats.UseVentCheat(__instance);
		MalumESP.ZoomOut(__instance);
		MalumESP.FreecamCheat();

		// Close PlayerPickMenu if there is no PPM cheat enabled
		if (PlayerPickMenu.playerpickMenu != null && CheatToggles.ShouldPPMClose())
		{
            PlayerPickMenu.playerpickMenu.Close();
        }
    }
}
