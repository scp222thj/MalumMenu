using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MalumMenu;

public sealed class Radar : MonoBehaviour
{
    public static float scale = 0.35f;
    public static Vector2 anchoredPosition = new Vector2(320f, 180f);

    private const float MinScale = 0.15f;
    private const float MaxScale = 0.75f;
    private const float BaseSizeAtDefaultScale = 300f;
    private const float DefaultScale = 0.35f;
    private const float IconSize = 8f;
    private const float HighlightSize = 12f;
    private const float TrailWidth = 2f;
    private const int TrailMaxWaypoints = 32;
    private const float TrailWaypointIntervalSeconds = 1f;

    private sealed class PlayerUi
    {
        public byte id;
        public RawImage highlight;
        public RawImage dot;
        public Trail trail;
    }

    private sealed class Trail
    {
        public Vector2[] points = new Vector2[TrailMaxWaypoints];
        public int start;
        public int count;
        public float nextRecordTime;
        public readonly List<RawImage> segments = new List<RawImage>(TrailMaxWaypoints);
        public Color color;
    }

    private readonly Dictionary<byte, PlayerUi> _uiByPlayer = new Dictionary<byte, PlayerUi>(16);
    private readonly List<byte> _tmpRemove = new List<byte>(16);

    private Canvas _canvas;
    private CanvasScaler _scaler;
    private RectTransform _window;
    private RawImage _background;
    private RectTransform _iconsRoot;
    private RectTransform _trailsRoot;
    private RectTransform _borderRoot;

    private bool _dragging;
    private Vector2 _dragStartMouse;
    private Vector2 _dragStartAnchored;
    private float _nextSaveTime;
    private Vector2 _lastSavedAnchored;
    private float _lastSavedScale;

    private MapBehaviour _template;
    private Texture _bgTex;
    private Rect _bgUv;
    private Vector2 _bgExtents;
    private float _nextTemplateScanTime;

    private void Update()
    {
        if (!CheatToggles.minimapAlwaysOn || MalumMenu.isPanicked)
        {
            SetVisible(false);
            _dragging = false;
            return;
        }

        EnsureUi();
        SetVisible(true);

        var s = Mathf.Clamp(scale, MinScale, MaxScale);
        scale = s;

        var size = BaseSizeAtDefaultScale * (s / DefaultScale);
        if (size < 64f) size = 64f;

        _window.sizeDelta = new Vector2(size, size);
        _window.anchoredPosition = anchoredPosition;
        _borderRoot.gameObject.SetActive(MenuUI.isGUIActive);

        if (MenuUI.isGUIActive)
        {
            HandleDrag();
        }
        else
        {
            _dragging = false;
        }

        if (Time.unscaledTime >= _nextTemplateScanTime)
        {
            _nextTemplateScanTime = Time.unscaledTime + 1f;
            RefreshTemplate();
        }

        UpdateBackground();
        UpdatePlayers();
        UpdateTrails();
        MaybeSaveWindow();
    }

    private void EnsureUi()
    {
        if (_canvas != null) return;

        var root = new GameObject("MalumRadarCanvas");
        Object.DontDestroyOnLoad(root);

        _canvas = root.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 1000;

        _scaler = root.AddComponent<CanvasScaler>();
        _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _scaler.referenceResolution = new Vector2(1920f, 1080f);

        root.AddComponent<GraphicRaycaster>();

        var windowGo = new GameObject("RadarWindow");
        _window = windowGo.AddComponent<RectTransform>();
        _window.SetParent(_canvas.transform, false);
        _window.anchorMin = new Vector2(0.5f, 0.5f);
        _window.anchorMax = new Vector2(0.5f, 0.5f);
        _window.pivot = new Vector2(0.5f, 0.5f);
        _window.anchoredPosition = anchoredPosition;

        var bgGo = new GameObject("Background");
        var bgRt = bgGo.AddComponent<RectTransform>();
        bgRt.SetParent(_window, false);
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.pivot = new Vector2(0.5f, 0.5f);
        bgRt.anchoredPosition = Vector2.zero;
        bgRt.sizeDelta = Vector2.zero;
        _background = bgGo.AddComponent<RawImage>();
        _background.raycastTarget = false;
        _background.color = new Color(0f, 0f, 0f, 0.35f);

        var borderGo = new GameObject("Border");
        _borderRoot = borderGo.AddComponent<RectTransform>();
        _borderRoot.SetParent(_window, false);
        _borderRoot.anchorMin = Vector2.zero;
        _borderRoot.anchorMax = Vector2.one;
        _borderRoot.pivot = new Vector2(0.5f, 0.5f);
        _borderRoot.anchoredPosition = Vector2.zero;
        _borderRoot.sizeDelta = Vector2.zero;
        CreateBorderEdges(_borderRoot);

        var iconsGo = new GameObject("Icons");
        _iconsRoot = iconsGo.AddComponent<RectTransform>();
        _iconsRoot.SetParent(_window, false);
        _iconsRoot.anchorMin = Vector2.zero;
        _iconsRoot.anchorMax = Vector2.one;
        _iconsRoot.pivot = new Vector2(0.5f, 0.5f);
        _iconsRoot.anchoredPosition = Vector2.zero;
        _iconsRoot.sizeDelta = Vector2.zero;

        var trailsGo = new GameObject("Trails");
        _trailsRoot = trailsGo.AddComponent<RectTransform>();
        _trailsRoot.SetParent(_window, false);
        _trailsRoot.anchorMin = Vector2.zero;
        _trailsRoot.anchorMax = Vector2.one;
        _trailsRoot.pivot = new Vector2(0.5f, 0.5f);
        _trailsRoot.anchoredPosition = Vector2.zero;
        _trailsRoot.sizeDelta = Vector2.zero;
    }

    private void SetVisible(bool visible)
    {
        if (_canvas == null) return;
        if (_canvas.gameObject.activeSelf == visible) return;
        _canvas.gameObject.SetActive(visible);
    }

    private static void CreateBorderEdges(RectTransform parent)
    {
        CreateEdge(parent, "Top", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, 2f));
        CreateEdge(parent, "Bottom", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 2f));
        CreateEdge(parent, "Left", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Vector2(2f, 0f));
        CreateEdge(parent, "Right", new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f), new Vector2(2f, 0f));
    }

    private static void CreateEdge(RectTransform parent, string name, Vector2 aMin, Vector2 aMax, Vector2 pivot, Vector2 sizeDelta)
    {
        var go = new GameObject(name);
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.pivot = pivot;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = sizeDelta;
        var img = go.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.25f);
        img.raycastTarget = false;
    }

    private void HandleDrag()
    {
        if (_window == null) return;

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
            return;
        }

        if (!_dragging)
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(_window, Input.mousePosition, null)) return;
            _dragging = true;
            _dragStartMouse = Input.mousePosition;
            _dragStartAnchored = anchoredPosition;
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            _dragging = false;
            return;
        }

        var sf = _scaler != null ? _scaler.scaleFactor : 1f;
        if (sf < 0.0001f) sf = 1f;
        var delta = (Vector2)Input.mousePosition - _dragStartMouse;
        anchoredPosition = _dragStartAnchored + (delta / sf);
    }

    private void RefreshTemplate()
    {
        var template = MapBehaviour.Instance;
        if (template == null)
        {
            try { template = Object.FindObjectOfType<MapBehaviour>(true); } catch { template = null; }
        }

        if (template == null)
        {
            _template = null;
            _bgTex = null;
            _bgUv = default;
            _bgExtents = default;
            return;
        }

        if (_template == template) return;
        _template = template;

        var sr = ResolveBackgroundRenderer(template);
        if (sr == null || sr.sprite == null || sr.sprite.texture == null)
        {
            _bgTex = null;
            _bgUv = default;
            _bgExtents = default;
            return;
        }

        _bgTex = sr.sprite.texture;
        var tr = sr.sprite.textureRect;
        var texW = sr.sprite.texture.width;
        var texH = sr.sprite.texture.height;
        _bgUv = new Rect(tr.x / texW, tr.y / texH, tr.width / texW, tr.height / texH);
        _bgExtents = sr.sprite.bounds.extents;

        if (CheatToggles.debugMinimap && MalumMenu.Log != null)
        {
            try
            {
                MalumMenu.Log.LogInfo($"Radar: bg tex={texW}x{texH} uv=({_bgUv.x:0.000},{_bgUv.y:0.000},{_bgUv.width:0.000},{_bgUv.height:0.000}) ext=({_bgExtents.x:0.00},{_bgExtents.y:0.00})");
            }
            catch
            {
            }
        }
    }

    private static SpriteRenderer ResolveBackgroundRenderer(MapBehaviour template)
    {
        if (template == null) return null;

        SpriteRenderer best = null;
        var bestScore = 0f;

        SpriteRenderer[] renderers = null;
        try { renderers = template.GetComponentsInChildren<SpriteRenderer>(true); } catch { }
        if (renderers == null) return null;

        for (var i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (r == null) continue;
            var s = r.sprite;
            if (s == null) continue;
            var tex = s.texture;
            if (tex == null) continue;
            var score = tex.width * tex.height;
            if (score <= bestScore) continue;
            bestScore = score;
            best = r;
        }

        return best;
    }

    private void UpdateBackground()
    {
        if (_background == null) return;

        if (_bgTex != null)
        {
            _background.texture = _bgTex;
            _background.uvRect = _bgUv;
            _background.color = new Color(1f, 1f, 1f, 0.95f);
        }
        else
        {
            _background.texture = null;
            _background.uvRect = new Rect(0f, 0f, 1f, 1f);
            _background.color = new Color(0f, 0f, 0f, 0.35f);
        }
    }

    private void UpdatePlayers()
    {
        if (_iconsRoot == null) return;

        _tmpRemove.Clear();
        foreach (var kvp in _uiByPlayer)
        {
            _tmpRemove.Add(kvp.Key);
        }

        var players = PlayerControl.AllPlayerControls;
        if (players != null)
        {
            for (var i = 0; i < players.Count; i++)
            {
                var p = players[i];
                if (p == null || p.Data == null) continue;

                var id = p.PlayerId;
                RemoveTmp(id);

                if (!_uiByPlayer.TryGetValue(id, out var ui) || ui == null)
                {
                    ui = CreatePlayerUi(id);
                    _uiByPlayer[id] = ui;
                }

                UpdatePlayerUi(p, ui);
            }
        }

        for (var i = 0; i < _tmpRemove.Count; i++)
        {
            var id = _tmpRemove[i];
            if (_uiByPlayer.TryGetValue(id, out var ui) && ui != null)
            {
                DestroyPlayerUi(ui);
            }
            _uiByPlayer.Remove(id);
        }
    }

    private void RemoveTmp(byte id)
    {
        for (var i = 0; i < _tmpRemove.Count; i++)
        {
            if (_tmpRemove[i] != id) continue;
            _tmpRemove.RemoveAt(i);
            return;
        }
    }

    private PlayerUi CreatePlayerUi(byte id)
    {
        var ui = new PlayerUi();
        ui.id = id;
        ui.trail = new Trail();

        ui.highlight = CreateDot(_iconsRoot, "Highlight", HighlightSize);
        ui.dot = CreateDot(_iconsRoot, "Dot", IconSize);

        ui.highlight.color = new Color(1f, 0f, 0f, 0.85f);
        ui.dot.color = Color.white;
        ui.highlight.enabled = false;
        ui.dot.enabled = false;

        return ui;
    }

    private static RawImage CreateDot(RectTransform parent, string name, float size)
    {
        var go = new GameObject(name);
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(size, size);
        rt.anchoredPosition = Vector2.zero;

        var img = go.AddComponent<RawImage>();
        img.texture = Texture2D.whiteTexture;
        img.raycastTarget = false;
        return img;
    }

    private static void DestroyPlayerUi(PlayerUi ui)
    {
        if (ui == null) return;
        if (ui.highlight != null) { try { Object.Destroy(ui.highlight.gameObject); } catch { } }
        if (ui.dot != null) { try { Object.Destroy(ui.dot.gameObject); } catch { } }
        if (ui.trail != null)
        {
            for (var i = 0; i < ui.trail.segments.Count; i++)
            {
                var seg = ui.trail.segments[i];
                if (seg == null) continue;
                try { Object.Destroy(seg.gameObject); } catch { }
            }
            ui.trail.segments.Clear();
        }
    }

    private void UpdatePlayerUi(PlayerControl p, PlayerUi ui)
    {
        if (p == null || p.Data == null) return;
        if (ui == null) return;

        var isImp = p.Data.Role != null && p.Data.Role.IsImpostor;
        var isDead = p.Data.IsDead;

        var show = false;
        if (isDead) show = CheatToggles.mapGhosts;
        else if (isImp) show = CheatToggles.mapImps;
        else show = CheatToggles.mapCrew;

        if (!show)
        {
            if (ui.dot != null) ui.dot.enabled = false;
            if (ui.highlight != null) ui.highlight.enabled = false;
            return;
        }

        var ext = _bgExtents;
        if (ext.x <= 0.0001f || ext.y <= 0.0001f) ext = new Vector2(6f, 6f);

        if (!Utils.isShip || ShipStatus.Instance == null)
        {
            if (ui.dot != null) ui.dot.enabled = false;
            if (ui.highlight != null) ui.highlight.enabled = false;
            return;
        }

        var pos = p.transform.position;
        pos /= ShipStatus.Instance.MapScale;
        pos.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);

        var u = (pos.x / ext.x + 1f) * 0.5f;
        var v = (pos.y / ext.y + 1f) * 0.5f;

        var x = Mathf.Clamp01(u) * _window.rect.width;
        var y = (1f - Mathf.Clamp01(v)) * _window.rect.height;
        var anchored = new Vector2(x - _window.rect.width * 0.5f, y - _window.rect.height * 0.5f);

        var color = ResolvePlayerColor(p, isImp, isDead);
        if (ui.dot != null)
        {
            ui.dot.enabled = true;
            ui.dot.color = color;
            ui.dot.rectTransform.anchoredPosition = anchored;
        }

        if (ui.highlight != null)
        {
            var highlight = CheatToggles.mapImpsHighlight && isImp && !isDead;
            ui.highlight.enabled = highlight;
            if (highlight)
            {
                ui.highlight.rectTransform.anchoredPosition = anchored;
            }
        }

        if (ui.trail != null)
        {
            ui.trail.color = color;
        }
    }

    private static Color ResolvePlayerColor(PlayerControl p, bool isImp, bool isDead)
    {
        if (p == null || p.Data == null) return Color.white;

        if (isDead)
        {
            if (CheatToggles.colorBasedMap) return p.Data.Color;
            return Palette.White;
        }

        if (CheatToggles.colorBasedMap) return p.Data.Color;
        if (p.Data.Role != null) return p.Data.Role.TeamColor;
        if (isImp) return Color.red;
        return Color.cyan;
    }

    private void UpdateTrails()
    {
        if (_trailsRoot == null) return;

        if (!CheatToggles.mapTrails || Utils.isMeeting || !Utils.isShip || ShipStatus.Instance == null)
        {
            foreach (var kvp in _uiByPlayer)
            {
                var ui = kvp.Value;
                if (ui == null || ui.trail == null) continue;
                ui.trail.start = 0;
                ui.trail.count = 0;
                HideAllSegments(ui.trail);
            }
            return;
        }

        var ext = _bgExtents;
        if (ext.x <= 0.0001f || ext.y <= 0.0001f) ext = new Vector2(6f, 6f);

        var now = Time.time;

        var players = PlayerControl.AllPlayerControls;
        if (players != null)
        {
            for (var i = 0; i < players.Count; i++)
            {
                var p = players[i];
                if (p == null || p.Data == null) continue;
                if (!_uiByPlayer.TryGetValue(p.PlayerId, out var ui) || ui == null || ui.trail == null) continue;

                var isImp = p.Data.Role != null && p.Data.Role.IsImpostor;
                var isDead = p.Data.IsDead;

                var show = false;
                if (isDead) show = CheatToggles.mapGhosts;
                else if (isImp) show = CheatToggles.mapImps;
                else show = CheatToggles.mapCrew;
                if (!show) continue;

                if (now < ui.trail.nextRecordTime) continue;
                ui.trail.nextRecordTime = now + TrailWaypointIntervalSeconds;

                var pos = p.transform.position;
                pos /= ShipStatus.Instance.MapScale;
                pos.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);

                AddTrailPoint(ui.trail, new Vector2(pos.x, pos.y));
            }
        }

        foreach (var kvp in _uiByPlayer)
        {
            var ui = kvp.Value;
            if (ui == null || ui.trail == null) continue;
            RenderTrail(ui.trail, ext);
        }
    }

    private static void AddTrailPoint(Trail trail, Vector2 pos)
    {
        if (trail.count > 0)
        {
            var lastIndex = (trail.start + trail.count - 1) % TrailMaxWaypoints;
            var lastPos = trail.points[lastIndex];
            if (Vector2.Distance(lastPos, pos) < 0.02f) return;
        }

        var idx = (trail.start + trail.count) % TrailMaxWaypoints;
        if (trail.count == TrailMaxWaypoints)
        {
            trail.start = (trail.start + 1) % TrailMaxWaypoints;
            idx = (trail.start + trail.count - 1) % TrailMaxWaypoints;
            trail.points[idx] = pos;
        }
        else
        {
            trail.points[idx] = pos;
            trail.count++;
        }
    }

    private void RenderTrail(Trail trail, Vector2 ext)
    {
        var count = trail.count;
        if (count <= 1)
        {
            HideAllSegments(trail);
            return;
        }

        var needed = count - 1;
        while (trail.segments.Count < needed)
        {
            var seg = CreateSegment();
            trail.segments.Add(seg);
        }

        for (var i = 0; i < trail.segments.Count; i++)
        {
            trail.segments[i].enabled = i < needed;
        }

        var w = _window.rect.width;
        var h = _window.rect.height;

        for (var i = 0; i < needed; i++)
        {
            var aIdx = (trail.start + i) % TrailMaxWaypoints;
            var bIdx = (trail.start + i + 1) % TrailMaxWaypoints;
            var a = trail.points[aIdx];
            var b = trail.points[bIdx];

            var au = (a.x / ext.x + 1f) * 0.5f;
            var av = (a.y / ext.y + 1f) * 0.5f;
            var bu = (b.x / ext.x + 1f) * 0.5f;
            var bv = (b.y / ext.y + 1f) * 0.5f;

            var ax = Mathf.Clamp01(au) * w;
            var ay = (1f - Mathf.Clamp01(av)) * h;
            var bx = Mathf.Clamp01(bu) * w;
            var by = (1f - Mathf.Clamp01(bv)) * h;

            var pa = new Vector2(ax - w * 0.5f, ay - h * 0.5f);
            var pb = new Vector2(bx - w * 0.5f, by - h * 0.5f);

            var d = pb - pa;
            var len = d.magnitude;
            if (len < 0.001f) continue;

            var mid = (pa + pb) * 0.5f;
            var angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;

            var seg = trail.segments[i];
            seg.color = new Color(trail.color.r, trail.color.g, trail.color.b, 0.85f);
            var rt = seg.rectTransform;
            rt.anchoredPosition = mid;
            rt.sizeDelta = new Vector2(len, TrailWidth);
            rt.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private RawImage CreateSegment()
    {
        var go = new GameObject("Seg");
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(_trailsRoot, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(10f, TrailWidth);

        var img = go.AddComponent<RawImage>();
        img.texture = Texture2D.whiteTexture;
        img.raycastTarget = false;
        img.enabled = false;
        return img;
    }

    private static void HideAllSegments(Trail trail)
    {
        for (var i = 0; i < trail.segments.Count; i++)
        {
            if (trail.segments[i] != null) trail.segments[i].enabled = false;
        }
    }

    private void MaybeSaveWindow()
    {
        var now = Time.unscaledTime;
        if (now < _nextSaveTime) return;
        _nextSaveTime = now + 0.25f;

        var s = Mathf.Clamp(scale, MinScale, MaxScale);
        var pos = anchoredPosition;

        var posChanged = Vector2.Distance(pos, _lastSavedAnchored) > 0.01f;
        var scaleChanged = Mathf.Abs(s - _lastSavedScale) > 0.0001f;
        if (!posChanged && !scaleChanged) return;

        _lastSavedAnchored = pos;
        _lastSavedScale = s;

        if (MalumMenu.Plugin == null) return;

        try
        {
            MalumMenu.minimapScale.Value = s;
            MalumMenu.minimapPosX.Value = pos.x;
            MalumMenu.minimapPosY.Value = pos.y;
            MalumMenu.Plugin.Config.Save();
        }
        catch
        {
        }
    }
}
