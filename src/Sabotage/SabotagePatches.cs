using HarmonyLib;
using System;
using Hazel;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcUpdateSystem), new Type[] { typeof(SystemTypes), typeof(byte) })]
public static class Sabotage_RpcUpdateSystemPrefix
{
    //Prefix patch of ShipStatus.RpcUpdateSystem to prevent anticheat mods from detecting hacked sabotages
    public static bool Prefix(ShipStatus __instance, SystemTypes systemType, byte amount)
    {
        if (!CheatSettings.reactorSab && !CheatSettings.blackOut && !CheatSettings.elecSab &&
            !CheatSettings.commsSab && !CheatSettings.mushSab && !CheatSettings.oxygenSab){
                return true; //Only works for hacked Sabotage RPC calls
        }
        
        if (AmongUsClient.Instance.AmHost)
		{
			__instance.UpdateSystem(systemType, PlayerControl.LocalPlayer, amount);
			return false;
		}

		var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected){

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(ShipStatus.Instance.NetId, (byte)RpcCalls.UpdateSystem, SendOption.None, AmongUsClient.Instance.HostId);
            writer.Write((byte)systemType);
            writer.WriteNetObject(HostData.Character); //Send hacked Sabotage RPC calls as the Host, so that the anticheat is not triggered
            writer.Write(amount);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

        }

		return false;
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class Ship_SabotagePostfix
{
    //Postfix patch of ShipStatus.FixedUpdate to sabotage different systems
    public static void Postfix(ShipStatus __instance)
    {
        byte currentMapID = Utils.getCurrentMapID();

        if (CheatSettings.reactorSab){ //Reactor sabotages

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

            CheatSettings.reactorSab = false; //Button behaviour

        }else if (CheatSettings.oxygenSab){ //Oxygen sabotages

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

            CheatSettings.oxygenSab = false; //Button behaviour
        
        }else if (CheatSettings.commsSab){ //Communications sabotages
            
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

            CheatSettings.commsSab = false; //Button behaviour

        }else if (CheatSettings.elecSab){ //Eletrical sabotage

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

            CheatSettings.elecSab = false; //Button behaviour
        }    
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class Ship_CloseAllDoorsPostfix
{

    //Postfix patch of ShipStatus.FixedUpdate to lock all doors on a ship
    public static void Postfix(ShipStatus __instance)
    {
        if (CheatSettings.fullLockdown){

            //Loop through all rooms and close their doors
            foreach (SystemTypes room in (SystemTypes[]) Enum.GetValues(typeof(SystemTypes)))
            {
                try{__instance.RpcCloseDoorsOfType(room);}catch{} //try-catch for rooms with no doors
            }

            CheatSettings.fullLockdown = false;

        }
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class Ship_BlackoutPostfix
{

    //Postfix patch of ShipStatus.FixedUpdate to disable lights completly
    public static void Postfix(ShipStatus __instance)
    {
        if (CheatSettings.blackOut)
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

            CheatSettings.blackOut = false;
        } 
    }
}