using HarmonyLib;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
public static class HudManager_Start
{
	/// <summary>
	/// Postfix patch of HudManager.Start to give minimap access to impostors too
	/// </summary>
	/// <param name="__instance">The <c>HudManager</c> instance.</param>
	public static void Postfix(HudManager __instance)
	{
		__instance.MapButton.OnClick.RemoveAllListeners(); //Remove previous OnClick action

		// Always open normal map when map button is clicked
		// To access sabotage map, sabotage button can be used
		__instance.MapButton.OnClick.AddListener((Action) (() =>
        {
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
		__instance.ShadowQuad.gameObject.SetActive(!MalumESP.IsFullbrightActive()); // Fullbright

		if (Utils.IsChatUiActive()){ // AlwaysChat
			__instance.Chat.gameObject.SetActive(true);
		} else {
			Utils.CloseChat();
			__instance.Chat.gameObject.SetActive(false);
		}

		MalumCheats.UseVentCheat(__instance);
		MalumESP.ZoomOut(__instance);
		MalumESP.FreecamCheat();

		// Close PlayerPickMenu if there is no PPM cheat enabled
		if (PlayerPickMenu.playerpickMenu != null && CheatToggles.ShouldPPMClose()){
            PlayerPickMenu.playerpickMenu.Close();
        }
    }
}
