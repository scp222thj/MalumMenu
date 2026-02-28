using UnityEngine;

namespace MalumMenu;

public class RolesUI : MonoBehaviour
{
    private Vector2 _scrollPosition = Vector2.zero;
    private Rect _windowRect = new(320, 10, 450, 100);

    private void OnGUI()
    {
        if (!CheatToggles.showRolesMenu) return;

        UIHelper.ApplyUIColor();

        _windowRect = GUI.Window(4, _windowRect, (GUI.WindowFunction)RolesWindow, "Assign Roles");
    }

    private void RolesWindow(int windowID)
    {
        GUILayout.BeginVertical();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.Data || !player.Data.Role || string.IsNullOrEmpty(player.Data.PlayerName) || player != PlayerControl.LocalPlayer) continue;

            GUILayout.BeginHorizontal();

            GUILayout.Label($"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{player.name}</color>", GUILayout.Width(140f));
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{CheatToggles.forcedRole}");
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Reset", GUILayout.Width(80f)))
            {
                CheatToggles.forcedRole = null;
            }
            if (GUILayout.Button("Assign", GUILayout.Width(80f)))
            {
                CheatToggles.forceRole = true;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.Label("Roles will be assigned on next game start");
        GUI.DragWindow();
    }
}
