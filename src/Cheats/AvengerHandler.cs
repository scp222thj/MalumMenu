using UnityEngine;

namespace MalumMenu;

public static class AvengerHandler
{
    private static PlayerControl _killer;
    private static NetworkedPlayerInfo _victimData;
    private static string _victimName;
    private static string _killerName;
    private static string _roomName;
    private static float _killTime;
    private static string _postKillAction;
    private static bool _active;
    private static bool _reported;

    private const float DisplayTime = 8f;
    private const float TrackWindow = 5f;

    private static GUIStyle _boxStyle;
    private static GUIStyle _labelStyle;
    private static GUIStyle _titleStyle;

    public static void OnKill(PlayerControl victim, PlayerControl killer, Vector2 bodyPos, string roomName)
    {
        if (!CheatToggles.avengerMode) return;
        if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data.IsDead) return;
        if (killer == null || victim == null) return;

        _killer       = killer;
        _victimData   = victim.Data;
        _victimName   = victim.Data?.PlayerName ?? "?";
        _killerName   = killer.Data?.PlayerName ?? "?";
        _roomName     = roomName;
        _killTime     = Time.time;
        _postKillAction = "Walking away...";
        _active       = true;
        _reported     = false;

        // Teleport to body
        try { PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(bodyPos); } catch { }

        // Add killer to overload and start it
        if (!killer.AmOwner && killer.Data != null)
        {
            OverloadHandler.AddCustomTarget(killer.Data);
            CheatToggles.runOverload = true;
        }
    }

    public static void OnVent(PlayerControl pc)
    {
        if (!_active || _killer == null || pc == null) return;
        if (pc.PlayerId != _killer.PlayerId) return;
        if (Time.time - _killTime > TrackWindow) return;
        _postKillAction = "Vented";
    }

    public static void OnShapeshift(PlayerControl pc)
    {
        if (!_active || _killer == null || pc == null) return;
        if (pc.PlayerId != _killer.PlayerId) return;
        if (Time.time - _killTime > TrackWindow) return;
        if (_postKillAction == "Vented") return;
        _postKillAction = "Shapeshifted";
    }

    public static void Update()
    {
        if (!_active) return;

        float elapsed = Time.time - _killTime;

        // Detect phantom vanish by polling
        if (_killer != null && _killer.Data != null && elapsed <= TrackWindow)
        {
            if (_postKillAction == "Walking away..." && Utils.IsVanished(_killer.Data))
                _postKillAction = "Vanished (Phantom)";
        }

        // Auto-report on first update tick (we're now at the body position)
        if (!_reported && _victimData != null && PlayerControl.LocalPlayer != null && !Utils.isMeeting)
        {
            try { PlayerControl.LocalPlayer.CmdReportDeadBody(_victimData); } catch { }
            _reported = true;
        }

        if (elapsed > DisplayTime)
        {
            _active = false;
            _killer = null;
            _victimData = null;
        }
    }

    public static void DrawAlert()
    {
        if (!_active) return;

        float remaining = DisplayTime - (Time.time - _killTime);

        float w = 300f, h = 115f;
        float x = (Screen.width - w) / 2f;
        float y = 40f;

        _boxStyle   ??= new GUIStyle(GUI.skin.box)   { fontSize = 13 };
        _labelStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 13, richText = true };
        _titleStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold, richText = true, alignment = TextAnchor.MiddleCenter };

        GUI.Box(new Rect(x - 2, y - 2, w + 4, h + 4), "", _boxStyle);

        GUILayout.BeginArea(new Rect(x, y, w, h));
        GUILayout.Label("<color=red>[!] KILL DETECTED</color>", _titleStyle);
        GUILayout.Label($"Victim: <b>{_victimName}</b>", _labelStyle);
        GUILayout.Label($"Killer: <b>{_killerName}</b>  |  Room: <b>{_roomName}</b>", _labelStyle);
        GUILayout.Label($"After kill: <b>{_postKillAction}</b>", _labelStyle);
        GUILayout.Label($"<size=10>Overload started on killer · closes in {remaining:F0}s</size>", _labelStyle);
        GUILayout.EndArea();
    }
}
