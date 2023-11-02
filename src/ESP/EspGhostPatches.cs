using HarmonyLib;
using UnityEngine;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ESP_PlayerGhostPostfix
{
	//Postfix patch of PlayerPhysics.LateUpdate to render all ghosts visible if CheatSettings.seeGhosts is enabled or if LocalPlayer is dead
    public static void Postfix(PlayerPhysics __instance){
        try{
            if(__instance.myPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsDead){
                __instance.myPlayer.Visible = CheatSettings.seeGhosts;
            }    
        }catch{}
    }
}    

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ProtectPlayer))]
public static class ESP_ProtectPlayerPrefix
{
	//Prefix patch of PlayerControl.ProtectPlayer to render all protections visible if CheatSettings.seeGhosts is enabled or if LocalPlayer is dead
	//Basically does what the original method did with the required modifications
    public static bool Prefix(PlayerControl target, int colorId, PlayerControl __instance){
		__instance.logger.Debug(string.Format("{0} protected {1}", __instance.PlayerId, target.PlayerId), null);
		if (__instance.AmOwner)
		{
			__instance.Data.Role.SetCooldown();
		}

		target.TurnOnProtection(CheatSettings.seeGhosts || PlayerControl.LocalPlayer.Data.IsDead, colorId, 0); //Render protection animation if LocalPlayer dead or CheatSettings.seeGhosts
    
        return false; //Skips the original method completly
    }
}    

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class ESP_GhostChatPrefix
{
	//Prefix patch of ChatController.AddChat to receive ghost messages if CheatSettings.seeGhosts is enabled even if LocalPlayer is alive
	//Basically does what the original method did with the required modifications
    public static bool Prefix(PlayerControl sourcePlayer, string chatText, bool censor, ChatController __instance)
    {
        if (!CheatSettings.seeGhosts || PlayerControl.LocalPlayer.Data.IsDead){
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