using UnityEngine;

namespace MalumMenu;

public class ForceTeleportUI : MonoBehaviour
{
    public static int windowHeight = 400;
    public static int windowWidth = 420;
    private Rect _windowRect;

    private PlayerControl _selectedTarget;
    private string _selectedDestName = "";
    private Vector2 _selectedDest;
    private Vector2 _playerScrollPos;
    private Vector2 _destScrollPos;
    private bool _wasInGame;

    private void Update()
    {
        // Clear stale selection when the game ends — PlayerId values (0-15) are
        // reused each round, so a held reference would silently target the wrong player.
        if (_wasInGame && !Utils.isInGame)
        {
            _selectedTarget = null;
            _selectedDestName = "";
        }
        _wasInGame = Utils.isInGame;
    }

    private void Start()
    {
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f + 220f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showForceTeleportMenu || !(MenuUI.isGUIActive || MalumMenu.menuKeepSubwindowsOpen.Value) || MalumMenu.isPanicked || CheatToggles.streamerMode) return;

        UIHelpers.ApplyUIColor();

        _windowRect = GUI.Window((int)WindowId.ForceTeleportUI, _windowRect, (GUI.WindowFunction)ForceTeleportWindow, "Force Teleport");
    }

    private void ForceTeleportWindow(int windowID)
    {
        if (!Utils.isHost || !Utils.isInGame)
        {
            GUILayout.Label("Host-only — must be in a game.", GUIStylePreset.TabSubtitle);
            GUI.DragWindow();
            return;
        }

        GUILayout.BeginVertical();

        // --- Target player ---
        GUILayout.Label("Target Player", GUIStylePreset.TabSubtitle);

        _playerScrollPos = GUILayout.BeginScrollView(_playerScrollPos, false, true, GUILayout.Height(120f));
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == null || player.Data == null) continue;
            var playerName = player.Data.PlayerName;
            var isSelected = _selectedTarget != null && _selectedTarget.PlayerId == player.PlayerId;
            if (GUILayout.Button(isSelected ? $"> {playerName} <" : playerName, GUIStylePreset.NormalButton))
            {
                _selectedTarget = player;
            }
        }
        GUILayout.EndScrollView();

        GUILayout.Space(6f);

        // --- Destination ---
        GUILayout.Label("Destination", GUIStylePreset.TabSubtitle);

        _destScrollPos = GUILayout.BeginScrollView(_destScrollPos, false, true, GUILayout.Height(150f));

        if (GUILayout.Button(_selectedDestName == "My Location" ? "> My Location <" : "My Location", GUIStylePreset.NormalButton))
        {
            _selectedDestName = "My Location";
            _selectedDest = PlayerControl.LocalPlayer.GetTruePosition();
        }

        foreach (var (roomName, pos) in ForceTeleportHandler.GetRoomsForCurrentMap())
        {
            if (GUILayout.Button(_selectedDestName == roomName ? $"> {roomName} <" : roomName, GUIStylePreset.NormalButton))
            {
                _selectedDestName = roomName;
                _selectedDest = pos;
            }
        }

        GUILayout.EndScrollView();

        GUILayout.Space(6f);
        GUILayout.Box("", GUIStylePreset.Separator, GUILayout.Height(1f), GUILayout.ExpandWidth(true));
        GUILayout.Space(2f);

        var targetLabel = _selectedTarget?.Data?.PlayerName ?? "None";
        var destLabel = _selectedDestName.Length > 0 ? _selectedDestName : "None";
        GUILayout.Label($"Target: {targetLabel}   Dest: {destLabel}");

        GUI.enabled = _selectedTarget != null && _selectedDestName.Length > 0;
        if (GUILayout.Button("Teleport", GUIStylePreset.NormalButton))
        {
            // Refresh "My Location" at the moment of click
            if (_selectedDestName == "My Location")
                _selectedDest = PlayerControl.LocalPlayer.GetTruePosition();

            ForceTeleportHandler.TeleportPlayer(_selectedTarget, _selectedDest);
        }
        GUI.enabled = true;

        GUILayout.EndVertical();

        GUI.DragWindow();
    }
}
