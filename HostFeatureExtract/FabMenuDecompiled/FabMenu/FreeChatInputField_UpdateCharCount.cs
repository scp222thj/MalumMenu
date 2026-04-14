using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FabMenu;

[HarmonyPatch(typeof(FreeChatInputField), "UpdateCharCount")]
public static class FreeChatInputField_UpdateCharCount
{
	public static void Postfix(FreeChatInputField __instance)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.chatJailbreak)
		{
			int length = __instance.textArea.text.Length;
			((TMP_Text)__instance.charCountText).SetText($"{length}/{__instance.textArea.characterLimit}", true);
			if (length < 90)
			{
				((Graphic)__instance.charCountText).color = Color.black;
			}
			else if (length < 119)
			{
				((Graphic)__instance.charCountText).color = new Color(1f, 1f, 0f, 1f);
			}
			else
			{
				((Graphic)__instance.charCountText).color = Color.red;
			}
		}
	}
}
