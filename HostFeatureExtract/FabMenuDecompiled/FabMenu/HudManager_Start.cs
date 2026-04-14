using System;
using HarmonyLib;
using UnityEngine.Events;

namespace FabMenu;

[HarmonyPatch(typeof(HudManager), "Start")]
public static class HudManager_Start
{
	public static void Postfix(HudManager __instance)
	{
		((UnityEventBase)__instance.MapButton.OnClick).RemoveAllListeners();
		((UnityEvent)__instance.MapButton.OnClick).AddListener(UnityAction.op_Implicit((Action)delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			__instance.ToggleMapVisible(new MapOptions
			{
				Mode = (Modes)1
			});
		}));
	}
}
