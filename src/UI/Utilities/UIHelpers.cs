using UnityEngine;

namespace MalumMenu;

public static class UIHelpers
{
    public static void ApplyUIColor()
    {
        EnsureSkinFont();

        if (CheatToggles.rgbMode)
        {
            var rgb = Color.HSVToRGB(MenuUI.hue, 1f, 1f);
            rgb.a = 1f;
            GUI.backgroundColor = rgb;
        }
        else
        {
            var configHtmlColor = MalumMenu.menuHtmlColor.Value;

            if (!ColorUtility.TryParseHtmlString(configHtmlColor, out var uiColor))
            {
                if (!configHtmlColor.StartsWith("#"))
                {
                    if (ColorUtility.TryParseHtmlString("#" + configHtmlColor, out uiColor))
                    {
                        uiColor.a = 1f;
                        GUI.backgroundColor = uiColor;
                    }
                    else if (GUI.backgroundColor.a == 0f)
                    {
                        GUI.backgroundColor = Color.white;
                    }
                }
                else if (GUI.backgroundColor.a == 0f)
                {
                    GUI.backgroundColor = Color.white;
                }
            }
            else
            {
                uiColor.a = 1f;
                GUI.backgroundColor = uiColor;
            }
        }

        if (GUI.backgroundColor.a == 0f)
        {
            var opaque = GUI.backgroundColor;
            opaque.a = 1f;
            GUI.backgroundColor = opaque;
        }

        GUI.color = Color.white;
        GUI.contentColor = Color.white;

        EnsureSkinTextColors();
        GUIStylePreset.RefreshButtonTextColors();
    }

    public static void EnsureSkinFont()
    {
        if (GUI.skin == null)
        {
            return;
        }

        var current = GUI.skin.font;
        bool needsFont = current == null || current.name == "LegacyRuntime";

        if (!needsFont)
        {
            return;
        }

        var fallback = GUIStylePreset.FallbackFont;
        if (fallback == null || fallback == current)
        {
            return;
        }

        GUI.skin.font = fallback;
        ApplyFont(GUI.skin.button);
        ApplyFont(GUI.skin.label);
        ApplyFont(GUI.skin.toggle);
        ApplyFont(GUI.skin.window);
        ApplyFont(GUI.skin.box);
        ApplyFont(GUI.skin.textField);
        ApplyFont(GUI.skin.textArea);
    }

    private static void ApplyFont(GUIStyle style)
    {
        if (style != null)
        {
            style.font = GUIStylePreset.FallbackFont;
        }
    }

    public static void EnsureSkinTextColors()
    {
        if (GUI.skin == null)
        {
            return;
        }

        SetStyleTextColor(GUI.skin.label, Color.white);
        SetStyleTextColor(GUI.skin.toggle, Color.white);
        SetStyleTextColor(GUI.skin.window, Color.white);
        SetStyleTextColor(GUI.skin.box, Color.white);
    }

    private static void SetStyleTextColor(GUIStyle style, Color color)
    {
        if (style == null)
        {
            return;
        }

        style.normal.textColor = color;
        style.hover.textColor = color;
        style.active.textColor = color;
        style.focused.textColor = color;
        style.onNormal.textColor = color;
        style.onHover.textColor = color;
        style.onActive.textColor = color;
        style.onFocused.textColor = color;
    }
}
