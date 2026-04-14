using System;
using HarmonyLib;
using Il2CppSystem;

namespace FabMenu;

[HarmonyPatch(typeof(EOSManager), "StartInitialLoginFlow")]
public static class EOSManager_StartInitialLoginFlow
{
	public static bool Prefix(EOSManager __instance)
	{
		__instance.DeleteDeviceID(Action.op_Implicit((Action)__instance.EndMergeGuestAccountFlow));
		if (!FabMenu.guestMode.Value)
		{
			return true;
		}
		__instance.StartTempAccountFlow();
		__instance.CloseStartupWaitScreen();
		return false;
	}
}
