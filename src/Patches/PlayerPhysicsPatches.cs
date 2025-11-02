using System;
using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class PlayerPhysics_LateUpdate
{
    public static void Postfix(PlayerPhysics __instance)
    {

        MalumESP.PlayerNametags(__instance);
        MalumESP.seeGhostsCheat(__instance);

        MalumCheats.noClipCheat();
        MalumCheats.ReviveCheat();
        MalumCheats.killAllCheat();
        MalumCheats.killAllCrewCheat();
        MalumCheats.killAllImpsCheat();
        MalumCheats.forceStartGameCheat();
        MalumCheats.teleportCursorCheat();
        MalumCheats.completeMyTasksCheat();
        MalumCheats.AnimationCheat();
        MalumCheats.ScanCheat();

        MalumPPMCheats.ejectPlayerPPM();
        MalumPPMCheats.spectatePPM();
        MalumPPMCheats.killPlayerPPM();
        MalumPPMCheats.telekillPlayerPPM();
        MalumPPMCheats.teleportPlayerPPM();
        MalumPPMCheats.ProtectPlayerPPM();
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

        try
        {
            if (CheatToggles.invertControls)
            {
                PlayerControl.LocalPlayer.MyPhysics.Speed = -Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.Speed);
                PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = -Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed);
            }
            else
            {
                PlayerControl.LocalPlayer.MyPhysics.Speed = Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.Speed);
                PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed);
            }
        }catch (NullReferenceException) {}

    }
}
