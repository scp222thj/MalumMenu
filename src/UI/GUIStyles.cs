using UnityEngine;

namespace MalumMenu;

public static class GUIStyles
{
    public static GUIStyle SeparatorStyle
    {
        get
        {
            field ??= new GUIStyle(GUI.skin.box)
            {
                normal = { background = Texture2D.whiteTexture },
                margin = new RectOffset { top = 4, bottom = 4 },
                padding = new RectOffset(),
                border = new RectOffset()
            };
            return field;
        }
    }

    public static GUIStyle NormalButtonStyle
    {
        get
        {
            field ??= new GUIStyle(GUI.skin.button)
            {
                fontSize = 13
            };
            return field;
        }
    }

    public static GUIStyle NormalToggleStyle
    {
        get
        {
            field ??= new GUIStyle(GUI.skin.toggle)
            {
                fontSize = 13
            };
            return field;
        }
    }
}
