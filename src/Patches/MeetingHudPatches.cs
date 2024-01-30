using HarmonyLib;
using Sentry.Internal.Extensions;

namespace MalumMenu;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class SeeRoles_MeetingHud_Update_Postfix
{
    //Postfix patch of MeetingHud.Update to get colored names in meetings
    public static void Postfix(MeetingHud __instance){

        MalumESP.meetingNametags(__instance);

        //Bugfix: NoClip staying active if meeting is called whilst climbing ladder
        PlayerControl.LocalPlayer.onLadder = false;
    }
}    