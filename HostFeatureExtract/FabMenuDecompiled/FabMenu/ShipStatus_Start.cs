using HarmonyLib;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(ShipStatus), "Start")]
public static class ShipStatus_Start
{
	public static void Postfix(ShipStatus __instance)
	{
		if (CheatToggles.destroyShip && Utils.isHost && (Object)(object)__instance != (Object)null)
		{
			try
			{
				((Component)__instance).gameObject.SetActive(false);
			}
			catch
			{
			}
		}
	}
}
