using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class NoClip_PlayerPhysics_LateUpdate_Postfix
{
    // Postfix patch of PlayerPhysics.LateUpdate that disables player collider when NoClipping or climbing a ladder
    public static void Postfix(PlayerPhysics __instance)
    {
        try
        {
            PlayerControl.LocalPlayer.Collider.enabled = !(CheatToggles.noClip || PlayerControl.LocalPlayer.onLadder);
        }
        catch { }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class SpeedBoost_PlayerPhysics_LateUpdate_Postfix
{
    // Postfix patch of PlayerPhysics.LateUpdate to double player speed
    public static void Postfix(PlayerPhysics __instance)
    {
        // try-catch to avoid some errors I was reciving in the logs related to this cheat
        try
        {
            // PlayerControl.LocalPlayer.MyPhysics.Speed is the base speed of a player
            // Among Us uses this value with the associated game setting to calculate the TrueSpeed of the player
            if (CheatToggles.speedBoost)
            {
                PlayerControl.LocalPlayer.MyPhysics.Speed = 2.5f * 2;
            }
            else
            {
                PlayerControl.LocalPlayer.MyPhysics.Speed = 2.5f; //By default, Speed is always 2.5f
            }
        }
        catch { }
    }
}