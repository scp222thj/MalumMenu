using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class CloseMeeting_ShipStatus_FixedUpdate_Postfix
{
    //Postfix patch of ShipStatus.FixedUpdate to close meeting or exile animation at will
    public static void Postfix(ShipStatus __instance)
    {
        if(CheatToggles.closeMeeting){
            
            if (MeetingHud.Instance){ //Closes MeetingHud window if it is open
                MeetingHud.Instance.DespawnOnDestroy = false;
                ExileController exileController = UnityEngine.Object.Instantiate<ExileController>(ShipStatus.Instance.ExileCutscenePrefab);
                UnityEngine.Object.Destroy(MeetingHud.Instance.gameObject);
                exileController.ReEnableGameplay();
                exileController.WrapUp();

            }else if (ExileController.Instance != null){ //Closes ExileController window if it is open
                ExileController.Instance.ReEnableGameplay();
                ExileController.Instance.WrapUp();
            }
            
            CheatToggles.closeMeeting = false; //Button behaviour
        }
    }
}