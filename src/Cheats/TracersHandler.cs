using UnityEngine;

namespace MalumMenu;

public static class TracersHandler
{
    // Draws vent-to-vent path tracer lines for connected vents on the active map.
    public static void DrawVentTracers()
    {
        if (!Utils.isShip) return;

        var color = CheatToggles.ventTracers && !CheatToggles.streamerMode
            ? new Color(0f, 1f, 1f, 0.8f)
            : Color.clear;

        foreach (var vent in ShipStatus.Instance.AllVents)
        {
            if (vent == null) continue;

            Vent[] connections = { vent.Left, vent.Right, vent.Center };

            for (int slot = 0; slot < 3; slot++)
            {
                var connected = connections[slot];
                if (connected == null) continue;

                // Draw each connection only once (from lower vent ID to higher)
                if (vent.Id >= connected.Id) continue;

                // Attach a dedicated child object per connection so each gets its own LineRenderer
                var childName = $"VT_{connected.Id}";
                var childTransform = vent.transform.Find(childName);
                GameObject lineObj;

                if (childTransform == null)
                {
                    lineObj = new GameObject(childName);
                    lineObj.transform.SetParent(vent.transform);
                    lineObj.transform.localPosition = Vector3.zero;
                }
                else
                {
                    lineObj = childTransform.gameObject;
                }

                Utils.DrawTracer(lineObj, connected.gameObject, color);
            }
        }
    }

    // Draws a tracer from LocalPlayer to another player.
    public static void DrawPlayerTracer(PlayerPhysics playerPhysics)
    {
        try
        {
            var color = Color.clear; // All tracers are invisible by default

            if (CheatToggles.streamerMode) // Streamer mode hides all tracers
            {
                Utils.DrawTracer(playerPhysics.myPlayer.gameObject, PlayerControl.LocalPlayer.gameObject, Color.clear);
                return;
            }

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
        } catch { }
    }

    // Draws a tracer LocalPlayer to a dead body. Only draws tracers for unreported dead bodies.
    public static void DrawBodyTracer(DeadBody deadBody)
    {
        var color = Color.clear; // All tracers are invisible by default

        if (CheatToggles.streamerMode) // Streamer mode hides all tracers
        {
            Utils.DrawTracer(deadBody.gameObject, PlayerControl.LocalPlayer.gameObject, Color.clear);
            return;
        }

        if (CheatToggles.tracersBodies)
        {
            if (CheatToggles.distanceBasedTracers)
            {
                color = GetDistanceBasedColor(deadBody.transform.position);
            }
            else if (CheatToggles.colorBasedTracers)
            {
                var deadBodyInfo = GameData.Instance.GetPlayerById(deadBody.ParentId);
                color = deadBodyInfo != null ? deadBodyInfo.Color : Color.yellow; // Color-Based Tracer
            }
            else
            {
                color = Color.yellow; // Dead Body Tracer (Yellow)
            }
        }

        // Draw tracer between the dead body and LocalPlayer using the right color
        Utils.DrawTracer(deadBody.gameObject, PlayerControl.LocalPlayer.gameObject, color);
    }

    // Gets a color based on the distance between the LocalPlayer and a target position.
    // Closer distances are red, medium distances are yellow, and farther distances are green.
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
