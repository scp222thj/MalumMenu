using UnityEngine;

namespace MalumMenu;

public class SabotageUI : MonoBehaviour
{
    public static int windowHeight = 300;
    public static int windowWidth = 500;
    public static Rect windowRect;

    private Vector2 _scrollPosition = Vector2.zero;
    private bool _keepEverythingSabotaged;

    private bool _keepReactor;
    private bool _keepOxygen;
    private bool _keepComms;
    private bool _keepElec;
    private bool _keepMush;

    private float _mushTimer = 0f;

    private void Start()
    {
        windowRect = new Rect(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void Update()
    {
        // Check if we are actually in a game, not in a lobby
        bool isInGame = ShipStatus.Instance != null && !Utils.isLobby;
        byte mapId = isInGame ? Utils.GetCurrentMapID() : (byte)255;

        // Determine availability based on map ID and game state
        bool canReactor = isInGame;
        bool canComms = isInGame;
        bool canOxygen = isInGame && (mapId != 4 && mapId != 2 && mapId != 5);
        bool canElec = isInGame && (mapId != 5);
        bool canMush = isInGame && (mapId == 5);

        // Reset Keep states if the game ends or map changes
        if (!canReactor) _keepReactor = false;
        if (!canComms) _keepComms = false;
        if (!canOxygen) _keepOxygen = false;
        if (!canElec) _keepElec = false;
        if (!canMush) _keepMush = false;

        // Keep logic: force toggle true every frame
        if (_keepReactor) CheatToggles.reactorSab = true;
        if (_keepOxygen) CheatToggles.oxygenSab = true;
        if (_keepComms) CheatToggles.commsSab = true;
        if (_keepElec) CheatToggles.elecSab = true;

        // Use timer for mushrooms to avoid RPC spam
        if (_keepMush)
        {
            _mushTimer -= Time.deltaTime;
            if (_mushTimer <= 0f)
            {
                CheatToggles.mushSab = true;
                _mushTimer = 10f;
            }
        }
    }

    private void OnGUI()
    {
        if (!CheatToggles.showSabotageMenu || !(MenuUI.isGUIActive || MalumMenu.menuKeepSubwindowsOpen.Value) || MalumMenu.isPanicked) return;

        UIHelpers.ApplyUIColor();
        windowRect = GUI.Window(500, windowRect, (GUI.WindowFunction)SabotageWindow, "Sabotage Manager");
    }

    private void SabotageWindow(int windowID)
    {
        GUILayout.BeginVertical();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        // Check again if we are in-game to disable UI in lobby
        bool isInGame = ShipStatus.Instance != null && !Utils.isLobby;
        byte mapId = isInGame ? Utils.GetCurrentMapID() : (byte)255;

        bool canReactor = isInGame;
        bool canComms = isInGame;
        bool canOxygen = isInGame && (mapId != 4 && mapId != 2 && mapId != 5);
        bool canElec = isInGame && (mapId != 5);
        bool canMush = isInGame && (mapId == 5);

        DrawSabotageRow("Reactor", "reactorSab", ref _keepReactor, canReactor);
        DrawSabotageRow("Oxygen", "oxygenSab", ref _keepOxygen, canOxygen);
        DrawSabotageRow("Lights", "elecSab", ref _keepElec, canElec);
        DrawSabotageRow("Comms", "commsSab", ref _keepComms, canComms);
        DrawSabotageRow("Mushroom Mixup", "mushSab", ref _keepMush, canMush);

        GUILayout.EndScrollView();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        GUI.enabled = isInGame; // Disable "Sabotage Everything" button in lobby
        if (GUILayout.Button("Sabotage Everything", GUIStylePreset.NormalButton))
        {
            if (canReactor) CheatToggles.reactorSab = true;
            if (canComms) CheatToggles.commsSab = true;
            if (canOxygen) CheatToggles.oxygenSab = true;
            if (canElec) CheatToggles.elecSab = true;
            if (canMush) CheatToggles.mushSab = true;
        }
        GUI.enabled = true; // Restore UI state

        GUILayout.FlexibleSpace();

        GUI.enabled = isInGame; // Disable "Keep Everything" toggle in lobby
        bool newKeepAll = GUILayout.Toggle(_keepEverythingSabotaged, " Keep Everything Sabotaged", GUIStylePreset.NormalToggle);
        GUI.enabled = true;

        if (newKeepAll != _keepEverythingSabotaged)
        {
            _keepEverythingSabotaged = newKeepAll;

            _keepReactor = newKeepAll && canReactor;
            _keepOxygen = newKeepAll && canOxygen;
            _keepComms = newKeepAll && canComms;
            _keepElec = newKeepAll && canElec;
            _keepMush = newKeepAll && canMush;
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUI.DragWindow();
    }

    private void DrawSabotageRow(string displayName, string toggleName, ref bool keepState, bool isAvailable)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(displayName, GUILayout.Width(140f));

        // Disable UI interaction if sabotage is not available (e.g., in lobby or wrong map)
        if (!isAvailable)
        {
            GUI.enabled = false;
        }

        bool isActive = GetSabToggleValue(toggleName);

        bool newActive = GUILayout.Toggle(isActive, "Active", GUIStylePreset.NormalToggle, GUILayout.Width(80f));
        if (newActive != isActive)
        {
            SetSabToggleValue(toggleName, newActive);
        }

        bool newKeep = GUILayout.Toggle(keepState, "Keep", GUIStylePreset.NormalToggle, GUILayout.Width(80f));
        if (newKeep != keepState)
        {
            keepState = newKeep;
        }

        GUI.enabled = true; // Re-enable UI for subsequent elements

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private bool GetSabToggleValue(string fieldName)
    {
        if (CheatToggles.ToggleFields.TryGetValue(fieldName, out var field))
        {
            return (bool)field.GetValue(null);
        }
        return false;
    }

    private void SetSabToggleValue(string fieldName, bool value)
    {
        if (CheatToggles.ToggleFields.TryGetValue(fieldName, out var field))
        {
            field.SetValue(null, value);
        }
    }
}