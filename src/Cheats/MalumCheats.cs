using Sentry.Internal.Extensions;
using UnityEngine;

namespace MalumMenu;

public static class MalumCheats
{
    public static void closeMeetingCheat()
    {
        if (!CheatToggles.closeMeeting) return;

        if (MeetingHud.Instance != null)
        {
            MeetingHud.Instance.DespawnOnDestroy = false;
            Object.Destroy(MeetingHud.Instance.gameObject);

            HudManager hudManager = DestroyableSingleton<HudManager>.Instance;
            if (hudManager != null)
            {
                hudManager.StartCoroutine(hudManager.CoFadeFullScreen(Color.black, Color.clear, 0.2f, false));
                hudManager.SetHudActive(true);
            }

            PlayerControl.LocalPlayer?.SetKillTimer(GameManager.Instance.LogicOptions.GetKillCooldown());
            if (ShipStatus.Instance != null)
                ShipStatus.Instance.EmergencyCooldown = GameManager.Instance.LogicOptions.GetEmergencyCooldown();

            FollowerCamera camera = Camera.main?.GetComponent<FollowerCamera>();
            if (camera != null) camera.Locked = false;

            ControllerManager.Instance?.CloseAndResetAll();
        }
        else if (ExileController.Instance != null)
        {
            ExileController.Instance.ReEnableGameplay();
            ExileController.Instance.WrapUp();
        }

        CheatToggles.closeMeeting = false;
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
        if (!CheatToggles.completeMyTasks) return;

        Utils.completeMyTasks();
        CheatToggles.completeMyTasks = false;
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
            var abilityBtn = DestroyableSingleton<HudManager>.Instance.AbilityButton;
            abilityBtn?.ResetCoolDown();
            abilityBtn?.SetCooldownFill(0f);
        }
    }

    public static void shapeshifterCheats(ShapeshifterRole shapeshifterRole)
    {
        if (CheatToggles.endlessSsDuration)
        {
            shapeshifterRole.durationSecondsRemaining = float.MaxValue;
        }
        else if (shapeshifterRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetShapeshifterDuration())
        {
            shapeshifterRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetShapeshifterDuration();
        }
    }

    public static void scientistCheats(ScientistRole scientistRole)
    {
        if (CheatToggles.noVitalsCooldown)
            scientistRole.currentCooldown = 0f;

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

            var abilityBtn = DestroyableSingleton<HudManager>.Instance.AbilityButton;
            abilityBtn?.ResetCoolDown();
            abilityBtn?.SetCooldownFill(0f);
        }

        if (CheatToggles.noTrackingDelay)
        {
            MapBehaviour.Instance.trackedPointDelayTime = GameManager.Instance.LogicOptions.GetTrackerDelay();
        }

        if (CheatToggles.endlessTracking)
        {
            trackerRole.durationSecondsRemaining = float.MaxValue;
        }
        else if (trackerRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetTrackerDuration())
        {
            trackerRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetTrackerDuration();
        }
    }

    public static void phantomCheats(PhantomRole phantomRole) { }

    public static void useVentCheat(HudManager hudManager)
    {
        try
        {
            if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                hudManager.ImpostorVentButton?.gameObject.SetActive(CheatToggles.useVents);
            }
        }
        catch { }
    }

    public static void sabotageCheat(ShipStatus shipStatus)
    {
        byte currentMapID = Utils.getCurrentMapID();

        MalumSabotageSystem.handleReactor(shipStatus, currentMapID);
        MalumSabotageSystem.handleOxygen(shipStatus, currentMapID);
        MalumSabotageSystem.handleComms(shipStatus, currentMapID);
        MalumSabotageSystem.handleElectrical(shipStatus, currentMapID);
        MalumSabotageSystem.handleMushMix(shipStatus, currentMapID);
        MalumSabotageSystem.handleDoors(shipStatus);
    }

    public static void walkInVentCheat()
    {
        try
        {
            if (CheatToggles.walkVent && PlayerControl.LocalPlayer != null)
            {
                PlayerControl.LocalPlayer.inVent = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }
        catch { }
    }

    public static void kickVentsCheat()
    {
        if (!CheatToggles.kickVents) return;

        foreach (var vent in ShipStatus.Instance.AllVents)
        {
            VentilationSystem.Update(VentilationSystem.Operation.BootImpostors, vent.Id);
        }

        CheatToggles.kickVents = false;
    }

    public static void killAllCheat()
    {
        if (!CheatToggles.killAll) return;

        if (Utils.isLobby)
        {
            HudManager.Instance?.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
        }
        else
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Utils.murderPlayer(player, MurderResultFlags.Succeeded);
            }
        }

        CheatToggles.killAll = false;
    }

    public static void killAllCrewCheat()
    {
        if (!CheatToggles.killAllCrew) return;

        if (Utils.isLobby)
        {
            HudManager.Instance?.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
        }
        else
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.Role.TeamType == RoleTeamTypes.Crewmate)
                {
                    Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                }
            }
        }

        CheatToggles.killAllCrew = false;
    }

    public static void killAllImpsCheat()
    {
        if (!CheatToggles.killAllImps) return;

        if (Utils.isLobby)
        {
            HudManager.Instance?.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");
        }
        else
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.Role.TeamType == RoleTeamTypes.Impostor)
                {
                    Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                }
            }
        }

        CheatToggles.killAllImps = false;
    }

    public static void teleportCursorCheat()
    {
        if (!CheatToggles.teleportCursor) return;

        if (Input.GetMouseButtonDown(1) && Camera.main != null)
        {
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PlayerControl.LocalPlayer?.NetTransform.RpcSnapTo(targetPos);
        }
    }

    public static void noClipCheat()
    {
        try
        {
            if (PlayerControl.LocalPlayer != null)
            {
                PlayerControl.LocalPlayer.Collider.enabled =
                    !(CheatToggles.noClip || PlayerControl.LocalPlayer.onLadder);
            }
        }
        catch { }
    }

    public static void speedBoostCheat()
    {
        const float defaultSpeed = 2.5f;
        const float defaultGhostSpeed = 3f;
        const float speedMultiplier = 2.0f;

        try
        {
            float newSpeed = CheatToggles.speedBoost ? defaultSpeed * speedMultiplier : defaultSpeed;
            float newGhostSpeed = CheatToggles.speedBoost ? defaultGhostSpeed * speedMultiplier : defaultGhostSpeed;

            var phys = PlayerControl.LocalPlayer?.MyPhysics;
            if (phys != null)
            {
                phys.Speed = newSpeed;
                phys.GhostSpeed = newGhostSpeed;
            }
        }
        catch { }
    }
}
