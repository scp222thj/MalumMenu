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

        // AmOwner guard ensures these run exactly once per frame (not once per player object)
        if (__instance.AmOwner)
        {
            MalumCheats.NoClipCheat();
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
            MalumPPMCheats.SetFakeRolePPM();
            MalumPPMCheats.SetFakeAlivePPM();
            MalumPPMCheats.FakeShapeshiftPPM();
            MalumPPMCheats.ForceRolePPM();
            MalumPPMCheats.FreezePlayerPPM();
            MalumPPMCheats.FrameAsShapeshifterPPM();
            MalumPPMCheats.TeleportPlayerToPlayerPPM();
            MalumPPMCheats.FakeVentOnPlayerPPM();

            OverloadHandler.Run();
            TracersHandler.DrawVentTracers();

            GameObject[] bodyObjects = GameObject.FindGameObjectsWithTag("DeadBody");
            foreach (GameObject bodyObject in bodyObjects)
            {
                DeadBody deadBody = bodyObject.GetComponent<DeadBody>();
                if (!deadBody || deadBody.Reported) continue;
                TracersHandler.DrawBodyTracer(deadBody);
            }

            MalumCheats.BlinkCheat();
            MalumCheats.VisionBoostCheat();
            MalumPPMCheats.TickFreezePlayer();
            FootprintHandler.Update();
        }

        TracersHandler.DrawPlayerTracer(__instance);

        if (__instance.AmOwner && PlayerControl.LocalPlayer != null)
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
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
public static class PlayerPhysics_HandleAnimation
{
    // Prefix patch of PlayerPhysics.HandleAnimation to disable walking animation
    public static bool Prefix(PlayerPhysics __instance)
    {
        if (CheatToggles.moonWalk && __instance.AmOwner)
        {
            __instance.ResetAnimState();

            return false;
        }

        return true;
    }
}
