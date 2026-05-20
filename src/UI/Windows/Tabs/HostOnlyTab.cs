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

        DrawMurder();

        GUILayout.Space(15);

        DrawGameState();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        DrawMeetings();

        GUILayout.Space(15);

        DrawSmartKill();

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawGeneral()
    {
        CheatToggles.killVanished = GUILayout.Toggle(CheatToggles.killVanished, " Kill While Vanished");

        CheatToggles.killAnyone = GUILayout.Toggle(CheatToggles.killAnyone, " Kill Anyone");

        CheatToggles.noKillCd = GUILayout.Toggle(CheatToggles.noKillCd, " No Kill Cooldown");

        CheatToggles.showProtectMenu = GUILayout.Toggle(CheatToggles.showProtectMenu, " Show Protect Menu");

        CheatToggles.showForceTeleportMenu = GUILayout.Toggle(CheatToggles.showForceTeleportMenu, " Force Teleport");

        CheatToggles.freezePlayer = GUILayout.Toggle(CheatToggles.freezePlayer, " Freeze Player");

        CheatToggles.showKillCdOverlay = GUILayout.Toggle(CheatToggles.showKillCdOverlay, " Kill CD Overlay");

        CheatToggles.showMeetingHistory = GUILayout.Toggle(CheatToggles.showMeetingHistory, " Meeting History");

        CheatToggles.antiBotKick = GUILayout.Toggle(CheatToggles.antiBotKick, " Anti-Bot Kick");

        CheatToggles.autoKickVentImpostors = GUILayout.Toggle(CheatToggles.autoKickVentImpostors, " Auto Kick Vent Impostors");

        // CheatToggles.forceRole = GUILayout.Toggle(CheatToggles.forceRole, " Force Role");

        // CheatToggles.noOptionsLimits = GUILayout.Toggle(CheatToggles.noOptionsLimits, " No Options Limits");
    }

    private void DrawMurder()
    {
        GUILayout.Label("Murder", GUIStylePreset.TabSubtitle);

        CheatToggles.killPlayer = GUILayout.Toggle(CheatToggles.killPlayer, " Kill Player");

        CheatToggles.telekillPlayer = GUILayout.Toggle(CheatToggles.telekillPlayer, " Telekill Player");

        CheatToggles.killAllCrew = GUILayout.Toggle(CheatToggles.killAllCrew, " Kill All Crewmates");

        CheatToggles.killAllImps = GUILayout.Toggle(CheatToggles.killAllImps, " Kill All Impostors");

        CheatToggles.killAll = GUILayout.Toggle(CheatToggles.killAll, " Kill Everyone");
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

    private void DrawSmartKill()
    {
        GUILayout.Label("Smart Kill", GUIStylePreset.TabSubtitle);

        var (canExecute, statusText) = MalumCheats.SmartKillStatus();

        var prevColor = GUI.contentColor;
        GUI.contentColor = canExecute ? Color.green : new Color(1f, 0.4f, 0.4f);
        GUILayout.Label(statusText);
        GUI.contentColor = prevColor;

        GUI.enabled = canExecute;
        if (GUILayout.Button("Close Door + Kill + Vent", GUIStylePreset.NormalButton))
            MalumCheats.SmartKillCombo();
        GUI.enabled = true;
    }
}
