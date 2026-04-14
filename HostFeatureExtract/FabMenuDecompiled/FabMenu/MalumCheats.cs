using System.Runtime.CompilerServices;
using AmongUs.GameOptions;
using AmongUs.InnerNet.GameDataMessages;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace FabMenu;

public static class MalumCheats
{
	private static bool _hasUsedScanCheatBefore;

	private static bool _hasUsedCamsCheatBefore;

	public static void closeMeetingCheat()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.closeMeeting)
		{
			if (Utils.isMeeting)
			{
				((InnerNetObject)MeetingHud.Instance).DespawnOnDestroy = false;
				Object.Destroy((Object)(object)((Component)MeetingHud.Instance).gameObject);
				((MonoBehaviour)DestroyableSingleton<HudManager>.Instance).StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.black, Color.clear, 0.2f, false));
				PlayerControl.LocalPlayer.SetKillTimer(GameManager.Instance.LogicOptions.GetKillCooldown());
				ShipStatus.Instance.EmergencyCooldown = GameManager.Instance.LogicOptions.GetEmergencyCooldown();
				((Component)Camera.main).GetComponent<FollowerCamera>().Locked = false;
				DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
				ControllerManager.Instance.CloseAndResetAll();
			}
			else if (Object.op_Implicit((Object)(object)ExileController.Instance))
			{
				ExileController.Instance.ReEnableGameplay();
				ExileController.Instance.WrapUp();
			}
			CheatToggles.closeMeeting = false;
		}
	}

	public static void skipMeetingCheat()
	{
		if (CheatToggles.skipMeeting)
		{
			if (Utils.isMeeting)
			{
				MeetingHud.Instance.RpcVotingComplete(new Il2CppStructArray<VoterState>(0L), (NetworkedPlayerInfo)null, true);
			}
			CheatToggles.skipMeeting = false;
		}
	}

	public static void callMeetingCheat()
	{
		if (CheatToggles.callMeeting)
		{
			MeetingRoomManager.Instance.AssignSelf(PlayerControl.LocalPlayer, (NetworkedPlayerInfo)null);
			DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(PlayerControl.LocalPlayer);
			PlayerControl.LocalPlayer.RpcStartMeeting((NetworkedPlayerInfo)null);
			CheatToggles.callMeeting = false;
		}
	}

	public static void forceStartGameCheat()
	{
		if (CheatToggles.forceStartGame)
		{
			if (Utils.isHost && Utils.isLobby)
			{
				((InnerNetClient)AmongUsClient.Instance).SendStartGame();
			}
			CheatToggles.forceStartGame = false;
		}
	}

	public static void noKillCdCheat(PlayerControl playerControl)
	{
		if (CheatToggles.zeroKillCd && playerControl.killTimer > 0f)
		{
			playerControl.SetKillTimer(0f);
		}
	}

	public static void completeMyTasksCheat()
	{
		if (CheatToggles.completeMyTasks)
		{
			Utils.completeMyTasks();
			CheatToggles.completeMyTasks = false;
		}
	}

	public static void engineerCheats(EngineerRole engineerRole)
	{
		if (CheatToggles.endlessVentTime)
		{
			engineerRole.inVentTimeRemaining = float.MaxValue;
		}
		else if (engineerRole.inVentTimeRemaining > engineerRole.GetCooldown())
		{
			engineerRole.inVentTimeRemaining = engineerRole.GetCooldown();
		}
		if (CheatToggles.noVentCooldown && engineerRole.cooldownSecondsRemaining > 0f)
		{
			engineerRole.cooldownSecondsRemaining = 0f;
			((ActionButton)DestroyableSingleton<HudManager>.Instance.AbilityButton).ResetCoolDown();
			((ActionButton)DestroyableSingleton<HudManager>.Instance.AbilityButton).SetCooldownFill(0f);
		}
	}

	public static void shapeshifterCheats(ShapeshifterRole shapeshifterRole)
	{
		if (CheatToggles.endlessSsDuration)
		{
			shapeshifterRole.durationSecondsRemaining = float.MaxValue;
		}
		else if (shapeshifterRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetRoleFloat((FloatOptionNames)1001))
		{
			shapeshifterRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetRoleFloat((FloatOptionNames)1001);
		}
	}

	public static void scientistCheats(ScientistRole scientistRole)
	{
		if (CheatToggles.noVitalsCooldown)
		{
			scientistRole.currentCooldown = 0f;
		}
		if (CheatToggles.endlessBattery)
		{
			scientistRole.currentCharge = float.MaxValue;
		}
		else if (scientistRole.currentCharge > scientistRole.RoleCooldownValue)
		{
			scientistRole.currentCharge = scientistRole.RoleCooldownValue;
		}
	}

	public static void trackerCheats(TrackerRole trackerRole)
	{
		if (CheatToggles.noTrackingCooldown)
		{
			trackerRole.cooldownSecondsRemaining = 0f;
			trackerRole.delaySecondsRemaining = 0f;
			((ActionButton)DestroyableSingleton<HudManager>.Instance.AbilityButton).ResetCoolDown();
			((ActionButton)DestroyableSingleton<HudManager>.Instance.AbilityButton).SetCooldownFill(0f);
		}
		if (CheatToggles.noTrackingDelay)
		{
			MapBehaviour instance = MapBehaviour.Instance;
			if (instance != null)
			{
				instance.trackedPointDelayTime = GameManager.Instance.LogicOptions.GetRoleFloat((FloatOptionNames)1552);
			}
		}
		if (CheatToggles.endlessTracking)
		{
			trackerRole.durationSecondsRemaining = float.MaxValue;
		}
		else if (trackerRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetRoleFloat((FloatOptionNames)1551))
		{
			trackerRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetRoleFloat((FloatOptionNames)1551);
		}
	}

	public static void phantomCheats(PhantomRole phantomRole)
	{
	}

	public static void useVentCheat(HudManager hudManager)
	{
		try
		{
			if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead)
			{
				((Component)hudManager.ImpostorVentButton).gameObject.SetActive(CheatToggles.useVents);
			}
		}
		catch
		{
		}
	}

	public static void sabotageCheat(ShipStatus shipStatus)
	{
		byte currentMapID = Utils.getCurrentMapID();
		MalumSabotageSystem.HandleReactor(shipStatus, currentMapID);
		MalumSabotageSystem.HandleOxygen(shipStatus, currentMapID);
		MalumSabotageSystem.HandleComms(shipStatus, currentMapID);
		MalumSabotageSystem.HandleElectrical(shipStatus, currentMapID);
		MalumSabotageSystem.HandleMushMix(shipStatus, currentMapID);
		MalumSabotageSystem.HandleDoors(shipStatus);
		MalumSabotageSystem.OpenSabotageMap();
	}

	public static void fungleSabotageCheat(FungleShipStatus shipStatus)
	{
		byte currentMapID = Utils.getCurrentMapID();
		MalumSabotageSystem.HandleSpores(shipStatus, currentMapID);
	}

	public static void walkInVentCheat()
	{
		try
		{
			if (CheatToggles.walkVent)
			{
				PlayerControl.LocalPlayer.inVent = false;
				PlayerControl.LocalPlayer.moveable = true;
			}
		}
		catch
		{
		}
	}

	public static void kickVentsCheat()
	{
		if (!CheatToggles.kickVents)
		{
			return;
		}
		foreach (Vent item in (Il2CppArrayBase<Vent>)(object)ShipStatus.Instance.AllVents)
		{
			VentilationSystem.Update((Operation)5, item.Id);
		}
		CheatToggles.kickVents = false;
	}

	public static void killAllCheat()
	{
		if (!CheatToggles.killAll)
		{
			return;
		}
		if (Utils.isLobby)
		{
			DestroyableSingleton<HudManager>.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
		}
		else
		{
			Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Utils.murderPlayer(enumerator.Current, (MurderResultFlags)1);
			}
		}
		CheatToggles.killAll = false;
	}

	public static void killAllCrewCheat()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.killAllCrew)
		{
			return;
		}
		if (Utils.isLobby)
		{
			DestroyableSingleton<HudManager>.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
		}
		else
		{
			Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PlayerControl current = enumerator.Current;
				if ((int)current.Data.Role.TeamType == 0)
				{
					Utils.murderPlayer(current, (MurderResultFlags)1);
				}
			}
		}
		CheatToggles.killAllCrew = false;
	}

	public static void killAllImpsCheat()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Invalid comparison between Unknown and I4
		if (!CheatToggles.killAllImps)
		{
			return;
		}
		if (Utils.isLobby)
		{
			DestroyableSingleton<HudManager>.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
		}
		else
		{
			Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PlayerControl current = enumerator.Current;
				if ((int)current.Data.Role.TeamType == 1)
				{
					Utils.murderPlayer(current, (MurderResultFlags)1);
				}
			}
		}
		CheatToggles.killAllImps = false;
	}

	public static void spamKillAllCheat()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.spamKillAll || Utils.isLobby)
		{
			return;
		}
		CheatToggles.spamTimer += Time.deltaTime;
		if (!(CheatToggles.spamTimer >= 0.2f))
		{
			return;
		}
		Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl current = enumerator.Current;
			if ((int)current.Data.Role.TeamType == 0)
			{
				Utils.murderPlayer(current, (MurderResultFlags)1);
			}
		}
		CheatToggles.spamTimer = 0f;
	}

	public static void teleportCursorCheat()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.teleportCursor && Input.GetMouseButtonDown(1))
		{
			PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
		}
	}

	public static void noClipCheat()
	{
		try
		{
			((Behaviour)PlayerControl.LocalPlayer.Collider).enabled = !CheatToggles.noClip && !PlayerControl.LocalPlayer.onLadder;
		}
		catch
		{
		}
	}

	public static void speedBoostCheat()
	{
		try
		{
			float speed = (CheatToggles.speedBoost ? 5f : 2.5f);
			float ghostSpeed = (CheatToggles.speedBoost ? 6f : 3f);
			PlayerControl.LocalPlayer.MyPhysics.Speed = speed;
			PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = ghostSpeed;
		}
		catch
		{
		}
	}

	public static void ReviveCheat()
	{
		if (CheatToggles.revive)
		{
			PlayerControl.LocalPlayer.Revive();
			CheatToggles.revive = false;
		}
	}

	private static void ForceSetScanner(PlayerControl player, bool toggle)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		byte b = (player.scannerCount += 1);
		byte b3 = b;
		player.SetScanner(toggle, b3);
		RpcSetScannerMessage o = new RpcSetScannerMessage(((InnerNetObject)player).NetId, toggle, b3);
		((InnerNetClient)AmongUsClient.Instance).LateBroadcastReliableMessage(Unsafe.As<IGameDataMessage>(o));
	}

	public static void ScanCheat()
	{
		if (CheatToggles.animScan && !_hasUsedScanCheatBefore)
		{
			ForceSetScanner(PlayerControl.LocalPlayer, toggle: true);
			_hasUsedScanCheatBefore = true;
		}
		else if (!CheatToggles.animScan && _hasUsedScanCheatBefore)
		{
			ForceSetScanner(PlayerControl.LocalPlayer, toggle: false);
			_hasUsedScanCheatBefore = false;
		}
	}

	private static void ForcePlayAnimation(byte animationType)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		PlayerControl.LocalPlayer.PlayAnimation(animationType);
		RpcPlayAnimationMessage o = new RpcPlayAnimationMessage(((InnerNetObject)PlayerControl.LocalPlayer).NetId, animationType);
		((InnerNetClient)AmongUsClient.Instance).LateBroadcastUnreliableMessage(Unsafe.As<IGameDataMessage>(o));
	}

	public static void AnimationCheat()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I4
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Invalid comparison between Unknown and I4
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Invalid comparison between Unknown and I4
		MapNames val = (MapNames)Utils.getCurrentMapID();
		if (CheatToggles.animShields)
		{
			if (((int)val == 0 || (int)val == 3) ? true : false)
			{
				ForcePlayAnimation(1);
			}
			CheatToggles.animShields = false;
		}
		if (CheatToggles.animAsteroids)
		{
			if (((int)val == 0 || val - 2 <= 1) ? true : false)
			{
				ForcePlayAnimation(6);
			}
			else
			{
				CheatToggles.animAsteroids = false;
			}
		}
		if (CheatToggles.animEmptyGarbage)
		{
			if (((int)val == 0 || (int)val == 3) ? true : false)
			{
				ForcePlayAnimation(10);
			}
			CheatToggles.animEmptyGarbage = false;
		}
		if (CheatToggles.animCamsInUse && !_hasUsedCamsCheatBefore)
		{
			if (((int)val == 1 || (int)val == 5) ? true : false)
			{
				CheatToggles.animCamsInUse = false;
				return;
			}
			ShipStatus.Instance.RpcUpdateSystem((SystemTypes)11, (byte)1);
			_hasUsedCamsCheatBefore = true;
		}
		else if (!CheatToggles.animCamsInUse && _hasUsedCamsCheatBefore)
		{
			ShipStatus.Instance.RpcUpdateSystem((SystemTypes)11, (byte)0);
			_hasUsedCamsCheatBefore = false;
		}
	}
}
