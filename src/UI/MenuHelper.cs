using UnityEngine;

namespace MalumMenu;

public static class MenuHelper
{
    public static void ApplyMenuColor()
    {
        
         if (CheatToggles.rgbMode)
         {
             GUI.backgroundColor = Color.HSVToRGB(hue, 1f, 1f); // Set background color based on hue
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
