using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class CloseMeeting_ShipStatus_FixedUpdate_Postfix
{
    //Postfix patch of ShipStatus.FixedUpdate
    public static void Postfix(ShipStatus __instance)
    {
        if (CheatToggles.closeMeeting)
        {
            if (MeetingHud.Instance)
            { // Closes MeetingHud window if it is open
                MeetingHud.Instance.DespawnOnDestroy = false;
                ExileController exileController = UnityEngine.Object.Instantiate<ExileController>(ShipStatus.Instance.ExileCutscenePrefab);
                UnityEngine.Object.Destroy(MeetingHud.Instance.gameObject);
                exileController.ReEnableGameplay();
                exileController.WrapUp();
            }

            CheatToggles.closeMeeting = false; // Disables the cheat, now that it's complete.
        }
    }
}