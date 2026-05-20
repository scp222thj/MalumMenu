using UnityEngine;

namespace MalumMenu;

public class PassiveTab : ITab
{
    public string name => "Passive";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawNameSpoof();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
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
        CheatToggles.freeCosmetics = GUILayout.Toggle(CheatToggles.freeCosmetics, " Unlock All Cosmetics Free");

        CheatToggles.avoidPenalties = GUILayout.Toggle(CheatToggles.avoidPenalties, " Skip Vote/Ban Penalties");

        CheatToggles.unlockFeatures = GUILayout.Toggle(CheatToggles.unlockFeatures, " Unlock Hidden Game Features");

        CheatToggles.copyLobbyCodeOnDisconnect = GUILayout.Toggle(CheatToggles.copyLobbyCodeOnDisconnect, " Copy Lobby Code When Disconnected");

        CheatToggles.spoofAprilFoolsDate = GUILayout.Toggle(CheatToggles.spoofAprilFoolsDate, " Enable April Fools Mode");
    }
}
