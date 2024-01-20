using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class ESP_HudManagerPostfix
{
    public static bool resolutionchangeNeeded;
    public static bool chatActive;
    public static bool fullBrightActive;

    //Postfix patch of HudManager.Update for several HUD modules
    public static void Postfix(HudManager __instance){
        //Remove all shadows if CheatSettings.fullBright or camera is zoomed-out or spectating/freecam
        fullBrightActive = CheatSettings.fullBright || Camera.main.orthographicSize > 3f || Camera.main.gameObject.GetComponent<FollowerCamera>().Target != PlayerControl.LocalPlayer;
        __instance.ShadowQuad.gameObject.SetActive(!fullBrightActive);

        //Allow seeing chat icon in-game even while you're not supposed to
        try{
            chatActive =  CheatSettings.alwaysChat || MeetingHud.Instance || !ShipStatus.Instance || PlayerControl.LocalPlayer.Data.IsDead;
        }catch{chatActive = false;}
        __instance.Chat.gameObject.SetActive(chatActive);

        //Allow zooming-out through mouse wheel if CheatSettings.zoomOut is enabled
        if(CheatSettings.zoomOut){
            resolutionchangeNeeded = true;
            if(Input.GetAxis("Mouse ScrollWheel") < 0f ){
                Camera.main.orthographicSize++; //Uses the orthographicSize of both the main camera and the UI camera
                __instance.UICamera.orthographicSize++;
                Utils.adjustResolution(); //Utils.AdjustResolution() is needed to properly position the game's UI after a change in orthographicSize
            } else if(Input.GetAxis("Mouse ScrollWheel") > 0f ){
                if (Camera.main.orthographicSize > 3f){
                    Camera.main.orthographicSize--;
                    __instance.UICamera.orthographicSize--;
                    Utils.adjustResolution();
                }
            }
        //Reset camera when CheatSettings.zoomOut is disabled
        } else {
            Camera.main.orthographicSize = 3f;
            __instance.UICamera.orthographicSize = 3f;
            if (resolutionchangeNeeded){ //Only invoked once when CheatSettings.zoomOut is disabled to prevent problems with the UI
                Utils.adjustResolution();
                resolutionchangeNeeded = false;
            }
        }

        //Close player pick menu when it is not being used
        if (Utils_PlayerPickMenu.playerpickMenu != null && !CheatSettings.copyAllOutfits && !CheatSettings.chatMimic && !CheatSettings.saveSpoofData && !CheatSettings.callMeeting && !CheatSettings.teleportPlayer && !CheatSettings.copyOutfit && !CheatSettings.murderPlayer && !CheatSettings.kickPlayer && !CheatSettings.spectate){
            Utils_PlayerPickMenu.playerpickMenu.Close();
        }
    }
}    