using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(DoorCardSwipeGame), "Begin")]
public static class DoorCardSwipeGame_Begin
{
	public static bool Prefix(DoorCardSwipeGame __instance)
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
