using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

public static class FootprintHandler
{
    // --- Footprints ---
    private const float FootprintInterval = 0.5f;
    private const float FootprintFade     = 8f;
    private const int   MaxPerPlayer      = 20;

    private struct Footprint { public Vector3 World; public float Born; public Color Color; }
    private static readonly Dictionary<byte, List<Footprint>> _prints     = new();
    private static readonly Dictionary<byte, float>           _lastSample = new();
    private static Texture2D _dot;

    // --- Player Trail (LocalPlayer only) ---
    private const int   TrailMax      = 50;
    private const float TrailInterval = 0.04f;
    private static GameObject    _trailObj;
    private static LineRenderer  _trailLr;
    private static readonly List<Vector3> _trailPts = new();
    private static float _lastTrailT;

    // --- Kill Range Indicator ---
    private const int CircleSegs = 48;
    private static GameObject   _rangeObj;
    private static LineRenderer _rangeLr;

    public static void Update()
    {
        if (!Utils.isShip) { Clear(); return; }
        float now = Time.time;

        // Footprints
        if (CheatToggles.footprints)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == null || player.Data == null || player.Data.IsDead) continue;
                byte pid = player.PlayerId;
                if (!_lastSample.TryGetValue(pid, out float last) || now - last >= FootprintInterval)
                {
                    _lastSample[pid] = now;
                    if (!_prints.TryGetValue(pid, out var list)) { list = new(); _prints[pid] = list; }
                    list.Add(new Footprint { World = player.GetTruePosition(), Born = now, Color = player.Data.Color });
                    if (list.Count > MaxPerPlayer) list.RemoveAt(0);
                }
            }
        }

        // Player Trail
        if (CheatToggles.playerTrail && PlayerControl.LocalPlayer != null)
        {
            if (_trailObj == null && DestroyableSingleton<HatManager>.Instance != null)
            {
                _trailObj = new GameObject("MM_Trail");
                _trailLr  = _trailObj.AddComponent<LineRenderer>();
                _trailLr.useWorldSpace = true;
                _trailLr.material      = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;
                _trailLr.SetWidth(0.08f, 0.01f);
                _trailPts.Clear();
            }

            if (now - _lastTrailT >= TrailInterval)
            {
                _lastTrailT = now;
                _trailPts.Add(PlayerControl.LocalPlayer.GetTruePosition());
                if (_trailPts.Count > TrailMax) _trailPts.RemoveAt(0);
            }

            _trailLr.positionCount = _trailPts.Count;
            var c = PlayerControl.LocalPlayer.Data?.Color ?? Color.white;
            _trailLr.startColor = new Color(c.r, c.g, c.b, 0.9f);
            _trailLr.endColor   = new Color(c.r, c.g, c.b, 0f);
            for (int i = 0; i < _trailPts.Count; i++) _trailLr.SetPosition(i, _trailPts[i]);
        }
        else if (_trailObj != null)
        {
            Object.Destroy(_trailObj); _trailObj = null; _trailPts.Clear();
        }

        // Kill Range Indicator
        if (CheatToggles.killRangeIndicator && Utils.isInGame && PlayerControl.LocalPlayer != null)
        {
            if (_rangeObj == null && DestroyableSingleton<HatManager>.Instance != null)
            {
                _rangeObj = new GameObject("MM_KillRange");
                _rangeLr  = _rangeObj.AddComponent<LineRenderer>();
                _rangeLr.useWorldSpace = true;
                _rangeLr.material      = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;
                _rangeLr.SetWidth(0.04f, 0.04f);
                _rangeLr.loop = true;
                _rangeLr.positionCount = CircleSegs;
            }

            float radius = GameManager.Instance.LogicOptions.GetKillDistance();
            var   center = (Vector2)PlayerControl.LocalPlayer.transform.position;
            for (int i = 0; i < CircleSegs; i++)
            {
                float a = 2f * Mathf.PI * i / CircleSegs;
                _rangeLr.SetPosition(i, new Vector3(center.x + Mathf.Cos(a) * radius, center.y + Mathf.Sin(a) * radius, -1f));
            }
            var rc = new Color(1f, 0.2f, 0.2f, 0.55f);
            _rangeLr.startColor = rc;
            _rangeLr.endColor   = rc;
        }
        else if (_rangeObj != null)
        {
            Object.Destroy(_rangeObj); _rangeObj = null;
        }
    }

    // Called from OnGUI to draw footprint dots
    public static void DrawGUI()
    {
        if (!CheatToggles.footprints || CheatToggles.streamerMode || Camera.main == null) return;

        if (_dot == null)
        {
            _dot = new Texture2D(1, 1);
            _dot.SetPixel(0, 0, Color.white);
            _dot.Apply();
        }

        float now = Time.time;
        foreach (var kvp in _prints)
        {
            foreach (var fp in kvp.Value)
            {
                float age = now - fp.Born;
                if (age > FootprintFade) continue;
                var sp = Camera.main.WorldToScreenPoint(fp.World);
                if (sp.z < 0f) continue;
                float alpha = (1f - age / FootprintFade) * 0.8f;
                GUI.color = new Color(fp.Color.r, fp.Color.g, fp.Color.b, alpha);
                GUI.DrawTexture(new Rect(sp.x - 5f, Screen.height - sp.y - 5f, 10f, 10f), _dot);
            }
        }
        GUI.color = Color.white;
    }

    public static void Clear()
    {
        _prints.Clear();
        _lastSample.Clear();
        _trailPts.Clear();
        _lastTrailT = 0f;
        if (_trailObj != null) { Object.Destroy(_trailObj); _trailObj = null; _trailLr = null; }
        if (_rangeObj != null) { Object.Destroy(_rangeObj); _rangeObj = null; _rangeLr = null; }
    }
}
