using System;
using UnityEngine;

namespace MalumMenu;

public class GameEvent
{
    public DateTime Time { get; set; }
    public GameEventType Type { get; set; }
    public string Message { get; set; }
    public string PlayerName { get; set; }
    public string RoleName { get; set; }
    public string Location { get; set; }
    public Color Color { get; set; }

    public GameEvent(GameEventType type, string message, string playerName = "", string roleName = "", string location = "")
    {
        Time = DateTime.Now;
        Type = type;
        Message = message;
        PlayerName = playerName;
        RoleName = roleName;
        Location = location;
        Color = GetColorForType(type);
    }

    private static Color GetColorForType(GameEventType type) => type switch
    {
        GameEventType.Kill => new Color(1f, 0.2f, 0.2f),
        GameEventType.Task => new Color(0.2f, 1f, 0.2f),
        GameEventType.Vent => new Color(0.5f, 0.5f, 1f),
        GameEventType.Sabotage => new Color(1f, 0.5f, 0f),
        GameEventType.Report => new Color(1f, 1f, 0.2f),
        GameEventType.Meeting => new Color(0.8f, 0.8f, 0.8f),
        GameEventType.Vote => new Color(0.6f, 0.4f, 1f),
        GameEventType.Shapeshift => new Color(1f, 0.2f, 1f),
        _ => Color.white,
    };
}
