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
        // MalumPPMCheats.ForceRolePPM();

        // This check ensures there is only one run per frame
        // so that OverloadHandler._timer progression remains accurate
        // if (__instance.AmOwner)
        // {
        //     OverloadHandler.Run();
        // }

        TracersHandler.DrawPlayerTracer(__instance);

        GameObject[] bodyObjects = GameObject.FindGameObjectsWithTag("DeadBody");
        foreach(GameObject bodyObject in bodyObjects) // Finds and loops through all dead bodies
        {
            DeadBody deadBody = bodyObject.GetComponent<DeadBody>();
            if (!deadBody) continue;

            TracersHandler.DrawBodyTracer(deadBody);

            if (CheatToggles.autoReportBodies)
            {
                if (deadBody.Reported) continue;

                deadBody.Reported = true;

                PlayerControl.LocalPlayer.CmdReportDeadBody(GameData.Instance.GetPlayerById(deadBody.ParentId));
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
        } catch (NullReferenceException) { }
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
