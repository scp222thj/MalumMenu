namespace MalumMenu;

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
                // Polus uses SystemTypes.Laboratory instead of SystemTypes.Reactor

                var labSys = shipStatus.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                if (CheatToggles.reactorSab != reactorSab)
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.Laboratory, reactorSab ? (byte)16 : (byte)128);
                    reactorSab = CheatToggles.reactorSab;
                }

                CheatToggles.reactorSab = reactorSab = labSys.IsActive;
                break;
            }
            case 4:
            {
                // Airship uses HeliSabotageSystem to sabotage reactor

                var heliSys = shipStatus.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();

                if (CheatToggles.reactorSab != reactorSab){

                    if (reactorSab){
                        shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0); // Repair
                        shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);
                    }else{
                        shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 128); // Sabotage
                    }

                    reactorSab = CheatToggles.reactorSab;
                }

                CheatToggles.reactorSab = reactorSab = heliSys.IsActive;
                break;
            }
            default:
            {
                // Other maps behave normally

                var reactorSys = shipStatus.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                if (CheatToggles.reactorSab != reactorSab)
                {
                    shipStatus.RpcUpdateSystem(SystemTypes.Reactor, reactorSab ? (byte)16 : (byte)128);
                    reactorSab = CheatToggles.reactorSab;
                }

                CheatToggles.reactorSab = reactorSab = reactorSys.IsActive;
                break;
            }
        }
    }

    public static void HandleOxygen(ShipStatus shipStatus, byte mapId)
    {
        if (mapId != 4 && mapId != 2 && mapId != 5) { // Maps without Oxygen system: Airship, MiraHQ, Fungle

            var oxygenSys = shipStatus.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

            if (CheatToggles.oxygenSab != oxygenSab)
            {
                shipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, oxygenSab ? (byte)16 : (byte)128);
                oxygenSab = CheatToggles.oxygenSab;
            }

            CheatToggles.oxygenSab = oxygenSab = oxygenSys.IsActive;

            return;

        }

        // Notify the player if they try to activate the cheat in a map without an oxygen system
        if (!CheatToggles.oxygenSab) return;
        HudManager.Instance.Notifier.AddDisconnectMessage("Oxygen system not present on this map");
        CheatToggles.oxygenSab = false;
    }

    public static void HandleComms(ShipStatus shipStatus, byte mapId)
    {
        if (mapId is 1 or 5) { // Fungle & Skeld use HqHudSystemType instead of HudOverrideSystemType

            var hqCommsSys = shipStatus.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

            if (CheatToggles.commsSab != commsSab){

                if (commsSab){
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 0); // Repair
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
                }else{
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 128); // Sabotage
                }

                commsSab = CheatToggles.commsSab;

            }

            CheatToggles.commsSab = commsSab = hqCommsSys.IsActive;

        }else{ // Other maps behave normally

            var commsSys = shipStatus.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

            if (CheatToggles.commsSab != commsSab)
            {
                shipStatus.RpcUpdateSystem(SystemTypes.Comms, commsSab ? (byte)16 : (byte)128);
                commsSab = CheatToggles.commsSab;
            }

            CheatToggles.commsSab = commsSab = commsSys.IsActive;

        }
    }

    public static void HandleElectrical(ShipStatus shipStatus, byte mapId)
    {
        if (mapId != 5) { // Fungle has no electrical system

            var elecSys = shipStatus.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

            // Handle unfixableLights cheat first to avoid the cheats messing with each other
            HandleUnfixLights(shipStatus);

            if (CheatToggles.elecSab != elecSab){
                if (elecSab){

                    // Repair

                    for (var i = 0; i < 5; i++)
                    {
                        var switchMask = 1 << (i & 0x1F);

                        if ((elecSys.ActualSwitches & switchMask) != (elecSys.ExpectedSwitches & switchMask))
                        {
                            shipStatus.RpcUpdateSystem(SystemTypes.Electrical, (byte)i);
                        }
                    }

                }else{

                    // Sabotage

                    CheatToggles.unfixableLights = false; // Replace unfixableLights cheat if it is already active

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

                elecSab = CheatToggles.elecSab;
            }

            CheatToggles.elecSab = elecSab = elecSys.IsActive && !unfixableLights;

            return;

        }

        // Notify the player if they try to activate the cheat in a map without an oxygen system
        if (!CheatToggles.elecSab && !CheatToggles.unfixableLights) return;
        HudManager.Instance.Notifier.AddDisconnectMessage("Electrical system not present on this map");
        CheatToggles.elecSab = CheatToggles.unfixableLights = false;
    }

    public static void HandleUnfixLights(ShipStatus shipStatus)
    {
        if (CheatToggles.unfixableLights == unfixableLights) return;
        // Apparently most values you put for amount in RpcUpdateSystem will break lights completely
        // They are unfixable through regular means (toggling switches)
        // They can only be repaired by repeating RpcUpdateSystem with the same amount

        if (!unfixableLights){
            CheatToggles.elecSab = false;
        }

        shipStatus.RpcUpdateSystem(SystemTypes.Electrical, 69); // Repair & Sabotage

        unfixableLights = CheatToggles.unfixableLights;
    }

    public static void HandleMushMix(ShipStatus shipStatus, byte mapId)
    {
        if (!CheatToggles.mushSab) return;
        if (mapId == 5){ // MushroomMixup only works on Fungle

            shipStatus.RpcUpdateSystem(SystemTypes.MushroomMixupSabotage, 1); // Sabotage

        } else {

            // Notify the player if they try to activate the cheat in a map without mushrooms

            HudManager.Instance.Notifier.AddDisconnectMessage("Mushrooms not present on this map");
        }

        // Repair (bugged)
        // var mushSys = shipStatus.Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();
        // mushSys.Deteriorate(mushSys.currentSecondsUntilHeal);

        CheatToggles.mushSab = false; //Button behaviour
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

    public static void OpenSabotageMap()
    {
        if (!CheatToggles.sabotageMap) return;
        DestroyableSingleton<HudManager>.Instance.ToggleMapVisible(new MapOptions
        {
            Mode = MapOptions.Modes.Sabotage
        });
        CheatToggles.sabotageMap = false;
    }
}
