using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(AirshipUploadTask), "FixedUpdate")]
public static class AirshipUploadTask_FixedUpdate_Patch
{
	public static void Postfix(AirshipUploadTask __instance)
	{
		if (__instance.Arrows == null)
		{
			return;
		}
		if (!CheatToggles.showTaskArrows)
		{
			if (((NormalPlayerTask)__instance).taskStep != 0)
			{
				return;
			}
			{
				foreach (ArrowBehaviour item in (Il2CppArrayBase<ArrowBehaviour>)(object)__instance.Arrows)
				{
					((Component)item).gameObject.SetActive(false);
				}
				return;
			}
		}
		List<Vector2> val = ((PlayerTask)__instance).FindValidConsolesPositions();
		for (int i = 0; i < ((Il2CppArrayBase<ArrowBehaviour>)(object)__instance.Arrows).Length; i++)
		{
			((Component)((Il2CppArrayBase<ArrowBehaviour>)(object)__instance.Arrows)[i]).gameObject.SetActive(i < val.Count && (Object)(object)((PlayerTask)__instance).Owner != (Object)null && ((InnerNetObject)((PlayerTask)__instance).Owner).AmOwner);
		}
	}
}
