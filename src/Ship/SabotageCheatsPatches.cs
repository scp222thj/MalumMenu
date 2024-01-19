using HarmonyLib;
using System;
using System.Runtime.CompilerServices;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class Sabotages_ShipStatus_FixedUpdate_Postfix
{
    //Postfix patch of ShipStatus.FixedUpdate to sabotage different systems
    public static void Postfix(ShipStatus __instance)
    {
        byte currentMapID = Utils.getCurrentMapID();

        if (CheatToggles.reactorSab && !BetterSabotage_ShipStatus_UpdateSystem_Postfix.reactorSab) { //Reactor sabotages

            if (currentMapID == 2) { //Polus uses has SystemTypes.Laboratory instead of SystemTypes.Reactor
                
                __instance.RpcUpdateSystem(SystemTypes.Laboratory, 128);

            } else if (currentMapID == 4) { //Airship uses HeliSabotageSystem to sabotage reactor

                __instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 128);

            } else { //Other maps behave normally 
                __instance.RpcUpdateSystem(SystemTypes.Reactor, 128);
            }

        } else if (CheatToggles.oxygenSab && !BetterSabotage_ShipStatus_UpdateSystem_Postfix.oxygenSab) { //Oxygen sabotages

            if (currentMapID != 4 && currentMapID != 2 && currentMapID != 5) { //Polus, Airship & Fungle have NO oxygen system

                __instance.RpcUpdateSystem(SystemTypes.LifeSupp, 128);

            } else {
                HudManager.Instance.Notifier.AddItem("Oxygen system not present on this map");
            }
        
        } else if (CheatToggles.commsSab && !BetterSabotage_ShipStatus_UpdateSystem_Postfix.commsSab) { //Communications sabotages

            __instance.RpcUpdateSystem(SystemTypes.Comms, 128);

        } else if (CheatToggles.elecSab && !BetterSabotage_ShipStatus_UpdateSystem_Postfix.elecSab) { //Eletrical sabotage

            if (currentMapID != 5) { //Fungle has no eletrical sabotage

                byte b = 4;
                for (int i = 0; i < 5; i++)
                {
                    if (BoolRange.Next(0.5f))
                    {
                        b |= (byte)(1 << i);
                    }
                }

                __instance.RpcUpdateSystem(SystemTypes.Electrical, (byte)(b | 128));

            } else {
                HudManager.Instance.Notifier.AddItem("Eletrical system not present on this map");
            }
            
        } else if (CheatToggles.mushSab) {

            if (currentMapID == 5){ //MushroomMixup only works on Fungle
                __instance.RpcUpdateSystem(SystemTypes.MushroomMixupSabotage, 1); //Sabotage MushroomMixup
            } else {
                HudManager.Instance.Notifier.AddItem("Mushrooms not present on this map");
            }

            CheatToggles.mushSab = false; //Button behaviour

        } else if (CheatToggles.doorsSab) {

            //Loop through all rooms and close their doors
            foreach (SystemTypes room in (SystemTypes[]) Enum.GetValues(typeof(SystemTypes)))
            {
                try{__instance.RpcCloseDoorsOfType(room);}catch{} //try-catch for rooms with no doors
            }

            CheatToggles.doorsSab = false; //Button behaviour

        }


        // repair sabotage
        if (!CheatToggles.reactorSab && BetterSabotage_ShipStatus_UpdateSystem_Postfix.reactorSab)
        {
            if (currentMapID == 2)
            {
                __instance.RpcUpdateSystem(SystemTypes.Laboratory, 16);
            }
            else if (currentMapID == 4)
            {

                __instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0);
                __instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);

            }
            else
            {
                __instance.RpcUpdateSystem(SystemTypes.Reactor, 16);
            }

            BetterSabotage_ShipStatus_UpdateSystem_Postfix.reactorSab = false;

        }
        else if (!CheatToggles.oxygenSab && BetterSabotage_ShipStatus_UpdateSystem_Postfix.oxygenSab)
        {

            if (currentMapID != 4 && currentMapID != 2 && currentMapID != 5)
            {

                __instance.RpcUpdateSystem(SystemTypes.LifeSupp, 16);

            }

            BetterSabotage_ShipStatus_UpdateSystem_Postfix.oxygenSab = false;

        }
        else if (!CheatToggles.commsSab && BetterSabotage_ShipStatus_UpdateSystem_Postfix.commsSab)
        {

            if (currentMapID == 1 || currentMapID == 5)
            {
                __instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
                __instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
            }
            else
            {
                __instance.RpcUpdateSystem(SystemTypes.Comms, 16);
            }

            BetterSabotage_ShipStatus_UpdateSystem_Postfix.commsSab = false;

        }
        else if (!CheatToggles.elecSab && BetterSabotage_ShipStatus_UpdateSystem_Postfix.elecSab)
        {

            if (currentMapID != 5)
            {
                var elecSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                for (var i = 0; i < 5; i++)
                {
                    var switchMask = 1 << (i & 0x1F);

                    if ((elecSystem.ActualSwitches & switchMask) != (elecSystem.ExpectedSwitches & switchMask))
                    {
                        __instance.RpcUpdateSystem(SystemTypes.Electrical, (byte)i);
                    }
                }

            }

            BetterSabotage_ShipStatus_UpdateSystem_Postfix.elecSab = false;

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

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.UpdateSystem))]
[HarmonyPatch(new Type[] { typeof(SystemTypes), typeof(PlayerControl), typeof(byte) })]
public static class BetterSabotage_ShipStatus_UpdateSystem_Postfix
{
    public static bool reactorSab;
    public static bool oxygenSab;
    public static bool commsSab;
    public static bool elecSab;
    public static void Postfix(SystemTypes systemType, PlayerControl player, byte amount, ShipStatus __instance)
    {
        if ((systemType == SystemTypes.HeliSabotage || systemType == SystemTypes.Laboratory || systemType == SystemTypes.Reactor) && amount == 128)
        {
            CheatToggles.reactorSab = true;
            reactorSab = true;
        }
        else if (systemType == SystemTypes.LifeSupp && amount == 128)
        {
            CheatToggles.oxygenSab = true;
            oxygenSab = true;
        }
        else if (systemType == SystemTypes.Comms && amount == 128)
        {
            CheatToggles.commsSab = true;
            commsSab = true;
        }
        else if (systemType == SystemTypes.Electrical && amount.HasBit(128))
        {
            CheatToggles.elecSab = true;
            elecSab = true;
        }
    }
}