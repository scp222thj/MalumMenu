using HarmonyLib;
using System;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class ChatController_AddChat
{
	//Prefix patch of ChatController.AddChat to receive ghost messages if CheatSettings.seeGhosts is enabled even if LocalPlayer is alive
	//Basically does what the original method did with the required modifications
    public static bool Prefix(PlayerControl sourcePlayer, string chatText, bool censor, ChatController __instance)
    {
        if (!CheatToggles.seeGhosts || PlayerControl.LocalPlayer.Data.IsDead){
            return true; //Run original method if seeGhosts is disabled or LocalPlayer already dead
        }

        if (!sourcePlayer || !PlayerControl.LocalPlayer)
		{
			return false;
		}

		GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
		GameData.PlayerInfo data2 = sourcePlayer.Data;

		if (data2 == null || data == null) //Remove isDead check for LocalPlayer
		{
			return false;
		}

		ChatBubble pooledBubble = __instance.GetPooledBubble();

		try
		{
			pooledBubble.transform.SetParent(__instance.scroller.Inner);
			pooledBubble.transform.localScale = Vector3.one;
			bool flag = sourcePlayer == PlayerControl.LocalPlayer;
			if (flag)
			{
				pooledBubble.SetRight();
			}
			else
			{
				pooledBubble.SetLeft();
			}
			bool didVote = MeetingHud.Instance && MeetingHud.Instance.DidVote(sourcePlayer.PlayerId);
			pooledBubble.SetCosmetics(data2);
			__instance.SetChatBubbleName(pooledBubble, data2, data2.IsDead, didVote, PlayerNameColor.Get(data2), null);
			if (censor && AmongUs.Data.DataManager.Settings.Multiplayer.CensorChat)
			{
				chatText = BlockedWords.CensorWords(chatText, false);
			}
			pooledBubble.SetText(chatText);
			pooledBubble.AlignChildren();
			__instance.AlignAllBubbles();
			if (!__instance.IsOpenOrOpening && __instance.notificationRoutine == null)
			{
				__instance.notificationRoutine = __instance.StartCoroutine(__instance.BounceDot());
			}
			if (!flag)
			{
				SoundManager.Instance.PlaySound(__instance.messageSound, false, 1f, null).pitch = 0.5f + (float)sourcePlayer.PlayerId / 15f;
			}
		}
		catch (Exception message)
		{
			ChatController.Logger.Error(message.ToString(), null);
			__instance.chatBubblePool.Reclaim(pooledBubble);
		}
        
        return false; //Skips the original method completly
    }
}

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
public static class ChatBubble_SetName
{
    //Postfix patch of ChatBubble.SetName to get colored names in chat messages
    public static void Postfix(ChatBubble __instance){
        MalumESP.chatNametags(__instance);
    }
}    