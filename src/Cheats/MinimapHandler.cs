using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;
public static class MinimapHandler
{
    public static bool minimapActive;
    public static List<HerePoint> herePoints = new List<HerePoint>();
    public static List<HerePoint> herePointsToRemove = new List<HerePoint>();
    public static float trailSeconds = 30f;

    private const float TrailWaypointIntervalSeconds = 1f;
    private const int TrailMaxWaypoints = 64;
    private static float _nextTrailRecordTime;

    private sealed class TrailLine
    {
        public byte playerId;
        public LineRenderer line;
        public Vector3[] positions = new Vector3[TrailMaxWaypoints];
        public float[] times = new float[TrailMaxWaypoints];
        public int start;
        public int count;
        public Vector3[] temp = new Vector3[TrailMaxWaypoints];
        public int lastRenderedCount;
        public Color color;
    }

    private static readonly Dictionary<byte, TrailLine> _trailsByPlayer = new Dictionary<byte, TrailLine>(16);

    public static bool IsCheatEnabled()
    {
        return CheatToggles.mapCrew || CheatToggles.mapGhosts || CheatToggles.mapImps;
    }

    public static void HandleHerePoint(HerePoint herePoint)
    {
        Color herePointColor = new Color();

        try // try-catch to fix issues caused by player disconnection
        {
            herePoint.sprite.gameObject.SetActive(false); // Initally make player icon invisible

            // Crewmate, alive
            if (CheatToggles.mapCrew && !herePoint.player.Data.Role.IsImpostor)
            {
                if (!herePoint.player.Data.IsDead)
                {
                    herePoint.sprite.gameObject.SetActive(true);
                    if (CheatToggles.colorBasedMap)
                    {
                        herePointColor = herePoint.player.Data.Color; // Color-Based Icon
                    }
                    else
                    {
                        herePointColor = herePoint.player.Data.Role.TeamColor; // Role-Based Icon
                    }
                }
            }
            // Impostor, alive
            else if (CheatToggles.mapImps && herePoint.player.Data.Role.IsImpostor)
            {
                if (!herePoint.player.Data.IsDead)
                {
                    herePoint.sprite.gameObject.SetActive(true);
                    if (CheatToggles.colorBasedMap)
                    {
                        herePointColor = herePoint.player.Data.Color; // Color-Based Icon
                    }
                    else
                    {
                        herePointColor = herePoint.player.Data.Role.TeamColor; // Role-Based Icon
                    }
                }
            }
            // Any Role, dead
            if (CheatToggles.mapGhosts && herePoint.player.Data.IsDead)
            {
                herePoint.sprite.gameObject.SetActive(true);
                if (CheatToggles.colorBasedMap)
                {
                    herePointColor = herePoint.player.Data.Color; // Color-Based Icon
                }
                else
                {
                    herePointColor = Palette.White;
                }
            }

            if (herePoint.sprite.gameObject.active)
            {
                // Set the right colors for active herePoint icons
                herePoint.sprite.material.SetColor(PlayerMaterial.BackColor, herePointColor);
                herePoint.sprite.material.SetColor(PlayerMaterial.BodyColor, herePointColor);
                herePoint.sprite.material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);

                if (CheatToggles.mapImpsHighlight && herePoint.player.Data.Role.IsImpostor && !herePoint.player.Data.IsDead)
                {
                    herePoint.sprite.material.SetColor(PlayerMaterial.BackColor, Color.red);
                }

                // Sync the position of active herePoint icons with their players
                var vector = herePoint.player.transform.position;
                vector /= ShipStatus.Instance.MapScale;
                vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                vector.z = -1f;
                herePoint.sprite.transform.localPosition = vector;
            }
        }
        catch
        {
            // Remove icons that are causing problems
            Object.Destroy(herePoint.sprite.gameObject);
            herePointsToRemove.Add(herePoint);
        }
    }

    public static void HandleTrails(MapBehaviour map)
    {
        if (!CheatToggles.mapTrails) return;
        if (map == null) return;
        if (!Utils.isShip) return;
        if (ShipStatus.Instance == null) return;
        if (Utils.isMeeting) { ClearTrails(); return; }

        var now = Time.time;
        if (now >= _nextTrailRecordTime)
        {
            _nextTrailRecordTime = now + TrailWaypointIntervalSeconds;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == null || player.Data == null) continue;

                var isImp = player.Data.Role != null && player.Data.Role.IsImpostor;
                var isDead = player.Data.IsDead;

                if (isDead)
                {
                    if (!CheatToggles.mapGhosts) continue;
                }
                else if (isImp)
                {
                    if (!CheatToggles.mapImps) continue;
                }
                else
                {
                    if (!CheatToggles.mapCrew) continue;
                }

                var pos = player.transform.position;
                pos /= ShipStatus.Instance.MapScale;
                pos.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                pos.z = -1.1f;

                var id = player.PlayerId;
                if (!_trailsByPlayer.TryGetValue(id, out var trail) || trail == null || trail.line == null)
                {
                    trail = CreateTrail(map, id);
                    _trailsByPlayer[id] = trail;
                }

                var c = player.Data.Color;
                if (trail.color != c)
                {
                    trail.color = c;
                    trail.line.startColor = c;
                    trail.line.endColor = c;
                }

                AddTrailPoint(trail, pos, now);
            }
        }

        TrimAndRenderAll(now);
    }

    public static void ClearTrails()
    {
        foreach (var kvp in _trailsByPlayer)
        {
            var trail = kvp.Value;
            if (trail == null) continue;
            if (trail.line != null)
            {
                Object.Destroy(trail.line.gameObject);
            }
        }
        _trailsByPlayer.Clear();
        _nextTrailRecordTime = 0f;
    }

    private static TrailLine CreateTrail(MapBehaviour map, byte playerId)
    {
        var go = new GameObject($"MalumTrail_{playerId}");
        go.transform.SetParent(map.HerePoint.transform.parent, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 0;
        lr.widthMultiplier = 0.025f;
        lr.numCapVertices = 2;
        lr.numCornerVertices = 2;
        lr.material = map.HerePoint.material;

        var t = new TrailLine();
        t.playerId = playerId;
        t.line = lr;
        t.start = 0;
        t.count = 0;
        t.lastRenderedCount = 0;
        t.color = Color.white;
        return t;
    }

    private static void AddTrailPoint(TrailLine trail, Vector3 pos, float now)
    {
        if (trail == null) return;

        if (trail.count > 0)
        {
            var lastIndex = (trail.start + trail.count - 1) % TrailMaxWaypoints;
            var lastPos = trail.positions[lastIndex];
            if (Vector3.Distance(lastPos, pos) < 0.02f) return;
        }

        var index = (trail.start + trail.count) % TrailMaxWaypoints;
        if (trail.count == TrailMaxWaypoints)
        {
            trail.start = (trail.start + 1) % TrailMaxWaypoints;
            index = (trail.start + trail.count - 1) % TrailMaxWaypoints;
            trail.positions[index] = pos;
            trail.times[index] = now;
        }
        else
        {
            trail.positions[index] = pos;
            trail.times[index] = now;
            trail.count++;
        }
    }

    private static void TrimAndRenderAll(float now)
    {
        var keepSeconds = trailSeconds;
        if (keepSeconds < 5f) keepSeconds = 5f;
        if (keepSeconds > 60f) keepSeconds = 60f;

        foreach (var kvp in _trailsByPlayer)
        {
            var trail = kvp.Value;
            if (trail == null || trail.line == null) continue;

            TrimTrail(trail, now, keepSeconds);
            RenderTrail(trail);
        }
    }

    private static void TrimTrail(TrailLine trail, float now, float keepSeconds)
    {
        while (trail.count > 0)
        {
            var t = trail.times[trail.start];
            if (now - t <= keepSeconds) break;
            trail.start = (trail.start + 1) % TrailMaxWaypoints;
            trail.count--;
        }
    }

    private static void RenderTrail(TrailLine trail)
    {
        var count = trail.count;
        if (count <= 1)
        {
            if (trail.lastRenderedCount != 0)
            {
                trail.line.positionCount = 0;
                trail.lastRenderedCount = 0;
            }
            return;
        }

        for (var i = 0; i < count; i++)
        {
            var idx = (trail.start + i) % TrailMaxWaypoints;
            trail.temp[i] = trail.positions[idx];
        }

        if (trail.lastRenderedCount != count)
        {
            trail.line.positionCount = count;
            trail.lastRenderedCount = count;
        }

        for (var i = 0; i < count; i++)
        {
            trail.line.SetPosition(i, trail.temp[i]);
        }
    }
}
