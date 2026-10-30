using UnityEngine;
using System;

namespace MalumMenu;

public class HostOnlyTab : ITab
{
    public string name => "Host-Only";
    // Integrated color picker state (persists while the tab exists)
    private int _selectedColorId = 0;

    public void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.Space(15);

        DrawColors();

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
        CheatToggles.killVanished = GUILayout.Toggle(CheatToggles.killVanished, " Kill While Vanished");

        CheatToggles.killAnyone = GUILayout.Toggle(CheatToggles.killAnyone, " Kill Anyone");

        CheatToggles.noKillCd = GUILayout.Toggle(CheatToggles.noKillCd, " No Kill Cooldown");

        CheatToggles.showProtectMenu = GUILayout.Toggle(CheatToggles.showProtectMenu, " Show Protect Menu");

        // The color picker is now integrated into this tab. The legacy separate Colors window toggle
        // is kept in code for backwards compatibility but is not exposed here.
        // CheatToggles.showColorsMenu = GUILayout.Toggle(CheatToggles.showColorsMenu, " Show Colors Menu");

        // CheatToggles.forceRole = GUILayout.Toggle(CheatToggles.forceRole, " Force Role");

        // CheatToggles.noOptionsLimits = GUILayout.Toggle(CheatToggles.noOptionsLimits, " No Options Limits");
    }

    private void DrawColors()
    {
        GUILayout.Label("Colors", GUIStylePreset.TabSubtitle);

        int colorsCount = 0;
        try
        {
            colorsCount = Palette.PlayerColors.Length;
        }
        catch
        {
            colorsCount = 0;
        }

        if (colorsCount == 0)
        {
            GUILayout.Label("No palette available", GUIStylePreset.TabSubtitle);
            return;
        }

        // Slider to pick a color id
        GUILayout.BeginHorizontal();
        GUILayout.Label("Color ID:", GUILayout.Width(70));
        byte newColorId = (byte)GUILayout.HorizontalSlider(_selectedColorId, 0, colorsCount - 1);
        if (newColorId != _selectedColorId)
        {
            _selectedColorId = (int)newColorId;
            CheatToggles.colorSetPlayerId = newColorId;
        }
        GUILayout.Label($"#{_selectedColorId}", GUILayout.Width(40));

        var swatch = Palette.PlayerColors[_selectedColorId];
        var swatchHtml = ColorUtility.ToHtmlStringRGB(swatch);
        if (GUILayout.Button($"<color=#{swatchHtml}>■</color>", GUILayout.Width(35))) { }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUI.enabled = Utils.isHost; // Only hosts may change other players' colors
        if (GUILayout.Button("Set Local Player", GUILayout.Height(25)))
        {
            if (Utils.isHost && Utils.isPlayer)
            {
                PlayerControl.LocalPlayer.RpcSetColor((byte)_selectedColorId);
            }
        }

        if (GUILayout.Button("Set All", GUILayout.Height(25)))
        {
            if (Utils.isHost)
            {
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    p.RpcSetColor((byte)_selectedColorId);
                }
            }
        }

        if (GUILayout.Button("Randomize All", GUILayout.Height(25)))
        {
            if (Utils.isHost)
            {
                for (int i = 0; i < PlayerControl.AllPlayerControls.Count; i++)
                {
                    var p = PlayerControl.AllPlayerControls[i];
                    var rnd = UnityEngine.Random.Range(0, colorsCount);
                    p.RpcSetColor((byte)rnd);
                }
            }
        }

        GUI.enabled = true;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        // Randomize Player button - driven by CheatToggle
        if (GUILayout.Button("Randomize Player", GUILayout.Height(25)))
        {
            CheatToggles.colorRandomizePlayer = true;
        }

        // Set Player button - driven by CheatToggle
        if (GUILayout.Button("Set Player...", GUILayout.Height(25)))
        {
            CheatToggles.colorSetPlayer = true;
        }
        GUILayout.EndHorizontal();
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
