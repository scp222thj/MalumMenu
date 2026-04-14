using HarmonyLib;

namespace FabMenu;

[HarmonyPatch(typeof(ChatController), "Update")]
public static class ChatController_Update
{
	public static void Postfix(ChatController __instance)
	{
		__instance.freeChatField.textArea.allowAllCharacters = CheatToggles.chatJailbreak;
		__instance.freeChatField.textArea.AllowSymbols = true;
		__instance.freeChatField.textArea.AllowEmail = CheatToggles.chatJailbreak;
		if (CheatToggles.chatJailbreak)
		{
			__instance.freeChatField.textArea.characterLimit = 119;
		}
		else
		{
			__instance.freeChatField.textArea.characterLimit = 100;
		}
	}
}
