using System;
using HarmonyLib;
using Il2CppSystem;

namespace FabMenu;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class AmongUsDateTime_UtcNow
{
	public static bool Prefix(ref DateTime __result)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.spoofAprilFoolsDate)
		{
			return true;
		}
		DateTime dateTime = new DateTime(DateTime.UtcNow.Year, 4, 2, 7, 1, 0, DateTimeKind.Utc);
		__result = new DateTime(dateTime.Ticks);
		return false;
	}
}
