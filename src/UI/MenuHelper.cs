using UnityEngine;

namespace MalumMenu;

public static class MenuHelper
{
    public static void ApplyMenuColor()
    {

        if (ColorUtility.TryParseHtmlString(MalumMenu.menuHtmlColor.Value, out var configUIColor))
        {
            GUI.backgroundColor = configUIColor;
        }
        
        if (CheatToggles.rgbMode)
        {
            GUI.backgroundColor = Color.HSVToRGB(MenuUI.hue, 1f, 1f);
        }
        else
        {
            var configHtmlColor = MalumMenu.menuHtmlColor.Value;

            if (ColorUtility.TryParseHtmlString(configHtmlColor, out var uiColor))
            {
                GUI.backgroundColor = uiColor;
            }
            else if (!configHtmlColor.StartsWith("#") && ColorUtility.TryParseHtmlString("#" + configHtmlColor, out uiColor))
            {
                GUI.backgroundColor = uiColor;
            }
        }
    }
}
