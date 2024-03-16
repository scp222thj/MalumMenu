using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MalumMenu;

public interface ITab
{
    string title { get; }
    bool isVisible();
    void drawContent();
}

public class TextTab : ITab
{
    private string textFieldContent = "";
    private bool isTextFieldFocused = false;
    private Rect textFieldRect;
    private float cursorBlinkTime = 0.5f; // Time in seconds for each blink state
    private float lastBlinkTime = 0.0f;
    private bool cursorVisible = true;

    public string title => "TextTest";

    public bool isVisible() => true;

    public void drawContent()
    {
        GUILayout.Label("Custom Text Field Below:");

        GUILayout.Box("", GUILayout.Width(200), GUILayout.Height(20));

        if (Event.current.type == EventType.Repaint)
        {
            textFieldRect = GUILayoutUtility.GetLastRect();
        }

        HandleTextInput();

        // Adjust GUI.Label to leave space for the cursor
        GUI.Label(new Rect(textFieldRect.x, textFieldRect.y, textFieldRect.width - 10, textFieldRect.height), textFieldContent);

        if (Event.current.type == EventType.MouseDown)
        {
            if (textFieldRect.Contains(Event.current.mousePosition))
            {
                isTextFieldFocused = true;
                lastBlinkTime = Time.time; // Reset blink timing on focus
                cursorVisible = true; // Ensure cursor is visible when field is focused
                Event.current.Use();
            }
            else
            {
                isTextFieldFocused = false;
            }
        }

        if (isTextFieldFocused)
        {
            ManageCursorBlinking();
        }
    }

    private void HandleTextInput()
    {
        if (isTextFieldFocused && Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Backspace)
            {
                if (textFieldContent.Length > 0)
                {
                    textFieldContent = textFieldContent.Substring(0, textFieldContent.Length - 1);
                    Event.current.Use();
                }
            }
            else if (Event.current.character != '\0' && !char.IsControl(Event.current.character))
            {
                textFieldContent += Event.current.character;
                Event.current.Use();
            }
        }
    }

    private void ManageCursorBlinking()
    {
        // Toggle cursor visibility based on blink time
        if (Time.time - lastBlinkTime > cursorBlinkTime)
        {
            cursorVisible = !cursorVisible;
            lastBlinkTime = Time.time;
        }

        // Draw cursor if visible
        if (cursorVisible)
        {
            Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(textFieldContent));
            GUI.Label(new Rect(textFieldRect.x + textSize.x + 2, textFieldRect.y + 2, 10, textFieldRect.height - 4), "|");
        }
    }
}


public class PlayerListTab : ITab
{
    public string title => "Players";
    public bool isVisible() => Utils.isPlayer;
    private List<string> selectedPlayerNames = new List<string>();
    private Dictionary<string, PlayerControl> allPlayers = new Dictionary<string, PlayerControl>();

    public void drawContent()
    {
        RefreshPlayerList();

        // Display checkboxes for player selection
        foreach (var player in allPlayers)
        {
            bool isSelected = selectedPlayerNames.Contains(player.Key);
            bool newIsSelected = GUILayout.Toggle(isSelected, $" {player.Key}");
            // Update the selection state based on the checkbox
            if (newIsSelected && !isSelected)
            {
                selectedPlayerNames.Add(player.Key);
            }
            else if (!newIsSelected && isSelected)
            {
                selectedPlayerNames.Remove(player.Key);
            }
        }
    }

    private void RefreshPlayerList()
    {
        // Get the current list of players and update the dictionary
        var currentPlayers = PlayerControl.AllPlayerControls.ToArray()
            .Where(pc => pc != null && pc.Data != null && pc.Data.DefaultOutfit != null)
            .ToList();

        allPlayers = currentPlayers.ToDictionary(pc => pc.Data.DefaultOutfit.PlayerName, pc => pc);

        // Remove any selected players that are no longer present
        selectedPlayerNames = selectedPlayerNames.Where(name => allPlayers.ContainsKey(name)).ToList();
    }

    private void KillSelectedPlayers()
    {
        foreach (var playerName in selectedPlayerNames)
        {
            if (allPlayers.TryGetValue(playerName, out PlayerControl playerControl))
            {
                Utils.murderPlayer(playerControl, MurderResultFlags.Succeeded);
            }
        }

        // Clear the selection after killing to prevent repeated actions on already killed players
        selectedPlayerNames.Clear();
    }
}