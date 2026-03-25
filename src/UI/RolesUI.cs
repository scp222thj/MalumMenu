using System.Linq;
using AmongUs.GameOptions;
using UnityEngine;

namespace MalumMenu;

public class RolesUI : MonoBehaviour
{
    private Vector2 _scrollPosition = Vector2.zero;
    private Rect _windowRect = new(320, 10, 500, 450);
    private int selectedRoleIdx = 0;
    private bool showRoleDropdown = false;

    private string GetRoleName(RoleTypes role)
    {
        if ((int)role == 18) return "Viper";
        return role.ToString();
    }

    private void OnGUI()
    {
        if (!CheatToggles.showRolesMenu || !MenuUI.isGUIActive || MalumMenu.isPanicked) return;

        UIHelpers.ApplyUIColor();
        _windowRect = GUI.Window(4, _windowRect, (GUI.WindowFunction)RolesWindow, "Assign Roles");
    }

    private void RolesWindow(int windowID)
    {
        GUILayout.BeginVertical();

        GUILayout.Box("Host Role Override");
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Selected Role:", GUILayout.Width(100f));
        
        if (GUILayout.Button(GetRoleName(ForceRole.SupportedRoles[selectedRoleIdx]), GUILayout.Width(150f)))
        {
            showRoleDropdown = !showRoleDropdown;
        }
        GUILayout.EndHorizontal();

        if (showRoleDropdown)
        {
            GUILayout.BeginVertical("box");
            for (int i = 0; i < ForceRole.SupportedRoles.Length; i++)
            {
                if (GUILayout.Button(GetRoleName(ForceRole.SupportedRoles[i])))
                {
                    selectedRoleIdx = i;
                    ForceRole.SetSelectedRoleForHost(ForceRole.SupportedRoles[i]);
                    showRoleDropdown = false;
                }
            }
            GUILayout.EndVertical();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply", GUILayout.Width(80f)))
        {
            ForceRole.SetSelectedRoleForHost(ForceRole.SupportedRoles[selectedRoleIdx]);
            ForceRole.ApplyHostRoleOnly();
            NotificationManager.Show($"Host role: {GetRoleName(ForceRole.SupportedRoles[selectedRoleIdx])}");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5f);
        
        if (GUILayout.Button("Force Apply Now (Mid-Game)", GUILayout.Width(220f)))
        {
            ForceRole.HostApplySelectedRoleNow();
        }

        GUILayout.Space(10f);
        GUILayout.Box("Player Role Assignment");

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Height(200f));

        if (PlayerControl.AllPlayerControls == null || PlayerControl.AllPlayerControls.Count == 0)
        {
            GUILayout.Label("No players in lobby");
        }
        else
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data || string.IsNullOrEmpty(player.Data.PlayerName)) continue;

                GUILayout.BeginHorizontal();
                
                string coloredName = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{player.Data.PlayerName}</color>";
                GUILayout.Label(coloredName, GUILayout.Width(150f));
                
                RoleTypes? assignedRole = ForceRole.GetAssignedRole(player.PlayerId);
                string roleText = assignedRole.HasValue ? GetRoleName(assignedRole.Value) : "Not Set";
                GUILayout.Label(roleText, GUILayout.Width(100f));
                
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button("Assign", GUILayout.Width(60f)))
                {
                    ForceRole.SetRoleForPlayer(player, ForceRole.SupportedRoles[selectedRoleIdx]);
                    NotificationManager.Show($"Assigned {GetRoleName(ForceRole.SupportedRoles[selectedRoleIdx])} to {player.Data.PlayerName}");
                }
                
                if (GUILayout.Button("Clear", GUILayout.Width(50f)))
                {
                    ForceRole.ClearRoleForPlayer(player.PlayerId);
                }
                
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.Space(5f);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Clear All", GUILayout.Width(100f)))
        {
            ForceRole.ClearAllAssignments();
        }
        GUILayout.EndHorizontal();

        GUI.DragWindow();
    }
}
