using HarmonyLib;
using UnityEngine;
using System;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Spectate_MainPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open spectator menu
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.spectate){

            //Open spectator menu when CheatSettings.spectate is first enabled
            if (!isActive){

                //New player pick menu made for spectating
                Utils_PlayerPickMenu.openPlayerPickMenu(PlayerControl.AllPlayerControls, (Action) (() =>
                {
                    Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(Utils_PlayerPickMenu.targetPlayer);
                }));

                isActive = true;

                PlayerControl.LocalPlayer.moveable = false; //Can't move while spectating

                CheatSettings.freeCam = false; //Disable freecam while spectating

            }

            //Deactivate spectator mode if menu is closed without spectating anyone
            if (Utils_PlayerPickMenu.playerpickMenu == null && Camera.main.gameObject.GetComponent<FollowerCamera>().Target == PlayerControl.LocalPlayer){
                CheatSettings.spectate = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }else{
            //Deactivate spectator mode when CheatSettings.spectate is disabled
            if (isActive){
                isActive = false;
                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            }

            //Close spectator menu when CheatSettings.spectate is disabled
            if (Utils_PlayerPickMenu.playerpickMenu != null){
                Utils_PlayerPickMenu.playerpickMenu.Close();
            }
        }
    }
}