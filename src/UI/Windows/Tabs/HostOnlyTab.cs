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
        CheatToggles.noKillCd = GUILayout.Toggle(CheatToggles.noKillCd, " No Kill Cooldown");

        CheatToggles.showProtectMenu = GUILayout.Toggle(CheatToggles.showProtectMenu, " Show Protect Menu");

        CheatToggles.showForceTeleportMenu = GUILayout.Toggle(CheatToggles.showForceTeleportMenu, " Force Teleport Menu");

        CheatToggles.showKillCdOverlay = GUILayout.Toggle(CheatToggles.showKillCdOverlay, " Show Impostor Kill Timer Above Players");

        CheatToggles.showMeetingHistory = GUILayout.Toggle(CheatToggles.showMeetingHistory, " Meeting History");

        CheatToggles.alwaysImpostor = GUILayout.Toggle(CheatToggles.alwaysImpostor, " Always Impostor");

        CheatToggles.antiBotKick = GUILayout.Toggle(CheatToggles.antiBotKick, " Auto-Kick Bots");

        CheatToggles.autoKickVentImpostors = GUILayout.Toggle(CheatToggles.autoKickVentImpostors, " Disconnect Players Who Vent");

        // CheatToggles.forceRole = GUILayout.Toggle(CheatToggles.forceRole, " Force Role");

        // CheatToggles.noOptionsLimits = GUILayout.Toggle(CheatToggles.noOptionsLimits, " No Options Limits");
    }

    private void DrawGameState()
    {
        GUILayout.Label("Game State", GUIStylePreset.TabSubtitle);

        CheatToggles.forceStartGame = GUILayout.Toggle(CheatToggles.forceStartGame, " Force Start Game");

        CheatToggles.noGameEnd = GUILayout.Toggle(CheatToggles.noGameEnd, " No Game End");

        GUILayout.Label("Force End Game", GUIStylePreset.TabSubtitle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Impostors Win", GUIStylePreset.NormalButton)) MalumCheats.ForceEndGameCheat(true);
        if (GUILayout.Button("Crewmates Win", GUIStylePreset.NormalButton)) MalumCheats.ForceEndGameCheat(false);
        GUILayout.EndHorizontal();
    }

    private void DrawMeetings()
    {
        GUILayout.Label("Meetings", GUIStylePreset.TabSubtitle);

        CheatToggles.skipMeeting = GUILayout.Toggle(CheatToggles.skipMeeting, " Skip Meeting");

        CheatToggles.voteImmune = GUILayout.Toggle(CheatToggles.voteImmune, " Vote Immune");

        CheatToggles.ejectPlayer = GUILayout.Toggle(CheatToggles.ejectPlayer, " Eject Player");

        CheatToggles.infiniteMeetings = GUILayout.Toggle(CheatToggles.infiniteMeetings, " Infinite Meetings");
    }
}
