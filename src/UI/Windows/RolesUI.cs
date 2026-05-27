using UnityEngine;
using AmongUs.GameOptions;

namespace MalumMenu;

public class RolesUI : MonoBehaviour
{
    public static int windowHeight = 380;
    public static int windowWidth = 560;
    private Rect _windowRect;

    private Vector2 _scrollPosition = Vector2.zero;

    // All assignable roles in display order
    private static readonly RoleTypes[] AllRoles = new[]
    {
        RoleTypes.Crewmate,
        RoleTypes.Engineer,
        RoleTypes.Scientist,
        RoleTypes.Tracker,
        RoleTypes.Noisemaker,
        RoleTypes.Detective,
        RoleTypes.Impostor,
        RoleTypes.Shapeshifter,
        RoleTypes.Phantom,
        RoleTypes.Viper
    };

    private void Start()
    {
        // Instantiate 2D area of RolesUI
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showRolesMenu || !(MenuUI.isGUIActive || MalumMenu.menuKeepSubwindowsOpen.Value) || MalumMenu.isPanicked) return;

        UIHelpers.ApplyUIColor();

        _windowRect = GUI.Window((int)WindowId.RolesUI, _windowRect, (GUI.WindowFunction)RolesWindow, "Assign Roles");
    }

    private void RolesWindow(int windowID)
    {
        GUILayout.BeginVertical();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.Data || !player.Data.Role || string.IsNullOrEmpty(player.Data.PlayerName)) continue;

            byte playerId = player.PlayerId;

            GUILayout.BeginHorizontal();

            // Player name colored by their Among Us color
            GUILayout.Label(
                $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{player.Data.PlayerName}</color>",
                GUILayout.Width(140f)
            );

            // ◄ button to cycle role backward
            if (GUILayout.Button("◄", GUILayout.Width(30f)))
            {
                CycleRole(playerId, -1);
            }

            // Display current role assignment (or "None") with team color
            string roleLabel;
            if (CheatToggles.forcedRoles.TryGetValue(playerId, out var assignedRole))
            {
                string roleColor = IsImpostorTeam(assignedRole) ? "FF1919" : "8CFFFF";
                roleLabel = $"<color=#{roleColor}>{GetRoleDisplayName(assignedRole)}</color>";
            }
            else
            {
                roleLabel = "<color=#888888>None</color>";
            }

            GUILayout.Label(roleLabel, new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                richText = true
            }, GUILayout.Width(130f));

            // ► button to cycle role forward
            if (GUILayout.Button("►", GUILayout.Width(30f)))
            {
                CycleRole(playerId, 1);
            }

            // Reset button for this player
            if (GUILayout.Button("Reset", GUIStylePreset.NormalButton, GUILayout.Width(60f)))
            {
                CheatToggles.forcedRoles.Remove(playerId);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        // Separator
        GUILayout.Box("", GUIStylePreset.DarkSeparator, GUILayout.Height(1f), GUILayout.ExpandWidth(true));

        // Bottom controls row
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset All", GUILayout.Width(90f)))
        {
            CheatToggles.forcedRoles.Clear();
        }

        GUILayout.FlexibleSpace();

        // Status summary
        int totalAssigned = CheatToggles.forcedRoles.Count;
        int impCount = 0;
        int crewCount = 0;

        foreach (var kvp in CheatToggles.forcedRoles)
        {
            if (IsImpostorTeam(kvp.Value))
                impCount++;
            else
                crewCount++;
        }

        int totalPlayers = 0;
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.Data != null && !string.IsNullOrEmpty(player.Data.PlayerName))
                totalPlayers++;
        }

        GUILayout.Label(
            $"Assigned: {totalAssigned}/{totalPlayers}  |  <color=#FF1919>Imps: {impCount}</color>  |  <color=#8CFFFF>Crew: {crewCount}</color>",
            new GUIStyle(GUI.skin.label) { richText = true, fontSize = 13 }
        );

        GUILayout.EndHorizontal();

        GUILayout.Label("Roles will be assigned on next game start", new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Italic,
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter
        });

        GUILayout.EndVertical();

        GUI.DragWindow();
    }

    /// <summary>
    /// Cycles the assigned role for a player by the given direction (+1 forward, -1 backward).
    /// If the player has no assignment, cycling forward starts at the first role,
    /// cycling backward starts at the last role.
    /// Cycling past the ends wraps to "None" (unassigned).
    /// </summary>
    private static void CycleRole(byte playerId, int direction)
    {
        if (CheatToggles.forcedRoles.TryGetValue(playerId, out var currentRole))
        {
            int currentIndex = System.Array.IndexOf(AllRoles, currentRole);

            if (currentIndex == -1)
            {
                // Unknown role in the dictionary — reset to first
                CheatToggles.forcedRoles[playerId] = AllRoles[0];
                return;
            }

            int newIndex = currentIndex + direction;

            if (newIndex < 0 || newIndex >= AllRoles.Length)
            {
                // Wrap to "None"
                CheatToggles.forcedRoles.Remove(playerId);
            }
            else
            {
                CheatToggles.forcedRoles[playerId] = AllRoles[newIndex];
            }
        }
        else
        {
            // Currently "None" — enter the list
            if (direction > 0)
            {
                CheatToggles.forcedRoles[playerId] = AllRoles[0];
            }
            else
            {
                CheatToggles.forcedRoles[playerId] = AllRoles[AllRoles.Length - 1];
            }
        }
    }

    /// <summary>
    /// Returns a human-readable name for the role type.
    /// </summary>
    private static string GetRoleDisplayName(RoleTypes role)
    {
        return role switch
        {
            RoleTypes.Crewmate => "Crewmate",
            RoleTypes.Engineer => "Engineer",
            RoleTypes.Scientist => "Scientist",
            RoleTypes.Tracker => "Tracker",
            RoleTypes.Noisemaker => "Noisemaker",
            RoleTypes.Detective => "Detective",
            RoleTypes.Impostor => "Impostor",
            RoleTypes.Shapeshifter => "Shapeshifter",
            RoleTypes.Phantom => "Phantom",
            RoleTypes.Viper => "Viper",
            _ => role.ToString()
        };
    }

    /// <summary>
    /// Returns true if the role belongs to the impostor team.
    /// </summary>
    private static bool IsImpostorTeam(RoleTypes role)
    {
        return role is RoleTypes.Impostor or RoleTypes.Shapeshifter or RoleTypes.Phantom or RoleTypes.Viper;
    }
}
