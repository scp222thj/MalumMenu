using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

public static class EventLogger
{
    internal static List<GameEvent> Events { get; private set; } = new();
    internal static int MaxEvents { get; set; } = 100;
    public static bool IsEnabled { get; set; } = false;
    public static bool ShowKills { get; set; } = true;
    public static bool ShowTasks { get; set; } = true;
    public static bool ShowVents { get; set; } = true;
    public static bool ShowSabotages { get; set; } = true;
    public static bool ShowReports { get; set; } = true;
    public static bool ShowVotes { get; set; } = true;
    public static bool ShowUI { get; set; } = false;

    public static void Log(GameEventType type, string message, string playerName = "", string roleName = "", string location = "")
    {
        if (!IsEnabled) return;

        var evt = new GameEvent(type, message, playerName, roleName, location);
        Events.Add(evt);

        if (Events.Count > MaxEvents)
        {
            int excess = Events.Count - MaxEvents;
            Events.RemoveRange(0, excess);
        }

        Debug.Log($"[EventLogger] {evt.Type}: {evt.Message}");
    }

    public static void LogKill(PlayerControl killer, PlayerControl victim, string location = "")
    {
        if (!IsEnabled || !ShowKills) return;

        string killerName = killer?.Data?.PlayerName ?? "Unknown";
        string killerRole = killer?.Data?.Role?.Role.ToString() ?? "Unknown";
        string victimName = victim?.Data?.PlayerName ?? "Unknown";

        if (string.IsNullOrEmpty(location))
        {
            try
            {
                var roomTracker = DestroyableSingleton<HudManager>.Instance?.roomTracker;
                if (roomTracker?.LastRoom != null)
                    location = roomTracker.LastRoom.RoomId.ToString();
            }
            catch { }
        }

        Log(GameEventType.Kill, $"{killerName} ({killerRole}) killed {victimName}", killerName, killerRole, location);
    }

    public static void LogTask(PlayerControl player, string taskName)
    {
        if (!IsEnabled || !ShowTasks) return;

        string playerName = player?.Data?.PlayerName ?? "Unknown";
        string roleName = player?.Data?.Role?.Role.ToString() ?? "Unknown";

        Log(GameEventType.Task, $"{playerName} completed {taskName}", playerName, roleName);
    }

    public static void LogVent(PlayerControl player, int ventId, bool entering)
    {
        if (!IsEnabled || !ShowVents) return;

        string playerName = player?.Data?.PlayerName ?? "Unknown";
        string roleName = player?.Data?.Role?.Role.ToString() ?? "Unknown";
        string action = entering ? "entered" : "exited";

        Log(GameEventType.Vent, $"{playerName} ({roleName}) {action} vent {ventId}", playerName, roleName);
    }

    public static void LogSabotage(SystemTypes system, PlayerControl player = null)
    {
        if (!IsEnabled || !ShowSabotages) return;

        string playerName = player?.Data?.PlayerName ?? "Someone";
        Log(GameEventType.Sabotage, $"{playerName} sabotaged {system}", playerName);
    }

    public static void LogReport(PlayerControl reporter, PlayerControl body)
    {
        if (!IsEnabled || !ShowReports) return;

        string reporterName = reporter?.Data?.PlayerName ?? "Unknown";
        string bodyName = body?.Data?.PlayerName ?? "Unknown";

        Log(GameEventType.Report, $"{reporterName} reported {bodyName}'s body", reporterName);
    }

    public static void LogVote(PlayerControl voter, PlayerControl target)
    {
        if (!IsEnabled || !ShowVotes) return;

        string voterName = voter?.Data?.PlayerName ?? "Unknown";
        string targetName = target?.Data?.PlayerName ?? "Skip";

        Log(GameEventType.Vote, $"{voterName} voted for {targetName}", voterName);
    }

    public static void Clear() => Events.Clear();

    public static List<GameEvent> GetFilteredEvents()
    {
        _filteredCache.Clear();

        foreach (var evt in Events)
        {
            bool show = evt.Type switch
            {
                GameEventType.Kill => ShowKills,
                GameEventType.Task => ShowTasks,
                GameEventType.Vent => ShowVents,
                GameEventType.Sabotage => ShowSabotages,
                GameEventType.Report => ShowReports,
                GameEventType.Vote => ShowVotes,
                _ => true,
            };

            if (show) _filteredCache.Add(evt);
        }

        return _filteredCache;
    }

    private static readonly List<GameEvent> _filteredCache = new();
}
