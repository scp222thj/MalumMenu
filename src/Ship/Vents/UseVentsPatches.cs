using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class UseVents_HudManager_Update_Postfix
{

    // Postfix patch of HudManager.Update to enable vent button
    public static void Postfix(HudManager __instance)
    {
        // try-catch to prevent errors when role is null
        try
        {
            // Engineers & Impostors don't need this cheat so it is disabled for them
            // Ghost venting causes issues so it is also disabled
            if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (CheatToggles.useVents)
                {
                    __instance.ImpostorVentButton.gameObject.SetActive(true);
                }
                else
                {
                    __instance.ImpostorVentButton.gameObject.SetActive(false);
                }
            }

        }
        catch { }
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
public static class UseVents_Vent_CanUse_Prefix
{
    // Prefix patch of Vent.CanUse to allow venting for cheaters
    // Basically does what the original method did with the required modifications
    public static bool Prefix(GameData.PlayerInfo pc, out bool canUse, out bool couldUse, Vent __instance, ref float __result)
    {
        canUse = false;
        couldUse = false;

        // try-catch to prevent errors when role is null
        try
        {
            // Engineers & Impostors don't need this cheat so it is disabled for them
            // Ghost venting causes issues so it is also disabled
            if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (CheatToggles.useVents)
                {
                    float num = float.MaxValue;
                    PlayerControl @object = pc.Object;

                    Vector3 center = @object.Collider.bounds.center;
                    Vector3 position = __instance.transform.position;
                    num = Vector2.Distance(center, position);

                    // Allow usage of vents unless the vent is too far or there are objects blocking the player's path
                    canUse = (num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false));
                    couldUse = true;
                    __result = num;
                    return false; // Skip original method
                }
            }

        }
        catch { }

        return true; // Run original method if cheat is disabled
    }
}