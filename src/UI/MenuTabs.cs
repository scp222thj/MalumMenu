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
    private TextField userInput = new TextField();

    public string title => "TextTest";

    public bool isVisible() => true;

    public void drawContent()
    {
        GUILayout.Label("Custom Text Field Below:");
        userInput.draw();
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