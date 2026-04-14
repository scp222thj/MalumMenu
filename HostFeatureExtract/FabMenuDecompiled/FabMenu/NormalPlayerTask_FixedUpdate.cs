using HarmonyLib;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(NormalPlayerTask), "FixedUpdate")]
public static class NormalPlayerTask_FixedUpdate
{
	public static void Postfix(NormalPlayerTask __instance)
	{
		if ((Object)(object)__instance.Arrow == (Object)null)
		{
			return;
		}
		if (!CheatToggles.showTaskArrows)
		{
			if (__instance.taskStep == 0)
			{
				((Component)__instance.Arrow).gameObject.SetActive(false);
			}
		}
		else if (ArrowHandler.IsOwnedAndIncomplete(__instance))
		{
			if (ArrowHandler.NeedsSpecialTarget(__instance))
			{
				ArrowHandler.SetArrowTargetForSpecialTasks(__instance);
			}
			((Component)__instance.Arrow).gameObject.SetActive(true);
		}
	}
}
