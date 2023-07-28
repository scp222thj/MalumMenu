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

            //Open custom spectator menu when spectate is first clicked
            if (!isActive){

                //Spectator menu based on shapeshifter menu
                Utils_PlayerPickMenu.openPlayerPickMenu(PlayerControl.AllPlayerControls, (Action) (() =>
                {
                    Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(Utils_PlayerPickMenu.targetPlayer);
                }));

                isActive = true;

                PlayerControl.LocalPlayer.moveable = false;

                CheatSettings.freeCam = false;

            }

            //Stop spectate if "X" button is clicked on spectator menu
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

            //Close spectator menu
            if (Utils_PlayerPickMenu.playerpickMenu != null){
                Utils_PlayerPickMenu.playerpickMenu.Close();
            }
        }
    }
}