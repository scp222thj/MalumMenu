using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(Mushroom), nameof(Mushroom.FixedUpdate))]
public static class ESP_SeePlayersInSporesPostfix
{
    public static void Postfix(Mushroom __instance)
    {
        if (ESP_HudManagerPostfix.fullBrightActive)
        {
            __instance.sporeMask.transform.position = new UnityEngine.Vector3(__instance.sporeMask.transform.position.x, __instance.sporeMask.transform.position.y, -1);
            return;
        } 

        __instance.sporeMask.transform.position = new UnityEngine.Vector3(__instance.sporeMask.transform.position.x, __instance.sporeMask.transform.position.y, 5f);
    }
}