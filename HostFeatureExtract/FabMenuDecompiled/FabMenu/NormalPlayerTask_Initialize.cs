using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(NormalPlayerTask), "Initialize")]
public static class NormalPlayerTask_Initialize
{
	public static void Postfix(NormalPlayerTask __instance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((PlayerTask)__instance).TaskType == 7 && __instance.taskStep == 0 && Utils.getCurrentMapID() == 4)
		{
			AirshipUploadTask component = ((Component)__instance).GetComponent<AirshipUploadTask>();
			List<Vector2> val = ((PlayerTask)component).FindValidConsolesPositions();
			for (int i = 0; i < val.Count && i < ((Il2CppArrayBase<ArrowBehaviour>)(object)component.Arrows).Length; i++)
			{
				((Il2CppArrayBase<ArrowBehaviour>)(object)component.Arrows)[i].target = Vector2.op_Implicit(val[i]);
			}
			((PlayerTask)component).LocationDirty = true;
		}
		else
		{
			ArrowHandler.EnsureArrowExists(__instance);
			if (!ArrowHandler.NeedsSpecialTarget(__instance))
			{
				__instance.UpdateArrowAndLocation();
			}
			else if (ArrowHandler.IsOwnedAndIncomplete(__instance))
			{
				ArrowHandler.SetArrowTargetForSpecialTasks(__instance);
			}
		}
	}
}
