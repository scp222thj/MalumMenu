using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace FabMenu;

public static class DoorsHandler
{
	public static List<SystemTypes> GetDoorRooms()
	{
		if (!Utils.isShip || ((Il2CppArrayBase<OpenableDoor>)(object)ShipStatus.Instance.AllDoors).Count <= 0)
		{
			return new List<SystemTypes>();
		}
		return ((IEnumerable<OpenableDoor>)ShipStatus.Instance.AllDoors).Select((OpenableDoor d) => d.Room).Distinct().ToList();
	}

	public static List<OpenableDoor> GetDoorsInRoom(SystemTypes room)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!Utils.isShip || ((Il2CppArrayBase<OpenableDoor>)(object)ShipStatus.Instance.AllDoors).Count <= 0)
		{
			return new List<OpenableDoor>();
		}
		return ((IEnumerable<OpenableDoor>)ShipStatus.Instance.AllDoors).Where((OpenableDoor d) => d.Room == room).ToList();
	}

	public static string GetStatusOfDoorsInRoom(SystemTypes room, bool colorize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		List<OpenableDoor> doorsInRoom = GetDoorsInRoom(room);
		if (doorsInRoom.Count <= 0)
		{
			return "N/A";
		}
		if (doorsInRoom.All((OpenableDoor d) => d.IsOpen))
		{
			if (!colorize)
			{
				return "Open";
			}
			return "<color=#00FF00>Open</color>";
		}
		if (doorsInRoom.All((OpenableDoor d) => !d.IsOpen))
		{
			if (!colorize)
			{
				return "Closed";
			}
			return "<color=#FF0000>Closed</color>";
		}
		if (!colorize)
		{
			return "Mixed";
		}
		return "<color=#FFFF00>Mixed</color>";
	}

	public static void OpenDoorsOfRoom(SystemTypes doorRoom)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		foreach (OpenableDoor item in GetDoorsInRoom(doorRoom))
		{
			OpenDoor(item);
		}
	}

	public static void CloseDoorsOfRoom(SystemTypes doorRoom)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ShipStatus.Instance.RpcCloseDoorsOfType(doorRoom);
		}
		catch
		{
		}
	}

	public static void OpenAllDoors()
	{
		foreach (OpenableDoor item in (Il2CppArrayBase<OpenableDoor>)(object)ShipStatus.Instance.AllDoors)
		{
			OpenDoor(item);
		}
	}

	public static void CloseAllDoors()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		foreach (OpenableDoor item in (Il2CppArrayBase<OpenableDoor>)(object)ShipStatus.Instance.AllDoors)
		{
			try
			{
				ShipStatus.Instance.RpcCloseDoorsOfType(item.Room);
			}
			catch
			{
			}
		}
	}

	public static void OpenDoor(OpenableDoor openableDoor)
	{
		try
		{
			ShipStatus.Instance.RpcUpdateSystem((SystemTypes)16, (byte)(openableDoor.Id | 0x40));
		}
		catch
		{
		}
	}
}
