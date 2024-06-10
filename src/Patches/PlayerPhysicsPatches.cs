using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class PlayerPhysics_LateUpdate
{
    public static void Postfix(PlayerPhysics __instance)
    {

        MalumESP.playerNametags(__instance);
        MalumESP.seeGhostsCheat(__instance);

        MalumCheats.noClipCheat();
        MalumCheats.murderAllCheat();
        MalumCheats.teleportCursorCheat();
        MalumCheats.completeMyTasksCheat();

        MalumPPMCheats.spectatePPM();
        MalumPPMCheats.murderPlayerPPM();
        MalumPPMCheats.teleportPlayerPPM();
        MalumPPMCheats.changeRolePPM();

        TracersHandler.drawPlayerTracer(__instance);

        GameObject[] bodyObjects = GameObject.FindGameObjectsWithTag("DeadBody");
        foreach(GameObject bodyObject in bodyObjects) // Finds and loops through all dead bodies
        {
            DeadBody deadBody = bodyObject.GetComponent<DeadBody>();

            if (deadBody){
                if (!deadBody.Reported){ // Only draw tracers for unreported dead bodies
                    TracersHandler.drawBodyTracer(deadBody);
                }
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetNormalizedVelocity))]
public static class PlayerPhysics_SetNormalizedVelocity
{
    // Prefix patch of PlayerPhysics.SetNormalizedVelocity to double the player's speed when the speedBoost cheat is enabled
    public static bool Prefix(PlayerPhysics __instance, Vector2 direction)
    {
        if (CheatToggles.speedBoost){

            // Cheat multiplier
            float cheatSpeed = 2f;

            // Actual speed of player (no hacks)
            float actualSpeed = PlayerControl.LocalPlayer.MyPhysics.Speed * PlayerControl.LocalPlayer.MyPhysics.SpeedMod;

            PlayerControl.LocalPlayer.MyPhysics.body.velocity = direction * actualSpeed * cheatSpeed;

            return false;
        }

        return true;
    }
} 