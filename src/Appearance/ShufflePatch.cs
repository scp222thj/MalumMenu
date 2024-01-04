using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ShuffleOutfit_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to shuffle/randomize the user's outfit
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.shuffleOutfit)
        {
            Utils.ShuffleOutfit(PlayerControl.LocalPlayer);

            CheatToggles.shuffleOutfit = false;
        }
    }
}
