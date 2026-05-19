using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

public static class ForceTeleportHandler
{
    public static readonly Dictionary<MapNames, List<(string Name, Vector2 Position)>> MapRooms = new()
    {
        [MapNames.Skeld] = new()
        {
            ("Cafeteria",      new(0.7f,   4.2f)),
            ("Weapons",        new(8.7f,   3.1f)),
            ("O2",             new(7.2f,   1.0f)),
            ("Navigation",     new(17.2f, -4.8f)),
            ("Shields",        new(9.4f, -10.4f)),
            ("Communications", new(0.0f, -15.1f)),
            ("Storage",        new(0.4f, -16.2f)),
            ("Admin",          new(0.4f,  -7.1f)),
            ("Electrical",     new(-7.0f, -9.8f)),
            ("Lower Engine",   new(-15.0f,-13.6f)),
            ("Reactor",        new(-20.7f, -6.7f)),
            ("Upper Engine",   new(-15.3f, -2.4f)),
            ("Security",       new(-13.2f, -5.7f)),
            ("Medbay",         new(-7.0f,  -4.6f)),
        },
        [MapNames.MiraHQ] = new()
        {
            ("Cafeteria",      new(23.5f, 23.3f)),
            ("Balcony",        new(24.5f, 19.5f)),
            ("Admin",          new(21.5f, 19.0f)),
            ("Locker Room",    new(7.5f,   1.5f)),
            ("Launchpad",      new(2.5f,   2.0f)),
            ("Greenhouse",     new(17.5f, 22.5f)),
            ("Office",         new(15.5f, 24.0f)),
            ("Laboratory",     new(17.5f, 14.0f)),
            ("Medbay",         new(15.5f,  8.5f)),
            ("Storage",        new(7.5f,  11.5f)),
            ("Communications", new(13.0f, 12.5f)),
            ("Reactor",        new(2.5f,  11.5f)),
        },
        [MapNames.Polus] = new()
        {
            ("Office",         new(16.5f, -15.0f)),
            ("Headquarters",   new(9.5f,   -7.0f)),
            ("Admin",          new(20.5f, -24.5f)),
            ("Electrical",     new(7.0f,  -18.5f)),
            ("O2",             new(4.0f,   -7.5f)),
            ("Communications", new(12.0f, -18.5f)),
            ("Weapons",        new(14.0f, -11.0f)),
            ("Security",       new(1.5f,  -11.5f)),
            ("Medbay",         new(36.5f,  -7.0f)),
            ("Storage",        new(21.5f, -11.5f)),
            ("Laboratory",     new(31.0f,  -7.5f)),
            ("Specimen Room",  new(36.0f, -20.5f)),
            ("Drill",          new(27.0f, -18.0f)),
            ("Dropship",       new(9.0f,  -11.5f)),
            ("Boiler Room",    new(3.5f,  -22.0f)),
        },
        [MapNames.Airship] = new()
        {
            ("Brig",           new(22.0f,  8.0f)),
            ("Engine",         new(5.0f,   0.0f)),
            ("Kitchen",        new(27.0f, -3.0f)),
            ("Gap Room",       new(13.5f, -1.0f)),
            ("Cargo Bay",      new(15.0f, -7.0f)),
            ("Communications", new(14.5f,  3.0f)),
            ("Main Hall",      new(19.5f,  3.0f)),
            ("Meeting Room",   new(10.5f, 14.0f)),
            ("Lounge",         new(33.5f, 10.0f)),
            ("Records",        new(21.0f, 14.0f)),
            ("Showers",        new(28.0f, 14.0f)),
            ("Vault",          new(12.5f, 14.0f)),
            ("Cockpit",        new(-2.0f,  0.5f)),
            ("Medical",        new(-4.5f,  3.0f)),
            ("Armory",         new(10.0f,-11.0f)),
            ("Electrical",     new(13.0f,-13.0f)),
            ("Viewing Deck",   new(27.0f,-13.0f)),
            ("Security",       new(7.0f,  -7.0f)),
            ("Reactor",        new(1.5f,  -6.5f)),
        },
        [MapNames.Fungle] = new()
        {
            ("Cafeteria",      new(2.0f,   8.0f)),
            ("Kitchen",        new(-10.5f, 4.5f)),
            ("Upper Engine",   new(-16.5f, 8.5f)),
            ("Launch Pad",     new(-18.5f, 3.5f)),
            ("Comms",          new(-15.5f,-2.5f)),
            ("Storage",        new(-3.5f, -4.5f)),
            ("Mining Pit",     new(-7.0f,-12.5f)),
            ("Jungle",         new(4.5f, -12.0f)),
            ("Reactor",        new(-16.5f,-9.5f)),
            ("Lookout",        new(10.0f, -5.5f)),
            ("Lab",            new(17.0f, -7.5f)),
            ("Medbay",         new(12.0f,  3.5f)),
            ("The Highlands",  new(11.0f, 12.0f)),
        },
    };

    public static List<(string Name, Vector2 Position)> GetRoomsForCurrentMap()
    {
        var mapName = (MapNames)Utils.GetCurrentMapID();
        return MapRooms.TryGetValue(mapName, out var rooms) ? rooms : new();
    }

    public static void TeleportPlayer(PlayerControl target, Vector2 destination)
    {
        if (!Utils.isHost) return;
        if (target == null || target.Data == null) return;

        if (target.Data.Disconnected)
        {
            ConsoleUI.Log("[ForceTeleport] Cannot teleport a disconnected player");
            return;
        }
        if (target.Data.IsDead)
        {
            ConsoleUI.Log("[ForceTeleport] Cannot teleport a dead player");
            return;
        }
        if (target.inVent)
        {
            ConsoleUI.Log("[ForceTeleport] Cannot teleport a player inside a vent");
            return;
        }
        if (Utils.isMeeting)
        {
            ConsoleUI.Log("[ForceTeleport] Cannot teleport during a meeting");
            return;
        }

        try
        {
            target.NetTransform.RpcSnapTo(destination);
            ConsoleUI.Log($"[ForceTeleport] Teleported {target.Data.PlayerName} to ({destination.x:F1}, {destination.y:F1})");
        }
        catch (Exception ex)
        {
            ConsoleUI.Log($"[ForceTeleport] RPC failed: {ex.Message}");
        }
    }
}
