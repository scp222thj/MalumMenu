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
    private const float IconSize = 14f;
    private const float HighlightSize = 18f;
    private const float TrailWidth = 2f;
    private const int TrailMaxWaypoints = 256;
    private const float TrailWaypointIntervalSeconds = 0.25f;
    private const int TrailMaxSegments = 64;
    private const float TrailAlphaStart = 0.85f;
    private const float TrailAlphaEnd = 0.60f;

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
        public float[] times = new float[TrailMaxWaypoints];
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
    private RectTransform _contentRoot;
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
    private Texture _iconTex;
    private Rect _iconUv;
    private Material _iconMatBase;
    private float _contentAspect = 1f;

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
        UpdateContentRect();

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

        var contentGo = new GameObject("Content");
        _contentRoot = contentGo.AddComponent<RectTransform>();
        _contentRoot.SetParent(_window, false);
        _contentRoot.anchorMin = new Vector2(0.5f, 0.5f);
        _contentRoot.anchorMax = new Vector2(0.5f, 0.5f);
        _contentRoot.pivot = new Vector2(0.5f, 0.5f);
        _contentRoot.anchoredPosition = Vector2.zero;
        _contentRoot.sizeDelta = _window.sizeDelta;

        var bgGo = new GameObject("Background");
        var bgRt = bgGo.AddComponent<RectTransform>();
        bgRt.SetParent(_contentRoot, false);
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
        _iconsRoot.SetParent(_contentRoot, false);
        _iconsRoot.anchorMin = Vector2.zero;
        _iconsRoot.anchorMax = Vector2.one;
        _iconsRoot.pivot = new Vector2(0.5f, 0.5f);
        _iconsRoot.anchoredPosition = Vector2.zero;
        _iconsRoot.sizeDelta = Vector2.zero;

        var trailsGo = new GameObject("Trails");
        _trailsRoot = trailsGo.AddComponent<RectTransform>();
        _trailsRoot.SetParent(_contentRoot, false);
        _trailsRoot.anchorMin = Vector2.zero;
        _trailsRoot.anchorMax = Vector2.one;
        _trailsRoot.pivot = new Vector2(0.5f, 0.5f);
        _trailsRoot.anchoredPosition = Vector2.zero;
        _trailsRoot.sizeDelta = Vector2.zero;

        _trailsRoot.SetSiblingIndex(1);
        _iconsRoot.SetSiblingIndex(2);
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
            _contentAspect = 1f;
            _iconTex = null;
            _iconUv = default;
            _iconMatBase = null;
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
            _contentAspect = 1f;
            _iconTex = null;
            _iconUv = default;
            _iconMatBase = null;
            return;
        }

        _bgTex = sr.sprite.texture;
        var tr = sr.sprite.textureRect;
        var texW = sr.sprite.texture.width;
        var texH = sr.sprite.texture.height;
        _bgUv = new Rect(tr.x / texW, tr.y / texH, tr.width / texW, tr.height / texH);
        _bgExtents = sr.sprite.bounds.extents;
        _contentAspect = tr.height > 0.0001f ? (tr.width / tr.height) : 1f;

        if (template.HerePoint != null && template.HerePoint.sprite != null && template.HerePoint.sprite.texture != null)
        {
            var iconSprite = template.HerePoint.sprite;
            _iconTex = iconSprite.texture;
            var itr = iconSprite.textureRect;
            var itw = iconSprite.texture.width;
            var ith = iconSprite.texture.height;
            _iconUv = new Rect(itr.x / itw, itr.y / ith, itr.width / itw, itr.height / ith);
            _iconMatBase = template.HerePoint.material;
        }
        else
        {
            _iconTex = null;
            _iconUv = default;
            _iconMatBase = null;
        }

        if (CheatToggles.debugMinimap && MalumMenu.Log != null)
        {
            try
            {
                var iconOk = _iconTex != null && _iconMatBase != null;
                MalumMenu.Log.LogInfo($"Radar: bg tex={texW}x{texH} uv=({_bgUv.x:0.000},{_bgUv.y:0.000},{_bgUv.width:0.000},{_bgUv.height:0.000}) ext=({_bgExtents.x:0.00},{_bgExtents.y:0.00}) aspect={_contentAspect:0.000} icon={iconOk}");
            }
            catch
            {
            }
        }
    }

    private static SpriteRenderer ResolveBackgroundRenderer(MapBehaviour template)
    {
        if (template == null) return null;

        SpriteRenderer[] renderers = null;
        try { renderers = template.GetComponentsInChildren<SpriteRenderer>(true); } catch { }
        if (renderers == null) return null;

        var maxArea = 0f;
        for (var i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (r == null) continue;
            var s = r.sprite;
            if (s == null) continue;
            var tex = s.texture;
            if (tex == null) continue;
            var area = tex.width * tex.height;
            if (area > maxArea) maxArea = area;
        }

        if (maxArea <= 0f) return null;

        SpriteRenderer best = null;
        var bestOrder = int.MaxValue;
        var bestArea = 0f;

        for (var i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (r == null) continue;
            var s = r.sprite;
            if (s == null) continue;
            var tex = s.texture;
            if (tex == null) continue;

            var area = tex.width * tex.height;
            if (area < maxArea * 0.5f) continue;

            var n = r.gameObject != null ? r.gameObject.name : "";
            var ln = n != null ? n.ToLowerInvariant() : "";
            var isOverlay = ln.Contains("overlay") || ln.Contains("highlight") || ln.Contains("room");
            if (isOverlay && area < maxArea * 0.9f) continue;

            var order = r.sortingOrder;
            if (order < bestOrder)
            {
                best = r;
                bestOrder = order;
                bestArea = area;
                continue;
            }

            if (order != bestOrder) continue;

            if (area > bestArea)
            {
                best = r;
                bestArea = area;
            }
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

    private void UpdateContentRect()
    {
        if (_window == null) return;
        if (_contentRoot == null) return;

        var win = _window.rect;
        if (win.width <= 0.0001f || win.height <= 0.0001f) return;

        var aspect = _contentAspect;
        if (aspect < 0.0001f) aspect = 1f;

        var w = win.width;
        var h = win.height;

        var targetW = w;
        var targetH = w / aspect;
        if (targetH > h)
        {
            targetH = h;
            targetW = h * aspect;
        }

        _contentRoot.sizeDelta = new Vector2(targetW, targetH);
        _contentRoot.anchoredPosition = Vector2.zero;
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

        ui.dot.color = Color.white;

        if (_iconTex != null)
        {
            ui.highlight.texture = _iconTex;
            ui.dot.texture = _iconTex;
            ui.highlight.uvRect = _iconUv;
            ui.dot.uvRect = _iconUv;
        }

        if (_iconMatBase != null)
        {
            ui.highlight.material = Object.Instantiate(_iconMatBase);
            ui.dot.material = Object.Instantiate(_iconMatBase);
        }

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

        var rect = _contentRoot != null ? _contentRoot.rect : _window.rect;
        var w = rect.width;
        var h = rect.height;
        if (w <= 0.0001f || h <= 0.0001f) return;

        var x = Mathf.Clamp01(u) * w;
        var y = Mathf.Clamp01(v) * h;
        var anchored = new Vector2(x - w * 0.5f, y - h * 0.5f);

        var color = ResolvePlayerColor(p, isImp, isDead);
        if (ui.dot != null)
        {
            ui.dot.enabled = true;
            ui.dot.rectTransform.anchoredPosition = anchored;

            if (_iconTex != null && ui.dot.texture != _iconTex)
            {
                ui.dot.texture = _iconTex;
                ui.dot.uvRect = _iconUv;
            }
            if (_iconMatBase != null && ui.dot.material == null)
            {
                ui.dot.material = Object.Instantiate(_iconMatBase);
            }

            var mat = ui.dot.material;
            if (mat != null)
            {
                mat.SetColor(PlayerMaterial.BackColor, color);
                mat.SetColor(PlayerMaterial.BodyColor, color);
                mat.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);
            }
        }

        if (ui.highlight != null)
        {
            var highlight = CheatToggles.mapImpsHighlight && isImp && !isDead;
            ui.highlight.enabled = highlight;
            if (highlight)
            {
                ui.highlight.rectTransform.anchoredPosition = anchored;

                if (_iconTex != null && ui.highlight.texture != _iconTex)
                {
                    ui.highlight.texture = _iconTex;
                    ui.highlight.uvRect = _iconUv;
                }
                if (_iconMatBase != null && ui.highlight.material == null)
                {
                    ui.highlight.material = Object.Instantiate(_iconMatBase);
                }

                var hmat = ui.highlight.material;
                if (hmat != null)
                {
                    hmat.SetColor(PlayerMaterial.BackColor, Color.red);
                    hmat.SetColor(PlayerMaterial.BodyColor, Color.red);
                    hmat.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);
                }
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
        var keepSeconds = MinimapHandler.trailSeconds;
        if (keepSeconds < 5f) keepSeconds = 5f;
        if (keepSeconds > 60f) keepSeconds = 60f;

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

                AddTrailPoint(ui.trail, new Vector2(pos.x, pos.y), now);
            }
        }

        foreach (var kvp in _uiByPlayer)
        {
            var ui = kvp.Value;
            if (ui == null || ui.trail == null) continue;
            TrimTrail(ui.trail, now, keepSeconds);
            RenderTrail(ui.trail, ext, now, keepSeconds);
        }
    }

    private static void AddTrailPoint(Trail trail, Vector2 pos, float now)
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
            trail.times[idx] = now;
        }
        else
        {
            trail.points[idx] = pos;
            trail.times[idx] = now;
            trail.count++;
        }
    }

    private static void TrimTrail(Trail trail, float now, float keepSeconds)
    {
        if (trail == null) return;
        if (keepSeconds <= 0.001f) return;

        while (trail.count > 0)
        {
            var idx = trail.start;
            var age = now - trail.times[idx];
            if (age <= keepSeconds) break;
            trail.start = (trail.start + 1) % TrailMaxWaypoints;
            trail.count--;
        }
    }

    private void RenderTrail(Trail trail, Vector2 ext, float now, float keepSeconds)
    {
        var count = trail.count;
        if (count <= 1)
        {
            HideAllSegments(trail);
            return;
        }

        var rawSegments = count - 1;
        var needed = rawSegments;
        if (needed > TrailMaxSegments) needed = TrailMaxSegments;
        while (trail.segments.Count < needed)
        {
            var seg = CreateSegment();
            trail.segments.Add(seg);
        }

        for (var i = 0; i < trail.segments.Count; i++)
        {
            trail.segments[i].enabled = i < needed;
        }

        var rect = _contentRoot != null ? _contentRoot.rect : _window.rect;
        var w = rect.width;
        var h = rect.height;
        if (w <= 0.0001f || h <= 0.0001f) return;

        var step = 1;
        if (rawSegments > TrailMaxSegments)
        {
            step = Mathf.CeilToInt(rawSegments / (float)TrailMaxSegments);
            if (step < 1) step = 1;
        }

        for (var i = 0; i < needed; i++)
        {
            var aRaw = i * step;
            var bRaw = (i + 1) * step;
            if (bRaw > rawSegments) bRaw = rawSegments;

            var aIdx = (trail.start + aRaw) % TrailMaxWaypoints;
            var bIdx = (trail.start + bRaw) % TrailMaxWaypoints;
            var a = trail.points[aIdx];
            var b = trail.points[bIdx];

            var au = (a.x / ext.x + 1f) * 0.5f;
            var av = (a.y / ext.y + 1f) * 0.5f;
            var bu = (b.x / ext.x + 1f) * 0.5f;
            var bv = (b.y / ext.y + 1f) * 0.5f;

            var ax = Mathf.Clamp01(au) * w;
            var ay = Mathf.Clamp01(av) * h;
            var bx = Mathf.Clamp01(bu) * w;
            var by = Mathf.Clamp01(bv) * h;

            var pa = new Vector2(ax - w * 0.5f, ay - h * 0.5f);
            var pb = new Vector2(bx - w * 0.5f, by - h * 0.5f);

            var d = pb - pa;
            var len = d.magnitude;
            if (len < 0.001f) continue;

            var mid = (pa + pb) * 0.5f;
            var angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;

            var seg = trail.segments[i];
            var age = now - trail.times[aIdx];
            var t = keepSeconds > 0.001f ? Mathf.Clamp01(age / keepSeconds) : 1f;
            var alpha = Mathf.Lerp(TrailAlphaStart, TrailAlphaEnd, t);
            seg.color = new Color(trail.color.r, trail.color.g, trail.color.b, alpha);
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
