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
        MalumESP.SeeGhostsCheat(__instance);

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

        MalumPPMCheats.EjectPlayerPPM();
        MalumPPMCheats.SpectatePPM();
        MalumPPMCheats.KillPlayerPPM();
        MalumPPMCheats.TelekillPlayerPPM();
        MalumPPMCheats.TeleportPlayerPPM();
        MalumPPMCheats.ChangeRolePPM();
        MalumPPMCheats.ForceRolePPM();

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
        } catch (NullReferenceException) { }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
public static class PlayerPhysics_HandleAnimation
{
    // Prefix patch of PlayerPhysics.HandleAnimation to disable walking animation
    public static bool Prefix(PlayerPhysics __instance)
    {
        return !(CheatToggles.moonWalk && __instance.AmOwner);
    }
}
