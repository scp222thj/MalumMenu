using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(Mushroom), nameof(Mushroom.FixedUpdate))]
public static class Mushrooms_SporeCloudVisionPostfix
{
    public static void Postfix(Mushroom __instance)
    {
        if (CheatSettings.sporeVision)
        {
            __instance.sporeMask.transform.position = new UnityEngine.Vector3(__instance.sporeMask.transform.position.x, __instance.sporeMask.transform.position.y, -1);
            return;
        } 

        __instance.sporeMask.transform.position = new UnityEngine.Vector3(__instance.sporeMask.transform.position.x, __instance.sporeMask.transform.position.y, 5f);
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class Mushrooms_MushroomMixupPostfix
{
    //Postfix patch of ShipStatus.FixedUpdate to sabotage different systems
    public static void Postfix(ShipStatus __instance)
    {
        if (CheatSettings.mushSab){
            byte currentMapID = Utils.getCurrentMapID();
            
            if (currentMapID == 5){ //MushroomMixup only works on Fungle
                __instance.RpcUpdateSystem(SystemTypes.MushroomMixupSabotage, 1); //Sabotage MushroomMixup
            }else{
                HudManager.Instance.Notifier.AddItem("Mushrooms not present on this map");
            }

            CheatSettings.mushSab = false; //Button behaviour
        }
        
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class Mushrooms_SporeTriggerPostfix
{
    public static void Postfix(FungleShipStatus __instance)
    {
        if (CheatSettings.mushSpore)
        {
            byte currentMapID = Utils.getCurrentMapID();
            if (currentMapID == 5)
            {
                foreach (Mushroom mushroom in __instance.sporeMushrooms.Values)
                {
                    PlayerControl.LocalPlayer.CmdCheckSporeTrigger(mushroom);
                }
            }
            else
            {
                HudManager.Instance.Notifier.AddItem("Mushrooms not present on this map");
            }

            CheatSettings.mushSpore = false;
        }
    }
}