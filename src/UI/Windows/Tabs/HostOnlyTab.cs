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

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void DrawGeneral()
    {
        CheatToggles.showForceRoleMenu = GUILayout.Toggle(CheatToggles.showForceRoleMenu, " Show Force Role Menu");

        CheatToggles.killVanished = GUILayout.Toggle(CheatToggles.killVanished, " Kill While Vanished");

        CheatToggles.killAnyone = GUILayout.Toggle(CheatToggles.killAnyone, " Kill Anyone");

        CheatToggles.noKillCd = GUILayout.Toggle(CheatToggles.noKillCd, " No Kill Cooldown");

        CheatToggles.showProtectMenu = GUILayout.Toggle(CheatToggles.showProtectMenu, " Show Protect Menu");

        // CheatToggles.forceRoleOld = GUILayout.Toggle(CheatToggles.forceRoleOld, " Force Role (Legacy)");

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
    }

    private void DrawMeetings()
    {
        GUILayout.Label("Meetings", GUIStylePreset.TabSubtitle);

        CheatToggles.skipMeeting = GUILayout.Toggle(CheatToggles.skipMeeting, " Skip Meeting");

        CheatToggles.voteImmune = GUILayout.Toggle(CheatToggles.voteImmune, " Vote Immune");

        CheatToggles.ejectPlayer = GUILayout.Toggle(CheatToggles.ejectPlayer, " Eject Player");
    }
}
