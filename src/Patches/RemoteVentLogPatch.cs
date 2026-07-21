using HarmonyLib;
using Hazel;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleRpc))]
public static class RemoteVentLogPatch
{
    public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        try
        {
            if (__instance?.myPlayer == null || __instance.myPlayer == PlayerControl.LocalPlayer) return;

            byte ENTER_VENT = 11;
            byte EXIT_VENT = 12;

            if (callId == ENTER_VENT || callId == EXIT_VENT)
            {
                int oldPos = reader.Position;
                try
                {
                    int ventId = reader.ReadPackedInt32();
                    bool entering = callId == ENTER_VENT;
                    EventLogger.LogVent(__instance.myPlayer, ventId, entering);
                }
                catch { }
                finally { reader.Position = oldPos; }
            }
        }
        catch { }
    }
}
