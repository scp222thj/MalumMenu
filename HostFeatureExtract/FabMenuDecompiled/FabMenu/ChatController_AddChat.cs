using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AmongUs.Data;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace FabMenu;

[HarmonyPatch(typeof(ChatController), "AddChat")]
public static class ChatController_AddChat
{
	public static bool Prefix(PlayerControl sourcePlayer, string chatText, bool censor, ChatController __instance)
	{
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(chatText) && chatText.StartsWith("[FABMOD]"))
		{
			try
			{
				string[] array = chatText.Split('|');
				if (array.Length >= 4)
				{
					string text = array[1];
					if (!int.TryParse(array[2], out var targetId))
					{
						return false;
					}
					string s = array[3];
					if (text == "REQ")
					{
						if (Utils.isHost)
						{
							try
							{
								if (CheatToggles.LastBroadcastKillCooldown >= 0f)
								{
									string text2 = "[FABMOD]|KILLCD|" + array[2] + "|" + CheatToggles.LastBroadcastKillCooldown.ToString(CultureInfo.InvariantCulture);
									PlayerControl.LocalPlayer.RpcSendChat(text2);
								}
								if (CheatToggles.LastBroadcastSpeed >= 0f)
								{
									string text3 = "[FABMOD]|SPEED|" + array[2] + "|" + CheatToggles.LastBroadcastSpeed.ToString(CultureInfo.InvariantCulture);
									PlayerControl.LocalPlayer.RpcSendChat(text3);
								}
							}
							catch
							{
							}
						}
						return false;
					}
					if (targetId == -1)
					{
						float result2;
						if (text == "KILLCD")
						{
							if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
							{
								try
								{
									CheatToggles.LastBroadcastKillCooldown = result;
									Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
									while (enumerator.MoveNext())
									{
										PlayerControl current = enumerator.Current;
										try
										{
											current.SetKillTimer(result);
										}
										catch
										{
										}
									}
								}
								catch
								{
								}
							}
						}
						else if (text == "SPEED" && float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out result2))
						{
							try
							{
								CheatToggles.LastBroadcastSpeed = result2;
								Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
								while (enumerator.MoveNext())
								{
									PlayerControl current2 = enumerator.Current;
									try
									{
										if ((Object)(object)current2 != (Object)null && (Object)(object)current2.MyPhysics != (Object)null)
										{
											current2.MyPhysics.Speed = result2;
											current2.MyPhysics.GhostSpeed = result2;
										}
									}
									catch
									{
									}
								}
							}
							catch
							{
							}
						}
						return false;
					}
					PlayerControl val = ((IEnumerable<PlayerControl>)PlayerControl.AllPlayerControls.ToArray()).FirstOrDefault((PlayerControl p) => p.PlayerId == targetId);
					if ((Object)(object)val == (Object)null || (Object)(object)val.Data == (Object)null || val.Data.Disconnected)
					{
						return false;
					}
					if (text == "NAME")
					{
						try
						{
							string text4 = Encoding.UTF8.GetString(Convert.FromBase64String(s));
							if (text4.Length > 10)
							{
								text4 = text4.Substring(0, 10);
							}
							PlayerOutfit defaultOutfit = val.Data.DefaultOutfit;
							defaultOutfit.PlayerName = text4;
							val.Data.Outfits[(PlayerOutfitType)0] = defaultOutfit;
							if ((Object)(object)val != (Object)null && (Object)(object)val.cosmetics != (Object)null)
							{
								val.cosmetics.SetName(text4);
							}
						}
						catch
						{
						}
					}
					else if (text == "COLOR")
					{
						try
						{
							if (int.TryParse(s, out var result3))
							{
								result3 = Mathf.Clamp(result3, 0, ((Il2CppArrayBase<Color32>)(object)Palette.PlayerColors).Length - 1);
								PlayerOutfit defaultOutfit2 = val.Data.DefaultOutfit;
								defaultOutfit2.ColorId = result3;
								val.Data.Outfits[(PlayerOutfitType)0] = defaultOutfit2;
								if ((Object)(object)val != (Object)null)
								{
									val.SetColor(result3);
								}
							}
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
			return false;
		}
		if (!CheatToggles.seeGhosts || PlayerControl.LocalPlayer.Data.IsDead)
		{
			return true;
		}
		if (!Object.op_Implicit((Object)(object)sourcePlayer) || !Object.op_Implicit((Object)(object)PlayerControl.LocalPlayer))
		{
			return true;
		}
		NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
		NetworkedPlayerInfo data2 = sourcePlayer.Data;
		if ((Object)(object)data2 == (Object)null || (Object)(object)data == (Object)null)
		{
			return true;
		}
		ChatBubble pooledBubble = __instance.GetPooledBubble();
		try
		{
			((Component)pooledBubble).transform.SetParent(__instance.scroller.Inner);
			((Component)pooledBubble).transform.localScale = Vector3.one;
			bool flag = (Object)(object)sourcePlayer == (Object)(object)PlayerControl.LocalPlayer;
			if (flag)
			{
				pooledBubble.SetRight();
			}
			else
			{
				pooledBubble.SetLeft();
			}
			bool flag2 = Object.op_Implicit((Object)(object)MeetingHud.Instance) && MeetingHud.Instance.DidVote(sourcePlayer.PlayerId);
			pooledBubble.SetCosmetics(data2);
			__instance.SetChatBubbleName(pooledBubble, data2, data2.IsDead, flag2, PlayerNameColor.Get(data2), (GetFormattedNameFunc)null);
			if (censor && DataManager.Settings.Multiplayer.CensorChat)
			{
				chatText = BlockedWords.CensorWords(chatText, false);
			}
			pooledBubble.SetText(chatText);
			if (CheatToggles.chatDarkMode)
			{
				try
				{
					if ((Object)(object)pooledBubble.Background != (Object)null)
					{
						pooledBubble.Background.color = new Color(0.08f, 0.08f, 0.08f, 0.95f);
					}
					if ((Object)(object)pooledBubble.NameText != (Object)null)
					{
						((Graphic)pooledBubble.NameText).color = Color.white;
					}
					if ((Object)(object)pooledBubble.TextArea != (Object)null)
					{
						((Graphic)pooledBubble.TextArea).color = Color.white;
					}
				}
				catch
				{
				}
			}
			pooledBubble.AlignChildren();
			__instance.AlignAllBubbles();
			if (!__instance.IsOpenOrOpening && __instance.notificationRoutine == null)
			{
				__instance.notificationRoutine = ((MonoBehaviour)__instance).StartCoroutine(__instance.BounceDot());
			}
			if (!flag)
			{
				SoundManager.Instance.PlaySound(__instance.messageSound, false, 1f, (AudioMixerGroup)null).pitch = 0.5f + (float)(int)sourcePlayer.PlayerId / 15f;
				__instance.chatNotification.SetUp(sourcePlayer, chatText);
			}
		}
		catch (Exception ex)
		{
			ChatController.Logger.Error(Object.op_Implicit(ex.ToString()), (Object)null);
			((IObjectPool)__instance.chatBubblePool).Reclaim((PoolableBehavior)(object)pooledBubble);
		}
		return false;
	}
}
