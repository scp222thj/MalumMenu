using UnityEngine;
using HarmonyLib;
using Hazel;
using InnerNet;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MalumMenu;
public static class MalumCheats
{
    public static void closeMeetingCheat()
    {
        if(CheatToggles.closeMeeting){
            
            if (MeetingHud.Instance){ // Closes MeetingHud window if it's open

                // Destroy MeetingHud window gameobject
                MeetingHud.Instance.DespawnOnDestroy = false;
                UnityEngine.Object.Destroy(MeetingHud.Instance.gameObject);

                // Gameplay must be reenabled
                ExileController exileController = UnityEngine.Object.Instantiate(ShipStatus.Instance.ExileCutscenePrefab);
                exileController.ReEnableGameplay();
                exileController.WrapUp();

            }else if (ExileController.Instance != null){ // Ends exile cutscene if it's playing
                ExileController.Instance.ReEnableGameplay();
                ExileController.Instance.WrapUp();
            }
            
            CheatToggles.closeMeeting = false; // Button behaviour
        }
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
        }else if (engineerRole.inVentTimeRemaining > GameManager.Instance.LogicOptions.GetEngineerCooldown()){
            
            engineerRole.inVentTimeRemaining = GameManager.Instance.LogicOptions.GetEngineerCooldown();
        
        }

        if (CheatToggles.noVentCooldown){

            engineerRole.cooldownSecondsRemaining = 0f;
        
        }
    }

    public static void shapeshifterCheats(ShapeshifterRole shapeshifterRole)
    {
        if (CheatToggles.endlessSsDuration){

            // Makes shapeshift duration so incredibly long (float.MaxValue) so that it never ends
            shapeshifterRole.durationSecondsRemaining = float.MaxValue; 
            
        // Shapeshift duration is reset to normal value after the cheat is disabled
        }else if (shapeshifterRole.durationSecondsRemaining > GameManager.Instance.LogicOptions.GetShapeshifterDuration()){
            
            shapeshifterRole.durationSecondsRemaining = GameManager.Instance.LogicOptions.GetShapeshifterDuration();
        
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
        }else if (scientistRole.currentCharge > GameManager.Instance.LogicOptions.GetScientistBatteryCharge()){
            
            scientistRole.currentCharge = GameManager.Instance.LogicOptions.GetScientistBatteryCharge();
        
        }
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

    public static void murderAllCheat()
    {
        if (CheatToggles.murderAll){

            // Kill all players by sending a successful MurderPlayer RPC call
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Utils.murderPlayer(player, MurderResultFlags.Succeeded);
            }

            CheatToggles.murderAll = false;

        }
    }

    public static async void ContinuousKillingAllCheck(){
        // var thread = new Thread(() =>
        // {
        try
        {
            if (CheatToggles.ContinuousKillingAll)
            {
                //狂欢起来
                //Kill all players by sending a successful MurderPlayer RPC call to all clients
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    Utils.murderPlayer(player, MurderResultFlags.Succeeded);
                }

                Thread.Sleep(500);
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
        //});
        //thread.Start();
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

    public static void speedBoostCheat()
    {
        //try-catch to avoid some errors I was reciving in the logs related to this cheat

        try{

            //PlayerControl.LocalPlayer.MyPhysics.Speed is the base speed of a player
            //Among Us uses this value with the associated game setting to calculate the TrueSpeed of the player
            
            if(CheatToggles.speedBoost){
                PlayerControl.LocalPlayer.MyPhysics.Speed = 4.5f * 2;
            }else{
                PlayerControl.LocalPlayer.MyPhysics.Speed = 4.5f; //By default, Speed is always 2.5f
            }

        }catch{}
    }

    public static void noClipCheat()
    {
        try{

            PlayerControl.LocalPlayer.Collider.enabled = !(CheatToggles.noClip || PlayerControl.LocalPlayer.onLadder);

        }catch{}
    }
}