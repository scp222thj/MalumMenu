using UnityEngine;
using AmongUs.GameOptions;

namespace MalumMenu;

public class RolesUI : MonoBehaviour
{
    private Vector2 _scrollPosition = Vector2.zero;
    private Rect _windowRect = new(320, 10, 450, 100);
    private int _selectedRoleIndex = 0;
    
    private readonly string[] _roleOptions = new string[]
    {
        "Shapeshifter",
        "Phantom",
        "Viper",
        "Impostor",
        "Tracker",
        "Noisemaker",
        "Engineer",
        "Scientist",
        "Detective",
        "Crewmate"
    };
    
    private readonly RoleTypes[] _roleTypes = new RoleTypes[]
    {
        RoleTypes.Shapeshifter,
        RoleTypes.Phantom,
        RoleTypes.Viper,
        RoleTypes.Impostor,
        RoleTypes.Tracker,
        RoleTypes.Noisemaker,
        RoleTypes.Engineer,
        RoleTypes.Scientist,
        RoleTypes.Detective,
        RoleTypes.Crewmate
    };

    private void OnGUI()
    {
        if (!CheatToggles.showRolesMenu || !MenuUI.isGUIActive || MalumMenu.isPanicked) return;

        UIHelpers.ApplyUIColor();

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

            GUILayout.Label($"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{player.Data.PlayerName}</color>", GUILayout.Width(140f));
            
            GUILayout.BeginVertical();
            
            // Role selection dropdown
            GUILayout.BeginHorizontal();
            GUILayout.Label("Role:", GUILayout.Width(40f));
            _selectedRoleIndex = GUILayout.SelectionGrid(_selectedRoleIndex, _roleOptions, 2, GUILayout.Width(200f));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Reset", GUILayout.Width(80f)))
            {
                CheatToggles.forcedRole = null;
            }
            if (GUILayout.Button("Assign", GUILayout.Width(80f)))
            {
                CheatToggles.forcedRole = _roleTypes[_selectedRoleIndex];
                CheatToggles.forceRole = true;
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.Label("Roles will be assigned on next game start");
        GUI.DragWindow();
    }
}
