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
        if (mapId == 2) { //Polus uses has SystemTypes.Laboratory instead of SystemTypes.Reactor
            
            ReactorSystemType labSys = shipStatus.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

            if (CheatToggles.reactorSab != reactorSab){

                if (reactorSab){
                    shipStatus.RpcUpdateSystem(SystemTypes.Laboratory, 16);
                }else{
                    shipStatus.RpcUpdateSystem(SystemTypes.Laboratory, 128);
                }

                reactorSab = CheatToggles.reactorSab;
            }

            CheatToggles.reactorSab = reactorSab = labSys.IsActive;

        } else if (mapId == 4) { //Airship uses HeliSabotageSystem to sabotage reactor

            HeliSabotageSystem heliSys = shipStatus.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();

            if (CheatToggles.reactorSab != reactorSab){

                if (reactorSab){
                    shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0);
                    shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);
                }else{
                    shipStatus.RpcUpdateSystem(SystemTypes.HeliSabotage, 128);
                }

                reactorSab = CheatToggles.reactorSab;
            }

            CheatToggles.reactorSab = reactorSab = heliSys.IsActive;

        } else { //Other maps behave normally 
            
            ReactorSystemType reactorSys = shipStatus.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
        
            if (CheatToggles.reactorSab != reactorSab){

                if (reactorSab){
                    shipStatus.RpcUpdateSystem(SystemTypes.Reactor, 16);
                }else{
                    shipStatus.RpcUpdateSystem(SystemTypes.Reactor, 128);
                }

                reactorSab = CheatToggles.reactorSab;
            }

            CheatToggles.reactorSab = reactorSab = reactorSys.IsActive;
        }
    }

    public static void handleOxygen(ShipStatus shipStatus, byte mapId)
    {
        if (mapId != 4 && mapId != 2 && mapId != 5) { //Polus uses has SystemTypes.Laboratory instead of SystemTypes.Reactor
            
            LifeSuppSystemType oxygenSys = shipStatus.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

            if (CheatToggles.oxygenSab != oxygenSab){

                if (oxygenSab){
                    shipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, 16);
                }else{
                    shipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, 128);
                }

                oxygenSab = CheatToggles.oxygenSab;
            }

            CheatToggles.oxygenSab = oxygenSab = oxygenSys.IsActive;

            return;

        }

        if (CheatToggles.oxygenSab){
            HudManager.Instance.Notifier.AddItem("Oxygen system not present on this map");
            CheatToggles.oxygenSab = false;
        }
    }

    public static void handleComms(ShipStatus shipStatus, byte mapId)
    {
        if (mapId == 1 || mapId == 5) { //Polus uses has SystemTypes.Laboratory instead of SystemTypes.Reactor
            
            HqHudSystemType hqCommsSys = shipStatus.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

            if (CheatToggles.commsSab != commsSab){
                
                if (commsSab){
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
                }else{
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 128);
                }

                commsSab = CheatToggles.commsSab;

            }

            CheatToggles.commsSab = commsSab = hqCommsSys.IsActive;

        }else{

            HudOverrideSystemType commsSys = shipStatus.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

            if (CheatToggles.commsSab != commsSab){

                if (commsSab){
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 16);
                }else{
                    shipStatus.RpcUpdateSystem(SystemTypes.Comms, 128);
                }
                
                commsSab = CheatToggles.commsSab;

            }

            CheatToggles.commsSab = commsSab = commsSys.IsActive;

        }
    }

    public static void handleElectrical(ShipStatus shipStatus, byte mapId)
    {
        if (mapId != 5) { //Polus uses has SystemTypes.Laboratory instead of SystemTypes.Reactor
            
            SwitchSystem elecSys = shipStatus.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            
            handleUnfixLights(shipStatus);

            if (CheatToggles.elecSab != elecSab){
                if (elecSab){

                    for (var i = 0; i < 5; i++)
                    {
                        var switchMask = 1 << (i & 0x1F);

                        if ((elecSys.ActualSwitches & switchMask) != (elecSys.ExpectedSwitches & switchMask))
                        {
                            shipStatus.RpcUpdateSystem(SystemTypes.Electrical, (byte)i);
                        }
                    }
                
                }else{

                    CheatToggles.unfixableLights = false;

                    byte b = 4;
                    for (int i = 0; i < 5; i++)
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

        if (CheatToggles.elecSab || CheatToggles.unfixableLights){
            HudManager.Instance.Notifier.AddItem("Eletrical system not present on this map");
            CheatToggles.elecSab = CheatToggles.unfixableLights = false;
        }
    }

    public static void handleUnfixLights(ShipStatus shipStatus){
        if (CheatToggles.unfixableLights != unfixableLights)
        {
            //Apparently most values you put for amount in RpcUpdateSystem will break lights completly
            //They are unfixable through regular means (toggling switches)
            //They can only be repaired by repeating RpcUpdateSystem with the same amount
            
            if (!unfixableLights){
                CheatToggles.elecSab = false;
            }

            shipStatus.RpcUpdateSystem(SystemTypes.Electrical, 69);

            unfixableLights = CheatToggles.unfixableLights;
        }
    }

    public static void handleMushMix(ShipStatus shipStatus, byte mapId)
    {
        if (CheatToggles.mushSab) {

            if (mapId == 5){ //MushroomMixup only works on Fungle
                
                shipStatus.RpcUpdateSystem(SystemTypes.MushroomMixupSabotage, 1); //Sabotage MushroomMixup
                return;
            
            } else {
                HudManager.Instance.Notifier.AddItem("Mushrooms not present on this map");
            }
            
            CheatToggles.mushSab = false; //Button behaviour

        }
    }

    public static void handleDoors(ShipStatus shipStatus)
    {
        if (CheatToggles.doorsSab) {

            //Loop through all rooms and close their doors
            foreach (OpenableDoor openableDoor in ShipStatus.Instance.AllDoors)
            {
                try{shipStatus.RpcCloseDoorsOfType(openableDoor.Room);}catch{}
            }

            CheatToggles.doorsSab = false; //Button behaviour

        }
    }
}