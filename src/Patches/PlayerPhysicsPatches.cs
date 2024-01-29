using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class PlayerPhysics_LateUpdate
{
    public static void Postfix(PlayerPhysics __instance)
    {
        PlayerCheatHandler.noClipCheat();
        PlayerCheatHandler.speedBoostCheat();
        PlayerCheatHandler.murderAllCheat();
        PlayerCheatHandler.teleportCursorCheat();
        PlayerCheatHandler.murderPlayerCheat();
        PlayerCheatHandler.teleportPlayerCheat();

        EspHandler.spectateCheat();

    }
}