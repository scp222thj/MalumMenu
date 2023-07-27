using HarmonyLib;
using UnityEngine;
using System;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Spectate_MainPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open spectator menu
    public static ShapeshifterMinigame shapeshifterMinigame;
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.spectate){

            //Open custom spectator menu when spectate is first clicked
            if (!isActive){

                //Spectator menu based on shapeshifter menu
                shapeshifterMinigame = UnityEngine.Object.Instantiate<ShapeshifterMinigame>(Utils.GetShapeshifterMenu());
			    
                shapeshifterMinigame.transform.SetParent(Camera.main.transform, false);
			    shapeshifterMinigame.transform.localPosition = new Vector3(0f, 0f, -50f);
			    shapeshifterMinigame.Begin(null);

                isActive = true;

            }

            //Stop spectate if "X" button is clicked on spectator menu
            if (shapeshifterMinigame == null && Camera.main.gameObject.GetComponent<FollowerCamera>().Target == PlayerControl.LocalPlayer){
                CheatSettings.spectate = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }else{
            //Deactivate spectator mode when CheatSettings.spectate is disabled
            if (isActive){
                isActive = false;
                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().Target = PlayerControl.LocalPlayer;
            }

            //Close spectator menu
            if (shapeshifterMinigame != null){
                shapeshifterMinigame.Close();
            }
        }
    }
}   

[HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
public static class Spectate_BeginPostfix
{
    //Prefix patch of ShapeshifterMinigame.Begin to implement spectator menu logic
    public static bool Prefix(PlayerTask task, ShapeshifterMinigame __instance)
    {
        if (CheatSettings.spectate){ //Spectator menu logic

            //All players are considered including ghosts & LocalPlayer
            List<PlayerControl> list = PlayerControl.AllPlayerControls;

            __instance.potentialVictims = new List<ShapeshifterPanel>();
            List<UiElement> list2 = new List<UiElement>();

            for (int i = 0; i < list.Count; i++)
            {
                PlayerControl player = list[i];
                int num = i % 3;
                int num2 = i / 3;
                bool flag = PlayerControl.LocalPlayer.Data.Role.NameColor == player.Data.Role.NameColor;
                ShapeshifterPanel shapeshifterPanel = UnityEngine.Object.Instantiate<ShapeshifterPanel>(__instance.PanelPrefab, __instance.transform);
                shapeshifterPanel.transform.localPosition = new Vector3(__instance.XStart + (float)num * __instance.XOffset, __instance.YStart + (float)num2 * __instance.YOffset, -1f);
                
                //Custom spectating code when clicking on player
                shapeshifterPanel.SetPlayer(i, player.Data, (Action) (() =>
                {
                    Camera.main.gameObject.GetComponent<FollowerCamera>().Target = player;

                    PlayerControl.LocalPlayer.moveable = false; //Can't move while spectating someone else

                    __instance.Close();
                }));

                shapeshifterPanel.NameText.color = (flag ? player.Data.Role.NameColor : Color.white);
                __instance.potentialVictims.Add(shapeshifterPanel);
                list2.Add(shapeshifterPanel.Button);
            }

            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2, false);
            
            return false; //Skip original method when spectating

        }

        return true; //Open normal shapeshifter menu if not spectating
    }
}   