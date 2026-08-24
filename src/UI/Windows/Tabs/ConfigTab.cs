using UnityEngine;

namespace MalumMenu;

public class ConfigTab : ITab
{
    public string name => "Config";

    // Track the currently selected profile slot index
    private int selectedProfileIndex = 0;

    // Define the available profile slots
    private readonly string[] profileSlots = { "1", "2", "3" };

    public void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();

        GUILayout.EndVertical();
    }

    private void DrawGeneral()
    {
        CheatToggles.openConfig = GUILayout.Toggle(CheatToggles.openConfig, " Open Config");

        CheatToggles.reloadConfig = GUILayout.Toggle(CheatToggles.reloadConfig, " Reload Config");

        GUILayout.Space(5);

        CheatToggles.autoLoadConfig = GUILayout.Toggle(CheatToggles.autoLoadConfig, " Auto Load Config on Startup");

        if (CheatToggles.autoLoadConfig)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("<b>Auto Load Slot:</b>", GUILayout.Width(100));

            for (int i = 0; i < profileSlots.Length; i++)
            {
                // Puts brackets around the actively chosen auto load profile number
                string autoButtonText = (i == CheatToggles.autoLoadSlotIndex) ? $"[{profileSlots[i]}]" : profileSlots[i];

                if (GUILayout.Button(autoButtonText, GUILayout.Width(35)))
                {
                    CheatToggles.autoLoadSlotIndex = i;
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("<b>Profile Slot:</b>", GUILayout.Width(85));
        for (int i = 0; i < profileSlots.Length; i++)
        {
            // Puts brackets around the actively chosen profile number
            string buttonText = (i == selectedProfileIndex) ? $"[{profileSlots[i]}]" : profileSlots[i];

            if (GUILayout.Button(buttonText, GUILayout.Width(35)))
            {
                selectedProfileIndex = i;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Save to the selected profile slot
        CheatToggles.saveProfile = GUILayout.Toggle(CheatToggles.saveProfile, " Save to Profile");
        if (CheatToggles.saveProfile)
        {
            CheatToggles.saveProfile = false; // Immediately uncheck/reset the toggle
            CheatToggles.SaveTogglesToProfile(profileSlots[selectedProfileIndex]);
        }

        // Load from the selected profile slot
        CheatToggles.loadProfile = GUILayout.Toggle(CheatToggles.loadProfile, " Load from Profile");
        if (CheatToggles.loadProfile)
        {
            CheatToggles.loadProfile = false; // Immediately uncheck/reset the toggle
            CheatToggles.LoadTogglesFromProfile(profileSlots[selectedProfileIndex]);
        }
    }
}