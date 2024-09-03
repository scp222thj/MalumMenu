using HarmonyLib;
using Sentry.Internal.Extensions;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MalumMenu;
public static class MalumCheats
{
    private static LoadSceneMode scene;

    public static void closeMeetingCheat()
    {
        if (CheatToggles.closeMeeting)
        {

            if (MeetingHud.Instance)
            { // Closes MeetingHud window if it's open

                // Destroy MeetingHud window gameobject
                MeetingHud.Instance.DespawnOnDestroy = false;
                UnityEngine.Object.Destroy(MeetingHud.Instance.gameObject);

                // Gameplay must be reenabled
                DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.black, Color.clear, 0.2f, false));
                PlayerControl.LocalPlayer.SetKillTimer(GameManager.Instance.LogicOptions.GetKillCooldown());
                ShipStatus.Instance.EmergencyCooldown = GameManager.Instance.LogicOptions.GetEmergencyCooldown();
                Camera.main.GetComponent<FollowerCamera>().Locked = false;
                DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
                ControllerManager.Instance.CloseAndResetAll();

            }
            else if (ExileController.Instance != null)
            { // Ends exile cutscene if it's playing
                ExileController.Instance.ReEnableGameplay();
                ExileController.Instance.WrapUp();
            }

            CheatToggles.closeMeeting = false; // Button behaviour
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

            // Makes vent time so incredibly long (float.MaxValue) so that it never ends
            engineerRole.inVentTimeRemaining = float.MaxValue;

            // Vent time is reset to normal value after the cheat is disabled
        }
        else if (engineerRole.inVentTimeRemaining > engineerRole.GetCooldown())
        {

            engineerRole.inVentTimeRemaining = engineerRole.GetCooldown();

        }

        if (CheatToggles.noVentCooldown)
        {

            if (engineerRole.cooldownSecondsRemaining > 0f)
            {

                engineerRole.cooldownSecondsRemaining = 0f;

                DestroyableSingleton<HudManager>.Instance.AbilityButton.ResetCoolDown();
                DestroyableSingleton<HudManager>.Instance.AbilityButton.SetCooldownFill(0f);

            }

        }
    }

    public static void shapeshifterCheats(ShapeshifterRole shapeshifterRole)
    {
        if (CheatToggles.endlessSsDuration)
        {

            // Makes shapeshift duration so incredibly long (float.MaxValue) so that it never ends
            shapeshifterRole.durationSecondsRemaining = float.MaxValue;

            // Shapeshift duration is reset to normal value after the cheat is disabled
        }
        else if (shapeshifterRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetShapeshifterDuration())
        {

            shapeshifterRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetShapeshifterDuration();

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

            // Makes vitals battery so incredibly long (float.MaxValue) so that it never ends
            scientistRole.currentCharge = float.MaxValue;

            // Battery charge is reset to normal value after the cheat is disabled
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

            DestroyableSingleton<HudManager>.Instance.AbilityButton.ResetCoolDown();
            DestroyableSingleton<HudManager>.Instance.AbilityButton.SetCooldownFill(0f);

        }

        if (CheatToggles.noTrackingDelay)
        {

            MapBehaviour.Instance.trackedPointDelayTime = GameManager.Instance.LogicOptions.GetTrackerDelay();

        }

        if (CheatToggles.endlessTracking)
        {

            // Makes vitals battery so incredibly long (float.MaxValue) so that it never ends
            trackerRole.durationSecondsRemaining = float.MaxValue;

            // Battery charge is reset to normal value after the cheat is disabled
        }
        else if (trackerRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetTrackerDuration())
        {

            trackerRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetTrackerDuration();

        }
    }
    public static void phantomCheats(PhantomRole phantomRole)
    {
        return;
    }

    public static void useVentCheat(HudManager hudManager)
    {
        // try-catch to prevent errors when role is null
        try
        {

            // Engineers & Impostors don't need this cheat so it is disabled for them
            // Ghost venting causes issues so it is also disabled

            if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                hudManager.ImpostorVentButton.gameObject.SetActive(CheatToggles.useVents);
            }

        }
        catch { }
    }

    public static void sabotageCheat(ShipStatus shipStatus)
    {
        byte currentMapID = Utils.getCurrentMapID();

        // Handle all sabotage systems
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

            if (CheatToggles.walkVent)
            {
                PlayerControl.LocalPlayer.inVent = false;
                PlayerControl.LocalPlayer.moveable = true;
            }

        }
        catch { }
    }

    public static void kickVentsCheat()
    {
        if (CheatToggles.kickVents)
        {

            foreach (var vent in ShipStatus.Instance.AllVents)
            {

                VentilationSystem.Update(VentilationSystem.Operation.BootImpostors, vent.Id);

            }

            CheatToggles.kickVents = false; // Button behaviour
        }
    }

    public static void killAllCheat()
    {
        if (CheatToggles.killAll)
        {

            if (Utils.isLobby)
            {

                HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");

            }
            else
            {

                // Kill all players by sending a successful MurderPlayer RPC call
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                }

            }

            CheatToggles.killAll = false;

        }
    }

    public static void killAllCrewCheat()
    {
        if (CheatToggles.killAllCrew)
        {

            if (Utils.isLobby)
            {

                HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");

            }
            else
            {

                // Kill all players by sending a successful MurderPlayer RPC call
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
    }

    public static void killAllImpsCheat()
    {
        if (CheatToggles.killAllImps)
        {

            if (Utils.isLobby)
            {

                HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");

            }
            else
            {

                // Kill all players by sending a successful MurderPlayer RPC call
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
    }

    public static void teleportCursorCheat()
    {
        if (CheatToggles.teleportCursor)
        {
            // Teleport player to cursor's in-world position on right-click
            if (Input.GetMouseButtonDown(1))
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }

    public static void noClipCheat()
    {
        try
        {

            PlayerControl.LocalPlayer.Collider.enabled = !(CheatToggles.noClip || PlayerControl.LocalPlayer.onLadder);

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
            // If the speedBoost cheat is enabled, the default speed is multiplied by the speed multiplier
            // Otherwise the default speed is used by itself

            float newSpeed = CheatToggles.speedBoost ? defaultSpeed * speedMultiplier : defaultSpeed;

            float newGhostSpeed = CheatToggles.speedBoost ? defaultGhostSpeed * speedMultiplier : defaultGhostSpeed;

            PlayerControl.LocalPlayer.MyPhysics.Speed = newSpeed;
            PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = newGhostSpeed;
        }
        catch { }
    }

    public static void PanicMode() //panic mode funktions
    {
        CheatToggles.noClip = false;
        CheatToggles.speedBoost = false;
        CheatToggles.teleportPlayer = false;
        CheatToggles.teleportCursor = false;
        CheatToggles.reportBody = false;
        CheatToggles.killPlayer = false;
        CheatToggles.telekillPlayer = false;
        CheatToggles.killAll = false;
        CheatToggles.killAllCrew = false;
        CheatToggles.killAllImps = false;

        CheatToggles.changeRole = false;
        CheatToggles.zeroKillCd = false;
        CheatToggles.completeMyTasks = false;
        CheatToggles.killReach = false;
        CheatToggles.killAnyone = false;
        CheatToggles.endlessSsDuration = false;
        CheatToggles.endlessBattery = false;
        CheatToggles.endlessTracking = false;
        CheatToggles.noTrackingCooldown = false;
        CheatToggles.noTrackingDelay = false;
        CheatToggles.noVitalsCooldown = false;
        CheatToggles.noVentCooldown = false;
        CheatToggles.endlessVentTime = false;
        CheatToggles.endlessVanish = false;
        CheatToggles.killVanished = false;
        CheatToggles.noVanishAnim = false;
        CheatToggles.noShapeshiftAnim = false;

        CheatToggles.fullBright = false;
        CheatToggles.alwaysChat = false;
        CheatToggles.seeGhosts = false;
        CheatToggles.seeRoles = false;
        CheatToggles.seeDisguises = false;
        CheatToggles.revealVotes = false;

        CheatToggles.spectate = false;
        CheatToggles.zoomOut = false;
        CheatToggles.freecam = false;

        CheatToggles.mapCrew = false;
        CheatToggles.mapImps = false;
        CheatToggles.mapGhosts = false;
        CheatToggles.colorBasedMap = false;

        CheatToggles.tracersImps = false;
        CheatToggles.tracersCrew = false;
        CheatToggles.tracersGhosts = false;
        CheatToggles.tracersBodies = false;
        CheatToggles.colorBasedTracers = false;
        CheatToggles.distanceBasedTracers = false;

        CheatToggles.closeMeeting = false;
        CheatToggles.doorsSab = false;
        CheatToggles.unfixableLights = false;
        CheatToggles.commsSab = false;
        CheatToggles.elecSab = false;
        CheatToggles.reactorSab = false;
        CheatToggles.oxygenSab = false;
        CheatToggles.mushSab = false;

        CheatToggles.useVents = false;
        CheatToggles.walkVent = false;
        CheatToggles.kickVents = false;

        // Host-Only settings can be included if needed
        // CheatToggles.impostorHack = false;
        // CheatToggles.godMode = false;
        // CheatToggles.evilVote = false;
        // CheatToggles.voteImmune = false;

        CheatToggles.unlockFeatures = false;
        CheatToggles.freeCosmetics = false;
        CheatToggles.avoidBans = false;
        ModManager.Instance.ModStamp.enabled = false;
    }
}