using System.Collections.Generic;
using System.Linq;

namespace MalumMenu;

public static class DoorsHandler
{
    // Returns a list of all rooms that have doors
    public static List<SystemTypes> GetRoomsWithDoors()
    {
        if (!Utils.isShip || ShipStatus.Instance.AllDoors.Count <= 0) return new List<SystemTypes>();

        return ShipStatus.Instance.AllDoors.Select(d => d.Room).Distinct().ToList();
    }

    // Returns a list of all doors in a specified room
    public static List<OpenableDoor> GetDoorsInRoom(SystemTypes room)
    {
        if (!Utils.isShip || ShipStatus.Instance.AllDoors.Count <= 0) return new List<OpenableDoor>();

        return ShipStatus.Instance.AllDoors.Where(d => d.Room == room).ToList();
    }

    // Returns the aggregate status of doors in a specified room
    public static string GetStatusOfDoorsInRoom(SystemTypes room, bool colorize)
    {
        var doorsInRoom = GetDoorsInRoom(room);
        if (doorsInRoom.Count <= 0) return "N/A";
        if (doorsInRoom.All(d => d.IsOpen)) return colorize ? "<color=#00FF00>Open</color>" : "Open";
        if (doorsInRoom.All(d => !d.IsOpen)) return colorize ? "<color=#FF0000>Closed</color>" : "Closed";
        return colorize ? "<color=#FFFF00>Mixed</color>" : "Mixed";
    }

    // Opens all doors in a specified room
    public static void OpenDoorsInRoom(SystemTypes doorRoom)
    {
        foreach (var door in GetDoorsInRoom(doorRoom))
        {
            OpenDoor(door);
        }
    }

    // Closes all doors in a specified room
    public static void CloseDoorsInRoom(SystemTypes doorRoom)
    {
        try { ShipStatus.Instance.RpcCloseDoorsOfType(doorRoom); } catch { }
    }

    // Opens all doors on the map
    public static void OpenAllDoors()
    {
        foreach (var door in ShipStatus.Instance.AllDoors)
        {
            OpenDoor(door);
        }
    }

    // Closes all doors on the map
    public static void CloseAllDoors()
    {
        foreach (var door in ShipStatus.Instance.AllDoors)
        {
            try { ShipStatus.Instance.RpcCloseDoorsOfType(door.Room); } catch { }
        }
    }

    // Opens a specific door
    public static void OpenDoor(OpenableDoor openableDoor)
    {
        try { ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Doors, (byte)(openableDoor.Id | 64)); } catch { }
    }
}
