using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

public class AlwaysOnMinimapController : MonoBehaviour
{
    private MapBehaviour _map;
    private bool _initialized;

    public static float scale = 0.35f;
    public static float x = 0.78f;
    public static float y = 0.78f;

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

        var baseMap = Object.FindObjectOfType<MapBehaviour>(true);
        if (baseMap == null) return;

        _map = Object.Instantiate(baseMap, baseMap.transform.parent);
        _map.name = "MalumMiniMap";
        _map.gameObject.SetActive(true);
        _initialized = false;
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

        var px = Mathf.Clamp01(x);
        var py = Mathf.Clamp01(y);

        var parent = _map.transform.parent as RectTransform;
        var rt = _map.transform as RectTransform;
        if (rt != null && parent != null)
        {
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0f, 0f);
            rt.localScale = new Vector3(s, s, 1f);
            rt.anchoredPosition = new Vector2(parent.rect.width * px, parent.rect.height * py);
            return;
        }

        _map.transform.localScale = new Vector3(s, s, 1f);
        _map.transform.localPosition = new Vector3(px * 10f, py * 10f, _map.transform.localPosition.z);
    }

    private void DestroyMap()
    {
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
        MinimapHandler.ClearTrails();
    }
}

