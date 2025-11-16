using System.Runtime.CompilerServices;
using AmongUs.GameOptions;
using AmongUs.InnerNet.GameDataMessages;
using Sentry.Internal.Extensions;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace MalumMenu;
public static class MalumCheats
{
    public static void closeMeetingCheat()
    {
        if(CheatToggles.closeMeeting){
            
            if (MeetingHud.Instance){ // Closes MeetingHud window if it's open

                // Destroy MeetingHud window gameobject
                MeetingHud.Instance.DespawnOnDestroy = false;
                Object.Destroy(MeetingHud.Instance.gameObject);

                // Gameplay must be reenabled
                DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.black, Color.clear, 0.2f, false));
                PlayerControl.LocalPlayer.SetKillTimer(GameManager.Instance.LogicOptions.GetKillCooldown());
                ShipStatus.Instance.EmergencyCooldown = GameManager.Instance.LogicOptions.GetEmergencyCooldown();
                Camera.main.GetComponent<FollowerCamera>().Locked = false;
                DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
                ControllerManager.Instance.CloseAndResetAll();

            }else if (ExileController.Instance != null){ // Ends exile cutscene if it's playing
                ExileController.Instance.ReEnableGameplay();
                ExileController.Instance.WrapUp();
            }
            
            CheatToggles.closeMeeting = false; // Button behaviour
        }
    }

    public static void skipMeetingCheat()
    {
        if (!CheatToggles.skipMeeting) return;

        if (Utils.isMeeting)
        {
            MeetingHud.Instance.RpcVotingComplete(new Il2CppStructArray<MeetingHud.VoterState>(0L), null, true);
        }

        CheatToggles.skipMeeting = false;
    }

    public static void forceStartGameCheat()
    {
        if (!CheatToggles.forceStartGame) return;
        if (Utils.isHost && Utils.isLobby)
        {
            AmongUsClient.Instance.SendStartGame();
        }

        CheatToggles.forceStartGame = false;
    }

    public static void noKillCdCheat(PlayerControl playerControl)
    {
        if (CheatToggles.zeroKillCd && playerControl.killTimer > 0f){
            playerControl.SetKillTimer(0f);
        }
    }

    public static void completeMyTasksCheat()
    {
        if (CheatToggles.completeMyTasks){
            Utils.completeMyTasks();

            CheatToggles.completeMyTasks = false;
        }
    }

    public static void engineerCheats(EngineerRole engineerRole)
    {
        if (CheatToggles.endlessVentTime){

            // Makes vent time so incredibly long (float.MaxValue) so that it never ends
            engineerRole.inVentTimeRemaining = float.MaxValue;

        // Vent time is reset to normal value after the cheat is disabled
        }else if (engineerRole.inVentTimeRemaining > engineerRole.GetCooldown()){

            engineerRole.inVentTimeRemaining = engineerRole.GetCooldown();

        }

        if (CheatToggles.noVentCooldown){

            if (engineerRole.cooldownSecondsRemaining > 0f){

                engineerRole.cooldownSecondsRemaining = 0f;

                DestroyableSingleton<HudManager>.Instance.AbilityButton.ResetCoolDown();
                DestroyableSingleton<HudManager>.Instance.AbilityButton.SetCooldownFill(0f);

            }

        }
    }

    public static void shapeshifterCheats(ShapeshifterRole shapeshifterRole)
    {
        if (CheatToggles.endlessSsDuration){

            // Makes shapeshift duration so incredibly long (float.MaxValue) so that it never ends
            shapeshifterRole.durationSecondsRemaining = float.MaxValue;

        // Shapeshift duration is reset to normal value after the cheat is disabled
        }else if (shapeshifterRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetRoleFloat(FloatOptionNames.ShapeshifterDuration)){

            shapeshifterRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetRoleFloat(FloatOptionNames.ShapeshifterDuration);

        }
    }

    public static void scientistCheats(ScientistRole scientistRole)
    {
        if (CheatToggles.noVitalsCooldown){

            scientistRole.currentCooldown = 0f;
        }

        if (CheatToggles.endlessBattery){

            // Makes vitals battery so incredibly long (float.MaxValue) so that it never ends
            scientistRole.currentCharge = float.MaxValue;

        // Battery charge is reset to normal value after the cheat is disabled
        }else if (scientistRole.currentCharge > scientistRole.RoleCooldownValue){

            scientistRole.currentCharge = scientistRole.RoleCooldownValue;

        }
    }

    public static void trackerCheats(TrackerRole trackerRole)
    {
        if (CheatToggles.noTrackingCooldown){

            trackerRole.cooldownSecondsRemaining = 0f;
            trackerRole.delaySecondsRemaining = 0f;

            DestroyableSingleton<HudManager>.Instance.AbilityButton.ResetCoolDown();
            DestroyableSingleton<HudManager>.Instance.AbilityButton.SetCooldownFill(0f);

        }

        if (CheatToggles.noTrackingDelay){

            MapBehaviour.Instance.trackedPointDelayTime = GameManager.Instance.LogicOptions.GetRoleFloat(FloatOptionNames.TrackerDelay);

        }

        if (CheatToggles.endlessTracking){

            // Makes vitals battery so incredibly long (float.MaxValue) so that it never ends
            trackerRole.durationSecondsRemaining = float.MaxValue;

        // Battery charge is reset to normal value after the cheat is disabled
        }else if (trackerRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetRoleFloat(FloatOptionNames.TrackerDuration)){

            trackerRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetRoleFloat(FloatOptionNames.TrackerDuration);

        }
    }
    public static void phantomCheats(PhantomRole phantomRole)
    {
        return;
    }

    public static void useVentCheat(HudManager hudManager)
    {
        // try-catch to prevent errors when role is null
        try{

			// Engineers & Impostors don't need this cheat so it is disabled for them
			// Ghost venting causes issues so it is also disabled

			if (!PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead){
				hudManager.ImpostorVentButton.gameObject.SetActive(CheatToggles.useVents);
			}

        }catch{}
    }

    public static void sabotageCheat(ShipStatus shipStatus)
    {
        var currentMapID = Utils.getCurrentMapID();

        // Handle all sabotage systems
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
        var currentMapID = Utils.getCurrentMapID();

        MalumSabotageSystem.HandleSpores(shipStatus, currentMapID);
    }

    public static void walkInVentCheat()
    {
        try{

            if (CheatToggles.walkVent){
                PlayerControl.LocalPlayer.inVent = false;
                PlayerControl.LocalPlayer.moveable = true;
            }

        }catch{}
    }

    public static void kickVentsCheat()
    {
        if (CheatToggles.kickVents){

            foreach(var vent in ShipStatus.Instance.AllVents){

                VentilationSystem.Update(VentilationSystem.Operation.BootImpostors, vent.Id);

            }

            CheatToggles.kickVents = false; // Button behaviour
        }
    }

    public static void killAllCheat()
    {
        if (CheatToggles.killAll){

            if (Utils.isLobby){

                HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");

            }else{

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
        if (CheatToggles.killAllCrew){

            if (Utils.isLobby){

                HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");

            }else{

                // Kill all players by sending a successful MurderPlayer RPC call
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.Role.TeamType == RoleTeamTypes.Crewmate){
                        Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                    }
                }

            }

            CheatToggles.killAllCrew = false;

        }
    }

    public static void killAllImpsCheat()
    {
        if (CheatToggles.killAllImps){

            if (Utils.isLobby){

                HudManager.Instance.Notifier.AddDisconnectMessage("Killing in lobby disabled for being too buggy");

            }else{

                // Kill all players by sending a successful MurderPlayer RPC call
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.Role.TeamType == RoleTeamTypes.Impostor){
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
        try{

            PlayerControl.LocalPlayer.Collider.enabled = !(CheatToggles.noClip || PlayerControl.LocalPlayer.onLadder);

        }catch{}
    }

    public static void speedBoostCheat()
    {
        const float speedMultiplier = 2.0f;

        try
        {
            // If the speedBoost cheat is enabled, the default speed is multiplied by the speed multiplier
            // Otherwise the default speed is used by itself

            var newSpeed = CheatToggles.speedBoost ? Utils.DefaultSpeed * speedMultiplier : Utils.DefaultSpeed;

            var newGhostSpeed = CheatToggles.speedBoost ? Utils.DefaultGhostSpeed * speedMultiplier : Utils.DefaultGhostSpeed;

            PlayerControl.LocalPlayer.MyPhysics.Speed = newSpeed;
            PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = newGhostSpeed;
        }catch{}
    }

    public static void ReviveCheat()
    {
        if (!CheatToggles.revive) return;

        PlayerControl.LocalPlayer.Revive();
        CheatToggles.revive = false;
    }

    private static bool _hasUsedScanCheatBefore;

    private static void ForceSetScanner(PlayerControl player, bool toggle)
    {
        var count = ++player.scannerCount;
        player.SetScanner(toggle, count);
        RpcSetScannerMessage rpcMessage = new(player.NetId, toggle, count);
        AmongUsClient.Instance.LateBroadcastReliableMessage(Unsafe.As<IGameDataMessage>(rpcMessage));
    }

    public static void ScanCheat()
    {
        if (CheatToggles.animScan && !_hasUsedScanCheatBefore)
        {
            ForceSetScanner(PlayerControl.LocalPlayer, true);
            _hasUsedScanCheatBefore = true;
        }
        else if (!CheatToggles.animScan && _hasUsedScanCheatBefore)
        {
            ForceSetScanner(PlayerControl.LocalPlayer, false);
            _hasUsedScanCheatBefore = false;
        }
    }

    private static void ForcePlayAnimation(byte animationType)
    {
        // PlayerControl.LocalPlayer.RpcPlayAnimation(1); wouldn't work if visual tasks are turned off
        // The below way makes sure it works regardless of visual task settings

        PlayerControl.LocalPlayer.PlayAnimation(animationType);
        RpcPlayAnimationMessage rpcMessage = new(PlayerControl.LocalPlayer.NetId, animationType);
        AmongUsClient.Instance.LateBroadcastUnreliableMessage(Unsafe.As<IGameDataMessage>(rpcMessage));
    }

    private static bool _hasUsedCamsCheatBefore;

    public static void AnimationCheat()
    {
        var map = (MapNames)Utils.getCurrentMapID();

        if (CheatToggles.animShields)
        {
            if (map is MapNames.Skeld or MapNames.Dleks)
            {
                ForcePlayAnimation((byte)TaskTypes.PrimeShields);
            }
            CheatToggles.animShields = false;
        }
        if (CheatToggles.animAsteroids)
        {
            if (map is MapNames.Skeld or MapNames.Dleks or MapNames.Polus)
            {
                ForcePlayAnimation((byte)TaskTypes.ClearAsteroids);
            }
            else
            {
                CheatToggles.animAsteroids = false;
            }
        }
        if (CheatToggles.animEmptyGarbage)
        {
            if (map is MapNames.Skeld or MapNames.Dleks)
            {
                ForcePlayAnimation((byte)TaskTypes.EmptyGarbage);
            }
            CheatToggles.animEmptyGarbage = false;
        }

        if (CheatToggles.animCamsInUse && !_hasUsedCamsCheatBefore)
        {
            // There is no cameras on Mira HQ and Fungle
            if (map is MapNames.MiraHQ or MapNames.Fungle)
            {
                CheatToggles.animCamsInUse = false;
            }
            else
            {
                // ShipStatus.Instance.UpdateSystem(SystemTypes.Security, PlayerControl.LocalPlayer, (byte)(CheatToggles.animCamsInUse ? 1 : 0));
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Security, 1);
                _hasUsedCamsCheatBefore = true;
            }
        }
        else if (!CheatToggles.animCamsInUse && _hasUsedCamsCheatBefore)
        {
            // Turn off cams if the cheat was used before and is now disabled
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Security, 0);
            _hasUsedCamsCheatBefore = false;
        }

    }
}
