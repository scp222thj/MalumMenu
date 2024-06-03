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
    public static bool Prefix(PlayerPhysics __instance, Vector2 direction)
    {
        if (CheatToggles.speedBoost){
            float cheatSpeed = 2f;

            float actualSpeed = PlayerControl.LocalPlayer.MyPhysics.Speed * PlayerControl.LocalPlayer.MyPhysics.SpeedMod;

            PlayerControl.LocalPlayer.MyPhysics.body.velocity = direction * actualSpeed * cheatSpeed;

            return false;
        }

        return true;
    }
} 