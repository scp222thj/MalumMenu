using HarmonyLib;

namespace MalumMenu;

public static class MalumSabotageCheats
{
    private static bool _reactorSab;
    private static bool _oxygenSab;
    private static bool _commsSab;
    private static bool _elecSab;
    private static bool _unfixableLights;

    [HarmonyPatch(typeof(SabotageSystemType), "Deteriorate")]
    public static class NoSabotageCooldownPatch
    {
        public static void Prefix(SabotageSystemType __instance)
        {
            if (CheatToggles.noSabotageCooldown)
            {
                __instance.Timer = 0f;
            }
        }
    }

    [HarmonyPatch(typeof(DoorCardSwipeGame), nameof(DoorCardSwipeGame.Begin))]
    public static class NoDoorCooldownPatch
    {
        public static bool Prefix(DoorCardSwipeGame __instance)
        {
            if (!CheatToggles.noSabotageCooldown) return true;

            if (__instance.MyDoor != null)
            {
                __instance.MyDoor.SetDoorway(true);
            }
            __instance.Close();
            return false;
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcCloseDoorsOfType))]
    public static class NoDoorCooldownRpcPatch
    {
        public static bool Prefix(ShipStatus __instance, SystemTypes type)
        {
            if (!CheatToggles.noSabotageCooldown) return true;

            var doors = ShipStatus.Instance.AllDoors;
            foreach (var door in doors)
            {
                if (door.Room == type)
                {
                    door.SetDoorway(false);
                }
            }
            return false;
        }
    }

    public static void HandleReactor(ShipStatus shipStatus, byte mapId)
    {
        switch (mapId)
        {
            case 2:
            {
                // Polus uses SystemTypes.Laboratory instead of SystemTypes.Reactor

                var labSys = shipStatus.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                if (CheatToggles.reactorSab != _reactorSab)
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.Laboratory, _reactorSab ? (byte)16 : (byte)128);
                    _reactorSab = CheatToggles.reactorSab;
                }

                CheatToggles.reactorSab = _reactorSab = labSys.IsActive;
                break;
            }
            case 4:
            {
                // Airship uses HeliSabotageSystem to sabotage reactor

                var heliSys = shipStatus.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();

                if (CheatToggles.reactorSab != _reactorSab)
                {
                    if (_reactorSab)
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0);
                        shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);
                    }
                    else
                    {
                        shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 128);
                    }

                    _reactorSab = CheatToggles.reactorSab;
                }

                CheatToggles.reactorSab = _reactorSab = heliSys.IsActive;
                break;
            }
            default:
            {
                // Other maps behave normally

                var reactorSys = shipStatus.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                if (CheatToggles.reactorSab != _reactorSab)
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.Reactor, _reactorSab ? (byte)16 : (byte)128);
                    _reactorSab = CheatToggles.reactorSab;
                }

                CheatToggles.reactorSab = _reactorSab = reactorSys.IsActive;
                break;
            }
        }
    }

    public static void HandleOxygen(ShipStatus shipStatus, byte mapId)
    {
        if (mapId != 4 && mapId != 2 && mapId != 5)
        {
            var oxygenSys = shipStatus.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

            if (CheatToggles.oxygenSab != _oxygenSab)
            {
                shipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, _oxygenSab ? (byte)16 : (byte)128);
                _oxygenSab = CheatToggles.oxygenSab;
            }

            CheatToggles.oxygenSab = _oxygenSab = oxygenSys.IsActive;

            return;
        }

        if (!CheatToggles.oxygenSab) return;
        HudManager.Instance.Notifier.AddDisconnectMessage("Oxygen system not present on this map");
        CheatToggles.oxygenSab = false;
    }

    public static void HandleComms(ShipStatus shipStatus, byte mapId)
    {
        if (mapId is 1 or 5)
        {
            // Fungle & Skeld use HqHudSystemType instead of HudOverrideSystemType

            var hqCommsSys = shipStatus.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

            if (CheatToggles.commsSab != _commsSab)
            {
                if (_commsSab)
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
                }
                else
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 128);
                }

                _commsSab = CheatToggles.commsSab;
            }

            CheatToggles.commsSab = _commsSab = hqCommsSys.IsActive;
        }
        else
        {
            var commsSys = shipStatus.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

            if (CheatToggles.commsSab != _commsSab)
            {
                shipStatus.RpcUpdateSystem(SystemTypes.Comms, _commsSab ? (byte)16 : (byte)128);
                _commsSab = CheatToggles.commsSab;
            }

            CheatToggles.commsSab = _commsSab = commsSys.IsActive;
        }
    }

    public static void HandleElectrical(ShipStatus shipStatus, byte mapId)
    {
        if (mapId != 5)
        {
            var elecSys = shipStatus.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

            HandleUnfixLights(shipStatus);

            if (CheatToggles.elecSab != _elecSab)
            {
                if (_elecSab)
                {
                    for (var i = 0; i < 5; i++)
                    {
                        var switchMask = 1 << (i & 0x1F);

                        if ((elecSys.ActualSwitches & switchMask) != (elecSys.ExpectedSwitches & switchMask))
                        {
                            shipStatus.RpcUpdateSystem(SystemTypes.Electrical, (byte)i);
                        }
                    }
                }
                else
                {
                    CheatToggles.unfixableLights = false;

                    byte b = 4;
                    for (var i = 0; i < 5; i++)
                    {
                        if (BoolRange.Next(0.5f))
                        {
                            b |= (byte)(1 << i);
                        }
                    }

                    shipStatus.RpcUpdateSystem(SystemTypes.Electrical, (byte)(b | 128));
                }

                _elecSab = CheatToggles.elecSab;
            }

            CheatToggles.elecSab = _elecSab = elecSys.IsActive && !_unfixableLights;

            return;
        }

        if (!CheatToggles.elecSab && !CheatToggles.unfixableLights) return;

        HudManager.Instance.Notifier.AddDisconnectMessage("Electrical system not present on this map");
        CheatToggles.elecSab = CheatToggles.unfixableLights = false;
    }

    public static void HandleUnfixLights(ShipStatus shipStatus)
    {
        if (CheatToggles.unfixableLights == _unfixableLights) return;

        if (!_unfixableLights)
        {
            CheatToggles.elecSab = false;
        }

        shipStatus.RpcUpdateSystem(SystemTypes.Electrical, 69);

        _unfixableLights = CheatToggles.unfixableLights;
    }

    public static void HandleMushMix(ShipStatus shipStatus, byte mapId)
    {
        if (!CheatToggles.mushSab) return;

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

    public static void HandleSpores(FungleShipStatus shipStatus, byte mapId)
    {
        if (!CheatToggles.mushSpore) return;

        if (mapId == 5)
        {
            foreach (var mushroom in shipStatus.sporeMushrooms.Values)
            {
                PlayerControl.LocalPlayer.CmdCheckSporeTrigger(mushroom);
            }
        }
        else
        {
            HudManager.Instance.Notifier.AddDisconnectMessage("Mushrooms not present on this map");
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

    public static void Process(ShipStatus shipStatus)
    {
        var currentMapID = Utils.GetCurrentMapID();

        HandleReactor(shipStatus, currentMapID);
        HandleOxygen(shipStatus, currentMapID);
        HandleComms(shipStatus, currentMapID);
        HandleElectrical(shipStatus, currentMapID);
        HandleDoors(shipStatus);
    }

    public static void ProcessFungle(FungleShipStatus shipStatus)
    {
        var currentMapID = Utils.GetCurrentMapID();

        HandleMushMix(shipStatus, currentMapID);
        HandleSpores(shipStatus, currentMapID);
    }
}
