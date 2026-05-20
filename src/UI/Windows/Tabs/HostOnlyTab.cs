using UnityEngine;

namespace MalumMenu;

public class HostOnlyTab : ITab
{
    public string name => "Host-Only";

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawGameState();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        DrawMeetings();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawGeneral()
    {
        CheatToggles.noKillCd = GUILayout.Toggle(CheatToggles.noKillCd, " No Kill Cooldown (Host Only)");

        CheatToggles.showProtectMenu = GUILayout.Toggle(CheatToggles.showProtectMenu, " Show Protect Menu (Host Only)");

        CheatToggles.showForceTeleportMenu = GUILayout.Toggle(CheatToggles.showForceTeleportMenu, " Force Teleport Menu (Host Only)");

        CheatToggles.showKillCdOverlay = GUILayout.Toggle(CheatToggles.showKillCdOverlay, " Show Impostor Kill Timer Above Players (Host Only)");

        CheatToggles.showMeetingHistory = GUILayout.Toggle(CheatToggles.showMeetingHistory, " Meeting History (Host Only)");

        CheatToggles.alwaysImpostor = GUILayout.Toggle(CheatToggles.alwaysImpostor, " Always Impostor (Host Only)");

        if (Utils.isHost)
            CheatToggles.assignImpostor = GUILayout.Toggle(CheatToggles.assignImpostor, " Assign Impostor (Host Only)");

        CheatToggles.antiBotKick = GUILayout.Toggle(CheatToggles.antiBotKick, " Auto-Kick Bots (Host Only)");

        CheatToggles.autoKickVentImpostors = GUILayout.Toggle(CheatToggles.autoKickVentImpostors, " Disconnect Players Who Vent (Host Only)");

        // CheatToggles.forceRole = GUILayout.Toggle(CheatToggles.forceRole, " Force Role (Host Only)");

        // CheatToggles.noOptionsLimits = GUILayout.Toggle(CheatToggles.noOptionsLimits, " No Options Limits (Host Only)");
    }

    private void DrawGameState()
    {
        GUILayout.Label("Game State", GUIStylePreset.TabSubtitle);

        CheatToggles.forceStartGame = GUILayout.Toggle(CheatToggles.forceStartGame, " Force Start Game (Host Only)");

        CheatToggles.noGameEnd = GUILayout.Toggle(CheatToggles.noGameEnd, " No Game End (Host Only)");

        GUILayout.Label("Force End Game (Host Only)", GUIStylePreset.TabSubtitle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Impostors Win", GUIStylePreset.NormalButton)) MalumCheats.ForceEndGameCheat(true);
        if (GUILayout.Button("Crewmates Win", GUIStylePreset.NormalButton)) MalumCheats.ForceEndGameCheat(false);
        GUILayout.EndHorizontal();
    }

    private void DrawMeetings()
    {
        GUILayout.Label("Meetings", GUIStylePreset.TabSubtitle);

        CheatToggles.skipMeeting = GUILayout.Toggle(CheatToggles.skipMeeting, " Skip Meeting (Host Only)");

        CheatToggles.voteImmune = GUILayout.Toggle(CheatToggles.voteImmune, " Vote Immune (Host Only)");

        CheatToggles.ejectPlayer = GUILayout.Toggle(CheatToggles.ejectPlayer, " Eject Player (Host Only)");

        CheatToggles.infiniteMeetings = GUILayout.Toggle(CheatToggles.infiniteMeetings, " Infinite Meetings (Host Only)");
    }
}
