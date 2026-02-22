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

        MalumCheats.NoClipCheat();
        MalumCheats.ReviveCheat();
        MalumCheats.ProtectCheat();
        MalumCheats.KillAllCheat();
        MalumCheats.KillAllCrewCheat();
        MalumCheats.KillAllImpsCheat();
        MalumCheats.ForceStartGameCheat();
        MalumCheats.TeleportCursorCheat();
        MalumCheats.CompleteMyTasksCheat();
        MalumCheats.PlayAnimationCheat();
        MalumCheats.PlayScannerCheat();

        MalumPPMCheats.ejectPlayerPPM();
        MalumPPMCheats.spectatePPM();
        MalumPPMCheats.killPlayerPPM();
        MalumPPMCheats.telekillPlayerPPM();
        MalumPPMCheats.teleportPlayerPPM();
        MalumPPMCheats.changeRolePPM();
        MalumPPMCheats.forceRolePPM();

        TracersHandler.DrawPlayerTracer(__instance);

        GameObject[] bodyObjects = GameObject.FindGameObjectsWithTag("DeadBody");
        foreach(GameObject bodyObject in bodyObjects) // Finds and loops through all dead bodies
        {
            DeadBody deadBody = bodyObject.GetComponent<DeadBody>();

            if (!deadBody || deadBody.Reported) continue;  // Only draw tracers for unreported dead bodies
            TracersHandler.DrawBodyTracer(deadBody);
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

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
public static class PlayerPhysics_HandleAnimation
{
    /// <summary>
    /// Prefix patch of PlayerPhysics.HandleAnimation to disable walking animation.
    /// </summary>
    /// <param name="__instance">The <c>PlayerPhysics</c> instance.</param>
    /// <param name="amDead">Whether to play the ghost animation.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(PlayerPhysics __instance, ref bool amDead)
    {
        return !(CheatToggles.moonWalk && __instance.AmOwner);
    }
}
