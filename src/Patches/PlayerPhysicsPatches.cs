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
        MalumCheats.speedBoostCheat();
        MalumCheats.killAllCheat();
        MalumCheats.killAllCrewCheat();
        MalumCheats.killAllImpsCheat();
        MalumCheats.teleportCursorCheat();
        MalumCheats.completeMyTasksCheat();

        MalumPPMCheats.spectatePPM();
        MalumPPMCheats.killPlayerPPM();
        //MalumPPMCheats.telekillPlayerPPM();
        MalumPPMCheats.teleportPlayerPPM();
        MalumPPMCheats.changeRolePPM();

        //if (MalumPPMCheats.teleKillWaitFrames == 0){
        //    KillAnimation.SetMovement(PlayerControl.LocalPlayer, true);
        //    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(MalumPPMCheats.teleKillPosition);
        //}

        //MalumPPMCheats.teleKillWaitFrames--;

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