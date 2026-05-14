using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace MalumMenu;

public class ColorsUI : MonoBehaviour
{
    public static int windowHeight = 400;
    public static int windowWidth = 400;
    private Rect _windowRect;

    private Vector2 _scrollPosition = Vector2.zero;
    private GUIStyle _playerHeaderStyle;
    private readonly Dictionary<string, bool> _expandedPlayers = new();

    private void Start()
    {
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showColorsMenu || !(MenuUI.isGUIActive || MalumMenu.menuKeepSubwindowsOpen.Value) || MalumMenu.isPanicked) return;

        _playerHeaderStyle ??= new GUIStyle(GUI.skin.button)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleLeft
        };

        UIHelpers.ApplyUIColor();

        _windowRect = GUI.Window((int)WindowId.ColorsUI, _windowRect, (GUI.WindowFunction)ColorsWindow, "Colors");
    }

    private void ColorsWindow(int windowID)
    {
        GUILayout.BeginVertical();

        if (!Utils.isShip)
        {
            GUILayout.Label("You're not in a lobby, join one to change people's colors", GUIStylePreset.TabSubtitle);
            GUILayout.EndVertical();
            GUI.DragWindow();
            return;
        }

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        // Keep track of taken colors
        var takenColors = PlayerControl.AllPlayerControls.ToArray().Select(p => (byte)p.CurrentOutfit.ColorId).ToHashSet();

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.Data || string.IsNullOrEmpty(player.Data.PlayerName)) continue;

            GUILayout.BeginVertical();

            var nameKey = player.Data.PlayerName;
            _expandedPlayers.TryGetValue(nameKey, out var expanded);
            var arrow = expanded ? "\u25BC" : "\u25B6";

            if (GUILayout.Button($"{arrow} <color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{nameKey}</color>", _playerHeaderStyle))
            {
                _expandedPlayers[nameKey] = !expanded;
                expanded = !expanded;
            }

            if (expanded)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(25);
                GUILayout.BeginVertical();

                int colorsPerRow = 8;
                int currentInRow = 0;

                GUILayout.BeginHorizontal();
                // Iterate through available color IDs
                byte colorsCount = (byte)Palette.PlayerColors.Length;

                for (byte colorId = 0; colorId < colorsCount; colorId++)
                {
                    if (currentInRow >= colorsPerRow)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        currentInRow = 0;
                    }

                    bool isTaken = takenColors.Contains(colorId);
                    bool isCurrent = player.CurrentOutfit.ColorId == colorId;

                    GUI.enabled = !isTaken && !isCurrent;

                    var color = Palette.PlayerColors[colorId];
                    if (GUILayout.Button($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>■</color>", GUILayout.Width(35)))
                    {
                        if (Utils.isHost) // Extra safety check before sending RPC
                        {
                            player.RpcSetColor(colorId);
                        }
                    }
                    GUI.enabled = true;
                    currentInRow++;
                }

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        GUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUI.DragWindow();
    }
}

