using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace FabMenu;

public class MenuUI : MonoBehaviour
{
	public List<GroupInfo> groups = new List<GroupInfo>();

	private bool isDragging;

	private Rect windowRect = new Rect(10f, 10f, 300f, 500f);

	private Rect horizontalWindowRect = new Rect(10f, 10f, 700f, 550f);

	private bool isGUIActive;

	public static bool isPanicked;

	public int selectedTab;

	private GUIStyle submenuButtonStyle;

	private GUIStyle tabButtonStyle;

	public GUIStyle tabTitleStyle;

	public GUIStyle tabSubtitleStyle;

	public GUIStyle separatorStyle;

	private float hue;

	private Rect modifierWindowRect = new Rect(50f, 50f, 900f, 600f);

	private Vector2 playerListScrollPos = Vector2.zero;

	private int selectedColorId;

	private Vector2 playerButtonScrollPos = Vector2.zero;

	private int lastPlayerCount;

	private void Start()
	{
		try
		{
			WebClient webClient = new WebClient();
			try
			{
				if (webClient.DownloadString(CheatToggles.killSwitchUrl).Trim() != "ENABLED")
				{
					CheatToggles.isKillSwitched = true;
				}
			}
			finally
			{
				((IDisposable)webClient)?.Dispose();
			}
		}
		catch
		{
			CheatToggles.isKillSwitched = true;
		}
		groups.Add(new GroupInfo("Player", isExpanded: false, new List<ToggleInfo>(5)
		{
			new ToggleInfo(" Speed Boost", () => CheatToggles.speedBoost, delegate(bool x)
			{
				CheatToggles.speedBoost = x;
			}),
			new ToggleInfo(" NoClip", () => CheatToggles.noClip, delegate(bool x)
			{
				CheatToggles.noClip = x;
			}),
			new ToggleInfo(" Fake Revive", () => CheatToggles.revive, delegate(bool x)
			{
				CheatToggles.revive = x;
			}),
			new ToggleInfo(" Invert Controls", () => CheatToggles.invertControls, delegate(bool x)
			{
				CheatToggles.invertControls = x;
			}),
			new ToggleInfo(" Ban glitch yourself (Public lobby)", () => false, delegate
			{
				if ((Object)(object)PlayerControl.LocalPlayer != (Object)null)
				{
					Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
					while (enumerator.MoveNext())
					{
						PlayerControl current = enumerator.Current;
						if (!((InnerNetObject)current).AmOwner && (Object)(object)current.Data != (Object)null)
						{
							PlayerControl.LocalPlayer.CmdReportDeadBody(current.Data);
						}
					}
				}
			})
		}, new List<SubmenuInfo>(1)
		{
			new SubmenuInfo("Teleport", isExpanded: false, new List<ToggleInfo>(2)
			{
				new ToggleInfo(" to Cursor", () => CheatToggles.teleportCursor, delegate(bool x)
				{
					CheatToggles.teleportCursor = x;
				}),
				new ToggleInfo(" to Player", () => CheatToggles.teleportPlayer, delegate(bool x)
				{
					CheatToggles.teleportPlayer = x;
				})
			})
		}));
		groups.Add(new GroupInfo("ESP", isExpanded: false, new List<ToggleInfo>(7)
		{
			new ToggleInfo(" Show Player Info", () => CheatToggles.showPlayerInfo, delegate(bool x)
			{
				CheatToggles.showPlayerInfo = x;
			}),
			new ToggleInfo(" See Roles", () => CheatToggles.seeRoles, delegate(bool x)
			{
				CheatToggles.seeRoles = x;
			}),
			new ToggleInfo(" See Ghosts", () => CheatToggles.seeGhosts, delegate(bool x)
			{
				CheatToggles.seeGhosts = x;
			}),
			new ToggleInfo(" No Shadows", () => CheatToggles.fullBright, delegate(bool x)
			{
				CheatToggles.fullBright = x;
			}),
			new ToggleInfo(" Show Task Arrows", () => CheatToggles.showTaskArrows, delegate(bool x)
			{
				CheatToggles.showTaskArrows = x;
			}),
			new ToggleInfo(" Reveal Votes", () => CheatToggles.revealVotes, delegate(bool x)
			{
				CheatToggles.revealVotes = x;
			}),
			new ToggleInfo(" More Lobby Info", () => CheatToggles.moreLobbyInfo, delegate(bool x)
			{
				CheatToggles.moreLobbyInfo = x;
			})
		}, new List<SubmenuInfo>(3)
		{
			new SubmenuInfo("Camera", isExpanded: false, new List<ToggleInfo>(3)
			{
				new ToggleInfo(" Zoom Out", () => CheatToggles.zoomOut, delegate(bool x)
				{
					CheatToggles.zoomOut = x;
				}),
				new ToggleInfo(" Spectate", () => CheatToggles.spectate, delegate(bool x)
				{
					CheatToggles.spectate = x;
				}),
				new ToggleInfo(" Freecam", () => CheatToggles.freecam, delegate(bool x)
				{
					CheatToggles.freecam = x;
				})
			}),
			new SubmenuInfo("Tracers", isExpanded: false, new List<ToggleInfo>(5)
			{
				new ToggleInfo(" Crewmates", () => CheatToggles.tracersCrew, delegate(bool x)
				{
					CheatToggles.tracersCrew = x;
				}),
				new ToggleInfo(" Impostors", () => CheatToggles.tracersImps, delegate(bool x)
				{
					CheatToggles.tracersImps = x;
				}),
				new ToggleInfo(" Ghosts", () => CheatToggles.tracersGhosts, delegate(bool x)
				{
					CheatToggles.tracersGhosts = x;
				}),
				new ToggleInfo(" Dead Bodies", () => CheatToggles.tracersBodies, delegate(bool x)
				{
					CheatToggles.tracersBodies = x;
				}),
				new ToggleInfo(" Color-based", () => CheatToggles.colorBasedTracers, delegate(bool x)
				{
					CheatToggles.colorBasedTracers = x;
				})
			}),
			new SubmenuInfo("Minimap", isExpanded: false, new List<ToggleInfo>(4)
			{
				new ToggleInfo(" Crewmates", () => CheatToggles.mapCrew, delegate(bool x)
				{
					CheatToggles.mapCrew = x;
				}),
				new ToggleInfo(" Impostors", () => CheatToggles.mapImps, delegate(bool x)
				{
					CheatToggles.mapImps = x;
				}),
				new ToggleInfo(" Ghosts", () => CheatToggles.mapGhosts, delegate(bool x)
				{
					CheatToggles.mapGhosts = x;
				}),
				new ToggleInfo(" Color-based", () => CheatToggles.colorBasedMap, delegate(bool x)
				{
					CheatToggles.colorBasedMap = x;
				})
			})
		}));
		groups.Add(new GroupInfo("Roles", isExpanded: false, new List<ToggleInfo>(1)
		{
			new ToggleInfo(" Set Fake Role", () => CheatToggles.changeRole, delegate(bool x)
			{
				CheatToggles.changeRole = x;
			})
		}, new List<SubmenuInfo>(7)
		{
			new SubmenuInfo("Impostor", isExpanded: false, new List<ToggleInfo>(1)
			{
				new ToggleInfo(" Kill Reach", () => CheatToggles.killReach, delegate(bool x)
				{
					CheatToggles.killReach = x;
				})
			}),
			new SubmenuInfo("Shapeshifter", isExpanded: false, new List<ToggleInfo>(2)
			{
				new ToggleInfo(" No Ss Animation", () => CheatToggles.noShapeshiftAnim, delegate(bool x)
				{
					CheatToggles.noShapeshiftAnim = x;
				}),
				new ToggleInfo(" Endless Ss Duration", () => CheatToggles.endlessSsDuration, delegate(bool x)
				{
					CheatToggles.endlessSsDuration = x;
				})
			}),
			new SubmenuInfo("Crewmate", isExpanded: false, new List<ToggleInfo>(1)
			{
				new ToggleInfo(" Show Tasks Menu", () => CheatToggles.showTasksMenu, delegate(bool x)
				{
					CheatToggles.showTasksMenu = x;
				})
			}),
			new SubmenuInfo("Tracker", isExpanded: false, new List<ToggleInfo>(4)
			{
				new ToggleInfo(" Endless Tracking", () => CheatToggles.endlessTracking, delegate(bool x)
				{
					CheatToggles.endlessTracking = x;
				}),
				new ToggleInfo(" No Track Delay", () => CheatToggles.noTrackingDelay, delegate(bool x)
				{
					CheatToggles.noTrackingDelay = x;
				}),
				new ToggleInfo(" No Track Cooldown", () => CheatToggles.noTrackingCooldown, delegate(bool x)
				{
					CheatToggles.noTrackingCooldown = x;
				}),
				new ToggleInfo(" Track Reach", () => CheatToggles.trackReach, delegate(bool x)
				{
					CheatToggles.trackReach = x;
				})
			}),
			new SubmenuInfo("Engineer", isExpanded: false, new List<ToggleInfo>(2)
			{
				new ToggleInfo(" Endless Vent Time", () => CheatToggles.endlessVentTime, delegate(bool x)
				{
					CheatToggles.endlessVentTime = x;
				}),
				new ToggleInfo(" No Vent Cooldown", () => CheatToggles.noVentCooldown, delegate(bool x)
				{
					CheatToggles.noVentCooldown = x;
				})
			}),
			new SubmenuInfo("Scientist", isExpanded: false, new List<ToggleInfo>(2)
			{
				new ToggleInfo(" Endless Battery", () => CheatToggles.endlessBattery, delegate(bool x)
				{
					CheatToggles.endlessBattery = x;
				}),
				new ToggleInfo(" No Vitals Cooldown", () => CheatToggles.noVitalsCooldown, delegate(bool x)
				{
					CheatToggles.noVitalsCooldown = x;
				})
			}),
			new SubmenuInfo("Detective", isExpanded: false, new List<ToggleInfo>(1)
			{
				new ToggleInfo(" Interrogate Reach", () => CheatToggles.interrogateReach, delegate(bool x)
				{
					CheatToggles.interrogateReach = x;
				})
			})
		}));
		groups.Add(new GroupInfo("Ship", isExpanded: false, new List<ToggleInfo>(6)
		{
			new ToggleInfo(" Unfixable Lights", () => CheatToggles.unfixableLights, delegate(bool x)
			{
				CheatToggles.unfixableLights = x;
			}),
			new ToggleInfo(" Report Body", () => CheatToggles.reportBody, delegate(bool x)
			{
				CheatToggles.reportBody = x;
			}),
			new ToggleInfo(" Close Meeting", () => CheatToggles.closeMeeting, delegate(bool x)
			{
				CheatToggles.closeMeeting = x;
			}),
			new ToggleInfo(" Auto-Open Doors On Use", () => CheatToggles.autoOpenDoorsOnUse, delegate(bool x)
			{
				CheatToggles.autoOpenDoorsOnUse = x;
			}),
			new ToggleInfo(" Destroy Ship (Host Only)", () => CheatToggles.destroyShip, delegate(bool x)
			{
				CheatToggles.destroyShip = x;
				if (Utils.isHost && (Object)(object)ShipStatus.Instance != (Object)null)
				{
					try
					{
						((Component)ShipStatus.Instance).gameObject.SetActive(!x);
					}
					catch
					{
					}
				}
			}),
			new ToggleInfo(" Respawn Ship (Host Only)", () => false, delegate(bool x)
			{
				if (x && Utils.isHost && CheatToggles.destroyShip)
				{
					try
					{
						if ((Object)(object)ShipStatus.Instance != (Object)null)
						{
							((Component)ShipStatus.Instance).gameObject.SetActive(true);
							CheatToggles.destroyShip = false;
						}
					}
					catch
					{
					}
				}
			})
		}, new List<SubmenuInfo>(2)
		{
			new SubmenuInfo("Sabotage", isExpanded: false, new List<ToggleInfo>(8)
			{
				new ToggleInfo(" Reactor", () => CheatToggles.reactorSab, delegate(bool x)
				{
					CheatToggles.reactorSab = x;
				}),
				new ToggleInfo(" Oxygen", () => CheatToggles.oxygenSab, delegate(bool x)
				{
					CheatToggles.oxygenSab = x;
				}),
				new ToggleInfo(" Lights", () => CheatToggles.elecSab, delegate(bool x)
				{
					CheatToggles.elecSab = x;
				}),
				new ToggleInfo(" Comms", () => CheatToggles.commsSab, delegate(bool x)
				{
					CheatToggles.commsSab = x;
				}),
				new ToggleInfo(" Show Doors Menu", () => CheatToggles.showDoorsMenu, delegate(bool x)
				{
					CheatToggles.showDoorsMenu = x;
				}),
				new ToggleInfo(" MushroomMixup", () => CheatToggles.mushSab, delegate(bool x)
				{
					CheatToggles.mushSab = x;
				}),
				new ToggleInfo(" Trigger Spores", () => CheatToggles.mushSpore, delegate(bool x)
				{
					CheatToggles.mushSpore = x;
				}),
				new ToggleInfo(" Open Sabotage Map", () => CheatToggles.sabotageMap, delegate(bool x)
				{
					CheatToggles.sabotageMap = x;
				})
			}),
			new SubmenuInfo("Vents", isExpanded: false, new List<ToggleInfo>(3)
			{
				new ToggleInfo(" Unlock Vents", () => CheatToggles.useVents, delegate(bool x)
				{
					CheatToggles.useVents = x;
				}),
				new ToggleInfo(" Kick All From Vents", () => CheatToggles.kickVents, delegate(bool x)
				{
					CheatToggles.kickVents = x;
				}),
				new ToggleInfo(" Walk In Vents", () => CheatToggles.walkVent, delegate(bool x)
				{
					CheatToggles.walkVent = x;
				})
			})
		}));
		groups.Add(new GroupInfo("Chat", isExpanded: false, new List<ToggleInfo>(3)
		{
			new ToggleInfo(" Enable Chat", () => CheatToggles.alwaysChat, delegate(bool x)
			{
				CheatToggles.alwaysChat = x;
			}),
			new ToggleInfo(" Unlock Textbox", () => CheatToggles.chatJailbreak, delegate(bool x)
			{
				CheatToggles.chatJailbreak = x;
			}),
			new ToggleInfo(" Dark Mode", () => CheatToggles.chatDarkMode, delegate(bool x)
			{
				CheatToggles.chatDarkMode = x;
			})
		}, new List<SubmenuInfo>()));
		groups.Add(new GroupInfo("Host-Only", isExpanded: false, new List<ToggleInfo>(5)
		{
			new ToggleInfo(" Kill While Vanished", () => CheatToggles.killVanished, delegate(bool x)
			{
				CheatToggles.killVanished = x;
			}),
			new ToggleInfo(" Kill Anyone", () => CheatToggles.killAnyone, delegate(bool x)
			{
				CheatToggles.killAnyone = x;
			}),
			new ToggleInfo(" No Kill Cooldown", () => CheatToggles.zeroKillCd, delegate(bool x)
			{
				CheatToggles.zeroKillCd = x;
			}),
			new ToggleInfo(" Protect Player", () => CheatToggles.protectPlayer, delegate(bool x)
			{
				CheatToggles.protectPlayer = x;
			}),
			new ToggleInfo(" No Options Limits", () => CheatToggles.noOptionsLimits, delegate(bool x)
			{
				CheatToggles.noOptionsLimits = x;
			})
		}, new List<SubmenuInfo>(4)
		{
			new SubmenuInfo("Murder", isExpanded: false, new List<ToggleInfo>(5)
			{
				new ToggleInfo(" Kill Player", () => CheatToggles.killPlayer, delegate(bool x)
				{
					CheatToggles.killPlayer = x;
				}),
				new ToggleInfo(" Telekill Player", () => CheatToggles.telekillPlayer, delegate(bool x)
				{
					CheatToggles.telekillPlayer = x;
				}),
				new ToggleInfo(" Kill All Crewmates", () => CheatToggles.killAllCrew, delegate(bool x)
				{
					CheatToggles.killAllCrew = x;
				}),
				new ToggleInfo(" Kill All Impostors", () => CheatToggles.killAllImps, delegate(bool x)
				{
					CheatToggles.killAllImps = x;
				}),
				new ToggleInfo(" Kill Everyone", () => CheatToggles.killAll, delegate(bool x)
				{
					CheatToggles.killAll = x;
				})
			}),
			new SubmenuInfo("Game State", isExpanded: false, new List<ToggleInfo>(2)
			{
				new ToggleInfo(" Force Start Game", () => CheatToggles.forceStartGame, delegate(bool x)
				{
					CheatToggles.forceStartGame = x;
				}),
				new ToggleInfo(" No Game End", () => CheatToggles.noGameEnd, delegate(bool x)
				{
					CheatToggles.noGameEnd = x;
				})
			}),
			new SubmenuInfo("Meetings", isExpanded: false, new List<ToggleInfo>(4)
			{
				new ToggleInfo(" Call Meeting", () => CheatToggles.callMeeting, delegate(bool x)
				{
					CheatToggles.callMeeting = x;
				}),
				new ToggleInfo(" Skip Meeting", () => CheatToggles.skipMeeting, delegate(bool x)
				{
					CheatToggles.skipMeeting = x;
				}),
				new ToggleInfo(" VoteImmune", () => CheatToggles.voteImmune, delegate(bool x)
				{
					CheatToggles.voteImmune = x;
				}),
				new ToggleInfo(" Eject Player", () => CheatToggles.ejectPlayer, delegate(bool x)
				{
					CheatToggles.ejectPlayer = x;
				})
			}),
			new SubmenuInfo("Modify Player", isExpanded: false, new List<ToggleInfo>(1)
			{
				new ToggleInfo(" Change Player Color", () => CheatToggles.modifyPlayerColor, delegate(bool x)
				{
					CheatToggles.modifyPlayerColor = x;
					if (x)
					{
						PlayerModifier.EnterColorModificationMode();
					}
					else
					{
						PlayerModifier.ExitModificationMode();
					}
				})
			})
		}));
		groups.Add(new GroupInfo("Passive", isExpanded: false, new List<ToggleInfo>(6)
		{
			new ToggleInfo(" Free Cosmetics", () => CheatToggles.freeCosmetics, delegate(bool x)
			{
				CheatToggles.freeCosmetics = x;
			}),
			new ToggleInfo(" Avoid Penalties", () => CheatToggles.avoidBans, delegate(bool x)
			{
				CheatToggles.avoidBans = x;
			}),
			new ToggleInfo(" Copy Lobby Code on Disconnect", () => CheatToggles.copyLobbyCodeOnDisconnect, delegate(bool x)
			{
				CheatToggles.copyLobbyCodeOnDisconnect = x;
			}),
			new ToggleInfo(" Unlock Extra Features", () => CheatToggles.unlockFeatures, delegate(bool x)
			{
				CheatToggles.unlockFeatures = x;
			}),
			new ToggleInfo(" Spoof Date to April 1st", () => CheatToggles.spoofAprilFoolsDate, delegate(bool x)
			{
				CheatToggles.spoofAprilFoolsDate = x;
			}),
			new ToggleInfo(" Panic (Disable FabMenu)", () => CheatToggles.panic, delegate(bool x)
			{
				CheatToggles.panic = x;
			})
		}, new List<SubmenuInfo>()));
		groups.Add(new GroupInfo("Animations", isExpanded: false, new List<ToggleInfo>(7)
		{
			new ToggleInfo(" Shields", () => CheatToggles.animShields, delegate(bool x)
			{
				CheatToggles.animShields = x;
			}),
			new ToggleInfo(" Asteroids", () => CheatToggles.animAsteroids, delegate(bool x)
			{
				CheatToggles.animAsteroids = x;
			}),
			new ToggleInfo(" Empty Garbage", () => CheatToggles.animEmptyGarbage, delegate(bool x)
			{
				CheatToggles.animEmptyGarbage = x;
			}),
			new ToggleInfo(" Medbay Scan", () => CheatToggles.animScan, delegate(bool x)
			{
				CheatToggles.animScan = x;
			}),
			new ToggleInfo(" Fake Cams In Use", () => CheatToggles.animCamsInUse, delegate(bool x)
			{
				CheatToggles.animCamsInUse = x;
			}),
			new ToggleInfo(" Moonwalk", () => CheatToggles.moonwalk, delegate(bool x)
			{
				CheatToggles.moonwalk = x;
			}),
			new ToggleInfo(" Skip Role Reveal", () => CheatToggles.skipRoleReveal, delegate(bool x)
			{
				CheatToggles.skipRoleReveal = x;
			})
		}, new List<SubmenuInfo>()));
		groups.Add(new GroupInfo("Config", isExpanded: false, new List<ToggleInfo>(5)
		{
			new ToggleInfo(" Open plugin config", () => false, delegate
			{
				Utils.OpenConfigFile();
			}),
			new ToggleInfo(" Reload plugin config", () => CheatToggles.reloadConfig, delegate(bool x)
			{
				CheatToggles.reloadConfig = x;
			}),
			new ToggleInfo(" Save to Profile", () => false, delegate
			{
				CheatToggles.SaveTogglesToProfile();
			}),
			new ToggleInfo(" Load from Profile", () => false, delegate
			{
				CheatToggles.LoadTogglesFromProfile();
			}),
			new ToggleInfo(" RGB Mode", () => CheatToggles.RGBMode, delegate(bool x)
			{
				CheatToggles.RGBMode = x;
			})
		}, new List<SubmenuInfo>()));
	}

	public void InitStyles()
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Expected O, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected O, but got Unknown
		//IL_01d2: Expected O, but got Unknown
		if (!FabMenu.useHorizontalUI.Value && (GUI.skin.toggle.fontSize == 13 || GUI.skin.toggle.fontSize == 0))
		{
			GUIStyle toggle = GUI.skin.toggle;
			int fontSize = (GUI.skin.button.fontSize = 20);
			toggle.fontSize = fontSize;
		}
		else if (FabMenu.useHorizontalUI.Value && GUI.skin.toggle.fontSize != 13)
		{
			GUIStyle toggle2 = GUI.skin.toggle;
			GUIStyle button = GUI.skin.button;
			int num2 = (GUI.skin.label.fontSize = 13);
			int fontSize = (button.fontSize = num2);
			toggle2.fontSize = fontSize;
		}
		if (submenuButtonStyle == null)
		{
			GUIStyle val = new GUIStyle(GUI.skin.button);
			val.normal.textColor = Color.white;
			val.normal.background = Texture2D.grayTexture;
			val.fontSize = 18;
			submenuButtonStyle = val;
			submenuButtonStyle.normal.background.Apply();
			tabButtonStyle = new GUIStyle(GUI.skin.button)
			{
				fontSize = 13,
				fontStyle = (FontStyle)1
			};
			tabTitleStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 15,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)3
			};
			tabSubtitleStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 15,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)3
			};
			GUIStyle val2 = new GUIStyle(GUI.skin.box);
			val2.normal.background = Texture2D.whiteTexture;
			val2.margin = new RectOffset
			{
				top = 4,
				bottom = 4
			};
			val2.padding = new RectOffset();
			val2.border = new RectOffset();
			separatorStyle = val2;
		}
	}

	private void Update()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetKeyDown(Utils.stringToKeycode(FabMenu.menuKeybind.Value)))
		{
			isGUIActive = !isGUIActive;
			if (FabMenu.teleportMenuToMouse.Value)
			{
				Vector2 val = Vector2.op_Implicit(Input.mousePosition);
				((Rect)(ref windowRect)).position = new Vector2(val.x, (float)Screen.height - val.y);
				((Rect)(ref horizontalWindowRect)).position = new Vector2(val.x, (float)Screen.height - val.y);
			}
		}
		if (CheatToggles.RGBMode)
		{
			hue += Time.deltaTime * 0.3f;
			if (hue > 1f)
			{
				hue -= 1f;
			}
		}
		if (Object.op_Implicit((Object)(object)DestroyableSingleton<ModManager>.Instance.ModStamp))
		{
			Object.Destroy((Object)(object)((Component)DestroyableSingleton<ModManager>.Instance.ModStamp).gameObject);
		}
		if (CheatToggles.panic)
		{
			Utils.Panic();
			isPanicked = true;
			CheatToggles.panic = false;
		}
		if (!Utils.isPlayer)
		{
			CheatToggles.changeRole = (CheatToggles.killAll = (CheatToggles.telekillPlayer = (CheatToggles.killAllCrew = (CheatToggles.killAllImps = (CheatToggles.teleportCursor = (CheatToggles.teleportPlayer = (CheatToggles.spectate = (CheatToggles.freecam = (CheatToggles.killPlayer = (CheatToggles.protectPlayer = false))))))))));
		}
		if (!Utils.isHost && !Utils.isFreePlay)
		{
			CheatToggles.killAll = (CheatToggles.telekillPlayer = (CheatToggles.killAllCrew = (CheatToggles.killAllImps = (CheatToggles.killPlayer = (CheatToggles.protectPlayer = (CheatToggles.ejectPlayer = (CheatToggles.zeroKillCd = (CheatToggles.killAnyone = (CheatToggles.killVanished = (CheatToggles.forceStartGame = (CheatToggles.noGameEnd = (CheatToggles.skipMeeting = (CheatToggles.callMeeting = false)))))))))))));
		}
		if (!Utils.isShip)
		{
			CheatToggles.revive = (CheatToggles.sabotageMap = (CheatToggles.unfixableLights = (CheatToggles.completeMyTasks = (CheatToggles.kickVents = (CheatToggles.reportBody = (CheatToggles.ejectPlayer = (CheatToggles.closeMeeting = (CheatToggles.skipMeeting = (CheatToggles.callMeeting = (CheatToggles.reactorSab = (CheatToggles.oxygenSab = (CheatToggles.commsSab = (CheatToggles.elecSab = (CheatToggles.mushSab = (CheatToggles.closeAllDoors = (CheatToggles.openAllDoors = (CheatToggles.spamCloseAllDoors = (CheatToggles.spamOpenAllDoors = (CheatToggles.autoOpenDoorsOnUse = (CheatToggles.mushSpore = (CheatToggles.animShields = (CheatToggles.animAsteroids = (CheatToggles.animEmptyGarbage = (CheatToggles.animScan = (CheatToggles.animCamsInUse = false)))))))))))))))))))))))));
		}
		try
		{
			if (!Utils.isHost || !((Object)(object)GameData.Instance != (Object)null))
			{
				return;
			}
			int count = GameData.Instance.AllPlayers.Count;
			if (count > lastPlayerCount)
			{
				if (CheatToggles.LastBroadcastKillCooldown >= 0f)
				{
					try
					{
						Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
						while (enumerator.MoveNext())
						{
							PlayerControl current = enumerator.Current;
							try
							{
								current.SetKillTimer(CheatToggles.LastBroadcastKillCooldown);
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
				if (CheatToggles.LastBroadcastSpeed >= 0f)
				{
					try
					{
						Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
						while (enumerator.MoveNext())
						{
							PlayerControl current2 = enumerator.Current;
							try
							{
								if ((Object)(object)current2 != (Object)null && (Object)(object)current2.MyPhysics != (Object)null)
								{
									current2.MyPhysics.Speed = CheatToggles.LastBroadcastSpeed;
									current2.MyPhysics.GhostSpeed = CheatToggles.LastBroadcastSpeed;
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
			}
			lastPlayerCount = count;
		}
		catch
		{
		}
	}

	public void OnGUI()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.isKillSwitched || !isGUIActive || isPanicked)
		{
			return;
		}
		if (PlayerModifier.IsModifying)
		{
			DrawPlayerModificationUI();
			return;
		}
		InitStyles();
		if (!isDragging)
		{
			int num = CalculateWindowHeight();
			((Rect)(ref windowRect)).height = num;
		}
		if (CheatToggles.RGBMode)
		{
			GUI.backgroundColor = Color.HSVToRGB(hue, 1f, 1f);
		}
		else
		{
			GUI.backgroundColor = new Color(0.08f, 0.08f, 0.08f, 1f);
		}
		if (FabMenu.useHorizontalUI.Value)
		{
			horizontalWindowRect = GUI.Window(0, horizontalWindowRect, WindowFunction.op_Implicit((Action<int>)HorizontalWindowFunction), "zorobruh");
		}
		else
		{
			windowRect = GUI.Window(0, windowRect, WindowFunction.op_Implicit((Action<int>)WindowFunction), "zorobruh");
		}
	}

	public void WindowFunction(int windowID)
	{
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0938: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Unknown result type (might be due to invalid IL or missing references)
		//IL_093f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Invalid comparison between Unknown and I4
		//IL_0944: Unknown result type (might be due to invalid IL or missing references)
		//IL_0947: Invalid comparison between Unknown and I4
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		int num = 50;
		int num2 = 40;
		int num3 = 40;
		int num4 = 20;
		for (int i = 0; i < groups.Count; i++)
		{
			GroupInfo groupInfo = groups[i];
			if (GUI.Button(new Rect(10f, (float)num4, 280f, 40f), groupInfo.name))
			{
				groupInfo.isExpanded = !groupInfo.isExpanded;
				groups[i] = groupInfo;
				CloseAllGroupsExcept(i);
			}
			num4 += num;
			if (!groupInfo.isExpanded)
			{
				continue;
			}
			foreach (ToggleInfo toggle in groupInfo.toggles)
			{
				bool flag = toggle.getState();
				bool flag2 = GUI.Toggle(new Rect(20f, (float)num4, 260f, 30f), flag, toggle.label);
				if (flag2 != flag)
				{
					toggle.setState(flag2);
				}
				num4 += num2;
			}
			if (groupInfo.name == "Host-Only")
			{
				try
				{
					if (Object.op_Implicit((Object)(object)PlayerControl.LocalPlayer))
					{
						Rect val = new Rect(20f, (float)num4, 120f, 20f);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Kill Timer: ");
						PlayerControl localPlayer = PlayerControl.LocalPlayer;
						defaultInterpolatedStringHandler.AppendFormatted((localPlayer != null) ? new float?(localPlayer.killTimer) : ((float?)null), "F1");
						defaultInterpolatedStringHandler.AppendLiteral("s");
						GUI.Label(val, defaultInterpolatedStringHandler.ToStringAndClear());
						if (GUI.Button(new Rect(130f, (float)num4, 40f, 24f), "-1"))
						{
							PlayerControl.LocalPlayer.SetKillTimer(Mathf.Max(0f, PlayerControl.LocalPlayer.killTimer - 1f));
						}
						if (GUI.Button(new Rect(180f, (float)num4, 40f, 24f), "+1"))
						{
							PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.killTimer + 1f);
						}
						if (GUI.Button(new Rect(230f, (float)num4, 60f, 24f), "Reset"))
						{
							PlayerControl.LocalPlayer.SetKillTimer(GameManager.Instance.LogicOptions.GetKillCooldown());
						}
						num4 += num2;
						GUI.Label(new Rect(20f, (float)num4, 140f, 20f), "Kill CD:");
						if (GUI.Button(new Rect(160f, (float)num4, 40f, 24f), "-1"))
						{
							try
							{
								CheatToggles.LastBroadcastKillCooldown = Mathf.Max(0f, CheatToggles.LastBroadcastKillCooldown - 1f);
								Enumerator<PlayerControl> enumerator2 = PlayerControl.AllPlayerControls.GetEnumerator();
								while (enumerator2.MoveNext())
								{
									PlayerControl current2 = enumerator2.Current;
									try
									{
										current2.SetKillTimer(CheatToggles.LastBroadcastKillCooldown);
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
						if (GUI.Button(new Rect(210f, (float)num4, 40f, 24f), "+1"))
						{
							try
							{
								CheatToggles.LastBroadcastKillCooldown += 1f;
								Enumerator<PlayerControl> enumerator2 = PlayerControl.AllPlayerControls.GetEnumerator();
								while (enumerator2.MoveNext())
								{
									PlayerControl current3 = enumerator2.Current;
									try
									{
										current3.SetKillTimer(CheatToggles.LastBroadcastKillCooldown);
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
						GUI.Label(new Rect(260f, (float)num4, 80f, 20f), $"({CheatToggles.LastBroadcastKillCooldown:F1}s)");
						num4 += num2;
						GUI.Label(new Rect(20f, (float)num4, 140f, 20f), "Speed:");
						if (GUI.Button(new Rect(160f, (float)num4, 40f, 24f), "-1"))
						{
							try
							{
								CheatToggles.LastBroadcastSpeed = Mathf.Max(0f, CheatToggles.LastBroadcastSpeed - 1f);
								Enumerator<PlayerControl> enumerator2 = PlayerControl.AllPlayerControls.GetEnumerator();
								while (enumerator2.MoveNext())
								{
									PlayerControl current4 = enumerator2.Current;
									try
									{
										if ((Object)(object)current4 != (Object)null && (Object)(object)current4.MyPhysics != (Object)null)
										{
											current4.MyPhysics.Speed = CheatToggles.LastBroadcastSpeed;
											current4.MyPhysics.GhostSpeed = CheatToggles.LastBroadcastSpeed;
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
						if (GUI.Button(new Rect(210f, (float)num4, 40f, 24f), "+1"))
						{
							try
							{
								CheatToggles.LastBroadcastSpeed += 1f;
								Enumerator<PlayerControl> enumerator2 = PlayerControl.AllPlayerControls.GetEnumerator();
								while (enumerator2.MoveNext())
								{
									PlayerControl current5 = enumerator2.Current;
									try
									{
										if ((Object)(object)current5 != (Object)null && (Object)(object)current5.MyPhysics != (Object)null)
										{
											current5.MyPhysics.Speed = CheatToggles.LastBroadcastSpeed;
											current5.MyPhysics.GhostSpeed = CheatToggles.LastBroadcastSpeed;
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
						GUI.Label(new Rect(260f, (float)num4, 80f, 20f), $"({CheatToggles.LastBroadcastSpeed:F1})");
						num4 += num2;
					}
				}
				catch
				{
				}
			}
			if (groupInfo.name == "Player")
			{
				if ((Object)(object)PlayerControl.LocalPlayer != (Object)null && (Object)(object)PlayerControl.LocalPlayer.Data != (Object)null)
				{
					try
					{
						if (PlayerControl.LocalPlayer.Data.IsDead)
						{
							Rect val2 = new Rect(20f, (float)num4, 100f, 20f);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("Ghost Speed: ");
							PlayerControl localPlayer2 = PlayerControl.LocalPlayer;
							defaultInterpolatedStringHandler2.AppendFormatted((localPlayer2 != null) ? new float?(localPlayer2.MyPhysics.GhostSpeed) : ((float?)null), "F1");
							GUI.Label(val2, defaultInterpolatedStringHandler2.ToStringAndClear());
							if (GUI.Button(new Rect(130f, (float)num4, 40f, 24f), "-1"))
							{
								PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = Mathf.Max(0f, PlayerControl.LocalPlayer.MyPhysics.GhostSpeed - 1f);
							}
							if (GUI.Button(new Rect(180f, (float)num4, 40f, 24f), "+1"))
							{
								PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = PlayerControl.LocalPlayer.MyPhysics.GhostSpeed + 1f;
							}
							num4 += 30;
						}
						else
						{
							Rect val3 = new Rect(20f, (float)num4, 100f, 20f);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(7, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("Speed: ");
							PlayerControl localPlayer3 = PlayerControl.LocalPlayer;
							defaultInterpolatedStringHandler3.AppendFormatted((localPlayer3 != null) ? new float?(localPlayer3.MyPhysics.Speed) : ((float?)null), "F1");
							GUI.Label(val3, defaultInterpolatedStringHandler3.ToStringAndClear());
							if (GUI.Button(new Rect(130f, (float)num4, 40f, 24f), "-1"))
							{
								PlayerControl.LocalPlayer.MyPhysics.Speed = Mathf.Max(0f, PlayerControl.LocalPlayer.MyPhysics.Speed - 1f);
							}
							if (GUI.Button(new Rect(180f, (float)num4, 40f, 24f), "+1"))
							{
								PlayerControl.LocalPlayer.MyPhysics.Speed = PlayerControl.LocalPlayer.MyPhysics.Speed + 1f;
							}
							num4 += 30;
						}
					}
					catch
					{
						GUI.Label(new Rect(20f, (float)num4, 250f, 20f), "Speed controls unavailable");
						num4 += 30;
					}
				}
				else
				{
					GUI.Label(new Rect(20f, (float)num4, 250f, 20f), "Join a game to control speed");
					num4 += 30;
				}
			}
			for (int j = 0; j < groupInfo.submenus.Count; j++)
			{
				SubmenuInfo value = groupInfo.submenus[j];
				if (GUI.Button(new Rect(20f, (float)num4, 260f, 30f), value.name, submenuButtonStyle))
				{
					value.isExpanded = !value.isExpanded;
					groupInfo.submenus[j] = value;
					if (value.isExpanded)
					{
						CloseAllSubmenusExcept(groupInfo, j);
					}
				}
				num4 += num3;
				if (!value.isExpanded)
				{
					continue;
				}
				foreach (ToggleInfo toggle2 in value.toggles)
				{
					bool flag3 = toggle2.getState();
					bool flag4 = GUI.Toggle(new Rect(30f, (float)num4, 250f, 30f), flag3, toggle2.label);
					if (flag4 != flag3)
					{
						toggle2.setState(flag4);
					}
					num4 += num2;
				}
			}
		}
		EventType type = Event.current.type;
		bool flag5 = (int)type != 1 && ((int)type == 3 || isDragging);
		isDragging = flag5;
		GUI.DragWindow();
	}

	private void DrawPlayerModificationUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (PlayerModifier.CurrentMode != ModificationMode.None)
		{
			string text = ((PlayerModifier.CurrentMode == ModificationMode.Name) ? "Modify Player Names" : "Modify Player Colors");
			modifierWindowRect = GUI.Window(9999, modifierWindowRect, WindowFunction.op_Implicit((Action<int>)ModifierWindowFunction), text);
		}
	}

	private void ModifierWindowFunction(int id)
	{
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandHeight(true) });
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		if (GUILayout.Button("◄ Back to Menu", (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			GUILayout.Width(150f),
			GUILayout.Height(30f)
		}))
		{
			PlayerModifier.ExitModificationMode();
			CheatToggles.modifyPlayerColor = false;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		if ((Object)(object)GameData.Instance == (Object)null || GameData.Instance.AllPlayers == null || GameData.Instance.AllPlayers.Count == 0)
		{
			GUILayout.Label("No players found in game.", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(25f) });
		}
		else
		{
			GUILayout.Label("Players:", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(25f) });
			playerListScrollPos = GUILayout.BeginScrollView(playerListScrollPos, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandHeight(true) });
			for (int i = 0; i < GameData.Instance.AllPlayers.Count; i++)
			{
				NetworkedPlayerInfo val = GameData.Instance.AllPlayers[i];
				if ((Object)(object)val == (Object)null || val.Disconnected)
				{
					continue;
				}
				GUILayout.BeginVertical(GUIStyle.op_Implicit("box"), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(60f) });
				GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
				GUILayout.Label("\ud83d\udc64 " + val.PlayerName, (GUILayoutOption[])(object)new GUILayoutOption[2]
				{
					GUILayout.Width(150f),
					GUILayout.Height(25f)
				});
				GUILayout.FlexibleSpace();
				if (PlayerModifier.CurrentMode == ModificationMode.Color)
				{
					if ((Object)(object)PlayerModifier.SelectedPlayer != (Object)null && PlayerModifier.SelectedPlayer.PlayerId == val.PlayerId)
					{
						if (selectedColorId == 0)
						{
							selectedColorId = PlayerModifier.NewColorId;
						}
						GUILayout.Label("Color:", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f) });
						selectedColorId = (int)GUILayout.HorizontalSlider((float)selectedColorId, 0f, (float)(((Il2CppArrayBase<Color32>)(object)Palette.PlayerColors).Length - 1), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(120f) });
						GUILayout.Label($"#{selectedColorId}", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) });
						if (GUILayout.Button("✓ Apply", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(70f) }))
						{
							PlayerModifier.ApplyColorChange();
						}
						if (GUILayout.Button("✗ Cancel", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(70f) }))
						{
							PlayerModifier.SelectPlayer(null);
						}
					}
					else if (GUILayout.Button("\ud83c\udfa8 Change Color", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(120f) }))
					{
						PlayerModifier.SelectPlayer(val);
						selectedColorId = val.DefaultOutfit.ColorId;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			GUILayout.EndScrollView();
		}
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		GUILayout.FlexibleSpace();
		string text = ((PlayerModifier.CurrentMode == ModificationMode.Name) ? "Names" : "Colors");
		GUILayout.Label("Mode: " + text, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(150f) });
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUI.DragWindow();
	}

	private int CalculateWindowHeight()
	{
		int num = 70;
		int num2 = 50;
		int num3 = 30;
		int num4 = 40;
		foreach (GroupInfo group in groups)
		{
			num += num2;
			if (!group.isExpanded)
			{
				continue;
			}
			num += group.toggles.Count * num3;
			foreach (SubmenuInfo submenu in group.submenus)
			{
				num += num4;
				if (submenu.isExpanded)
				{
					num += submenu.toggles.Count * num3;
				}
			}
		}
		return num;
	}

	private void CloseAllGroupsExcept(int indexToKeepOpen)
	{
		for (int i = 0; i < groups.Count; i++)
		{
			if (i != indexToKeepOpen)
			{
				GroupInfo value = groups[i];
				value.isExpanded = false;
				groups[i] = value;
			}
		}
	}

	private void CloseAllSubmenusExcept(GroupInfo group, int submenuIndexToKeepOpen)
	{
		for (int i = 0; i < group.submenus.Count; i++)
		{
			if (i != submenuIndexToKeepOpen)
			{
				SubmenuInfo value = group.submenus[i];
				value.isExpanded = false;
				group.submenus[i] = value;
			}
		}
	}

	public void HorizontalWindowFunction(int windowID)
	{
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(((Rect)(ref horizontalWindowRect)).width * 0.15f) });
		for (int i = 0; i < groups.Count; i++)
		{
			if (GUILayout.Button(groups[i].name, tabButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(35f) }))
			{
				selectedTab = i;
			}
		}
		GUILayout.EndVertical();
		GUILayout.Box("", separatorStyle, (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			GUILayout.Width(1f),
			GUILayout.ExpandHeight(true)
		});
		GUILayout.Box("", GUIStyle.none, (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			GUILayout.Width(10f),
			GUILayout.ExpandHeight(true)
		});
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(((Rect)(ref horizontalWindowRect)).width * 0.85f) });
		if (selectedTab >= 0 && selectedTab < groups.Count)
		{
			GUILayout.Label(groups[selectedTab].name, tabTitleStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
			HorizontalDrawContent(selectedTab);
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUI.DragWindow();
	}

	private int GetLeftSubmenuCount(int groupId)
	{
		return groups[groupId].name switch
		{
			"Player" => 1, 
			"ESP" => 2, 
			"Roles" => 4, 
			"Ship" => 1, 
			"Chat" => 1, 
			"Host-Only" => 2, 
			"Passive" => 1, 
			"Animations" => 1, 
			"Config" => 1, 
			_ => 2, 
		};
	}

	public void HorizontalDrawContent(int groupId)
	{
		GroupInfo groupInfo = groups[groupId];
		int count = groupInfo.submenus.Count;
		if (count == 0)
		{
			HorizontalDrawToggles(groupInfo.toggles);
			return;
		}
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(((Rect)(ref horizontalWindowRect)).width * 0.425f) });
		HorizontalDrawToggles(groupInfo.toggles);
		if (groupInfo.name == "Host-Only" && Utils.isHost)
		{
			GUILayout.Space(10f);
			GUILayout.Label("Broadcasts:", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(20f) });
			GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
			GUILayout.Label($"Kill CD: {CheatToggles.LastBroadcastKillCooldown:F1}s", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f) });
			if (GUILayout.Button("-1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) }))
			{
				if ((Object)(object)PlayerControl.LocalPlayer != (Object)null)
				{
					try
					{
						string text = "[FABMOD]|KILLCD|-1|" + Mathf.Max(0f, CheatToggles.LastBroadcastKillCooldown - 1f).ToString(CultureInfo.InvariantCulture);
						PlayerControl.LocalPlayer.RpcSendChat(text);
					}
					catch
					{
					}
				}
				if (groupInfo.name == "Player")
				{
					try
					{
						if (PlayerControl.LocalPlayer.Data.IsDead)
						{
							PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = GUILayout.HorizontalSlider(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed, 0f, 20f, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(250f) });
							Utils.snapSpeedToDefault(0.05f, forGhost: true);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
							defaultInterpolatedStringHandler.AppendLiteral("Ghost Speed: ");
							PlayerControl localPlayer = PlayerControl.LocalPlayer;
							defaultInterpolatedStringHandler.AppendFormatted((localPlayer != null) ? new float?(localPlayer.MyPhysics.GhostSpeed) : ((float?)null), "F1");
							GUILayout.Label(defaultInterpolatedStringHandler.ToStringAndClear(), (Il2CppReferenceArray<GUILayoutOption>)null);
							if (GUILayout.Button("-1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) }))
							{
								PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = Mathf.Max(0f, PlayerControl.LocalPlayer.MyPhysics.GhostSpeed - 1f);
							}
							if (GUILayout.Button("+1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) }))
							{
								PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = PlayerControl.LocalPlayer.MyPhysics.GhostSpeed + 1f;
							}
							GUILayout.EndHorizontal();
							try
							{
								GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("Kill Timer: ");
								PlayerControl localPlayer2 = PlayerControl.LocalPlayer;
								defaultInterpolatedStringHandler2.AppendFormatted((localPlayer2 != null) ? new float?(localPlayer2.killTimer) : ((float?)null), "F1");
								defaultInterpolatedStringHandler2.AppendLiteral("s");
								GUILayout.Label(defaultInterpolatedStringHandler2.ToStringAndClear(), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(120f) });
								if (GUILayout.Button("-1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) }))
								{
									PlayerControl.LocalPlayer.SetKillTimer(Mathf.Max(0f, PlayerControl.LocalPlayer.killTimer - 1f));
								}
								if (GUILayout.Button("+1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) }))
								{
									PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.killTimer + 1f);
								}
								if (GUILayout.Button("Reset", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f) }))
								{
									PlayerControl.LocalPlayer.SetKillTimer(GameManager.Instance.LogicOptions.GetKillCooldown());
								}
								GUILayout.EndHorizontal();
							}
							catch
							{
							}
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(7, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("Speed: ");
							PlayerControl localPlayer3 = PlayerControl.LocalPlayer;
							defaultInterpolatedStringHandler3.AppendFormatted((localPlayer3 != null) ? new float?(localPlayer3.MyPhysics.Speed) : ((float?)null), "F1");
							GUILayout.Label(defaultInterpolatedStringHandler3.ToStringAndClear(), (Il2CppReferenceArray<GUILayoutOption>)null);
							if (GUILayout.Button("-1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) }))
							{
								PlayerControl.LocalPlayer.MyPhysics.Speed = Mathf.Max(0f, PlayerControl.LocalPlayer.MyPhysics.Speed - 1f);
							}
							if (GUILayout.Button("+1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) }))
							{
								PlayerControl.LocalPlayer.MyPhysics.Speed = PlayerControl.LocalPlayer.MyPhysics.Speed + 1f;
							}
							GUILayout.EndHorizontal();
						}
					}
					catch (NullReferenceException)
					{
					}
				}
			}
			if (GUILayout.Button("+1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) }) && (Object)(object)PlayerControl.LocalPlayer != (Object)null)
			{
				try
				{
					string text2 = "[FABMOD]|KILLCD|-1|" + (CheatToggles.LastBroadcastKillCooldown + 1f).ToString(CultureInfo.InvariantCulture);
					PlayerControl.LocalPlayer.RpcSendChat(text2);
				}
				catch
				{
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
			GUILayout.Label($"Speed: {CheatToggles.LastBroadcastSpeed:F1}", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f) });
			if (GUILayout.Button("-1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) }) && (Object)(object)PlayerControl.LocalPlayer != (Object)null)
			{
				try
				{
					string text3 = "[FABMOD]|SPEED|-1|" + Mathf.Max(0f, CheatToggles.LastBroadcastSpeed - 1f).ToString(CultureInfo.InvariantCulture);
					PlayerControl.LocalPlayer.RpcSendChat(text3);
				}
				catch
				{
				}
			}
			if (GUILayout.Button("+1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(40f) }) && (Object)(object)PlayerControl.LocalPlayer != (Object)null)
			{
				try
				{
					string text4 = "[FABMOD]|SPEED|-1|" + (CheatToggles.LastBroadcastSpeed + 1f).ToString(CultureInfo.InvariantCulture);
					PlayerControl.LocalPlayer.RpcSendChat(text4);
				}
				catch
				{
				}
			}
			GUILayout.EndHorizontal();
		}
		int num = Mathf.Clamp(GetLeftSubmenuCount(groupId), 0, count);
		foreach (SubmenuInfo item in groupInfo.submenus.GetRange(0, num))
		{
			GUILayout.Label(item.name, tabSubtitleStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(30f) });
			HorizontalDrawToggles(item.toggles);
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
		if (count > num)
		{
			foreach (SubmenuInfo item2 in groupInfo.submenus.GetRange(num, count - num))
			{
				GUILayout.Label(item2.name, tabSubtitleStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(30f) });
				HorizontalDrawToggles(item2.toggles);
			}
			if (PlayerModifier.IsModifying)
			{
				GUILayout.Label("Player Modifier", tabSubtitleStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(30f) });
				GUILayout.Label("Use the window that appeared to modify players", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(30f) });
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	public void HorizontalDrawToggles(List<ToggleInfo> toggles)
	{
		foreach (ToggleInfo toggle in toggles)
		{
			bool flag = toggle.getState();
			bool flag2 = GUILayout.Toggle(flag, toggle.label, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(20f) });
			if (flag2 != flag)
			{
				toggle.setState(flag2);
			}
		}
	}
}
