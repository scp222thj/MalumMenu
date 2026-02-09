using UnityEngine;

namespace MalumMenu;

public class RolesUI : MonoBehaviour
{
    private Vector2 _scrollPosition = Vector2.zero;
    private Rect _windowRect = new(320, 10, 450, 100);

    private void OnGUI()
    {
        if (!CheatToggles.showRolesMenu) return;

        if(ColorUtility.TryParseHtmlString(MalumMenu.menuHtmlColor.Value, out var configUIColor))
        {
            GUI.backgroundColor = configUIColor;
        }

        _windowRect = GUI.Window(4, _windowRect, (GUI.WindowFunction)RolesWindow, "Assign Roles");
    }

    private void RolesWindow(int windowID)
    {
        GUILayout.BeginVertical();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        foreach (var pc in PlayerControl.AllPlayerControls)
        {
            if (!pc.Data || !pc.Data.Role || string.IsNullOrEmpty(pc.Data.PlayerName) || pc != PlayerControl.LocalPlayer) continue;

            GUILayout.BeginHorizontal();

            GUILayout.Label($"<color=#{ColorUtility.ToHtmlStringRGB(pc.Data.Color)}>{pc.name}</color>", GUILayout.Width(140f));
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
        GUILayout.Label("Roles will be assigned on next game start.");
        GUI.DragWindow();
    }
}
