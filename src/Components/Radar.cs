using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

public sealed class Radar : MonoBehaviour
{
    public static float scale = 0.35f;
    public static Vector2 anchoredPosition = new Vector2(320f, 180f);

    private const float MinScale = 0.15f;
    private const float MaxScale = 0.75f;
    private const float BaseSizeAtDefaultScale = 300f;
    private const float DefaultScale = 0.35f;

    private static readonly List<PlayerControl> Players = new List<PlayerControl>(16);

    private float _nextSaveTime;
    private Vector2 _lastSavedAnchored;
    private float _lastSavedScale;

    private bool _dragging;
    private Vector2 _dragStartMouse;
    private Vector2 _dragStartAnchored;

    private MapBehaviour _template;
    private Sprite _backgroundSprite;
    private Texture _backgroundTexture;
    private Rect _backgroundUv;
    private Vector2 _backgroundExtents;
    private float _nextTemplateScanTime;

    private void Update()
    {
        if (!CheatToggles.minimapAlwaysOn)
        {
            _dragging = false;
            return;
        }

        if (Time.unscaledTime >= _nextTemplateScanTime)
        {
            _nextTemplateScanTime = Time.unscaledTime + 1f;
            RefreshTemplate();
        }

        MaybeSaveWindow();
    }

    private void OnGUI()
    {
        if (!CheatToggles.minimapAlwaysOn) return;
        if (MalumMenu.isPanicked) return;

        var s = Mathf.Clamp(scale, MinScale, MaxScale);
        scale = s;

        var size = BaseSizeAtDefaultScale * (s / DefaultScale);
        if (size < 64f) size = 64f;

        var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        var topLeft = center + anchoredPosition - new Vector2(size * 0.5f, size * 0.5f);
        var rect = new Rect(topLeft.x, topLeft.y, size, size);

        if (MenuUI.isGUIActive)
        {
            HandleDrag(rect);
            DrawBorder(rect);
        }
        else
        {
            _dragging = false;
        }

        DrawBackground(rect);
        DrawPlayers(rect);
        DrawTrails(rect);
    }

    private void HandleDrag(Rect rect)
    {
        var mouse = Event.current != null ? Event.current.mousePosition : (Vector2)Input.mousePosition;

        if (Event.current != null && Event.current.type == EventType.MouseUp)
        {
            _dragging = false;
            return;
        }

        if (!_dragging)
        {
            if (Event.current == null || Event.current.type != EventType.MouseDown) return;
            if (!rect.Contains(mouse)) return;
            _dragging = true;
            _dragStartMouse = mouse;
            _dragStartAnchored = anchoredPosition;
            return;
        }

        if (Event.current == null || (Event.current.type != EventType.MouseDrag && Event.current.type != EventType.MouseMove)) return;

        var delta = mouse - _dragStartMouse;
        anchoredPosition = _dragStartAnchored + delta;
    }

    private void DrawBorder(Rect rect)
    {
        var c = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);

        var t = 2f;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, t), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.yMax - t, rect.width, t), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y, t, rect.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax - t, rect.y, t, rect.height), Texture2D.whiteTexture);

        GUI.color = c;
    }

    private void DrawBackground(Rect rect)
    {
        if (_backgroundTexture != null)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.95f);
            GUI.DrawTextureWithTexCoords(rect, _backgroundTexture, _backgroundUv);
            GUI.color = Color.white;
            return;
        }

        GUI.color = new Color(0f, 0f, 0f, 0.35f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawPlayers(Rect rect)
    {
        Players.Clear();
        try
        {
            foreach (var p in PlayerControl.AllPlayerControls)
            {
                if (p == null || p.Data == null) continue;
                Players.Add(p);
            }
        }
        catch
        {
            return;
        }

        if (!Utils.isShip) return;
        if (ShipStatus.Instance == null) return;

        var ext = _backgroundExtents;
        if (ext.x <= 0.0001f || ext.y <= 0.0001f)
        {
            ext = new Vector2(6f, 6f);
        }

        for (var i = 0; i < Players.Count; i++)
        {
            var p = Players[i];
            if (p == null || p.Data == null) continue;

            var isImp = p.Data.Role != null && p.Data.Role.IsImpostor;
            var isDead = p.Data.IsDead;

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

            var color = ResolvePlayerColor(p, isImp, isDead);

            var pos = p.transform.position;
            pos /= ShipStatus.Instance.MapScale;
            pos.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);

            var u = (pos.x / ext.x + 1f) * 0.5f;
            var v = (pos.y / ext.y + 1f) * 0.5f;

            var px = rect.x + Mathf.Clamp01(u) * rect.width;
            var py = rect.y + (1f - Mathf.Clamp01(v)) * rect.height;

            var r = 4f;
            if (CheatToggles.mapImpsHighlight && isImp && !isDead)
            {
                DrawDot(px, py, r + 2f, Color.red);
            }
            DrawDot(px, py, r, color);
        }
    }

    private void DrawTrails(Rect rect)
    {
        if (!CheatToggles.mapTrails) return;
        if (!Utils.isShip) return;
        if (ShipStatus.Instance == null) return;
        if (Utils.isMeeting) return;

        var ext = _backgroundExtents;
        if (ext.x <= 0.0001f || ext.y <= 0.0001f)
        {
            ext = new Vector2(6f, 6f);
        }

        MinimapHandler.DrawTrailsGUI(rect, ext);
    }

    private static void DrawDot(float x, float y, float radius, Color color)
    {
        var c = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(new Rect(x - radius, y - radius, radius * 2f, radius * 2f), Texture2D.whiteTexture);
        GUI.color = c;
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

    private void RefreshTemplate()
    {
        var template = MapBehaviour.Instance;
        if (template == null)
        {
            try
            {
                template = Object.FindObjectOfType<MapBehaviour>(true);
            }
            catch
            {
                template = null;
            }
        }

        if (template == null)
        {
            _template = null;
            _backgroundSprite = null;
            _backgroundTexture = null;
            _backgroundUv = default;
            _backgroundExtents = default;
            return;
        }

        if (_template == template) return;
        _template = template;

        var sr = ResolveBackgroundRenderer(template);
        if (sr == null || sr.sprite == null || sr.sprite.texture == null)
        {
            _backgroundSprite = null;
            _backgroundTexture = null;
            _backgroundUv = default;
            _backgroundExtents = default;
            return;
        }

        _backgroundSprite = sr.sprite;
        _backgroundTexture = sr.sprite.texture;

        var tr = sr.sprite.textureRect;
        var texW = sr.sprite.texture.width;
        var texH = sr.sprite.texture.height;
        _backgroundUv = new Rect(tr.x / texW, tr.y / texH, tr.width / texW, tr.height / texH);

        var b = sr.sprite.bounds;
        _backgroundExtents = b.extents;

        if (CheatToggles.debugMinimap && MalumMenu.Log != null)
        {
            try
            {
                MalumMenu.Log.LogInfo($"Radar: template={template.name} bgSprite={sr.sprite.name} tex={texW}x{texH} ext=({_backgroundExtents.x:0.00},{_backgroundExtents.y:0.00})");
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

