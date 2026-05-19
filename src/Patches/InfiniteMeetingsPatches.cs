using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcStartMeeting))]
public static class PlayerControl_RpcStartMeeting
{
    // Postfix patch of PlayerControl.RpcStartMeeting to immediately restore the host's
    // remaining emergency meeting count after each use, enabling infinite meetings.
    public static void Postfix()
    {
        if (!CheatToggles.infiniteMeetings || !Utils.isHost || PlayerControl.LocalPlayer == null) return;

        PlayerControl.LocalPlayer.RemainingEmergencies = 99;
    }
}
