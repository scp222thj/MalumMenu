using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

public static class BodyIntelHandler
{
    private const float NearRadius       = 3.5f;
    private const float AutoReportRadius = 1.8f;

    public struct BodyRecord
    {
        public string       VictimName;
        public string       KillerName;
        public string       RoomName;
        public Vector2      Position;
        public List<string> NearbyAtDeath;
        public List<string> PassedBy;
    }

    public static readonly List<BodyRecord> Records = new();

    // Set<playerName> per record index, tracks who has already been logged as passing
    private static readonly Dictionary<int, HashSet<string>> _loggedPass = new();

    public static void OnMurder(PlayerControl victim, PlayerControl killer)
    {
        if (victim == null || victim.Data == null) return;
        if (!CheatToggles.bodyIntelLogger && !CheatToggles.autoReport) return;

        var room     = Utils.GetRoomFromPosition(victim.GetTruePosition());
        var roomName = room != null ? room.RoomId.ToString() : "Unknown";
        var pos      = victim.GetTruePosition();

        var victimRoom = Utils.GetRoomFromPosition(pos);

        var nearby = new List<string>();
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p == null || p == victim || p == killer || p.Data == null || p.Data.IsDead) continue;
            if (Vector2.Distance(p.GetTruePosition(), pos) > NearRadius) continue;
            // Only count players confirmed in the same room — prevents adjacent-room false positives
            if (victimRoom != null)
            {
                var pRoom = Utils.GetRoomFromPosition(p.GetTruePosition());
                if (pRoom == null || pRoom.RoomId != victimRoom.RoomId) continue;
            }
            nearby.Add(p.Data.PlayerName);
        }

        var record = new BodyRecord
        {
            VictimName    = victim.Data.PlayerName,
            KillerName    = killer?.Data?.PlayerName ?? "Unknown",
            RoomName      = roomName,
            Position      = pos,
            NearbyAtDeath = nearby,
            PassedBy      = new(nearby) // seed with anyone nearby at time of death
        };

        Records.Add(record);
        _loggedPass[Records.Count - 1] = new HashSet<string>(nearby);

        if (CheatToggles.bodyIntelLogger)
        {
            ConsoleUI.Log($"[BodyIntel] {record.VictimName} killed by {record.KillerName} in {roomName}");
            if (nearby.Count > 0)
                ConsoleUI.Log($"[BodyIntel] Nearby at death: {string.Join(", ", nearby)}");
        }
    }

    public static void Update()
    {
        if (!Utils.isShip || !Utils.isInGame) return;

        var bodies = GameObject.FindGameObjectsWithTag("DeadBody");

        // Track passers-by
        if (CheatToggles.bodyIntelLogger)
        {
            foreach (var bodyObj in bodies)
            {
                var dead = bodyObj.GetComponent<DeadBody>();
                if (dead == null || dead.Reported) continue;

                int idx = FindRecord((Vector2)bodyObj.transform.position);
                if (idx < 0) continue;

                if (!_loggedPass.TryGetValue(idx, out var already)) { already = new(); _loggedPass[idx] = already; }

                var bodyPos  = (Vector2)bodyObj.transform.position;
                var bodyRoom = Utils.GetRoomFromPosition(bodyPos);

                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p == null || p.Data == null || p.Data.IsDead) continue;
                    if (Vector2.Distance(p.GetTruePosition(), bodyPos) > NearRadius) continue;
                    // Reject players in a different room — prevents door/wall false positives
                    if (bodyRoom != null)
                    {
                        var pRoom = Utils.GetRoomFromPosition(p.GetTruePosition());
                        if (pRoom == null || pRoom.RoomId != bodyRoom.RoomId) continue;
                    }
                    string name = p.Data.PlayerName;
                    if (already.Add(name))
                    {
                        Records[idx].PassedBy.Add(name);
                        ConsoleUI.Log($"[BodyIntel] {name} passed body of {Records[idx].VictimName} in {Records[idx].RoomName}");
                    }
                }
            }
        }

        // Auto Report
        if (CheatToggles.autoReport && PlayerControl.LocalPlayer != null
            && !PlayerControl.LocalPlayer.Data.IsDead && !Utils.isMeeting)
        {
            foreach (var bodyObj in bodies)
            {
                var dead = bodyObj.GetComponent<DeadBody>();
                if (dead == null || dead.Reported) continue;
                if (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), (Vector2)bodyObj.transform.position) <= AutoReportRadius)
                {
                    try { PlayerControl.LocalPlayer.CmdReportDeadBody(GameData.Instance.GetPlayerById(dead.ParentId)); } catch { }
                    break;
                }
            }
        }
    }

    private static int FindRecord(Vector2 bodyPos)
    {
        for (int i = Records.Count - 1; i >= 0; i--)
            if (Vector2.Distance(Records[i].Position, bodyPos) < 1f) return i;
        return -1;
    }

    public static void Clear()
    {
        Records.Clear();
        _loggedPass.Clear();
    }
}
