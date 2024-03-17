using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TextInput
{
    public string Text { get; set; } = "";
    private static TextInput focusedTextInput = null;
    private Rect textFieldRect;
    private float cursorBlinkTime = 0.5f;
    private float lastBlinkTime = 0.0f;
    private bool cursorVisible = true;

    public void draw()
    {
        GUILayout.Box("");
        
        if (Event.current.type == EventType.Repaint)
        {
            textFieldRect = GUILayoutUtility.GetLastRect();
        }

        handleTextInput();

        GUI.Label(new Rect(textFieldRect.x, textFieldRect.y, textFieldRect.width - 10, textFieldRect.height), Text + (cursorVisible && this == focusedTextInput ? "|" : ""));

        if (Input.GetMouseButtonDown(0))
        {
            if (textFieldRect.Contains(Event.current.mousePosition))
            {
                focusedTextInput = this;
                lastBlinkTime = Time.time;
                cursorVisible = true;
            }
            else if (focusedTextInput == this)
            {
                focusedTextInput = null;
            }
        }

        if (this == focusedTextInput)
        {
            manageCursor();
        }
    }

    private void handleTextInput()
    {
        if (this == focusedTextInput && Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Backspace)
            {
                if (Text.Length > 0)
                {
                    Text = Text.Substring(0, Text.Length - 1);
                    Event.current.Use();
                }
            }
            else if (Event.current.character != '\0' && !char.IsControl(Event.current.character))
            {
                Text += Event.current.character;
                Event.current.Use();
            }
        }
    }

    private void manageCursor()
    {
        if (Time.time - lastBlinkTime > cursorBlinkTime)
        {
            cursorVisible = !cursorVisible;
            lastBlinkTime = Time.time;
        }
    }
}

public class PlayerListInput
{
    public List<string> selectedPlayerNames = new List<string>();
    private Dictionary<string, PlayerControl> allPlayers = new Dictionary<string, PlayerControl>();

    public void draw()
    {
        refreshPlayerList();

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

    private void refreshPlayerList()
    {
        // Get the current list of players and update the dictionary
        var currentPlayers = PlayerControl.AllPlayerControls.ToArray()
            .Where(pc => pc != null && pc.Data != null && pc.Data.DefaultOutfit != null)
            .ToList();

        allPlayers = currentPlayers.ToDictionary(pc => pc.Data.DefaultOutfit.PlayerName, pc => pc);

        // Remove any selected players that are no longer present
        selectedPlayerNames = selectedPlayerNames.Where(name => allPlayers.ContainsKey(name)).ToList();
    }

    public List<PlayerControl> getSelectedPlayers()
    {
        return selectedPlayerNames
            .Where(allPlayers.ContainsKey)
            .Select(playerName => allPlayers[playerName])
            .ToList();
    }
}