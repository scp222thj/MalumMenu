using UnityEngine;

namespace MalumMenu;

public static class GUIStylePreset
{
    private static GUIStyle _separator;
    private static GUIStyle _darkSeparator;
    private static GUIStyle _normalButton;
    private static GUIStyle _normalToggle;
    private static GUIStyle _flatButton;
    private static GUIStyle _tabButton;
    private static GUIStyle _tabTitle;
    private static GUIStyle _tabSubtitle;

    private static Texture2D _flatButtonTex;

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
            }

            return _normalToggle;
        }
    }

    public static GUIStyle FlatButton
    {
        get
        {
            if (_flatButton == null)
            {
                var style = new GUIStyle(GUI.skin.label)
                {
                    wordWrap = false,
                    alignment = TextAnchor.MiddleCenter
                };

                var toggle = GUI.skin.toggle;
                style.normal.textColor = toggle.normal.textColor;
                style.hover.textColor = toggle.hover.textColor;

                _flatButtonTex = new Texture2D(1, 1)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                _flatButtonTex.SetPixels(new[] { new Color(0.5f, 0.5f, 0.5f, 0.5f) });
                _flatButtonTex.Apply();
                style.hover.background = _flatButtonTex;

                style.name = nameof(_flatButton);
                _flatButton = style;
            }

            return _flatButton;
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
            }

            return _tabSubtitle;
        }
    }
}