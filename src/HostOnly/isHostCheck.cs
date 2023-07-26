using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class isHostCheck
{
    public static bool isHost;

    //Patch of PlayerPhysics.LateUpdate just to check if LocalPlayer is hosting the game
    public static void Postfix(PlayerPhysics __instance)
    {
        isHost = AmongUsClient.Instance.AmHost;
    }
}