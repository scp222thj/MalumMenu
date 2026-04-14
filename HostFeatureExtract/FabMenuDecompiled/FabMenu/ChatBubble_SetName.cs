using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace FabMenu;

[HarmonyPatch(typeof(ChatBubble), "SetName")]
public static class ChatBubble_SetName
{
	public static void Postfix(ChatBubble __instance)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		MalumESP.ChatNametags(__instance);
		if (!CheatToggles.chatDarkMode)
		{
			return;
		}
		try
		{
			if ((Object)(object)__instance.Background != (Object)null)
			{
				__instance.Background.color = new Color(0.08f, 0.08f, 0.08f, 0.95f);
			}
			if ((Object)(object)__instance.NameText != (Object)null)
			{
				((Graphic)__instance.NameText).color = Color.white;
			}
			if ((Object)(object)__instance.TextArea != (Object)null)
			{
				((Graphic)__instance.TextArea).color = Color.white;
			}
		}
		catch
		{
		}
	}
}
