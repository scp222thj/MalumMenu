using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

public interface ITab
{
    string title { get; }
    bool isVisible();
    void drawContent();
}

public class TextTab : ITab
{
    private TextInput userInput1 = new TextInput();
    private TextInput userInput2 = new TextInput();

    public string title => "TextTest";

    public bool isVisible() => true;

    public void drawContent()
    {
        GUILayout.Label("Text Field 1");
        userInput1.draw();
        GUILayout.Label("Text Field 2");
        userInput2.draw();
    }
}



public class PlayerListTab : ITab
{
    public string title => "Players";
    public bool isVisible() => Utils.isPlayer;
    private PlayerListInput playerInput = new PlayerListInput();

    public void drawContent()
    {
        playerInput.draw();
        List<PlayerControl> selectedPlayers = playerInput.getSelectedPlayers();
    }
}