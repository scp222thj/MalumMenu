using UnityEngine;

namespace MalumMenu;

public static class UIHelper
{
    public static void ApplyUIColor()
    {
        
         if (CheatToggles.rgbMode)
         {
             GUI.backgroundColor = Color.HSVToRGB(MenuUI.hue, 1f, 1f); // Set background color based on hue
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
                         GUI.backgroundColor = uiColor;
                     }
                 }
             }
             else
             {
                 GUI.backgroundColor = uiColor;
             }
         }
    }
}
