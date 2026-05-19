using UnityEngine;

namespace MalumMenu;

public class PassiveTab : ITab
{
    public string name => "Passive";

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawNameSpoof();

        GUILayout.EndVertical();
    }

    private void DrawNameSpoof()
    {
        GUILayout.Label("Name Spoof", GUIStylePreset.TabSubtitle);
        CheatToggles.spoofedName = GUILayout.TextField(CheatToggles.spoofedName, 20, GUILayout.Width(200f));
        if (GUILayout.Button("Apply Name", GUIStylePreset.NormalButton))
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(CheatToggles.spoofedName))
                    PlayerControl.LocalPlayer.RpcSetName(CheatToggles.spoofedName);
            }
            catch { }
        }
    }

    private void DrawGeneral()
    {
        CheatToggles.freeCosmetics = GUILayout.Toggle(CheatToggles.freeCosmetics, " Free Cosmetics");

        CheatToggles.avoidPenalties = GUILayout.Toggle(CheatToggles.avoidPenalties, " Avoid Penalties");

        CheatToggles.unlockFeatures = GUILayout.Toggle(CheatToggles.unlockFeatures, " Unlock Extra Features");

        CheatToggles.copyLobbyCodeOnDisconnect = GUILayout.Toggle(CheatToggles.copyLobbyCodeOnDisconnect, " Copy Lobby Code on Disconnect");

        CheatToggles.spoofAprilFoolsDate = GUILayout.Toggle(CheatToggles.spoofAprilFoolsDate, " Spoof Date to April 1st");
    }
}
