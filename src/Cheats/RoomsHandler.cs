using MS.Internal.Xml.XPath;
using System.Collections.Generic;
using System.Linq;

namespace MalumMenu;

public static class RoomsHandler
{
    public static List<SystemTypes> GetValidRooms()
    {
        if (!Utils.isShip || ShipStatus.Instance == null || ShipStatus.Instance.AllRooms == null)
            return new List<SystemTypes>();

        return ShipStatus.Instance.AllRooms
            .Select(r => r.RoomId)
            .Distinct()
            .Where(id => id != SystemTypes.Hallway && id != SystemTypes.Outside)
            .ToList();
    }

    public static void TeleportToRoom(SystemTypes room)
    {
        if (PlayerControl.LocalPlayer == null || ShipStatus.Instance == null) return;

        foreach (var mapRoom in ShipStatus.Instance.AllRooms)
        {
            if (mapRoom != null && mapRoom.RoomId == room)
            {
                PlayerControl.LocalPlayer.transform.position = mapRoom.transform.position;
                return;
            }
        }
    }
}
