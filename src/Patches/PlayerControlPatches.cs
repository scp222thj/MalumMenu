using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.TurnOnProtection))]
public static class PlayerControl_TurnOnProtection
{
	//Prefix patch of PlayerControl.ProtectPlayer to render all protections visible if CheatSettings.seeGhosts is enabled or if LocalPlayer is dead
	//Basically does what the original method did with the required modifications
    public static void Prefix(ref bool visible){
		if (CheatToggles.seeGhosts){
            visible = true;
        }
    }
}    