using UnityEngine;

namespace MalumMenu;

public static class TracersHandler
{
    public static void drawPlayerTracer(PlayerPhysics playerPhysics)
    {
        if (playerPhysics?.myPlayer?.Data == null || PlayerControl.LocalPlayer == null)
            return;

        try
        {
            Color tracerColor = Color.clear;
            var data = playerPhysics.myPlayer.Data;

            bool isImpostor = data.Role.IsImpostor;
            bool isDead = data.IsDead;

            if (!isDead)
            {
                if (CheatToggles.tracersCrew && !isImpostor)
                {
                    tracerColor = CheatToggles.colorBasedTracers ? data.Color : data.Role.TeamColor;
                }
                else if (CheatToggles.tracersImps && isImpostor)
                {
                    tracerColor = CheatToggles.colorBasedTracers ? data.Color : data.Role.TeamColor;
                }
            }
            else if (CheatToggles.tracersGhosts)
            {
                tracerColor = CheatToggles.colorBasedTracers ? data.Color : Palette.White;
            }

            Utils.drawTracer(playerPhysics.myPlayer.gameObject, PlayerControl.LocalPlayer.gameObject, tracerColor);
        }
        catch
        {
            // Ignore draw failures silently
        }
    }

    public static void drawBodyTracer(DeadBody deadBody)
    {
        if (deadBody == null || PlayerControl.LocalPlayer == null || GameData.Instance == null)
            return;

        Color tracerColor = Color.clear;

        if (CheatToggles.tracersBodies)
        {
            tracerColor = CheatToggles.colorBasedTracers
                ? GameData.Instance.GetPlayerById(deadBody.ParentId)?.Color ?? Color.yellow
                : Color.yellow;
        }

        Utils.drawTracer(deadBody.gameObject, PlayerControl.LocalPlayer.gameObject, tracerColor);
    }
}
