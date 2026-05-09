using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using InnerNet;
using System.Collections.Generic;

namespace MalumMenu;

public class AlwaysOnMinimapController : MonoBehaviour
{
    private RectTransform _container;
    private GameObject _borderRoot;
    private Transform _visualRoot;
    private Transform _iconsRoot;
    private Transform _trailsRoot;
    private SpriteRenderer _background;
    private MapBehaviour _template;
    private int _templateInstanceId;
    private float _nextIconSyncTime;
    private readonly List<HerePoint> _windowHerePoints = new List<HerePoint>(16);
    private bool _dragging;
    private Vector2 _dragStartMouse;
    private Vector2 _dragStartAnchored;
    private float _dragScaleFactor = 1f;
    private float _nextSaveTime;
    private Vector2 _lastSavedAnchored;
    private float _lastSavedScale;
    private readonly Vector3[] _tmpWorldCorners = new Vector3[4];
    private float _nextDebugTime;

    public static float scale = 0.35f;
    public static Vector2 anchoredPosition = new Vector2(320f, 180f);

    private static readonly FieldInfo HudMapField = FindHudMapField();

    private void Update()
    {
        if (CheatToggles.debugMinimap)
        {
            MaybeDebugTick();
        }

        if (!CheatToggles.minimapAlwaysOn)
        {
            DestroyWindow();
            return;
        }

        if (!IsAllowedGameState())
        {
            DestroyWindow();
            return;
        }

        EnsureWindow();
        EnsureVisuals();
        UpdateTransform();
        UpdateIconsAndTrails();
    }

    private static bool IsAllowedGameState()
    {
        var client = AmongUsClient.Instance;
        if (client == null) return false;

        var state = client.GameState;
        return state == InnerNetClient.GameStates.Joined || state == InnerNetClient.GameStates.Started;
    }

    private void MaybeDebugTick()
    {
        var now = Time.unscaledTime;
        if (now < _nextDebugTime) return;
        _nextDebugTime = now + 1f;

        if (MalumMenu.Log == null) return;

        try
        {
            var state = AmongUsClient.Instance != null ? AmongUsClient.Instance.GameState.ToString() : "null";
            var netMode = AmongUsClient.Instance != null ? AmongUsClient.Instance.NetworkMode.ToString() : "null";
            var hasHud = DestroyableSingleton<HudManager>.Instance != null;
            var hasTemplate = ResolveTemplate() != null;
            var hasContainer = _container != null;
            var hasVisual = _visualRoot != null;
            var hasBg = _background != null;
            MalumMenu.Log.LogInfo($"MinimapWindow: tick enabled={CheatToggles.minimapAlwaysOn} state={state} net={netMode} inGame={Utils.isInGame} lobby={Utils.isLobby} freeplay={Utils.isFreePlay} hud={hasHud} template={hasTemplate} container={hasContainer} visual={hasVisual} bg={hasBg}");
        }
        catch
        {
        }
    }

    private void EnsureWindow()
    {
        if (_container != null) return;

        var uiParent = ResolveUiParent();
        if (uiParent == null)
        {
            if (CheatToggles.debugMinimap && MalumMenu.Log != null)
            {
                try
                {
                    MalumMenu.Log.LogInfo("MinimapWindow: no UI parent yet");
                }
                catch
                {
                }
            }
            return;
        }

        var containerGo = new GameObject("MalumMiniMapContainer");
        var containerRt = containerGo.AddComponent<RectTransform>();
        containerRt.SetParent(uiParent, false);
        containerRt.anchorMin = new Vector2(0.5f, 0.5f);
        containerRt.anchorMax = new Vector2(0.5f, 0.5f);
        containerRt.pivot = new Vector2(0.5f, 0.5f);
        containerRt.anchoredPosition = anchoredPosition;
        containerRt.localScale = Vector3.one;
        containerRt.SetAsLastSibling();

        _container = containerRt;
        _borderRoot = CreateBorder(_container);

        var rootGo = new GameObject("VisualRoot");
        rootGo.transform.SetParent(_container, false);
        rootGo.transform.localPosition = Vector3.zero;
        rootGo.transform.localRotation = Quaternion.identity;
        rootGo.transform.localScale = Vector3.one;
        _visualRoot = rootGo.transform;

        var bgGo = new GameObject("Background");
        bgGo.transform.SetParent(_visualRoot, false);
        bgGo.transform.localPosition = Vector3.zero;
        bgGo.transform.localRotation = Quaternion.identity;
        bgGo.transform.localScale = Vector3.one;
        _background = bgGo.AddComponent<SpriteRenderer>();

        var iconsGo = new GameObject("Icons");
        iconsGo.transform.SetParent(_visualRoot, false);
        iconsGo.transform.localPosition = Vector3.zero;
        iconsGo.transform.localRotation = Quaternion.identity;
        iconsGo.transform.localScale = Vector3.one;
        _iconsRoot = iconsGo.transform;

        var trailsGo = new GameObject("Trails");
        trailsGo.transform.SetParent(_visualRoot, false);
        trailsGo.transform.localPosition = Vector3.zero;
        trailsGo.transform.localRotation = Quaternion.identity;
        trailsGo.transform.localScale = Vector3.one;
        _trailsRoot = trailsGo.transform;

        if (CheatToggles.debugMinimap && MalumMenu.Log != null)
        {
            try
            {
                var parentName = uiParent.name;
                MalumMenu.Log.LogInfo($"MinimapWindow: window created parent={parentName}");
            }
            catch
            {
            }
        }
    }

    private static MapBehaviour ResolveTemplate()
    {
        if (MapBehaviour.Instance != null) return MapBehaviour.Instance;

        var hud = DestroyableSingleton<HudManager>.Instance;
        if (hud == null) return null;

        if (HudMapField != null)
        {
            try
            {
                var obj = HudMapField.GetValue(hud);
                var map = obj as MapBehaviour;
                if (map != null) return map;
            }
            catch
            {
            }
        }

        try
        {
            var any = Object.FindObjectOfType<MapBehaviour>(true);
            if (any != null) return any;
        }
        catch
        {
        }

        return null;
    }

    private static Transform ResolveUiParent()
    {
        var template = ResolveTemplate();
        if (template != null && template.transform != null && template.transform.parent != null)
        {
            return template.transform.parent;
        }

        var hud = DestroyableSingleton<HudManager>.Instance;
        if (hud != null) return hud.transform;

        return null;
    }

    private static FieldInfo FindHudMapField()
    {
        try
        {
            var fields = typeof(HudManager).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var f in fields)
            {
                if (f == null) continue;
                if (!typeof(MapBehaviour).IsAssignableFrom(f.FieldType)) continue;
                return f;
            }
        }
        catch
        {
        }

        return null;
    }

    private void UpdateTransform()
    {
        if (_container == null) return;

        var s = scale;
        if (s < 0.15f) s = 0.15f;
        if (s > 0.75f) s = 0.75f;

        _container.anchorMin = new Vector2(0.5f, 0.5f);
        _container.anchorMax = new Vector2(0.5f, 0.5f);
        _container.pivot = new Vector2(0.5f, 0.5f);
        HandleDrag(_container, s);
        _container.anchoredPosition = anchoredPosition;
        _container.localScale = new Vector3(s, s, 1f);
        MaybeSaveWindow(s);

        if (_borderRoot != null)
        {
            _borderRoot.SetActive(MenuUI.isGUIActive);
        }
    }

    private void HandleDrag(RectTransform rt, float _)
    {
        if (rt == null) return;
        var parent = rt.parent as RectTransform;
        if (parent == null) return;

        if (!MenuUI.isGUIActive)
        {
            _dragging = false;
            return;
        }

        var eventCamera = ResolveEventCamera(rt);

        if (Input.GetMouseButtonUp(0))
        {
            if (_dragging && CheatToggles.debugMinimap && MalumMenu.Log != null)
            {
                try
                {
                    MalumMenu.Log.LogInfo($"MinimapWindow: drag end pos=({anchoredPosition.x:0.0},{anchoredPosition.y:0.0})");
                }
                catch
                {
                }
            }
            _dragging = false;
            return;
        }

        if (!_dragging)
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var over = IsMouseOverRect(rt, Input.mousePosition, eventCamera);
            if (!over)
            {
                over = IsMouseOverRectWorld(rt, Input.mousePosition, eventCamera);
            }
            if (!over)
            {
                if (CheatToggles.debugMinimap && MalumMenu.Log != null)
                {
                    try
                    {
                        MalumMenu.Log.LogInfo($"MinimapWindow: click not on window mouse=({Input.mousePosition.x:0},{Input.mousePosition.y:0})");
                    }
                    catch
                    {
                    }
                }
                return;
            }

            _dragStartMouse = Input.mousePosition;
            _dragStartAnchored = anchoredPosition;
            _dragScaleFactor = ResolveScaleFactor(rt);
            _dragging = true;

            if (CheatToggles.debugMinimap && MalumMenu.Log != null)
            {
                try
                {
                    MalumMenu.Log.LogInfo($"MinimapWindow: drag start mouse=({_dragStartMouse.x:0},{_dragStartMouse.y:0}) pos=({_dragStartAnchored.x:0.0},{_dragStartAnchored.y:0.0}) sf={_dragScaleFactor:0.00}");
                }
                catch
                {
                }
            }
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            _dragging = false;
            return;
        }

        var mouseDelta = (Vector2)Input.mousePosition - _dragStartMouse;
        if (_dragScaleFactor > 0.0001f)
        {
            mouseDelta /= _dragScaleFactor;
        }
        anchoredPosition = _dragStartAnchored + mouseDelta;
    }

    private static bool IsMouseOverRect(RectTransform rt, Vector2 mousePosition, Camera eventCamera)
    {
        if (rt == null) return false;

        if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePosition, eventCamera)) return true;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, mousePosition, eventCamera, out var lp)) return false;
        return rt.rect.Contains(lp);
    }

    private bool IsMouseOverRectWorld(RectTransform rt, Vector2 mousePosition, Camera eventCamera)
    {
        if (rt == null) return false;

        try
        {
            rt.GetWorldCorners(_tmpWorldCorners);
        }
        catch
        {
            return false;
        }

        var minX = float.MaxValue;
        var minY = float.MaxValue;
        var maxX = float.MinValue;
        var maxY = float.MinValue;

        for (var i = 0; i < 4; i++)
        {
            var w = _tmpWorldCorners[i];
            var sp = RectTransformUtility.WorldToScreenPoint(eventCamera, w);

            if (sp.x < minX) minX = sp.x;
            if (sp.y < minY) minY = sp.y;
            if (sp.x > maxX) maxX = sp.x;
            if (sp.y > maxY) maxY = sp.y;
        }

        var r = new Rect(minX, minY, maxX - minX, maxY - minY);
        return r.Contains(mousePosition);
    }

    private void MaybeSaveWindow(float clampedScale)
    {
        var now = Time.unscaledTime;
        if (now < _nextSaveTime) return;
        _nextSaveTime = now + 0.25f;

        var pos = anchoredPosition;
        var scaleNow = clampedScale;

        var posChanged = Vector2.Distance(pos, _lastSavedAnchored) > 0.01f;
        var scaleChanged = Mathf.Abs(scaleNow - _lastSavedScale) > 0.0001f;
        if (!posChanged && !scaleChanged) return;

        _lastSavedAnchored = pos;
        _lastSavedScale = scaleNow;

        if (MalumMenu.Plugin == null) return;

        try
        {
            MalumMenu.minimapScale.Value = scaleNow;
            MalumMenu.minimapPosX.Value = pos.x;
            MalumMenu.minimapPosY.Value = pos.y;
            MalumMenu.Plugin.Config.Save();
        }
        catch
        {
        }
    }

    private static float ResolveScaleFactor(RectTransform rt)
    {
        if (rt == null) return 1f;

        Canvas canvas = null;
        try
        {
            canvas = rt.GetComponentInParent<Canvas>();
        }
        catch
        {
        }

        if (canvas == null) return 1f;

        var sf = canvas.scaleFactor;
        if (sf < 0.0001f) sf = 1f;
        return sf;
    }

    private static Camera ResolveEventCamera(RectTransform rt)
    {
        if (rt == null) return null;

        Canvas canvas = null;
        try
        {
            canvas = rt.GetComponentInParent<Canvas>();
        }
        catch
        {
        }

        if (canvas != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
            if (canvas.worldCamera != null) return canvas.worldCamera;
        }

        var hud = DestroyableSingleton<HudManager>.Instance;
        if (hud != null && hud.UICamera != null) return hud.UICamera;

        return Camera.main;
    }

    private void EnsureVisuals()
    {
        var template = ResolveTemplate();
        if (template == null) return;

        var templateId = template.GetInstanceID();
        if (_templateInstanceId != templateId)
        {
            _template = template;
            _templateInstanceId = templateId;
            RebuildBackground(template);
            RebuildIcons(template);
        }

        if (_background != null && _background.sprite == null)
        {
            RebuildBackground(template);
        }
    }

    private void RebuildBackground(MapBehaviour template)
    {
        if (_background == null) return;
        if (template == null) return;

        var src = ResolveBackgroundRenderer(template);
        if (src == null || src.sprite == null) return;

        _background.sprite = src.sprite;
        _background.material = src.sharedMaterial;
        _background.color = src.color;
        _background.sortingLayerID = src.sortingLayerID;
        _background.sortingOrder = src.sortingOrder;

        _background.transform.localPosition = Vector3.zero;
        _background.transform.localRotation = Quaternion.identity;
        _background.transform.localScale = Vector3.one;
    }

    private static SpriteRenderer ResolveBackgroundRenderer(MapBehaviour template)
    {
        if (template == null) return null;

        SpriteRenderer best = null;
        var bestScore = 0f;

        SpriteRenderer[] renderers = null;
        try
        {
            renderers = template.GetComponentsInChildren<SpriteRenderer>(true);
        }
        catch
        {
        }

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

    private void RebuildIcons(MapBehaviour template)
    {
        if (_iconsRoot == null) return;
        if (template == null) return;
        if (template.HerePoint == null) return;

        for (var i = 0; i < _windowHerePoints.Count; i++)
        {
            var hp = _windowHerePoints[i];
            if (hp == null) continue;
            if (hp.sprite != null)
            {
                try { Object.Destroy(hp.sprite.gameObject); } catch { }
            }
        }
        _windowHerePoints.Clear();
        _nextIconSyncTime = 0f;
    }

    private void UpdateIconsAndTrails()
    {
        if (_template == null) return;
        if (_iconsRoot == null) return;

        var now = Time.unscaledTime;
        if (now >= _nextIconSyncTime)
        {
            _nextIconSyncTime = now + 1f;
            SyncIcons();
        }

        for (var i = 0; i < _windowHerePoints.Count; i++)
        {
            var hp = _windowHerePoints[i];
            if (hp == null) continue;
            MinimapHandler.HandleHerePoint(hp);
        }

        if (_trailsRoot != null)
        {
            MinimapHandler.RecordTrails();
            MinimapHandler.RenderTrailsWindow(_trailsRoot, _template.HerePoint != null ? _template.HerePoint.material : null);
        }
    }

    private void SyncIcons()
    {
        if (_template == null) return;
        if (_template.HerePoint == null) return;

        for (var i = 0; i < _windowHerePoints.Count; i++)
        {
            var hp = _windowHerePoints[i];
            if (hp == null) continue;
            if (hp.player == null || hp.player.Data == null)
            {
                if (hp.sprite != null)
                {
                    try { Object.Destroy(hp.sprite.gameObject); } catch { }
                }
                _windowHerePoints.RemoveAt(i);
                i--;
            }
        }

        var players = PlayerControl.AllPlayerControls;
        if (players == null) return;

        for (var i = 0; i < players.Count; i++)
        {
            var p = players[i];
            if (p == null || p.Data == null) continue;

            var exists = false;
            for (var j = 0; j < _windowHerePoints.Count; j++)
            {
                var hp = _windowHerePoints[j];
                if (hp != null && hp.player == p)
                {
                    exists = true;
                    break;
                }
            }
            if (exists) continue;

            SpriteRenderer sr = null;
            try
            {
                sr = Object.Instantiate(_template.HerePoint, _iconsRoot);
            }
            catch
            {
            }

            if (sr == null) continue;

            sr.gameObject.SetActive(true);
            _windowHerePoints.Add(new HerePoint(p, sr));
        }
    }

    private void DestroyWindow()
    {
        if (_container != null)
        {
            try { Object.Destroy(_container.gameObject); } catch { }
        }

        _container = null;
        _borderRoot = null;
        _visualRoot = null;
        _iconsRoot = null;
        _trailsRoot = null;
        _background = null;
        _template = null;
        _templateInstanceId = 0;

        _dragging = false;
        _nextSaveTime = 0f;
        _nextDebugTime = 0f;
        _nextIconSyncTime = 0f;

        _windowHerePoints.Clear();
    }

    private static GameObject CreateBorder(RectTransform container)
    {
        if (container == null) return null;

        var root = new GameObject("Border");
        var rt = root.AddComponent<RectTransform>();
        rt.SetParent(container, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;

        var thickness = 2f;
        CreateBorderEdge(rt, "Top", 0f, 1f, thickness);
        CreateBorderEdge(rt, "Bottom", 0f, 0f, thickness);
        CreateBorderEdgeVertical(rt, "Left", 0f, thickness);
        CreateBorderEdgeVertical(rt, "Right", 1f, thickness);

        root.SetActive(false);
        rt.SetAsLastSibling();
        return root;
    }

    private static void CreateBorderEdge(RectTransform parent, string name, float xAnchor, float yAnchor, float thickness)
    {
        var go = new GameObject(name);
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0f, yAnchor);
        rt.anchorMax = new Vector2(1f, yAnchor);
        rt.pivot = new Vector2(0.5f, yAnchor);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(0f, thickness);

        var img = go.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.25f);
        img.raycastTarget = false;
    }

    private static void CreateBorderEdgeVertical(RectTransform parent, string name, float xAnchor, float thickness)
    {
        var go = new GameObject(name);
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(xAnchor, 0f);
        rt.anchorMax = new Vector2(xAnchor, 1f);
        rt.pivot = new Vector2(xAnchor, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(thickness, 0f);

        var img = go.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.25f);
        img.raycastTarget = false;
    }
}
