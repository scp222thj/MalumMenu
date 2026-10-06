using UnityEngine;

namespace MalumMenu;

public static class GUIStylePreset
{
    private static GUIStyle _separator;
    private static GUIStyle _darkSeparator;
    private static GUIStyle _normalButton;
    private static GUIStyle _normalToggle;
    private static GUIStyle _tabButton;
    private static GUIStyle _tabTitle;
    private static GUIStyle _tabSubtitle;
    private static Font _fallbackFont;

    public static Font FallbackFont
    {
        get
        {
            if (_fallbackFont == null)
            {
                _fallbackFont = TryCreateFont(new string[] { "Arial", "Tahoma", "Verdana", "Liberation Sans", "DejaVu Sans", "Noto Sans", "Segoe UI", "Helvetica", "SansSerif" });
            }

            if (_fallbackFont == null)
            {
                _fallbackFont = TryCreateFont(new string[] { "Arial" });
            }

            if (_fallbackFont == null)
            {
                _fallbackFont = TryCreateFont(new string[] { "SansSerif" });
            }

            return _fallbackFont;
        }
    }

    private static Font TryCreateFont(string[] candidates)
    {
        try
        {
            var font = Font.CreateDynamicFontFromOSFont(candidates, 16);
            if (font != null && font.HasCharacter('A') && font.HasCharacter('g') && font.HasCharacter('0'))
            {
                return font;
            }

            return font;
        }
        catch
        {
            return null;
        }
    }

    public static Font ResolveFont()
    {
        var skinFont = GUI.skin != null ? GUI.skin.font : null;
        return skinFont != null ? skinFont : FallbackFont;
    }

    public static Color ContrastTextColor(Color background)
    {
        float luminance = 0.299f * background.r + 0.587f * background.g + 0.114f * background.b;
        return luminance > 0.5f ? Color.black : Color.white;
    }

    private static void ApplyTextColor(GUIStyle style, Color color)
    {
        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;
        style.focused.textColor = color;
        style.onNormal.textColor = color;
        style.onHover.textColor = color;
        style.onActive.textColor = color;
        style.onFocused.textColor = color;
    }

    private static void ApplyFont(GUIStyle style)
    {
        var font = ResolveFont();
        if (font != null)
        {
            style.font = font;
        }
    }

    public static void RefreshButtonTextColors()
    {
        var color = ContrastTextColor(GUI.backgroundColor);

        if (_normalButton != null)
        {
            ApplyTextColor(_normalButton, color);
            ApplyFont(_normalButton);
        }

        if (_tabButton != null)
        {
            ApplyTextColor(_tabButton, color);
            ApplyFont(_tabButton);
        }

        if (_normalToggle != null)
        {
            ApplyTextColor(_normalToggle, Color.white);
            ApplyFont(_normalToggle);
        }

        if (_tabTitle != null)
        {
            ApplyTextColor(_tabTitle, Color.white);
            ApplyFont(_tabTitle);
        }

        if (_tabSubtitle != null)
        {
            ApplyTextColor(_tabSubtitle, Color.white);
            ApplyFont(_tabSubtitle);
        }
    }

    public static GUIStyle Separator
    {
        get
        {
            if (_separator == null)
            {
                _separator = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = Texture2D.whiteTexture },
                    margin = new RectOffset { top = 4, bottom = 4 },
                    padding = new RectOffset(),
                    border = new RectOffset()
                };
            }

            return _separator;
        }
    }

    public static GUIStyle DarkSeparator
    {
        get
        {
            if (_darkSeparator == null)
            {
                _darkSeparator = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = Texture2D.grayTexture },
                    margin = new RectOffset { top = 4, bottom = 4 },
                    padding = new RectOffset(),
                    border = new RectOffset()
                };
            }

            return _darkSeparator;
        }
    }

    public static GUIStyle NormalButton
    {
        get
        {
            if (_normalButton == null)
            {
                _normalButton = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 13
                };
                ApplyFont(_normalButton);
                ApplyTextColor(_normalButton, ContrastTextColor(GUI.backgroundColor));
            }
            else
            {
                ApplyFont(_normalButton);
            }

            return _normalButton;
        }
    }

    public static GUIStyle NormalToggle
    {
        get
        {
            if (_normalToggle == null)
            {
                _normalToggle = new GUIStyle(GUI.skin.toggle)
                {
                    fontSize = 13
                };
                ApplyFont(_normalToggle);
                ApplyTextColor(_normalToggle, Color.white);
            }
            else
            {
                ApplyFont(_normalToggle);
            }

            return _normalToggle;
        }
    }

    public static GUIStyle TabButton
    {
        get
        {
            if (_tabButton == null)
            {
                _tabButton = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 17,
                    fontStyle = FontStyle.Bold,
                };
                ApplyFont(_tabButton);
                ApplyTextColor(_tabButton, ContrastTextColor(GUI.backgroundColor));
            }
            else
            {
                ApplyFont(_tabButton);
            }

            return _tabButton;
        }
    }

    public static GUIStyle TabTitle
    {
        get
        {
            if (_tabTitle == null)
            {
                _tabTitle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                };
                ApplyFont(_tabTitle);
                ApplyTextColor(_tabTitle, Color.white);
            }
            else
            {
                ApplyFont(_tabTitle);
            }

            return _tabTitle;
        }
    }

    public static GUIStyle TabSubtitle
    {
        get
        {
            if (_tabSubtitle == null)
            {
                _tabSubtitle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                };
                ApplyFont(_tabSubtitle);
                ApplyTextColor(_tabSubtitle, Color.white);
            }
            else
            {
                ApplyFont(_tabSubtitle);
            }

            return _tabSubtitle;
        }
    }
}
