using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class ESP_HudManagerPostfix
{
    public static bool resolutionchangeNeeded;

    //Postfix patch of HudManager.Update for several HUD modules
    public static void Postfix(HudManager __instance){
        //Remove ShadowQuad if CheatSettings.fullBright or camera is zoomed-out
        if(CheatSettings.fullBright || Camera.main.orthographicSize > 3f){
            __instance.ShadowQuad.gameObject.SetActive(false);
        }else{
            __instance.ShadowQuad.gameObject.SetActive(true);
        }

        //Allow zooming-out through mouse wheel if CheatSettings.zoomOut is enabled
        if(CheatSettings.zoomOut){
            resolutionchangeNeeded = true;
            if(Input.GetAxis("Mouse ScrollWheel") < 0f ){
                Camera.main.orthographicSize++; //Uses the orthographicSize of both the main camera and the UI camera
                __instance.UICamera.orthographicSize++;
                Utils.AdjustResolution(); //Utils.AdjustResolution() is needed to properly position the game's UI after a change in orthographicSize
            } else if(Input.GetAxis("Mouse ScrollWheel") > 0f ){
                if (Camera.main.orthographicSize > 3f){
                    Camera.main.orthographicSize--;
                    __instance.UICamera.orthographicSize--;
                    Utils.AdjustResolution();
                }
            }
        //Reset camera when CheatSettings.zoomOut is disabled
        } else {
            Camera.main.orthographicSize = 3f;
            __instance.UICamera.orthographicSize = 3f;
            if (resolutionchangeNeeded){ //Only invoked once when CheatSettings.zoomOut is disabled to prevent problems with the UI
                Utils.AdjustResolution();
                resolutionchangeNeeded = false;
            }
        }
    }
}    