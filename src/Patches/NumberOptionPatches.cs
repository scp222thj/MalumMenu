using HarmonyLib;

namespace MalumMenu;

// Found here: https://github.com/astra1dev/AUnlocker/blob/main/src/OptionsPatches.cs

[HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
public static class NumberOption_Increase
{
    // Increases the value of a numerical game option without limits
    public static bool Prefix(NumberOption __instance)
    {
        if (!CheatToggles.noOptionsLimits) return true;

        // Avoid bypassing imp amount and player speed restrictions in non-HnS games
        // due to anticheat restrictions
        if (!Utils.isHideNSeek && __instance.Title is StringNames.GameNumImpostors or StringNames.GamePlayerSpeed) return true;

        __instance.Value += __instance.Increment;
        __instance.UpdateValue();
        __instance.OnValueChanged.Invoke(__instance);
        __instance.AdjustButtonsActiveState();

        return false;
    }
}

[HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
public static class NumberOption_Decrease
{
    // Decreases the value of a numerical game option without limits
    public static bool Prefix(NumberOption __instance)
    {
        if (!CheatToggles.noOptionsLimits) return true;

        // Avoids bypassing imp amount and player speed restrictions in non-HnS games
        // due to anticheat restrictions
        if (!Utils.isHideNSeek && __instance.Title is StringNames.GameNumImpostors or StringNames.GamePlayerSpeed) return true;

        __instance.Value -= __instance.Increment;
        __instance.UpdateValue();
        __instance.OnValueChanged.Invoke(__instance);
        __instance.AdjustButtonsActiveState();

        return false;
    }
}

[HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Initialize))]
public static class NumberOption_Initialize
{
    // Sets the valid range of a numerical game option to be practically unlimited
    public static void Postfix(NumberOption __instance)
    {
        if (!CheatToggles.noOptionsLimits) return;

        // Avoids bypassing imp amount and player speed restrictions in non-HnS games
        // due to anticheat restrictions
        if (!Utils.isHideNSeek && __instance.Title is StringNames.GameNumImpostors or StringNames.GamePlayerSpeed) return;

        __instance.ValidRange = new FloatRange(-999f, 999f);
    }
}
