using UnityEngine;

namespace MalumMenu;

public class ForceRoleUI : MonoBehaviour
{
    public static int windowHeight = 250;
    public static int windowWidth = 500;
    private Rect _windowRect;

    private void Start()
    {
        // Instantiate 2D area of ForceRoleUI
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showForceRoleMenu || !(MenuUI.isGUIActive || MalumMenu.menuKeepSubwindowsOpen.Value) || MalumMenu.isPanicked) return;

        UIHelpers.ApplyUIColor();

        _windowRect = GUI.Window((int)WindowId.ForceRoleUI, _windowRect, (GUI.WindowFunction)ForceRoleWindow, "Force Role");
    }

    private void ForceRoleWindow(int windowID)
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Select a role to force for yourself at game start:");

        GUILayout.Space(5);

        var roles = System.Enum.GetValues(typeof(CheatToggles.ForcedRole));
        var roleNames = new string[roles.Length];
        for (int i = 0; i < roles.Length; i++)
        {
            roleNames[i] = roles.GetValue(i).ToString();
        }

        int currentIndex = (int)CheatToggles.forcedRoleSelection;

        // Display roles using buttons in a 3-column grid layout
        int newIndex = currentIndex;
        GUILayout.BeginVertical();
        for (int i = 0; i < roleNames.Length; i += 3)
        {
            GUILayout.BeginHorizontal();
            
            for (int j = 0; j < 3 && i + j < roleNames.Length; j++)
            {
                int roleIndex = i + j;
                bool isSelected = roleIndex == currentIndex;
                
                // Use different style or color for selected button
                if (isSelected)
                {
                    GUI.backgroundColor = new Color(0.5f, 0.8f, 1f);
                }
                
                if (GUILayout.Button(roleNames[roleIndex], GUILayout.Width(130f)))
                {
                    newIndex = roleIndex;
                }
                
                GUI.backgroundColor = Color.white;
            }
            
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        CheatToggles.forcedRoleSelection = (CheatToggles.ForcedRole)newIndex;

        GUILayout.Space(10);

        CheatToggles.forceRole = GUILayout.Toggle(CheatToggles.forceRole, " Enable Force Role");

        if (CheatToggles.forceRole)
        {
            CheatToggles.forceRoleLegit = GUILayout.Toggle(CheatToggles.forceRoleLegit, " Legit Mode (don't force impossible role)");
        }

        GUILayout.EndVertical();

        GUI.DragWindow();
    }
}
