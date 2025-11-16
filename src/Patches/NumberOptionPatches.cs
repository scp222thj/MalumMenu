using HarmonyLib;

namespace MalumMenu;

// https://github.com/astra1dev/AUnlocker/blob/main/src/OptionsPatches.cs

[HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
public static class NumberOption_Increase
{
    /// <summary>
    /// Increase the value of a NumberOption without limits.
    /// </summary>
    /// <param name="__instance">The <c>NumberOption</c> instance.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(NumberOption __instance)
    {
        if (!CheatToggles.noOptionsLimits) return true;
        __instance.Value +=  __instance.Increment;
        __instance.UpdateValue();
        __instance.OnValueChanged.Invoke(__instance);
        __instance.AdjustButtonsActiveState();
        return false;
    }
}

[HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
public static class NumberOption_Decrease
{
    /// <summary>
    /// Decrease the value of a NumberOption without limits.
    /// </summary>
    /// <param name="__instance">The <c>NumberOption</c> instance.</param>
    /// <returns><c>false</c> to skip the original method, <c>true</c> to allow the original method to run.</returns>
    public static bool Prefix(NumberOption __instance)
    {
        if (!CheatToggles.noOptionsLimits) return true;
        __instance.Value -=  __instance.Increment;
        __instance.UpdateValue();
        __instance.OnValueChanged.Invoke(__instance);
        __instance.AdjustButtonsActiveState();
        return false;
    }
}

[HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Initialize))]
public static class NumberOption_Initialize
{
    /// <summary>
    /// Set the valid range of a NumberOption to be unlimited.
    /// </summary>
    /// <param name="__instance">The <c>NumberOption</c> instance.</param>
    public static void Postfix(NumberOption __instance)
    {
        if (!CheatToggles.noOptionsLimits) return;
        __instance.ValidRange = new FloatRange(-999f, 999f);
    }
}
