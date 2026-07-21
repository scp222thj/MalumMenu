using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcEnterVent))]
public static class PlayerPhysics_RpcEnterVent_VentLog
{
    public static void Postfix(PlayerPhysics __instance, int ventId)
    {
        var room = Utils.GetRoomFromPosition(__instance.transform.position);
        var location = room != null ? room.RoomId.ToString() : "Unknown";

        EventLogger.LogVent(__instance.myPlayer, ventId, true);
    }
}
