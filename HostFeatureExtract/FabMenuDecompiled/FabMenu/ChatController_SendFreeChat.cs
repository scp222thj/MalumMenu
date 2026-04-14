using System.Text.RegularExpressions;
using HarmonyLib;
using Il2CppSystem;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(ChatController), "SendFreeChat")]
public static class ChatController_SendFreeChat
{
	public static bool Prefix(ChatController __instance)
	{
		if (!CheatToggles.chatJailbreak)
		{
			return true;
		}
		string text = CensorUrlsAndEmails(__instance.freeChatField.Text);
		ChatController.Logger.Debug(Object.op_Implicit("SendFreeChat () :: Sending message: '" + text + "'"), (Object)null);
		PlayerControl.LocalPlayer.RpcSendChat(text);
		return false;
	}

	private static string CensorUrlsAndEmails(string text)
	{
		return new Regex("(http[s]?://)?([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}(/[\\w-./?%&=]*)?|([a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+)").Replace(text, (Match match) => match.Value.Replace('.', ','));
	}
}
