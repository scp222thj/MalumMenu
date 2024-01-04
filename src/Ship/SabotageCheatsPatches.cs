using HarmonyLib;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class Sabotages_ShipStatus_FixedUpdate_Postfix
{
    //Postfix patch of ShipStatus.FixedUpdate to sabotage different systems
    public static void Postfix(ShipStatus __instance)
    {
        byte currentMapID = Utils.getCurrentMapID();

        if (CheatToggles.reactorSab){ //Reactor sabotages

            if (currentMapID == 2){ //Polus uses has SystemTypes.Laboratory instead of SystemTypes.Reactor
                
                var labSystem = __instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                
                if (labSystem.IsActive){

                    __instance.RpcUpdateSystem(SystemTypes.Laboratory, 16); //Repair reactor
                
                }else{
                    
                    __instance.RpcUpdateSystem(SystemTypes.Laboratory, 128); //Sabotage reactor
                
                }

            }else if (currentMapID == 4){ //Airship uses HeliSabotageSystem to sabotage reactor

                var HeliSystem = __instance.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
                
                if (HeliSystem.IsActive){
                    
                    //Repair reactor
                    __instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0);
                    __instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);

                }else{

                    //Sabotage reactor
                    __instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 128);

                }

            }else{ //Other maps behave normally 
                var reactSystem = __instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (reactSystem.IsActive){
                    __instance.RpcUpdateSystem(SystemTypes.Reactor, 16);
                }else{
                    __instance.RpcUpdateSystem(SystemTypes.Reactor, 128);
                }
            }

            CheatToggles.reactorSab = false; //Button behaviour

        }else if (CheatToggles.oxygenSab){ //Oxygen sabotages

            if (currentMapID != 4 && currentMapID != 2 && currentMapID != 5){ //Polus, Airship & Fungle have NO oxygen system

                var oxygenSystem = __instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                
                if (oxygenSystem.IsActive){
                    __instance.RpcUpdateSystem(SystemTypes.LifeSupp, 16); //Repair oxygen
                }else{
                    __instance.RpcUpdateSystem(SystemTypes.LifeSupp, 128); //Sabotage oxygen
                }

            }else{
                HudManager.Instance.Notifier.AddItem("Oxygen system not present on this map");
            }

            CheatToggles.oxygenSab = false; //Button behaviour
        
        }else if (CheatToggles.commsSab){ //Communications sabotages
            
            if (currentMapID == 1 || currentMapID == 5){ //MiraHQ and Fungle use HqHudSystemType to sabotage communications
                var hqcommsSystem = __instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                if (hqcommsSystem.IsActive){
                    __instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 0); //Repair communications
                    __instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
                }else{
                    __instance.RpcUpdateSystem(SystemTypes.Comms, 128); //Sabotage communications
                }
            }else{//Polus, Skeld and Airship have normal behaviour
                var commsSystem = __instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (commsSystem.IsActive){
                    __instance.RpcUpdateSystem(SystemTypes.Comms, 16); //Repair communications
                }else{
                    __instance.RpcUpdateSystem(SystemTypes.Comms, 128); //Sabotage communications
                }
            }

            CheatToggles.commsSab = false; //Button behaviour

        }else if (CheatToggles.elecSab){ //Eletrical sabotage

            if (currentMapID != 5){ //Fungle has no eletrical sabotage

                var elecSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                if (elecSystem.ActualSwitches != elecSystem.ExpectedSwitches){
                    
                    for (var i = 0; i < 5; i++) {
                        var switchMask = 1 << (i & 0x1F);

                        if ((elecSystem.ActualSwitches & switchMask) != (elecSystem.ExpectedSwitches & switchMask)){
                            __instance.RpcUpdateSystem(SystemTypes.Electrical, (byte)i); //Repair electrical
                        }
                    }    

                }else{

                    byte b = 4;
                    for (int i = 0; i < 5; i++)
                    {
                        if (BoolRange.Next(0.5f))
                        {
                            b |= (byte)(1 << i);
                        }
                    }

                    __instance.RpcUpdateSystem(SystemTypes.Electrical, (byte)(b | 128)); //Sabotage electrical

                }

            }else{
                HudManager.Instance.Notifier.AddItem("Eletrical system not present on this map");
            }

            CheatToggles.elecSab = false; //Button behaviour
            
        }else if (CheatToggles.mushSab){

            if (currentMapID == 5){ //MushroomMixup only works on Fungle
                __instance.RpcUpdateSystem(SystemTypes.MushroomMixupSabotage, 1); //Sabotage MushroomMixup
            }else{
                HudManager.Instance.Notifier.AddItem("Mushrooms not present on this map");
            }

            CheatToggles.mushSab = false; //Button behaviour

        }else if (CheatToggles.doorsSab){

            //Loop through all rooms and close their doors
            foreach (SystemTypes room in (SystemTypes[]) Enum.GetValues(typeof(SystemTypes)))
            {
                try{__instance.RpcCloseDoorsOfType(room);}catch{} //try-catch for rooms with no doors
            }

            CheatToggles.doorsSab = false; //Button behaviour

        }
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class UnfixableLights_ShipStatus_FixedUpdate_Postfix
{

    //Postfix patch of ShipStatus.FixedUpdate to break lights completly & make them unable to be fixed normally
    public static void Postfix(ShipStatus __instance)
    {
        if (CheatToggles.unfixableLights)
        {
            //Apparently most values you put for amount in RpcUpdateSystem will break lights completly
            //They are unfixable through regular means (toggling switches)
            //They can only be repaired by repeating RpcUpdateSystem with the same amount
            
            byte currentMapID = Utils.getCurrentMapID();

            if (currentMapID != 5){ //Fungle has no lights, so blackout can't trigger there

                __instance.RpcUpdateSystem(SystemTypes.Electrical, 69);

            }else{

                HudManager.Instance.Notifier.AddItem("Eletrical system not present on this map");
                
                }

            CheatToggles.unfixableLights = false;
        } 
    }
}