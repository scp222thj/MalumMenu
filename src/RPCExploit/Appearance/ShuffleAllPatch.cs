using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ShuffleAllOutfit_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to shuffle/randomize the outfit of each player
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.shuffleAllOutfits)
        {
            foreach (var sender in PlayerControl.AllPlayerControls)
            {
                Utils.ShuffleOutfit(sender);
            }

            CheatToggles.shuffleAllOutfits = false;
        }
    }
}
