using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;
public static class MinimapHandler
{
    public static bool minimapActive;
    public static List<HerePoint> herePoints = new List<HerePoint>();
    public static List<HerePoint> herePointsToRemove = new List<HerePoint>();
    public static float trailSeconds = 30f;

    private const float TrailWaypointIntervalSeconds = 0.25f;
    private const int TrailMaxWaypoints = 256;
    private static float _nextTrailRecordTime;

    private sealed class TrailLine
    {
        public byte playerId;
        public LineRenderer lineMap;
        public LineRenderer lineWindow;
        public Vector3[] positions = new Vector3[TrailMaxWaypoints];
        public float[] times = new float[TrailMaxWaypoints];
        public int start;
        public int count;
        public Vector3[] temp = new Vector3[TrailMaxWaypoints];
        public int lastRenderedCountMap;
        public int lastRenderedCountWindow;
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
        if (map == null) return;
        RecordTrails();
        RenderTrails(map);
    }

    public static void RecordTrails()
    {
        if (!CheatToggles.mapTrails) return;
        if (!Utils.isShip) return;
        if (ShipStatus.Instance == null) return;
        if (Utils.isMeeting) { ClearTrails(); return; }

        var now = Time.time;
        if (now < _nextTrailRecordTime) return;
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
            if (!_trailsByPlayer.TryGetValue(id, out var trail) || trail == null)
            {
                trail = CreateTrail(id);
                _trailsByPlayer[id] = trail;
            }

            var c = player.Data.Color;
            if (trail.color != c)
            {
                trail.color = c;
                if (trail.lineMap != null)
                {
                    trail.lineMap.startColor = c;
                    trail.lineMap.endColor = c;
                }
                if (trail.lineWindow != null)
                {
                    trail.lineWindow.startColor = c;
                    trail.lineWindow.endColor = c;
                }
            }

            AddTrailPoint(trail, pos, now);
        }

        TrimAll(now);
    }

    private static void RenderTrails(MapBehaviour map)
    {
        if (!CheatToggles.mapTrails) return;
        if (!Utils.isShip) return;
        if (ShipStatus.Instance == null) return;
        if (Utils.isMeeting) return;

        var now = Time.time;
        var keepSeconds = trailSeconds;
        if (keepSeconds < 5f) keepSeconds = 5f;
        if (keepSeconds > 60f) keepSeconds = 60f;

        foreach (var kvp in _trailsByPlayer)
        {
            var trail = kvp.Value;
            if (trail == null) continue;

            if (trail.count > 1 && trail.lineMap == null)
            {
                AttachTrailRenderer(map, trail);
            }

            if (trail.lineMap == null) continue;

            TrimTrail(trail, now, keepSeconds);
            RenderTrail(trail, false);
        }
    }

    public static void RenderTrailsWindow(Transform parent, Material material)
    {
        if (!CheatToggles.mapTrails) return;
        if (parent == null) return;
        if (!Utils.isShip) return;
        if (ShipStatus.Instance == null) return;
        if (Utils.isMeeting) return;

        var now = Time.time;
        var keepSeconds = trailSeconds;
        if (keepSeconds < 5f) keepSeconds = 5f;
        if (keepSeconds > 60f) keepSeconds = 60f;

        foreach (var kvp in _trailsByPlayer)
        {
            var trail = kvp.Value;
            if (trail == null) continue;

            if (trail.count > 1 && trail.lineWindow == null)
            {
                AttachTrailRenderer(parent, material, trail, true);
            }

            if (trail.lineWindow == null) continue;

            TrimTrail(trail, now, keepSeconds);
            RenderTrail(trail, true);
        }
    }

    public static void ClearTrails()
    {
        foreach (var kvp in _trailsByPlayer)
        {
            var trail = kvp.Value;
            if (trail == null) continue;
            if (trail.lineMap != null)
            {
                Object.Destroy(trail.lineMap.gameObject);
            }
            if (trail.lineWindow != null)
            {
                Object.Destroy(trail.lineWindow.gameObject);
            }
        }
        _trailsByPlayer.Clear();
        _nextTrailRecordTime = 0f;
    }

    public static void DetachTrailRenderers()
    {
        foreach (var kvp in _trailsByPlayer)
        {
            var trail = kvp.Value;
            if (trail == null) continue;
            if (trail.lineMap != null)
            {
                Object.Destroy(trail.lineMap.gameObject);
                trail.lineMap = null;
                trail.lastRenderedCountMap = 0;
            }
            if (trail.lineWindow != null)
            {
                Object.Destroy(trail.lineWindow.gameObject);
                trail.lineWindow = null;
                trail.lastRenderedCountWindow = 0;
            }
        }
    }

    private static TrailLine CreateTrail(byte playerId)
    {
        var t = new TrailLine();
        t.playerId = playerId;
        t.lineMap = null;
        t.lineWindow = null;
        t.start = 0;
        t.count = 0;
        t.lastRenderedCountMap = 0;
        t.lastRenderedCountWindow = 0;
        t.color = Color.white;
        return t;
    }

    private static void AttachTrailRenderer(MapBehaviour map, TrailLine trail)
    {
        if (map == null) return;
        if (trail == null) return;
        if (trail.lineMap != null) return;

        var go = new GameObject($"MalumTrail_{trail.playerId}");
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

        lr.startColor = trail.color;
        lr.endColor = trail.color;

        trail.lineMap = lr;
    }

    private static void AttachTrailRenderer(Transform parent, Material material, TrailLine trail, bool isWindow)
    {
        if (parent == null) return;
        if (trail == null) return;
        if (isWindow)
        {
            if (trail.lineWindow != null) return;
        }
        else
        {
            if (trail.lineMap != null) return;
        }

        var go = new GameObject($"MalumTrail_{trail.playerId}");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 0;
        lr.widthMultiplier = 0.025f;
        lr.numCapVertices = 2;
        lr.numCornerVertices = 2;
        if (material != null) lr.material = material;

        lr.startColor = trail.color;
        lr.endColor = trail.color;

        if (isWindow)
        {
            trail.lineWindow = lr;
        }
        else
        {
            trail.lineMap = lr;
        }
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
        RenderTrails(MapBehaviour.Instance);
    }

    private static void TrimAll(float now)
    {
        var keepSeconds = trailSeconds;
        if (keepSeconds < 5f) keepSeconds = 5f;
        if (keepSeconds > 60f) keepSeconds = 60f;

        foreach (var kvp in _trailsByPlayer)
        {
            var trail = kvp.Value;
            if (trail == null) continue;
            TrimTrail(trail, now, keepSeconds);
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

    private static void RenderTrail(TrailLine trail, bool isWindow)
    {
        var count = trail.count;
        var lr = isWindow ? trail.lineWindow : trail.lineMap;
        if (lr == null) return;
        var lastCount = isWindow ? trail.lastRenderedCountWindow : trail.lastRenderedCountMap;
        if (count <= 1)
        {
            if (lastCount != 0)
            {
                lr.positionCount = 0;
                lastCount = 0;
            }
            if (isWindow) trail.lastRenderedCountWindow = lastCount;
            else trail.lastRenderedCountMap = lastCount;
            return;
        }

        for (var i = 0; i < count; i++)
        {
            var idx = (trail.start + i) % TrailMaxWaypoints;
            trail.temp[i] = trail.positions[idx];
        }

        if (lastCount != count)
        {
            lr.positionCount = count;
            lastCount = count;
        }

        for (var i = 0; i < count; i++)
        {
            lr.SetPosition(i, trail.temp[i]);
        }

        if (isWindow) trail.lastRenderedCountWindow = lastCount;
        else trail.lastRenderedCountMap = lastCount;
    }

    public static void DrawTrailsGUI(Rect rect, Vector2 extents)
    {
        if (!CheatToggles.mapTrails) return;
        if (!Utils.isShip) return;
        if (ShipStatus.Instance == null) return;
        if (Utils.isMeeting) return;

        RecordTrails();

        var now = Time.time;
        var keepSeconds = trailSeconds;
        if (keepSeconds < 5f) keepSeconds = 5f;
        if (keepSeconds > 60f) keepSeconds = 60f;

        var extX = extents.x;
        var extY = extents.y;
        if (extX < 0.0001f) extX = 6f;
        if (extY < 0.0001f) extY = 6f;

        foreach (var kvp in _trailsByPlayer)
        {
            var trail = kvp.Value;
            if (trail == null) continue;

            TrimTrail(trail, now, keepSeconds);
            var count = trail.count;
            if (count <= 1) continue;

            var prev = Vector2.zero;
            var hasPrev = false;

            for (var i = 0; i < count; i++)
            {
                var idx = (trail.start + i) % TrailMaxWaypoints;
                var p = trail.positions[idx];

                var u = (p.x / extX + 1f) * 0.5f;
                var v = (p.y / extY + 1f) * 0.5f;

                var px = rect.x + Mathf.Clamp01(u) * rect.width;
                var py = rect.y + (1f - Mathf.Clamp01(v)) * rect.height;
                var cur = new Vector2(px, py);

                if (hasPrev)
                {
                    DrawLineGUI(prev, cur, trail.color, 1.5f);
                }

                prev = cur;
                hasPrev = true;
            }
        }
    }

    private static void DrawLineGUI(Vector2 a, Vector2 b, Color color, float width)
    {
        var delta = b - a;
        var len = delta.magnitude;
        if (len < 0.001f) return;

        var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        var savedColor = GUI.color;
        var savedMatrix = GUI.matrix;

        GUI.color = color;
        GUIUtility.RotateAroundPivot(angle, a);
        GUI.DrawTexture(new Rect(a.x, a.y - width * 0.5f, len, width), Texture2D.whiteTexture);
        GUI.matrix = savedMatrix;
        GUI.color = savedColor;
    }
}
