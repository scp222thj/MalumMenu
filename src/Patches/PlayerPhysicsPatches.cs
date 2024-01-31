using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class PlayerPhysics_LateUpdate
{
    public static void Postfix(PlayerPhysics __instance)
    {
        
        MalumESP.playerNametags(__instance);
        MalumESP.seeGhostsCheat(__instance);

        MalumCheats.noClipCheat();
        MalumCheats.speedBoostCheat();
        MalumCheats.murderAllCheat();
        MalumCheats.teleportCursorCheat();
        MalumCheats.murderPlayerCheat();
        MalumCheats.teleportPlayerCheat();

        MalumESP.spectateCheat();

    }
}