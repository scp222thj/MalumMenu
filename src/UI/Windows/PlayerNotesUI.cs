using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

public class PlayerNotesUI : MonoBehaviour
{
    public static int windowHeight = 340;
    public static int windowWidth  = 360;
    private Rect    _windowRect;
    private Vector2 _scrollPos;

    private byte?  _editingId;
    private string _inputBuf = "";

    public static readonly Dictionary<byte, string> Notes = new();

    private void Start()
    {
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f + 460f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showPlayerNotes || !(MenuUI.isGUIActive || MalumMenu.menuKeepSubwindowsOpen.Value) || MalumMenu.isPanicked || CheatToggles.streamerMode) return;
        UIHelpers.ApplyUIColor();
        _windowRect = GUI.Window((int)WindowId.PlayerNotesUI, _windowRect, (GUI.WindowFunction)DrawWindow, "Player Notes");
    }

    private void DrawWindow(int id)
    {
        GUILayout.BeginVertical();
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, true);

        if (!Utils.isInGame && !Utils.isMeeting)
        {
            GUILayout.Label("Available in-game.", GUIStylePreset.TabSubtitle);
        }
        else
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == null || player.Data == null) continue;
                byte   pid  = player.PlayerId;
                Notes.TryGetValue(pid, out var note);

                GUILayout.BeginHorizontal();
                GUILayout.Label(player.Data.PlayerName, GUILayout.Width(110f));

                if (_editingId == pid)
                {
                    _inputBuf = GUILayout.TextField(_inputBuf, 30, GUILayout.Width(140f));
                    if (GUILayout.Button("✓", GUIStylePreset.NormalButton, GUILayout.Width(28f)))
                    {
                        var t = _inputBuf.Trim();
                        if (t.Length > 0) Notes[pid] = t; else Notes.Remove(pid);
                        _editingId = null;
                    }
                }
                else
                {
                    GUILayout.Label(note ?? "", GUILayout.Width(140f));
                    if (GUILayout.Button("✎", GUIStylePreset.NormalButton, GUILayout.Width(28f)))
                    {
                        _editingId = pid;
                        _inputBuf  = note ?? "";
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
        GUILayout.Box("", GUIStylePreset.Separator, GUILayout.Height(1f), GUILayout.ExpandWidth(true));
        GUILayout.Space(1f);
        if (GUILayout.Button("Clear All", GUIStylePreset.NormalButton))
        { Notes.Clear(); _editingId = null; }
        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    // Returns "[note]" suffix for nametag, or empty string
    public static string GetNoteSuffix(byte playerId)
    {
        return Notes.TryGetValue(playerId, out var n) && n.Length > 0 ? $" <size=70%><color=#ffff00>[{n}]</color></size>" : "";
    }

    public static void Clear() => Notes.Clear();
}
