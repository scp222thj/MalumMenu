using HarmonyLib;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MalumMenu;

[HarmonyPatch(typeof(Mushroom), nameof(Mushroom.FixedUpdate))]
public static class ESP_SeePlayersInSporesPostfix
{
    public static void Postfix(Mushroom __instance)
    {
        if (CheatSettings.seeInSpore)
        {
            __instance.sporeMask.transform.position = new UnityEngine.Vector3(__instance.sporeMask.transform.position.x, __instance.sporeMask.transform.position.y, -1);
        } 
        else
        {
            if (__instance.Id == 2)
                __instance.sporeMask.transform.position = new UnityEngine.Vector3(__instance.sporeMask.transform.position.x, __instance.sporeMask.transform.position.y, 4.8f);
            else
                __instance.sporeMask.transform.position = new UnityEngine.Vector3(__instance.sporeMask.transform.position.x, __instance.sporeMask.transform.position.y, 4.3f);
        }
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class ESP_MixupOffPostfix
{
    public static void Postfix(FungleShipStatus __instance)
    {
        if (CheatSettings.seeMixup)
        {
            byte currentMapID = Utils.getCurrentMapID();
            if (currentMapID == 5)
            {
                MushroomMixupSabotageSystem mixupSystem = __instance.specialSabotage;
                if (mixupSystem.IsActive)
                {
                    mixupSystem.Deteriorate(mixupSystem.currentSecondsUntilHeal);
                    if (AmongUsClient.Instance.AmHost)
                    {
                        HudManager.Instance.Notifier.AddItem("Since you are HOST, Mixup is turned off for everyone.");
                    }
                    else
                    {
                        HudManager.Instance.Notifier.AddItem("Since you are not HOST, Mixup is turned off for only you.");
                    }
                }
                else
                {
                    HudManager.Instance.Notifier.AddItem("Mushroom Mixup is Inactivated.");
                }
            }
            else
            {
                HudManager.Instance.Notifier.AddItem("It only works in The Fungle.");
            }

            CheatSettings.seeMixup = false;
        }
    }
}