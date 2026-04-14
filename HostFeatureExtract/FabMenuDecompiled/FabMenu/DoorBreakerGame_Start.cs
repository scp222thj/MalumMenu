using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(DoorBreakerGame), "Start")]
public static class DoorBreakerGame_Start
{
	public static bool Prefix(DoorBreakerGame __instance)
	{
		if (!CheatToggles.autoOpenDoorsOnUse)
		{
			return true;
		}
		DoorsHandler.OpenDoor(__instance.MyDoor);
		((SomeKindaDoor)__instance.MyDoor).SetDoorway(true);
		((Minigame)__instance).Close();
		return false;
	}
}
