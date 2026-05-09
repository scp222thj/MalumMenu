using UnityEngine;

namespace MalumMenu;

public class PassiveTab : ITab
{
    public string name => "Passive";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.EndVertical();
    }

    private void DrawGeneral()
    {
        CheatToggles.freeCosmetics = GUILayout.Toggle(CheatToggles.freeCosmetics, " Free Cosmetics");

        CheatToggles.avoidPenalties = GUILayout.Toggle(CheatToggles.avoidPenalties, " Avoid Penalties");

        CheatToggles.unlockFeatures = GUILayout.Toggle(CheatToggles.unlockFeatures, " Unlock Extra Features");

        CheatToggles.copyLobbyCodeOnDisconnect = GUILayout.Toggle(CheatToggles.copyLobbyCodeOnDisconnect, " Copy Lobby Code on Disconnect");

        CheatToggles.spoofAprilFoolsDate = GUILayout.Toggle(CheatToggles.spoofAprilFoolsDate, " Spoof Date to April 1st");

        CheatToggles.antiCrashProtection = GUILayout.Toggle(CheatToggles.antiCrashProtection, " Anti-Crash Protection");

        CheatToggles.antiCrashPopups = GUILayout.Toggle(CheatToggles.antiCrashPopups, " Anti-Crash Popups");

        CheatToggles.autoBanCrashers = GUILayout.Toggle(CheatToggles.autoBanCrashers, " Auto Ban Crashers (Host)");
    }
}
