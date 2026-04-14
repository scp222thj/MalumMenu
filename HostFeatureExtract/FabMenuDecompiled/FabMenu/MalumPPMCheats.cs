using System;
using System.Collections;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace FabMenu;

public static class MalumPPMCheats
{
	public static bool telekillPlayerActive;

	public static bool killPlayerActive;

	public static bool spectateActive;

	public static bool teleportPlayerActive;

	public static bool protectPlayerActive;

	public static bool reportBodyActive;

	public static bool ejectPlayerActive;

	public static bool changeRoleActive;

	public static RoleTypes? oldRole;

	public static void reportBodyPPM()
	{
		if (CheatToggles.reportBody)
		{
			if (!reportBodyActive)
			{
				if ((Object)(object)PlayerPickMenu.playerpickMenu != (Object)null)
				{
					((Minigame)PlayerPickMenu.playerpickMenu).Close();
					CheatToggles.DisablePPMCheats("reportBody");
				}
				PlayerPickMenu.openPlayerPickMenu(Utils.GetAllPlayerData(), Action.op_Implicit((Action)delegate
				{
					Utils.reportDeadBody(PlayerPickMenu.targetPlayerData);
				}));
				reportBodyActive = true;
			}
			if ((Object)(object)PlayerPickMenu.playerpickMenu == (Object)null)
			{
				CheatToggles.reportBody = false;
			}
		}
		else if (reportBodyActive)
		{
			reportBodyActive = false;
		}
	}

	public static void ejectPlayerPPM()
	{
		if (CheatToggles.ejectPlayer)
		{
			if (!ejectPlayerActive)
			{
				if ((Object)(object)PlayerPickMenu.playerpickMenu != (Object)null)
				{
					((Minigame)PlayerPickMenu.playerpickMenu).Close();
					CheatToggles.DisablePPMCheats("ejectPlayer");
				}
				if (!Utils.isMeeting)
				{
					CheatToggles.ejectPlayer = false;
					return;
				}
				List<NetworkedPlayerInfo> val = new List<NetworkedPlayerInfo>();
				Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
				while (enumerator.MoveNext())
				{
					PlayerControl current = enumerator.Current;
					if (!current.Data.IsDead && !current.Data.Disconnected)
					{
						val.Add(current.Data);
					}
				}
				PlayerPickMenu.openPlayerPickMenu(val, Action.op_Implicit((Action)delegate
				{
					NetworkedPlayerInfo targetPlayerData = PlayerPickMenu.targetPlayerData;
					MeetingHud.Instance.RpcVotingComplete(new Il2CppStructArray<VoterState>(0L), targetPlayerData, false);
				}));
				ejectPlayerActive = true;
			}
			if ((Object)(object)PlayerPickMenu.playerpickMenu == (Object)null)
			{
				CheatToggles.ejectPlayer = false;
			}
		}
		else if (ejectPlayerActive)
		{
			ejectPlayerActive = false;
		}
	}

	public static void killPlayerPPM()
	{
		if (CheatToggles.killPlayer)
		{
			if (!killPlayerActive)
			{
				if ((Object)(object)PlayerPickMenu.playerpickMenu != (Object)null)
				{
					((Minigame)PlayerPickMenu.playerpickMenu).Close();
					CheatToggles.DisablePPMCheats("killPlayer");
				}
				if (Utils.isLobby)
				{
					DestroyableSingleton<HudManager>.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
					CheatToggles.killPlayer = false;
					return;
				}
				PlayerPickMenu.openPlayerPickMenu(Utils.GetAllPlayerData(), Action.op_Implicit((Action)delegate
				{
					Utils.murderPlayer(PlayerPickMenu.targetPlayerData.Object, (MurderResultFlags)1);
				}));
				killPlayerActive = true;
			}
			if ((Object)(object)PlayerPickMenu.playerpickMenu == (Object)null)
			{
				CheatToggles.killPlayer = false;
			}
		}
		else if (killPlayerActive)
		{
			killPlayerActive = false;
		}
	}

	public static void telekillPlayerPPM()
	{
		if (CheatToggles.telekillPlayer)
		{
			if (!telekillPlayerActive)
			{
				if ((Object)(object)PlayerPickMenu.playerpickMenu != (Object)null)
				{
					((Minigame)PlayerPickMenu.playerpickMenu).Close();
					CheatToggles.DisablePPMCheats("telekillPlayer");
				}
				if (Utils.isLobby)
				{
					DestroyableSingleton<HudManager>.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
					CheatToggles.telekillPlayer = false;
					return;
				}
				PlayerPickMenu.openPlayerPickMenu(Utils.GetAllPlayerData(), Action.op_Implicit((Action)delegate
				{
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_000a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
					Utils.murderPlayer(PlayerPickMenu.targetPlayerData.Object, (MurderResultFlags)1);
					MonoBehaviourExtensions.StartCoroutine((MonoBehaviour)(object)AmongUsClient.Instance, DelayedTeleportBack(truePosition));
				}));
				telekillPlayerActive = true;
			}
			if ((Object)(object)PlayerPickMenu.playerpickMenu == (Object)null)
			{
				CheatToggles.telekillPlayer = false;
			}
		}
		else if (telekillPlayerActive)
		{
			telekillPlayerActive = false;
		}
	}

	public static IEnumerator DelayedTeleportBack(Vector2 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		yield return (object)new WaitForSeconds(0.25f);
		PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(position);
	}

	public static void teleportPlayerPPM()
	{
		if (CheatToggles.teleportPlayer)
		{
			if (!teleportPlayerActive)
			{
				if ((Object)(object)PlayerPickMenu.playerpickMenu != (Object)null)
				{
					((Minigame)PlayerPickMenu.playerpickMenu).Close();
					CheatToggles.DisablePPMCheats("teleportPlayer");
				}
				List<NetworkedPlayerInfo> val = new List<NetworkedPlayerInfo>();
				Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
				while (enumerator.MoveNext())
				{
					PlayerControl current = enumerator.Current;
					if (!((InnerNetObject)current).AmOwner)
					{
						val.Add(current.Data);
					}
				}
				PlayerPickMenu.openPlayerPickMenu(val, Action.op_Implicit((Action)delegate
				{
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Vector2.op_Implicit(((Component)PlayerPickMenu.targetPlayerData.Object).transform.position));
				}));
				teleportPlayerActive = true;
			}
			if ((Object)(object)PlayerPickMenu.playerpickMenu == (Object)null)
			{
				CheatToggles.teleportPlayer = false;
			}
		}
		else if (teleportPlayerActive)
		{
			teleportPlayerActive = false;
		}
	}

	public static void ProtectPlayerPPM()
	{
		if (CheatToggles.protectPlayer)
		{
			if (!protectPlayerActive && !Utils.isLobby)
			{
				if ((Object)(object)PlayerPickMenu.playerpickMenu != (Object)null)
				{
					((Minigame)PlayerPickMenu.playerpickMenu).Close();
					CheatToggles.DisablePPMCheats("protectPlayer");
				}
				PlayerPickMenu.openPlayerPickMenu(Utils.GetAllPlayerData(), Action.op_Implicit((Action)delegate
				{
					PlayerControl val = PlayerPickMenu.targetPlayerData.Object;
					if ((Object)(object)val != (Object)null)
					{
						int colorId = PlayerControl.LocalPlayer.cosmetics.ColorId;
						PlayerControl.LocalPlayer.RpcProtectPlayer(val, colorId);
					}
				}));
				protectPlayerActive = true;
			}
			if ((Object)(object)PlayerPickMenu.playerpickMenu == (Object)null)
			{
				CheatToggles.protectPlayer = false;
			}
		}
		else if (protectPlayerActive)
		{
			protectPlayerActive = false;
		}
	}

	public static void changeRolePPM()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.changeRole)
		{
			if (!changeRoleActive)
			{
				if ((Object)(object)PlayerPickMenu.playerpickMenu != (Object)null)
				{
					((Minigame)PlayerPickMenu.playerpickMenu).Close();
					CheatToggles.DisablePPMCheats("changeRole");
				}
				List<NetworkedPlayerInfo> val = new List<NetworkedPlayerInfo>();
				if (oldRole == (RoleTypes?)5 || Utils.isFreePlay)
				{
					val.Add(PlayerPickMenu.customPPMChoice("Shapeshifter", Outfits.shapeshifter, Utils.getBehaviourByRoleType((RoleTypes)5)));
				}
				if (oldRole == (RoleTypes?)9 || Utils.isFreePlay)
				{
					val.Add(PlayerPickMenu.customPPMChoice("Phantom", Outfits.phantom, Utils.getBehaviourByRoleType((RoleTypes)9)));
				}
				if (oldRole == (RoleTypes?)18 || Utils.isFreePlay)
				{
					val.Add(PlayerPickMenu.customPPMChoice("Viper", Outfits.viper, Utils.getBehaviourByRoleType((RoleTypes)18)));
				}
				if (oldRole == (RoleTypes?)1 || Utils.isFreePlay || Utils.isHost)
				{
					val.Add(PlayerPickMenu.customPPMChoice("Impostor", Outfits.impostor, Utils.getBehaviourByRoleType((RoleTypes)1)));
				}
				val.Add(PlayerPickMenu.customPPMChoice("Tracker", Outfits.tracker, Utils.getBehaviourByRoleType((RoleTypes)10)));
				val.Add(PlayerPickMenu.customPPMChoice("Noisemaker", Outfits.noisemaker, Utils.getBehaviourByRoleType((RoleTypes)8)));
				val.Add(PlayerPickMenu.customPPMChoice("Engineer", Outfits.engineer, Utils.getBehaviourByRoleType((RoleTypes)3)));
				val.Add(PlayerPickMenu.customPPMChoice("Scientist", Outfits.scientist, Utils.getBehaviourByRoleType((RoleTypes)2)));
				val.Add(PlayerPickMenu.customPPMChoice("Detective", Outfits.detective, Utils.getBehaviourByRoleType((RoleTypes)12)));
				val.Add(PlayerPickMenu.customPPMChoice("Crewmate", Outfits.crewmate, Utils.getBehaviourByRoleType((RoleTypes)0)));
				PlayerPickMenu.openPlayerPickMenu(val, Action.op_Implicit((Action)delegate
				{
					//IL_008c: Unknown result type (might be due to invalid IL or missing references)
					//IL_004e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0054: Invalid comparison between Unknown and I4
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					if (!Utils.isLobby && !Utils.isFreePlay && !oldRole.HasValue)
					{
						oldRole = PlayerControl.LocalPlayer.Data.RoleType;
					}
					if (PlayerControl.LocalPlayer.Data.IsDead)
					{
						if ((int)PlayerPickMenu.targetPlayerData.Role.TeamType == 1)
						{
							DestroyableSingleton<RoleManager>.Instance.SetRole(PlayerControl.LocalPlayer, (RoleTypes)7);
						}
						else
						{
							DestroyableSingleton<RoleManager>.Instance.SetRole(PlayerControl.LocalPlayer, (RoleTypes)6);
						}
					}
					else
					{
						DestroyableSingleton<RoleManager>.Instance.SetRole(PlayerControl.LocalPlayer, PlayerPickMenu.targetPlayerData.Role.Role);
					}
				}));
				changeRoleActive = true;
			}
			if ((Object)(object)PlayerPickMenu.playerpickMenu == (Object)null)
			{
				CheatToggles.changeRole = false;
			}
		}
		else if (changeRoleActive)
		{
			changeRoleActive = false;
		}
	}

	public static void spectatePPM()
	{
		if (CheatToggles.spectate)
		{
			if (!spectateActive)
			{
				if ((Object)(object)PlayerPickMenu.playerpickMenu != (Object)null)
				{
					((Minigame)PlayerPickMenu.playerpickMenu).Close();
					CheatToggles.DisablePPMCheats("spectate");
				}
				List<NetworkedPlayerInfo> val = new List<NetworkedPlayerInfo>();
				Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
				while (enumerator.MoveNext())
				{
					PlayerControl current = enumerator.Current;
					if (!((InnerNetObject)current).AmOwner)
					{
						val.Add(current.Data);
					}
				}
				PlayerPickMenu.openPlayerPickMenu(val, Action.op_Implicit((Action)delegate
				{
					((Component)Camera.main).gameObject.GetComponent<FollowerCamera>().SetTarget((MonoBehaviour)(object)PlayerPickMenu.targetPlayerData.Object);
				}));
				spectateActive = true;
				PlayerControl.LocalPlayer.moveable = false;
				CheatToggles.freecam = false;
			}
			if ((Object)(object)PlayerPickMenu.playerpickMenu == (Object)null && (Object)(object)((Component)Camera.main).gameObject.GetComponent<FollowerCamera>().Target == (Object)(object)PlayerControl.LocalPlayer)
			{
				CheatToggles.spectate = false;
				PlayerControl.LocalPlayer.moveable = true;
			}
		}
		else if (spectateActive)
		{
			spectateActive = false;
			PlayerControl.LocalPlayer.moveable = true;
			((Component)Camera.main).gameObject.GetComponent<FollowerCamera>().SetTarget((MonoBehaviour)(object)PlayerControl.LocalPlayer);
		}
	}

	public static void modifyPlayerNamePPM()
	{
		if (!Utils.isHost)
		{
			return;
		}
		if (CheatToggles.modifyPlayerName)
		{
			if (PlayerModifier.CurrentMode != ModificationMode.Name)
			{
				PlayerModifier.EnterNameModificationMode();
			}
		}
		else if (PlayerModifier.CurrentMode == ModificationMode.Name)
		{
			PlayerModifier.ExitModificationMode();
		}
	}

	public static void modifyPlayerColorPPM()
	{
		if (!Utils.isHost)
		{
			return;
		}
		if (CheatToggles.modifyPlayerColor)
		{
			if (PlayerModifier.CurrentMode != ModificationMode.Color)
			{
				PlayerModifier.EnterColorModificationMode();
			}
		}
		else if (PlayerModifier.CurrentMode == ModificationMode.Color)
		{
			PlayerModifier.ExitModificationMode();
		}
	}
}
