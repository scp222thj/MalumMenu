using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Collections.Generic;

namespace FabMenu;

public static class MalumSabotageSystem
{
	public static bool reactorSab;

	public static bool oxygenSab;

	public static bool commsSab;

	public static bool elecSab;

	public static bool unfixableLights;

	public static void HandleReactor(ShipStatus shipStatus, byte mapId)
	{
		switch (mapId)
		{
		case 2:
		{
			ReactorSystemType obj2 = ((Il2CppObjectBase)shipStatus.Systems[(SystemTypes)21]).Cast<ReactorSystemType>();
			if (CheatToggles.reactorSab != reactorSab)
			{
				shipStatus.RpcUpdateSystem((SystemTypes)21, (byte)(reactorSab ? 16 : 128));
				reactorSab = CheatToggles.reactorSab;
			}
			CheatToggles.reactorSab = (reactorSab = obj2.IsActive);
			break;
		}
		case 4:
		{
			HeliSabotageSystem obj3 = ((Il2CppObjectBase)shipStatus.Systems[(SystemTypes)58]).Cast<HeliSabotageSystem>();
			if (CheatToggles.reactorSab != reactorSab)
			{
				if (reactorSab)
				{
					shipStatus.RpcUpdateSystem((SystemTypes)58, (byte)16);
					shipStatus.RpcUpdateSystem((SystemTypes)58, (byte)17);
				}
				else
				{
					shipStatus.RpcUpdateSystem((SystemTypes)58, (byte)128);
				}
				reactorSab = CheatToggles.reactorSab;
			}
			CheatToggles.reactorSab = (reactorSab = obj3.IsActive);
			break;
		}
		default:
		{
			ReactorSystemType obj = ((Il2CppObjectBase)shipStatus.Systems[(SystemTypes)3]).Cast<ReactorSystemType>();
			if (CheatToggles.reactorSab != reactorSab)
			{
				shipStatus.RpcUpdateSystem((SystemTypes)3, (byte)(reactorSab ? 16 : 128));
				reactorSab = CheatToggles.reactorSab;
			}
			CheatToggles.reactorSab = (reactorSab = obj.IsActive);
			break;
		}
		}
	}

	public static void HandleOxygen(ShipStatus shipStatus, byte mapId)
	{
		if (mapId != 4 && mapId != 2 && mapId != 5)
		{
			LifeSuppSystemType obj = ((Il2CppObjectBase)shipStatus.Systems[(SystemTypes)8]).Cast<LifeSuppSystemType>();
			if (CheatToggles.oxygenSab != oxygenSab)
			{
				shipStatus.RpcUpdateSystem((SystemTypes)8, (byte)(oxygenSab ? 16 : 128));
				oxygenSab = CheatToggles.oxygenSab;
			}
			CheatToggles.oxygenSab = (oxygenSab = obj.IsActive);
		}
		else if (CheatToggles.oxygenSab)
		{
			DestroyableSingleton<HudManager>.Instance.Notifier.AddDisconnectMessage("Oxygen system not present on this map");
			CheatToggles.oxygenSab = false;
		}
	}

	public static void HandleComms(ShipStatus shipStatus, byte mapId)
	{
		if ((mapId == 1 || mapId == 5) ? true : false)
		{
			HqHudSystemType obj = ((Il2CppObjectBase)shipStatus.Systems[(SystemTypes)14]).Cast<HqHudSystemType>();
			if (CheatToggles.commsSab != commsSab)
			{
				if (commsSab)
				{
					shipStatus.RpcUpdateSystem((SystemTypes)14, (byte)16);
					shipStatus.RpcUpdateSystem((SystemTypes)14, (byte)17);
				}
				else
				{
					shipStatus.RpcUpdateSystem((SystemTypes)14, (byte)128);
				}
				commsSab = CheatToggles.commsSab;
			}
			CheatToggles.commsSab = (commsSab = obj.IsActive);
		}
		else
		{
			HudOverrideSystemType obj2 = ((Il2CppObjectBase)shipStatus.Systems[(SystemTypes)14]).Cast<HudOverrideSystemType>();
			if (CheatToggles.commsSab != commsSab)
			{
				shipStatus.RpcUpdateSystem((SystemTypes)14, (byte)(commsSab ? 16 : 128));
				commsSab = CheatToggles.commsSab;
			}
			CheatToggles.commsSab = (commsSab = obj2.IsActive);
		}
	}

	public static void HandleElectrical(ShipStatus shipStatus, byte mapId)
	{
		if (mapId != 5)
		{
			SwitchSystem val = ((Il2CppObjectBase)shipStatus.Systems[(SystemTypes)7]).Cast<SwitchSystem>();
			HandleUnfixLights(shipStatus);
			if (CheatToggles.elecSab != elecSab)
			{
				if (elecSab)
				{
					for (int i = 0; i < 5; i++)
					{
						int num = 1 << i;
						if ((val.ActualSwitches & num) != (val.ExpectedSwitches & num))
						{
							shipStatus.RpcUpdateSystem((SystemTypes)7, (byte)i);
						}
					}
				}
				else
				{
					CheatToggles.unfixableLights = false;
					byte b = 4;
					for (int j = 0; j < 5; j++)
					{
						if (BoolRange.Next(0.5f))
						{
							b |= (byte)(1 << j);
						}
					}
					shipStatus.RpcUpdateSystem((SystemTypes)7, (byte)(b | 0x80));
				}
				elecSab = CheatToggles.elecSab;
			}
			CheatToggles.elecSab = (elecSab = val.IsActive && !unfixableLights);
		}
		else if (CheatToggles.elecSab || CheatToggles.unfixableLights)
		{
			DestroyableSingleton<HudManager>.Instance.Notifier.AddDisconnectMessage("Electrical system not present on this map");
			CheatToggles.elecSab = (CheatToggles.unfixableLights = false);
		}
	}

	public static void HandleUnfixLights(ShipStatus shipStatus)
	{
		if (CheatToggles.unfixableLights != unfixableLights)
		{
			if (!unfixableLights)
			{
				CheatToggles.elecSab = false;
			}
			shipStatus.RpcUpdateSystem((SystemTypes)7, (byte)69);
			unfixableLights = CheatToggles.unfixableLights;
		}
	}

	public static void HandleMushMix(ShipStatus shipStatus, byte mapId)
	{
		if (CheatToggles.mushSab)
		{
			if (mapId == 5)
			{
				shipStatus.RpcUpdateSystem((SystemTypes)57, (byte)1);
			}
			else
			{
				DestroyableSingleton<HudManager>.Instance.Notifier.AddDisconnectMessage("Mushrooms not present on this map");
			}
			CheatToggles.mushSab = false;
		}
	}

	public static void HandleSpores(FungleShipStatus shipStatus, byte mapId)
	{
		if (!CheatToggles.mushSpore)
		{
			return;
		}
		if (mapId == 5)
		{
			Enumerator<int, Mushroom> enumerator = shipStatus.sporeMushrooms.Values.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Mushroom current = enumerator.Current;
				PlayerControl.LocalPlayer.CmdCheckSporeTrigger(current);
			}
		}
		else
		{
			DestroyableSingleton<HudManager>.Instance.Notifier.AddDisconnectMessage("Mushrooms not present on this map");
		}
		CheatToggles.mushSpore = false;
	}

	public static void HandleDoors(ShipStatus shipStatus)
	{
		if (CheatToggles.closeAllDoors)
		{
			DoorsHandler.CloseAllDoors();
			CheatToggles.closeAllDoors = false;
		}
		if (CheatToggles.openAllDoors)
		{
			DoorsHandler.OpenAllDoors();
			CheatToggles.openAllDoors = false;
		}
		if (CheatToggles.spamCloseAllDoors)
		{
			DoorsHandler.CloseAllDoors();
		}
		if (CheatToggles.spamOpenAllDoors)
		{
			DoorsHandler.OpenAllDoors();
		}
	}

	public static void OpenSabotageMap()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		if (CheatToggles.sabotageMap)
		{
			DestroyableSingleton<HudManager>.Instance.ToggleMapVisible(new MapOptions
			{
				Mode = (Modes)3
			});
			CheatToggles.sabotageMap = false;
		}
	}
}
