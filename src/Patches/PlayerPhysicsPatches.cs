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
        // MalumPPMCheats.telekillPlayerPPM(); // Optional: Enable if fixed
        MalumPPMCheats.teleportPlayerPPM();
        MalumPPMCheats.changeRolePPM();

        // Uncomment this if telekill is restored:
        // if (MalumPPMCheats.teleKillWaitFrames == 0)
        // {
        //     KillAnimation.SetMovement(PlayerControl.LocalPlayer, true);
        //     PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(MalumPPMCheats.teleKillPosition);
        // }
        //
        // MalumPPMCheats.teleKillWaitFrames--;

        TracersHandler.drawPlayerTracer(__instance);

        // Draw tracers to unreported bodies
        foreach (var bodyObject in GameObject.FindGameObjectsWithTag("DeadBody"))
        {
            var deadBody = bodyObject.GetComponent<DeadBody>();
            if (deadBody != null && !deadBody.Reported)
            {
                TracersHandler.drawBodyTracer(deadBody);
            }
        }
    }
}