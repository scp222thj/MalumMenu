using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(NumberOption), "Increase")]
public static class NumberOption_Increase
{
	public static bool Prefix(NumberOption __instance)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		if (!CheatToggles.noOptionsLimits)
		{
			return true;
		}
		bool flag = !Utils.isHideNSeek;
		if (flag)
		{
			StringNames title = ((OptionBehaviour)__instance).Title;
			bool flag2 = (((int)title == 133 || (int)title == 137) ? true : false);
			flag = flag2;
		}
		if (flag)
		{
			return true;
		}
		__instance.Value += __instance.Increment;
		__instance.UpdateValue();
		((OptionBehaviour)__instance).OnValueChanged.Invoke((OptionBehaviour)(object)__instance);
		__instance.AdjustButtonsActiveState();
		return false;
	}
}
