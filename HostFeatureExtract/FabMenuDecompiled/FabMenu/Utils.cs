using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using BepInEx;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using Sentry.Internal.Extensions;
using TMPro;
using UnityEngine;

namespace FabMenu;

public static class Utils
{
	public class PanicCleaner : MonoBehaviour
	{
		public static void Create()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			ClassInjector.RegisterTypeInIl2Cpp<PanicCleaner>();
			new GameObject("FabMenu_PanicCleaner")
			{
				hideFlags = (HideFlags)61
			}.AddComponent<PanicCleaner>();
		}

		private void LateUpdate()
		{
			try
			{
				Harmony.UnpatchID("FabMenu");
			}
			catch
			{
			}
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}

	public static ReferenceDataManager referenceDataManager = DestroyableSingleton<ReferenceDataManager>.Instance;

	public const float DefaultSpeed = 2.5f;

	public const float DefaultGhostSpeed = 3f;

	public static Dictionary<string, Sprite> CachedSprites = new Dictionary<string, Sprite>();

	public static SabotageSystemType SabotageSystem => ((Il2CppObjectBase)ShipStatus.Instance.Systems[(SystemTypes)17]).Cast<SabotageSystemType>();

	public static bool isShip => Object.op_Implicit((Object)(object)ShipStatus.Instance);

	public static bool isLobby
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			if (Object.op_Implicit((Object)(object)AmongUsClient.Instance) && (int)((InnerNetClient)AmongUsClient.Instance).GameState == 1)
			{
				return !isFreePlay;
			}
			return false;
		}
	}

	public static bool isOnlineGame
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			if (Object.op_Implicit((Object)(object)AmongUsClient.Instance))
			{
				return (int)((InnerNetClient)AmongUsClient.Instance).NetworkMode == 1;
			}
			return false;
		}
	}

	public static bool isLocalGame
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			if (Object.op_Implicit((Object)(object)AmongUsClient.Instance))
			{
				return (int)((InnerNetClient)AmongUsClient.Instance).NetworkMode == 0;
			}
			return false;
		}
	}

	public static bool isFreePlay
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			if (Object.op_Implicit((Object)(object)AmongUsClient.Instance))
			{
				return (int)((InnerNetClient)AmongUsClient.Instance).NetworkMode == 2;
			}
			return false;
		}
	}

	public static bool isPlayer => Object.op_Implicit((Object)(object)PlayerControl.LocalPlayer);

	public static bool isHost
	{
		get
		{
			if (Object.op_Implicit((Object)(object)AmongUsClient.Instance))
			{
				return ((InnerNetClient)AmongUsClient.Instance).AmHost;
			}
			return false;
		}
	}

	public static bool isInGame
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			if (Object.op_Implicit((Object)(object)AmongUsClient.Instance) && (int)((InnerNetClient)AmongUsClient.Instance).GameState == 2)
			{
				return isPlayer;
			}
			return false;
		}
	}

	public static bool isMeeting => Object.op_Implicit((Object)(object)MeetingHud.Instance);

	public static bool isMeetingVoting
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			bool flag = isMeeting;
			if (flag)
			{
				VoteStates state = MeetingHud.Instance.state;
				bool flag2 = state - 2 <= 1;
				flag = flag2;
			}
			return flag;
		}
	}

	public static bool isMeetingProceeding
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			if (isMeeting)
			{
				return (int)MeetingHud.Instance.state == 5;
			}
			return false;
		}
	}

	public static bool isExiling
	{
		get
		{
			if (Object.op_Implicit((Object)(object)ExileController.Instance))
			{
				if (AirshipIsActive)
				{
					return !((Behaviour)Minigame.Instance).isActiveAndEnabled;
				}
				return true;
			}
			return false;
		}
	}

	public static bool isAnySabotageActive
	{
		get
		{
			if (Object.op_Implicit((Object)(object)ShipStatus.Instance))
			{
				return SabotageSystem.AnyActive;
			}
			return false;
		}
	}

	public static bool isNormalGame => (int)GameOptionsManager.Instance.CurrentGameOptions.GameMode == 1;

	public static bool isHideNSeek => (int)GameOptionsManager.Instance.CurrentGameOptions.GameMode == 2;

	public static bool SkeldIsActive => GameOptionsManager.Instance.CurrentGameOptions.MapId == 0;

	public static bool MiraHQIsActive => GameOptionsManager.Instance.CurrentGameOptions.MapId == 1;

	public static bool PolusIsActive => GameOptionsManager.Instance.CurrentGameOptions.MapId == 2;

	public static bool DleksIsActive => GameOptionsManager.Instance.CurrentGameOptions.MapId == 3;

	public static bool AirshipIsActive => GameOptionsManager.Instance.CurrentGameOptions.MapId == 4;

	public static bool FungleIsActive => GameOptionsManager.Instance.CurrentGameOptions.MapId == 5;

	public static bool isSpeedDefault(bool forGhost = false)
	{
		if (!forGhost)
		{
			return Mathf.Approximately(PlayerControl.LocalPlayer.MyPhysics.Speed, 2.5f);
		}
		return Mathf.Approximately(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed, 3f);
	}

	public static void snapSpeedToDefault(float snapRange, bool forGhost = false)
	{
		if (forGhost)
		{
			PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = ((Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed - 3f) < snapRange) ? 3f : PlayerControl.LocalPlayer.MyPhysics.GhostSpeed);
		}
		else
		{
			PlayerControl.LocalPlayer.MyPhysics.Speed = ((Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.Speed - 2.5f) < snapRange) ? 2.5f : PlayerControl.LocalPlayer.MyPhysics.Speed);
		}
	}

	public static ClientData getClientByPlayer(PlayerControl player)
	{
		try
		{
			return ((IEnumerable<ClientData>)((InnerNetClient)AmongUsClient.Instance).allClients.ToArray()).FirstOrDefault((ClientData cd) => cd.Character.PlayerId == player.PlayerId);
		}
		catch
		{
			return null;
		}
	}

	public static int getClientIdByPlayer(PlayerControl player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return -1;
		}
		ClientData clientByPlayer = getClientByPlayer(player);
		if (clientByPlayer != null)
		{
			return clientByPlayer.Id;
		}
		return -1;
	}

	public static bool isVanished(NetworkedPlayerInfo playerInfo)
	{
		RoleBehaviour role = playerInfo.Role;
		PhantomRole val = (PhantomRole)(object)((role is PhantomRole) ? role : null);
		if ((Object)(object)val != (Object)null)
		{
			if (!val.fading)
			{
				return val.isInvisible;
			}
			return true;
		}
		return false;
	}

	public static bool isValidTarget(NetworkedPlayerInfo target)
	{
		bool flag = Object.op_Implicit((Object)(object)target) && !target.Disconnected && target.Object.Visible && target.PlayerId != PlayerControl.LocalPlayer.PlayerId && Object.op_Implicit((Object)(object)target.Role) && Object.op_Implicit((Object)(object)target.Object);
		bool result = flag && !target.IsDead && !target.Object.inVent && !target.Object.inMovingPlat && target.Role.CanBeKilled;
		if (!CheatToggles.killAnyone)
		{
			return result;
		}
		return flag;
	}

	public static List<NetworkedPlayerInfo> GetAllPlayerData()
	{
		List<NetworkedPlayerInfo> val = new List<NetworkedPlayerInfo>();
		Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl current = enumerator.Current;
			if ((Object)(object)current != (Object)null && (Object)(object)current.Data != (Object)null)
			{
				val.Add(current.Data);
			}
		}
		return val;
	}

	public static void adjustResolution()
	{
		ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / (float)Screen.height, Screen.width, Screen.height, Screen.fullScreen);
	}

	public static RoleBehaviour getBehaviourByRoleType(RoleTypes roleType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return ((IEnumerable<RoleBehaviour>)DestroyableSingleton<RoleManager>.Instance.AllRoles.ToArray()).First((RoleBehaviour r) => r.Role == roleType);
	}

	public static void murderPlayer(PlayerControl target, MurderResultFlags result)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected I4, but got Unknown
		if (isFreePlay)
		{
			PlayerControl.LocalPlayer.MurderPlayer(target, (MurderResultFlags)1);
			return;
		}
		Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl current = enumerator.Current;
			MessageWriter val = ((InnerNetClient)AmongUsClient.Instance).StartRpcImmediately(((InnerNetObject)PlayerControl.LocalPlayer).NetId, (byte)12, (SendOption)0, ((InnerNetClient)AmongUsClient.Instance).GetClientIdFromCharacter(current));
			MessageExtensions.WriteNetObject(val, (InnerNetObject)(object)target);
			val.Write((int)result);
			((InnerNetClient)AmongUsClient.Instance).FinishRpcImmediately(val);
		}
	}

	public static void reportDeadBody(NetworkedPlayerInfo playerData)
	{
		if (isFreePlay)
		{
			PlayerControl.LocalPlayer.CmdReportDeadBody(playerData);
			return;
		}
		ClientData host = ((InnerNetClient)AmongUsClient.Instance).GetHost();
		if (host != null && !host.Character.Data.Disconnected)
		{
			MessageWriter val = ((InnerNetClient)AmongUsClient.Instance).StartRpcImmediately(((InnerNetObject)PlayerControl.LocalPlayer).NetId, (byte)11, (SendOption)0, host.Id);
			val.Write(playerData.PlayerId);
			((InnerNetClient)AmongUsClient.Instance).FinishRpcImmediately(val);
		}
	}

	public static void completeMyTasks()
	{
		Enumerator<PlayerTask> enumerator;
		if (isFreePlay)
		{
			enumerator = PlayerControl.LocalPlayer.myTasks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PlayerTask current = enumerator.Current;
				PlayerControl.LocalPlayer.RpcCompleteTask(current.Id);
			}
			return;
		}
		ClientData host = ((InnerNetClient)AmongUsClient.Instance).GetHost();
		if (host == null || host.Character.Data.Disconnected)
		{
			return;
		}
		enumerator = PlayerControl.LocalPlayer.myTasks.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerTask current2 = enumerator.Current;
			if (!current2.IsComplete)
			{
				Enumerator<PlayerControl> enumerator2 = PlayerControl.AllPlayerControls.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					PlayerControl current3 = enumerator2.Current;
					MessageWriter val = ((InnerNetClient)AmongUsClient.Instance).StartRpcImmediately(((InnerNetObject)PlayerControl.LocalPlayer).NetId, (byte)1, (SendOption)0, ((InnerNetClient)AmongUsClient.Instance).GetClientIdFromCharacter(current3));
					val.WritePacked(current2.Id);
					((InnerNetClient)AmongUsClient.Instance).FinishRpcImmediately(val);
				}
			}
		}
	}

	public static void completeTask(PlayerTask task)
	{
		if (isFreePlay)
		{
			PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
			return;
		}
		ClientData host = ((InnerNetClient)AmongUsClient.Instance).GetHost();
		if (host != null && !host.Character.Data.Disconnected && !task.IsComplete)
		{
			Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PlayerControl current = enumerator.Current;
				MessageWriter val = ((InnerNetClient)AmongUsClient.Instance).StartRpcImmediately(((InnerNetObject)PlayerControl.LocalPlayer).NetId, (byte)1, (SendOption)0, ((InnerNetClient)AmongUsClient.Instance).GetClientIdFromCharacter(current));
				val.WritePacked(task.Id);
				((InnerNetClient)AmongUsClient.Instance).FinishRpcImmediately(val);
			}
		}
	}

	public static void openChat()
	{
		if (!DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening)
		{
			DestroyableSingleton<HudManager>.Instance.Chat.chatScreen.SetActive(true);
			PlayerControl.LocalPlayer.NetTransform.Halt();
			((MonoBehaviour)DestroyableSingleton<HudManager>.Instance.Chat).StartCoroutine(DestroyableSingleton<HudManager>.Instance.Chat.CoOpen());
			if (DestroyableSingleton<FriendsListManager>.InstanceExists)
			{
				DestroyableSingleton<FriendsListManager>.Instance.SetFriendButtonColor(true);
			}
		}
	}

	public static void drawTracer(GameObject sourceObject, GameObject targetObject, Color color)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		LineRenderer val = sourceObject.GetComponent<LineRenderer>();
		if (!Object.op_Implicit((Object)(object)val))
		{
			val = sourceObject.AddComponent<LineRenderer>();
		}
		val.SetVertexCount(2);
		val.SetWidth(0.02f, 0.02f);
		Material playerMaterial = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;
		((Renderer)val).material = playerMaterial;
		val.SetColors(color, color);
		val.SetPosition(0, sourceObject.transform.position);
		val.SetPosition(1, targetObject.transform.position);
	}

	public static bool chatUiActive()
	{
		try
		{
			return CheatToggles.alwaysChat || Object.op_Implicit((Object)(object)MeetingHud.Instance) || !Object.op_Implicit((Object)(object)ShipStatus.Instance) || PlayerControl.LocalPlayer.Data.IsDead;
		}
		catch
		{
			return false;
		}
	}

	public static void closeChat()
	{
		if (DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening)
		{
			DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
		}
	}

	public static float getDistanceFrom(PlayerControl target, PlayerControl source = null)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (MiscExtensions.IsNull((Object)(object)source))
		{
			source = PlayerControl.LocalPlayer;
		}
		Vector2 val = target.GetTruePosition() - source.GetTruePosition();
		return ((Vector2)(ref val)).magnitude;
	}

	public static List<PlayerControl> getPlayersSortedByDistance(PlayerControl source = null)
	{
		if (MiscExtensions.IsNull((Object)(object)source))
		{
			source = PlayerControl.LocalPlayer;
		}
		List<PlayerControl> list = new List<PlayerControl>();
		list.Clear();
		Enumerator<NetworkedPlayerInfo> enumerator = GameData.Instance.AllPlayers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl val = enumerator.Current.Object;
			if (Object.op_Implicit((Object)(object)val))
			{
				list.Add(val);
			}
		}
		list = list.OrderBy((PlayerControl target) => getDistanceFrom(target, source)).ToList();
		if (list.Count > 0)
		{
			return list;
		}
		return null;
	}

	public static byte getCurrentMapID()
	{
		if (isFreePlay)
		{
			return (byte)AmongUsClient.Instance.TutorialMapId;
		}
		return GameOptionsManager.Instance.currentGameOptions.MapId;
	}

	public static SystemTypes getCurrentRoom()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return DestroyableSingleton<HudManager>.Instance.roomTracker.LastRoom.RoomId;
	}

	public static string getColoredPingText(int ping)
	{
		if (ping > 100)
		{
			if (ping < 400)
			{
				return $"<color=#ffff00ff>PING: {ping} ms</color>";
			}
			return $"<color=#ff0000ff>PING: {ping} ms</color>";
		}
		return $"<color=#00ff00ff>PING: {ping} ms</color>";
	}

	public static KeyCode stringToKeycode(string keyCodeStr)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(keyCodeStr))
		{
			try
			{
				return (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeStr, ignoreCase: true);
			}
			catch
			{
			}
		}
		return (KeyCode)127;
	}

	public static bool stringToPlatformType(string platformStr, out Platforms? platform)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(platformStr))
		{
			try
			{
				platform = (Platforms)Enum.Parse(typeof(Platforms), platformStr, ignoreCase: true);
				return true;
			}
			catch
			{
			}
		}
		platform = null;
		return false;
	}

	public static string PlatformTypeToString(Platforms platform)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected I4, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		switch (platform - 1)
		{
		default:
			if ((int)platform == 112)
			{
				return "Starlight";
			}
			return "Unknown";
		case 0:
			return "Epic";
		case 1:
			return "Steam";
		case 2:
			return "Mac";
		case 3:
			return "Microsoft Store";
		case 4:
			return "Itch.io";
		case 5:
			return "iPhone / iPad";
		case 6:
			return "Android";
		case 7:
			return "Nintendo Switch";
		case 8:
			return "Xbox";
		case 9:
			return "PlayStation";
		}
	}

	public static string getRoleName(NetworkedPlayerInfo playerData)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		string text = DestroyableSingleton<TranslationController>.Instance.GetString(playerData.Role.StringName, Il2CppArrayBase<Object>.op_Implicit(Array.Empty<Object>()));
		if (text != "STRMISS")
		{
			return text;
		}
		if (playerData.RoleWhenAlive.HasValue)
		{
			return DestroyableSingleton<TranslationController>.Instance.GetString(getBehaviourByRoleType(playerData.RoleWhenAlive.Value).StringName, Il2CppArrayBase<Object>.op_Implicit(Array.Empty<Object>()));
		}
		return "Ghost";
	}

	public static string GetNameTag(NetworkedPlayerInfo playerInfo, string playerName, bool isChat = false)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		string text = playerName;
		if (MiscExtensions.IsNull((Object)(object)playerInfo.Role) || MiscExtensions.IsNull((Object)(object)playerInfo) || playerInfo.Disconnected || MiscExtensions.IsNull((Object)(object)playerInfo.Object.CurrentOutfit))
		{
			return text;
		}
		ClientData clientFromPlayerInfo = ((InnerNetClient)AmongUsClient.Instance).GetClientFromPlayerInfo(playerInfo);
		ClientData host = ((InnerNetClient)AmongUsClient.Instance).GetHost();
		uint value = playerInfo.PlayerLevel + 1;
		string value2 = "Unknown";
		try
		{
			value2 = PlatformTypeToString(clientFromPlayerInfo.PlatformData.Platform);
		}
		catch
		{
		}
		string value3 = ColorUtility.ToHtmlStringRGB(playerInfo.Role.TeamColor);
		string value4 = ((clientFromPlayerInfo == host) ? "Host - " : "");
		if (CheatToggles.seeRoles)
		{
			if (CheatToggles.showPlayerInfo)
			{
				if (!isChat)
				{
					return $"<size=70%><color=#fb0>{value4}Lv:{value} - {value2}</color></size>\r\n<color=#{value3}><size=70%>{getRoleName(playerInfo)}</size>\r\n{text}</color>";
				}
				return $"<color=#{value3}>{text} <size=70%>{getRoleName(playerInfo)}</size></color> <size=70%><color=#fb0>{value4}Lv:{value} - {value2}</color></size>";
			}
			if (!isChat)
			{
				return $"<color=#{value3}><size=70%>{getRoleName(playerInfo)}</size>\r\n{text}</color>";
			}
			return $"<color=#{value3}>{text} <size=70%>{getRoleName(playerInfo)}</size></color>";
		}
		if (CheatToggles.showPlayerInfo)
		{
			if (PlayerControl.LocalPlayer.Data.Role.NameColor == playerInfo.Role.NameColor)
			{
				if (!isChat)
				{
					return $"<size=70%><color=#fb0>{value4}Lv:{value} - {value2}</color></size>\r\n<color=#{ColorUtility.ToHtmlStringRGB(playerInfo.Role.NameColor)}>{text}";
				}
				return $"<color=#{ColorUtility.ToHtmlStringRGB(playerInfo.Role.NameColor)}>{text}</color> <size=70%><color=#fb0>{value4}Lv:{value} - {value2}</color></size>";
			}
			if (!isChat)
			{
				return $"<size=70%><color=#fb0>{value4}Lv:{value} - {value2}</color></size>\r\n{text}";
			}
			return $"{text} <size=70%><color=#fb0>{value4}Lv:{value} - {value2}</color></size>";
		}
		if (PlayerControl.LocalPlayer.Data.Role.NameColor != playerInfo.Role.NameColor || isChat)
		{
			return text;
		}
		return $"<color=#{ColorUtility.ToHtmlStringRGB(playerInfo.Role.NameColor)}>{text}</color>";
	}

	public static string GetRandomName()
	{
		int count = Random.Range(1, 13);
		return new string((from s in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", count)
			select s[Random.Range(0, s.Length)]).ToArray());
	}

	public static void showPopup(string text)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		GenericPopup obj = Object.Instantiate<GenericPopup>(DestroyableSingleton<DiscordManager>.Instance.discordPopup, ((Component)Camera.main).transform);
		SpriteRenderer component = ((Component)((Component)obj).transform.Find("Background")).GetComponent<SpriteRenderer>();
		Vector2 size = component.size;
		size.x *= 2.5f;
		component.size = size;
		((TMP_Text)obj.TextAreaTMP).fontSizeMin = 2f;
		obj.Show(text);
	}

	public static void ShowNewPopup(string text)
	{
		DestroyableSingleton<DisconnectPopup>.Instance.ShowCustom(text);
	}

	public static Sprite LoadSprite(string path, float pixelsPerUnit = 1f)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Sprite result = default(Sprite);
			if (CachedSprites.TryGetValue(path + pixelsPerUnit, ref result))
			{
				return result;
			}
			Texture2D val = LoadTextureFromResources(path);
			result = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
			Sprite obj = result;
			((Object)obj).hideFlags = (HideFlags)(((Object)obj).hideFlags | 0x3D);
			return CachedSprites[path + pixelsPerUnit] = result;
		}
		catch
		{
			Debug.LogError(Object.op_Implicit("Failed to read Texture: " + path));
		}
		return null;
	}

	public static Texture2D LoadTextureFromResources(string path)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		try
		{
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
			Texture2D val = new Texture2D(1, 1, (TextureFormat)5, false);
			using MemoryStream memoryStream = new MemoryStream();
			manifestResourceStream.CopyTo(memoryStream);
			ImageConversion.LoadImage(val, Il2CppStructArray<byte>.op_Implicit(memoryStream.ToArray()), false);
			return val;
		}
		catch
		{
			Debug.LogError(Object.op_Implicit("Failed to read Texture: " + path));
		}
		return null;
	}

	public static void OpenConfigFile()
	{
		string text = Path.Combine(Paths.ConfigPath, "FabMenu.cfg");
		if (File.Exists(text))
		{
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = text,
					UseShellExecute = true,
					Verb = "edit"
				});
				return;
			}
			catch (Exception ex)
			{
				Debug.LogError(Object.op_Implicit("Failed to open config file: " + ex.Message + ". If you are on Linux, this is expected."));
				return;
			}
		}
		Debug.LogError(Object.op_Implicit("Config file does not exist."));
	}

	public static void Panic()
	{
		CheatToggles.DisableAll();
		((Renderer)DestroyableSingleton<ModManager>.Instance.ModStamp).enabled = false;
		PanicCleaner.Create();
	}
}
