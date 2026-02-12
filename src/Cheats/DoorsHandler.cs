using System.Collections.Generic;
using System.Linq;

namespace MalumMenu;

public static class DoorsHandler
{
    /// <summary>
    /// Gets a list of all rooms that have doors.
    /// </summary>
    /// <returns>List of SystemTypes representing rooms with doors.</returns>
    public static List<SystemTypes> GetDoorRooms()
    {
        if (!Utils.isShip || ShipStatus.Instance.AllDoors.Count <= 0) return [];

        return ShipStatus.Instance.AllDoors.Select(d => d.Room).Distinct().ToList();
    }

    /// <summary>
    /// Gets all doors in a specified room.
    /// </summary>
    /// <param name="room">The room to get doors from.</param>
    /// <returns>List of OpenableDoor objects in the specified room.</returns>
    public static List<OpenableDoor> GetDoorsInRoom(SystemTypes room)
    {
        if (!Utils.isShip || ShipStatus.Instance.AllDoors.Count <= 0) return [];

        return ShipStatus.Instance.AllDoors.Where(d => d.Room == room).ToList();
    }

    /// <summary>
    /// Gets the status of doors in a specified room.
    /// </summary>
    /// <param name="room">The room to check door status for.</param>
    /// <param name="colorize">Whether to colorize the status string.</param>
    /// <returns>A string representing the status: "Open", "Closed", "Mixed", or "N/A".</returns>
    public static string GetStatusOfDoorsInRoom(SystemTypes room, bool colorize)
    {
        var doorsInRoom = GetDoorsInRoom(room);
        if (doorsInRoom.Count <= 0) return "N/A";
        if (doorsInRoom.All(d => d.IsOpen)) return colorize ? "<color=#00FF00>Open</color>" : "Open";
        if (doorsInRoom.All(d => !d.IsOpen)) return colorize ? "<color=#FF0000>Closed</color>" : "Closed";
        return colorize ? "<color=#FFFF00>Mixed</color>" : "Mixed";
    }

    /// <summary>
    /// Opens all doors in a specified room.
    /// </summary>
    /// <param name="doorRoom">The room to open doors in.</param>
    public static void OpenDoorsOfRoom(SystemTypes doorRoom)
    {
        foreach (var door in GetDoorsInRoom(doorRoom))
            OpenDoor(door);
    }

    /// <summary>
    /// Closes all doors in a specified room.
    /// </summary>
    /// <param name="doorRoom">The room to close doors in.</param>
    public static void CloseDoorsOfRoom(SystemTypes doorRoom)
    {
        try { ShipStatus.Instance.RpcCloseDoorsOfType(doorRoom); } catch { }
    }

    /// <summary>
    /// Opens all doors on the map.
    /// </summary>
    public static void OpenAllDoors()
    {
        foreach (var door in ShipStatus.Instance.AllDoors)
            OpenDoor(door);
    }

    /// <summary>
    /// Closes all doors on the map.
    /// </summary>
    public static void CloseAllDoors()
    {
        foreach (var door in ShipStatus.Instance.AllDoors)
        {
            try { ShipStatus.Instance.RpcCloseDoorsOfType(door.Room); } catch { }
        }
    }

    /// <summary>
    /// Opens a specific door.
    /// </summary>
    /// <param name="openableDoor">The door to open.</param>
    public static void OpenDoor(OpenableDoor openableDoor)
    {
        try { ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Doors, (byte)(openableDoor.Id | 64)); } catch { }
    }
}
