using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using UnityEngine;

namespace FabMenu;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct CheatToggles
{
	public class KeybindListener : MonoBehaviour
	{
		public FabMenu Plugin { get; internal set; }

		public void Update()
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			if (MenuUI.isPanicked || (DestroyableSingleton<HudManager>.InstanceExists && Object.op_Implicit((Object)(object)DestroyableSingleton<HudManager>.Instance.Chat) && DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening))
			{
				return;
			}
			if (reloadConfig)
			{
				((BasePlugin)Plugin).Config.Reload();
				((BasePlugin)Plugin).Log.LogInfo((object)"Plugin config reloaded.");
				reloadConfig = false;
			}
			foreach (var (key, val2) in Keybinds)
			{
				if ((int)val2 != 0 && Input.GetKeyDown(val2) && ToggleFields.TryGetValue(key, out var value))
				{
					bool flag = (bool)value.GetValue(null);
					value.SetValue(null, !flag);
				}
			}
		}
	}

	public static bool noClip;

	public static bool speedBoost;

	public static bool teleportPlayer;

	public static bool teleportCursor;

	public static bool reportBody;

	public static bool ejectPlayer;

	public static bool killPlayer;

	public static bool telekillPlayer;

	public static bool killAll;

	public static bool killAllCrew;

	public static bool spamKillAll;

	public static float spamTimer;

	public static bool killAllImps;

	public static bool revive;

	public static bool protectPlayer;

	public static bool invertControls;

	public static bool moonwalk;

	public static bool skipRoleReveal;

	public static bool isKillSwitched;

	public static string killSwitchUrl;

	public static bool changeRole;

	public static bool zeroKillCd;

	public static bool showTasksMenu;

	public static bool completeMyTasks;

	public static bool killReach;

	public static bool killAnyone;

	public static bool endlessSsDuration;

	public static bool endlessBattery;

	public static bool endlessTracking;

	public static bool noTrackingCooldown;

	public static bool noTrackingDelay;

	public static bool trackReach;

	public static bool interrogateReach;

	public static bool noVitalsCooldown;

	public static bool noVentCooldown;

	public static bool endlessVentTime;

	public static bool endlessVanish;

	public static bool killVanished;

	public static bool noVanishAnim;

	public static bool noShapeshiftAnim;

	public static bool fullBright;

	public static bool seeGhosts;

	public static bool seeRoles;

	public static bool showPlayerInfo;

	public static bool seeDisguises;

	public static bool showTaskArrows;

	public static bool revealVotes;

	public static bool moreLobbyInfo;

	public static bool spectate;

	public static bool zoomOut;

	public static bool freecam;

	public static bool mapCrew;

	public static bool mapImps;

	public static bool mapGhosts;

	public static bool colorBasedMap;

	public static bool tracersImps;

	public static bool tracersCrew;

	public static bool tracersGhosts;

	public static bool tracersBodies;

	public static bool colorBasedTracers;

	public static bool distanceBasedTracers;

	public static bool alwaysChat;

	public static bool chatJailbreak;

	public static bool chatDarkMode;

	public static bool closeMeeting;

	public static bool sabotageMap;

	public static bool openAllDoors;

	public static bool closeAllDoors;

	public static bool spamOpenAllDoors;

	public static bool spamCloseAllDoors;

	public static bool autoOpenDoorsOnUse;

	public static bool destroyShip;

	public static bool unfixableLights;

	public static bool commsSab;

	public static bool elecSab;

	public static bool reactorSab;

	public static bool oxygenSab;

	public static bool mushSab;

	public static bool mushSpore;

	public static bool showDoorsMenu;

	public static bool useVents;

	public static bool walkVent;

	public static bool kickVents;

	public static bool voteImmune;

	public static bool skipMeeting;

	public static bool callMeeting;

	public static bool forceStartGame;

	public static bool noGameEnd;

	public static bool noOptionsLimits;

	public static bool modifyPlayerName;

	public static bool modifyPlayerColor;

	public static bool unlockFeatures;

	public static bool freeCosmetics;

	public static bool avoidBans;

	public static bool copyLobbyCodeOnDisconnect;

	public static bool spoofAprilFoolsDate;

	public static bool stealthMode;

	public static bool panic;

	public static bool animShields;

	public static bool animAsteroids;

	public static bool animEmptyGarbage;

	public static bool animScan;

	public static bool animCamsInUse;

	public static bool reloadConfig;

	public static bool RGBMode;

	public static float LastBroadcastKillCooldown;

	public static float LastBroadcastSpeed;

	public static readonly Dictionary<string, KeyCode> Keybinds;

	private static readonly Dictionary<string, FieldInfo> ToggleFields;

	public static readonly string ProfilePath;

	static CheatToggles()
	{
		spamTimer = 0f;
		isKillSwitched = false;
		killSwitchUrl = "https://pastebin.com/raw/LK1SkeDh";
		LastBroadcastKillCooldown = -1f;
		LastBroadcastSpeed = -1f;
		Keybinds = new Dictionary<string, KeyCode>();
		ToggleFields = new Dictionary<string, FieldInfo>();
		ProfilePath = Path.Combine(Paths.ConfigPath, "FabMenuProfile.txt");
		FieldInfo[] fields = typeof(CheatToggles).GetFields(BindingFlags.Static | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!(fieldInfo.FieldType != typeof(bool)))
			{
				ToggleFields[fieldInfo.Name] = fieldInfo;
				Keybinds[fieldInfo.Name] = (KeyCode)0;
			}
		}
	}

	public static void DisablePPMCheats(string variableToKeep)
	{
		ejectPlayer = variableToKeep == "ejectPlayer" && ejectPlayer;
		reportBody = variableToKeep == "reportBody" && reportBody;
		killPlayer = variableToKeep == "killPlayer" && killPlayer;
		telekillPlayer = variableToKeep == "telekillPlayer" && telekillPlayer;
		spectate = variableToKeep == "spectate" && spectate;
		changeRole = variableToKeep == "changeRole" && changeRole;
		teleportPlayer = variableToKeep == "teleportPlayer" && teleportPlayer;
		protectPlayer = variableToKeep == "protectPlayer" && protectPlayer;
	}

	public static bool shouldPPMClose()
	{
		if (!changeRole && !ejectPlayer && !reportBody && !telekillPlayer && !killPlayer && !spectate && !teleportPlayer)
		{
			return !protectPlayer;
		}
		return false;
	}

	public static void DisableAll()
	{
		foreach (FieldInfo value in ToggleFields.Values)
		{
			value.SetValue(null, false);
		}
	}

	public static void SaveTogglesToProfile()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		using StreamWriter streamWriter = new StreamWriter(ProfilePath);
		streamWriter.WriteLine("# FabMenuProfile");
		streamWriter.WriteLine("# Format: ToggleName = True/False = KeyCode.Foo");
		streamWriter.WriteLine("# - List of keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html");
		streamWriter.WriteLine("# - KeyCode part is optional; use KeyCode.None for no key");
		streamWriter.WriteLine("# - Multiple toggles may have the same key, but multiple keys per toggle are NOT supported");
		streamWriter.WriteLine("# - Keybinds are only applied after loading this profile by pressing 'Load from Profile' (Config category)");
		streamWriter.WriteLine();
		foreach (FieldInfo value2 in ToggleFields.Values)
		{
			Keybinds.TryGetValue(value2.Name, out var value);
			streamWriter.WriteLine($"{value2.Name} = {value2.GetValue(null)} = KeyCode.{value}");
		}
	}

	public static void LoadTogglesFromProfile()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (!File.Exists(ProfilePath))
		{
			return;
		}
		using StreamReader streamReader = new StreamReader(ProfilePath);
		while (true)
		{
			string text = streamReader.ReadLine();
			if (text == null)
			{
				break;
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				continue;
			}
			text = text.Trim();
			if (text.StartsWith("#"))
			{
				continue;
			}
			string[] array = text.Split('=', 3);
			if (array.Length < 2)
			{
				continue;
			}
			string key = array[0].Trim();
			if (!ToggleFields.TryGetValue(key, out var value))
			{
				continue;
			}
			if (bool.TryParse(array[1].Trim(), out var result))
			{
				value.SetValue(null, result);
			}
			KeyCode value2 = (KeyCode)0;
			if (array.Length >= 3)
			{
				string text2 = array[2].Trim();
				if (text2.StartsWith("KeyCode."))
				{
					string text3 = text2;
					int length = "KeyCode.".Length;
					text2 = text3.Substring(length, text3.Length - length);
				}
				if (!string.IsNullOrEmpty(text2) && Enum.TryParse<KeyCode>(text2, true, out KeyCode result2))
				{
					value2 = result2;
				}
			}
			Keybinds[key] = value2;
		}
	}
}
