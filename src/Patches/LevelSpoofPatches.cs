using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetLevel))]
public static class PlayerControl_RpcSetLevel_Patch
{
    public static void Prefix(PlayerControl __instance, ref uint level)
    {
        if ((Object)(object)__instance == (Object)(object)PlayerControl.LocalPlayer && !string.IsNullOrEmpty(MalumMenu.spoofLevel.Value))
        {
            if (uint.TryParse(MalumMenu.spoofLevel.Value, out uint spoofLevel))
            {
                // Enforce hard cap at 100k to prevent kick
                if (spoofLevel >= 1 && spoofLevel <= 100000)
                {
                    level = spoofLevel;
                }
                else
                {
                    level = 100000;  // Cap at 100k
                }
            }
        }
    }
}
