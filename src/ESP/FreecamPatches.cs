using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class FreeCam_HudManager_Update_Postfix
{
    public static bool isActive;

    //Postifx patch of HudManager.Update to implement a freecam
    public static void Postfix(HudManager __instance){
        if(CheatToggles.freeCam){
            //Disable FollowerCamera & prevent the player from moving while in freecam
            if (!isActive){

                Camera.main.gameObject.GetComponent<FollowerCamera>().enabled = false;
                Camera.main.gameObject.GetComponent<FollowerCamera>().Target = null;

                isActive = true;

            }

            PlayerControl.LocalPlayer.moveable = false;

            //Get keyboard input & turn it into movement for the camera
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

            //Change the camera's position depending on the input
            //Speed: 10f
            Camera.main.transform.position = Camera.main.transform.position + movement * 10f * Time.deltaTime;
            
        }else{
            if (isActive){

                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().enabled = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
                isActive = false;

            }
        }
    }
}    