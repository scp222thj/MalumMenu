using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class isShipCheck
{
    public static bool isShip;

    //Patch of PlayerPhysics.LateUpdate just to check if the ship is present
    //This way certain cheats can't activate in lobbies
    public static void Postfix(PlayerPhysics __instance)
    {
        isShip = (bool)ShipStatus.Instance;
    }
}