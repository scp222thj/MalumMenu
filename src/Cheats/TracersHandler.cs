using UnityEngine;

namespace MalumMenu;

public static class TracersHandler
{
    /// <summary>
    /// Draws a tracer from the local player to another player.
    /// </summary>
    /// <param name="playerPhysics">The <c>PlayerPhysics</c> of the target player.</param>
    public static void DrawPlayerTracer(PlayerPhysics playerPhysics)
    {
        try
        {
            var color = Color.clear; // All tracers are invisible by default

            if (!playerPhysics.myPlayer.Data.IsDead)
            {
                if (CheatToggles.tracersCrew && !playerPhysics.myPlayer.Data.Role.IsImpostor ||
                    CheatToggles.tracersImps && playerPhysics.myPlayer.Data.Role.IsImpostor)
                {
                    if (CheatToggles.distanceBasedTracers)
                    {
                        color = GetDistanceBasedColor(playerPhysics.myPlayer.transform.position);
                    }
                    else if (CheatToggles.colorBasedTracers)
                    {
                        color = playerPhysics.myPlayer.Data.Color; // Color-Based Tracer
                    }
                    else
                    {
                        color = playerPhysics.myPlayer.Data.Role.TeamColor; // Team-Based Tracer
                    }
                }
            }
            else
            {
                if (CheatToggles.tracersGhosts)
                {
                    if (CheatToggles.distanceBasedTracers)
                    {
                        color = GetDistanceBasedColor(playerPhysics.myPlayer.transform.position);
                    }
                    else if (CheatToggles.colorBasedTracers)
                    {
                        color = playerPhysics.myPlayer.Data.Color; // Color-Based Tracer
                    }
                    else
                    {
                        color = Palette.White; // Ghost Tracer (White)
                    }
                }
            }

            // Draw tracer between the player and LocalPlayer using the right color
            Utils.DrawTracer(playerPhysics.myPlayer.gameObject, PlayerControl.LocalPlayer.gameObject, color);
        }catch{}
    }

    /// <summary>
    /// Draws a tracer from the local player to a dead body. Only draws tracers for unreported dead bodies.
    /// </summary>
    /// <param name="deadBody">The <c>DeadBody</c> to draw a tracer to.</param>
    public static void DrawBodyTracer(DeadBody deadBody)
    {
        var color = Color.clear; // All tracers are invisible by default

        if (CheatToggles.tracersBodies)
        {
            if (CheatToggles.distanceBasedTracers)
            {
                color = GetDistanceBasedColor(deadBody.transform.position);
            }
            else if (CheatToggles.colorBasedTracers)
            {
                color = GameData.Instance.GetPlayerById(deadBody.ParentId).Color; // Color-Based Tracer
            }
            else
            {
                color = Color.yellow; // Dead Body Tracer (Yellow)
            }
        }

        // Draw tracer between the dead body and LocalPlayer using the right color
        Utils.DrawTracer(deadBody.gameObject, PlayerControl.LocalPlayer.gameObject, color);
    }

    /// <summary>
    /// Gets a color based on the distance between the local player and a target position.
    /// Closer distances are red, medium distances are yellow, and farther distances are green.
    /// </summary>
    /// <param name="targetPosition">The position to calculate the distance from.</param>
    /// <returns>A Color that represents the distance (red for close, yellow for medium, green for far).</returns>
    private static Color GetDistanceBasedColor(Vector3 targetPosition)
    {
        const float maxDistance = 20f; // Green at 20+ units
        const float minDistance = 2f;  // Red at 2 units or fewer

        var distance = Vector3.Distance(targetPosition, PlayerControl.LocalPlayer.transform.position);
        var normalized = Mathf.InverseLerp(minDistance, maxDistance, distance);

        // Interpolate: Red (close) -> Yellow (medium) -> Green (far)
        return normalized < 0.5f
            ? Color.Lerp(Color.red, Color.yellow, normalized * 2f)
            : Color.Lerp(Color.yellow, Color.green, (normalized - 0.5f) * 2f);
    }
}
