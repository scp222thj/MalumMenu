using HarmonyLib;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(GameCode), nameof(GameCode.IntToGameName))]
public static class GameCode_IntToGameName
{
    // Postfix patch of GameCode.IntToGameName to replace the lobby code with "XXXXXX"
    // whenever streamer mode is active, preventing it from being visible on screen or in recordings.
    public static void Postfix(ref string __result)
    {
        if (CheatToggles.streamerMode) __result = "XXXXXX";
    }
}
