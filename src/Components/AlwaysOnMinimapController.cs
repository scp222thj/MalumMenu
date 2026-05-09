using System.Reflection;
using HarmonyLib;
using UnityEngine;
using AmongUs.GameOptions;

namespace MalumMenu;

public class AlwaysOnMinimapController : MonoBehaviour
{
    private MapBehaviour _map;
    private bool _initialized;
    private bool _requestedCreate;
    private bool _dragging;
    private Vector2 _dragOffset;

    public static float scale = 0.35f;
    public static Vector2 anchoredPosition = new Vector2(320f, 180f);

    private static readonly FieldInfo HudMapField = FindHudMapField();

    private void Update()
    {
        if (!CheatToggles.minimapAlwaysOn)
        {
            DestroyMap();
            return;
        }

        if (!Utils.isInGame)
        {
            DestroyMap();
            return;
        }

        EnsureMap();
        UpdateTransform();
    }

    private void EnsureMap()
    {
        if (_map != null) return;

        var template = ResolveTemplate();
        if (template == null)
        {
            TryRequestCreate();
            return;
        }

        _map = Object.Instantiate(template, template.transform.parent);
        _map.name = "MalumMiniMap";
        _map.gameObject.SetActive(true);
        _initialized = false;

        if (MapBehaviour.Instance != null)
        {
            try
            {
                MapBehaviour.Instance.Close();
            }
            catch
            {
            }
            _requestedCreate = false;
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

    private void TryRequestCreate()
    {
        if (_requestedCreate) return;

        var hud = DestroyableSingleton<HudManager>.Instance;
        if (hud == null) return;

        _requestedCreate = true;
        try
        {
            hud.ToggleMapVisible(new MapOptions { Mode = MapOptions.Modes.Normal });
        }
        catch
        {
        }
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
        if (_map == null) return;

        if (!_initialized)
        {
            _initialized = true;
            try
            {
                _map.ShowNormalMap();
            }
            catch
            {
            }
        }

        var s = scale;
        if (s < 0.15f) s = 0.15f;
        if (s > 0.75f) s = 0.75f;

        var rt = _map.transform as RectTransform;
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            HandleDrag(rt, s);
            rt.anchoredPosition = anchoredPosition;
            rt.localScale = new Vector3(s, s, 1f);
        }
        else
        {
            _map.transform.localScale = new Vector3(s, s, 1f);
        }
    }

    private void HandleDrag(RectTransform rt, float currentScale)
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
            _dragging = false;
            return;
        }

        if (!_dragging)
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (!RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition, eventCamera)) return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, Input.mousePosition, eventCamera, out var localPoint)) return;
            _dragOffset = anchoredPosition - localPoint;
            _dragging = true;
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            _dragging = false;
            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, Input.mousePosition, eventCamera, out var lp)) return;
        var next = lp + _dragOffset;
        anchoredPosition = ClampToParent(parent, rt, next, currentScale);
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

    private static Vector2 ClampToParent(RectTransform parent, RectTransform child, Vector2 pos, float currentScale)
    {
        var rect = parent.rect;
        var halfW = rect.width * 0.5f;
        var halfH = rect.height * 0.5f;

        var childHalfW = 0f;
        var childHalfH = 0f;
        if (child != null)
        {
            var cr = child.rect;
            childHalfW = cr.width * 0.5f * currentScale;
            childHalfH = cr.height * 0.5f * currentScale;
        }

        var minX = -halfW + childHalfW;
        var maxX = halfW - childHalfW;
        var minY = -halfH + childHalfH;
        var maxY = halfH - childHalfH;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        return pos;
    }

    private void DestroyMap()
    {
        if (_requestedCreate && MapBehaviour.Instance != null)
        {
            try
            {
                MapBehaviour.Instance.Close();
            }
            catch
            {
            }
        }

        if (_map == null) return;
        try
        {
            Object.Destroy(_map.gameObject);
        }
        catch
        {
        }
        _map = null;
        _initialized = false;
        _requestedCreate = false;
        _dragging = false;
        MinimapHandler.DetachTrailRenderers();
    }
}
