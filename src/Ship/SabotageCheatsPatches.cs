using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class Sabotages_ShipStatus_FixedUpdate_Postfix
{
    public static bool reactorSab;
    public static bool oxygenSab;
    public static bool commsSab;
    public static bool elecSab;
    //Postfix patch of ShipStatus.FixedUpdate to sabotage different systems
    public static void Postfix(ShipStatus __instance)
    {
        byte currentMapID = Utils.getCurrentMapID();

        if (CheatToggles.reactorSab && !reactorSab) { //Reactor sabotages

            if (currentMapID == 2) { //Polus uses has SystemTypes.Laboratory instead of SystemTypes.Reactor
                
                __instance.RpcUpdateSystem(SystemTypes.Laboratory, 128);

            } else if (currentMapID == 4) { //Airship uses HeliSabotageSystem to sabotage reactor

                __instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 128);

            } else { //Other maps behave normally 
                __instance.RpcUpdateSystem(SystemTypes.Reactor, 128);
            }
            reactorSab = true;

        } else if (CheatToggles.oxygenSab && !oxygenSab) { //Oxygen sabotages

            if (currentMapID != 4 && currentMapID != 2 && currentMapID != 5) { //Polus, Airship & Fungle have NO oxygen system

                __instance.RpcUpdateSystem(SystemTypes.LifeSupp, 128);
                oxygenSab = true;

            } else {
                HudManager.Instance.Notifier.AddItem("Oxygen system not present on this map");
                CheatToggles.oxygenSab = false;
            }
        
        } else if (CheatToggles.commsSab && !commsSab) { //Communications sabotages

            __instance.RpcUpdateSystem(SystemTypes.Comms, 128);
            commsSab = true;

        } else if (CheatToggles.elecSab && !elecSab) { //Eletrical sabotage

            if (currentMapID != 5) { //Fungle has no eletrical sabotage

                CheatToggles.unfixableLights = false;

                byte b = 4;
                for (int i = 0; i < 5; i++)
                {
                    if (BoolRange.Next(0.5f))
                    {
                        b |= (byte)(1 << i);
                    }
                }

                __instance.RpcUpdateSystem(SystemTypes.Electrical, (byte)(b | 128));
                elecSab = true;

            } else {
                HudManager.Instance.Notifier.AddItem("Eletrical system not present on this map");
                CheatToggles.elecSab = false;
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
            foreach (SystemTypes room in (SystemTypes[]) System.Enum.GetValues(typeof(SystemTypes)))
            {
                try{__instance.RpcCloseDoorsOfType(room);}catch{} //try-catch for rooms with no doors
            }

            CheatToggles.doorsSab = false; //Button behaviour

        }


        if (!CheatToggles.reactorSab && reactorSab)
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

            reactorSab = false;

        }
        else if (!CheatToggles.oxygenSab && oxygenSab)
        {

            if (currentMapID != 4 && currentMapID != 2 && currentMapID != 5)
            {

                __instance.RpcUpdateSystem(SystemTypes.LifeSupp, 16);

            }

            oxygenSab = false;

        }
        else if (!CheatToggles.commsSab && commsSab)
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

            commsSab = false;

        }
        else if (!CheatToggles.elecSab && elecSab)
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

            elecSab = false;

        }

        ReactorSystemType reactorSys = null;
        ReactorSystemType labSys = null;
        HeliSabotageSystem heliSys = null;
        LifeSuppSystemType oxygenSys = null;
        HqHudSystemType hqCommsSys = null;
        HudOverrideSystemType commsSys = null;
        SwitchSystem elecSys = null;

        switch (currentMapID)
        {
            case 0:
            case 3:
                reactorSys = __instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                oxygenSys = __instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                commsSys = __instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                elecSys = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                break;
            case 1:
                reactorSys = __instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                oxygenSys = __instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                hqCommsSys = __instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                elecSys = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                break;
            case 2:
                labSys = __instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                commsSys = __instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                elecSys = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                break;
            case 4:
                heliSys = __instance.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
                commsSys = __instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                elecSys = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                break;
            case 5:
                reactorSys = __instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                hqCommsSys = __instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                break;
        }

        if (reactorSys != null || labSys != null || heliSys != null)
        {
            var reactorSabActive = reactorSys == null ? labSys == null ? heliSys.IsActive : labSys.IsActive : reactorSys.IsActive;
            CheatToggles.reactorSab = reactorSab = reactorSabActive;
        }
        if (hqCommsSys != null || commsSys != null)
        {
            var commsSabActive = hqCommsSys == null ? commsSys.IsActive : hqCommsSys.IsActive;
            CheatToggles.commsSab = commsSab = commsSabActive;
        }
        if (oxygenSys != null)
        {
            CheatToggles.oxygenSab = oxygenSab = oxygenSys.IsActive;
        }
        if (elecSys != null)
        {
            var elecSabActive = elecSys.IsActive && !UnfixableLights_ShipStatus_FixedUpdate_Postfix.isActive;

            CheatToggles.elecSab = elecSab = elecSabActive;
        }
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class UnfixableLights_ShipStatus_FixedUpdate_Postfix
{
    public static bool isActive;
    //Postfix patch of ShipStatus.FixedUpdate to break lights completly & make them unable to be fixed normally
    public static void Postfix(ShipStatus __instance)
    {
        if (CheatToggles.unfixableLights && !isActive)
        {
            //Apparently most values you put for amount in RpcUpdateSystem will break lights completly
            //They are unfixable through regular means (toggling switches)
            //They can only be repaired by repeating RpcUpdateSystem with the same amount
            
            CheatToggles.elecSab = false;

            byte currentMapID = Utils.getCurrentMapID();

            if (currentMapID != 5){ //Fungle has no lights, so blackout can't trigger there

                __instance.RpcUpdateSystem(SystemTypes.Electrical, 69);

            }else{

                HudManager.Instance.Notifier.AddItem("Eletrical system not present on this map");

                CheatToggles.unfixableLights = false;
                
            }

            isActive = true;
        }
        else if (!CheatToggles.unfixableLights && isActive)
        {
            __instance.RpcUpdateSystem(SystemTypes.Electrical, 69);

            isActive = false;
        }
    }
}