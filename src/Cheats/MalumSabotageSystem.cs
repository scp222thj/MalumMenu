using UnityEngine;

namespace MalumMenu;

public static class MalumSabotageSystem
{
    public static bool reactorSab;
    public static bool oxygenSab;
    public static bool commsSab;
    public static bool elecSab;
    public static bool unfixableLights;

    public static void handleReactor(ShipStatus shipStatus, byte mapId)
    {
        if (mapId == 2) // Polus: Laboratory reactor
        {
            var labSys = shipStatus.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
            ToggleSabotage(SystemTypes.Laboratory, labSys.IsActive, ref reactorSab, CheatToggles.reactorSab, shipStatus);
            CheatToggles.reactorSab = reactorSab = labSys.IsActive;
        }
        else if (mapId == 4) // Airship: HeliSabotage
        {
            var heliSys = shipStatus.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
            if (CheatToggles.reactorSab != reactorSab)
            {
                if (CheatToggles.reactorSab)
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 128); // Sabotage
                }
                else
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0); // Repair both pads
                    shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);
                }
                reactorSab = CheatToggles.reactorSab;
            }
            CheatToggles.reactorSab = reactorSab = heliSys.IsActive;
        }
        else // Other maps: Standard Reactor
        {
            var reactorSys = shipStatus.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
            ToggleSabotage(SystemTypes.Reactor, reactorSys.IsActive, ref reactorSab, CheatToggles.reactorSab, shipStatus);
            CheatToggles.reactorSab = reactorSab = reactorSys.IsActive;
        }
    }

    public static void handleOxygen(ShipStatus shipStatus, byte mapId)
    {
        if (mapId is not (2 or 4 or 5))
        {
            var oxygenSys = shipStatus.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
            ToggleSabotage(SystemTypes.LifeSupp, oxygenSys.IsActive, ref oxygenSab, CheatToggles.oxygenSab, shipStatus);
            CheatToggles.oxygenSab = oxygenSab = oxygenSys.IsActive;
        }
        else if (CheatToggles.oxygenSab)
        {
            HudManager.Instance.Notifier.AddDisconnectMessage("Oxygen system not present on this map");
            CheatToggles.oxygenSab = false;
        }
    }

    public static void handleComms(ShipStatus shipStatus, byte mapId)
    {
        if (mapId is 1 or 5)
        {
            var hqCommsSys = shipStatus.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
            if (CheatToggles.commsSab != commsSab)
            {
                if (CheatToggles.commsSab)
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 128);
                }
                else
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
                }
                commsSab = CheatToggles.commsSab;
            }
            CheatToggles.commsSab = commsSab = hqCommsSys.IsActive;
        }
        else
        {
            var commsSys = shipStatus.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
            ToggleSabotage(SystemTypes.Comms, commsSys.IsActive, ref commsSab, CheatToggles.commsSab, shipStatus);
            CheatToggles.commsSab = commsSab = commsSys.IsActive;
        }
    }

    public static void handleElectrical(ShipStatus shipStatus, byte mapId)
    {
        if (mapId == 5)
        {
            if (CheatToggles.elecSab || CheatToggles.unfixableLights)
            {
                HudManager.Instance.Notifier.AddDisconnectMessage("Electrical system not present on this map");
                CheatToggles.elecSab = CheatToggles.unfixableLights = false;
            }
            return;
        }

        var elecSys = shipStatus.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
        handleUnfixLights(shipStatus); // Check lights override first

        if (CheatToggles.elecSab != elecSab)
        {
            if (CheatToggles.elecSab)
            {
                // Sabotage (randomized switches)
                CheatToggles.unfixableLights = false;
                byte b = 4;
                for (int i = 0; i < 5; i++) if (BoolRange.Next(0.5f)) b |= (byte)(1 << i);
                shipStatus.RpcUpdateSystem(SystemTypes.Electrical, (byte)(b | 128));
            }
            else
            {
                // Repair
                for (var i = 0; i < 5; i++)
                {
                    var mask = 1 << (i & 0x1F);
                    if ((elecSys.ActualSwitches & mask) != (elecSys.ExpectedSwitches & mask))
                        shipStatus.RpcUpdateSystem(SystemTypes.Electrical, (byte)i);
                }
            }
            elecSab = CheatToggles.elecSab;
        }

        CheatToggles.elecSab = elecSab = elecSys.IsActive && !unfixableLights;
    }

    public static void handleUnfixLights(ShipStatus shipStatus)
    {
        if (CheatToggles.unfixableLights != unfixableLights)
        {
            if (!unfixableLights) CheatToggles.elecSab = false;
            shipStatus.RpcUpdateSystem(SystemTypes.Electrical, 69); // Magic number of doom
            unfixableLights = CheatToggles.unfixableLights;
        }
    }

    public static void handleMushMix(ShipStatus shipStatus, byte mapId)
    {
        if (CheatToggles.mushSab)
        {
            if (mapId == 5)
            {
                shipStatus.RpcUpdateSystem(SystemTypes.MushroomMixupSabotage, 1);
            }
            else
            {
                HudManager.Instance.Notifier.AddDisconnectMessage("Mushrooms not present on this map");
            }
            CheatToggles.mushSab = false;
        }
    }

    public static void handleDoors(ShipStatus shipStatus)
    {
        if (CheatToggles.doorsSab)
        {
            foreach (var door in ShipStatus.Instance.AllDoors)
            {
                try { shipStatus.RpcCloseDoorsOfType(door.Room); } catch { }
            }
            CheatToggles.doorsSab = false;
        }
    }

    private static void ToggleSabotage(SystemTypes type, bool isActive, ref bool internalFlag, bool toggle, ShipStatus status)
    {
        if (toggle != internalFlag)
        {
            status.RpcUpdateSystem(type, toggle ? (byte)128 : (byte)16);
            internalFlag = toggle;
        }
    }
}
