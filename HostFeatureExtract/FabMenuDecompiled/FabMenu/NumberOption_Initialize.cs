using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(NumberOption), "Initialize")]
public static class NumberOption_Initialize
{
	public static void Postfix(NumberOption __instance)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		if (CheatToggles.noOptionsLimits)
		{
			bool flag = !Utils.isHideNSeek;
			if (flag)
			{
				StringNames title = ((OptionBehaviour)__instance).Title;
				bool flag2 = (((int)title == 133 || (int)title == 137) ? true : false);
				flag = flag2;
			}
			if (!flag)
			{
				__instance.ValidRange = new FloatRange(-999f, 999f);
			}
		}
	}
}
