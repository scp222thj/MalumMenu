using UnityEngine;

namespace MalumMenu;

public class SabotageUI : MonoBehaviour
{
    public static int windowHeight = 300;
    public static int windowWidth = 500;
    public static Rect windowRect;

    private bool _keepEverythingSabotaged;

    private bool _keepReactor;
    private bool _keepOxygen;
    private bool _keepComms;
    private bool _keepElec;
    private bool _keepMush;

    // Local UI states to prevent flickering
    private bool _activeReactor;
    private bool _activeOxygen;
    private bool _activeComms;
    private bool _activeElec;
    private bool _activeMush;

    private float _mushTimer = 0f;
    private float _uiLockTimer = 0f; // Prevents UI from syncing with game state for 1 second after clicking
    private float _keepCooldown = 0f; // Prevents RPC spam when Keep mode re-activates a sabotage

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
        // Decrease timers
        if (_uiLockTimer > 0f) _uiLockTimer -= Time.deltaTime;
        if (_keepCooldown > 0f) _keepCooldown -= Time.deltaTime;

        bool isInGame = ShipStatus.Instance != null && !Utils.isLobby;
        byte mapId = isInGame ? Utils.GetCurrentMapID() : (byte)255;

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

        // Keep logic: only force true if the game reports it as false AND cooldown is 0
        // This prevents sending 60 RPCs per second while the server processes the sabotage
        if (_keepCooldown <= 0f)
        {
            bool triggered = false;
            if (_keepReactor && !CheatToggles.reactorSab) { CheatToggles.reactorSab = true; triggered = true; }
            if (_keepOxygen && !CheatToggles.oxygenSab) { CheatToggles.oxygenSab = true; triggered = true; }
            if (_keepComms && !CheatToggles.commsSab) { CheatToggles.commsSab = true; triggered = true; }
            if (_keepElec && !CheatToggles.elecSab) { CheatToggles.elecSab = true; triggered = true; }

            if (triggered) _keepCooldown = 1.0f;
        }

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
        bool isInGame = ShipStatus.Instance != null && !Utils.isLobby;
        if (!isInGame)
        {
            GUI.DragWindow();
            return;
        }

        GUILayout.BeginVertical();

        byte mapId = Utils.GetCurrentMapID();

        bool canReactor = true;
        bool canComms = true;
        bool canOxygen = (mapId != 4 && mapId != 2 && mapId != 5);
        bool canElec = (mapId != 5);
        bool canMush = (mapId == 5);

        DrawSabotageRow("Reactor", "reactorSab", ref _activeReactor, ref _keepReactor, canReactor);
        DrawSabotageRow("Oxygen", "oxygenSab", ref _activeOxygen, ref _keepOxygen, canOxygen);
        DrawSabotageRow("Lights", "elecSab", ref _activeElec, ref _keepElec, canElec);
        DrawSabotageRow("Comms", "commsSab", ref _activeComms, ref _keepComms, canComms);
        DrawSabotageRow("Mushroom Mixup", "mushSab", ref _activeMush, ref _keepMush, canMush);

        GUILayout.FlexibleSpace();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Sabotage Everything", GUIStylePreset.NormalButton))
        {
            _uiLockTimer = 1.0f; // Lock UI updates
            if (canReactor) { CheatToggles.reactorSab = true; _activeReactor = true; }
            if (canComms) { CheatToggles.commsSab = true; _activeComms = true; }
            if (canOxygen) { CheatToggles.oxygenSab = true; _activeOxygen = true; }
            if (canElec) { CheatToggles.elecSab = true; _activeElec = true; }
            if (canMush) { CheatToggles.mushSab = true; _activeMush = true; }
        }

        if (GUILayout.Button("Repair All", GUIStylePreset.NormalButton))
        {
            _uiLockTimer = 1.0f; // Lock UI updates
            _keepEverythingSabotaged = false;
            _keepReactor = false; _keepOxygen = false; _keepComms = false; _keepElec = false; _keepMush = false;

            CheatToggles.reactorSab = false; _activeReactor = false;
            CheatToggles.oxygenSab = false; _activeOxygen = false;
            CheatToggles.elecSab = false; _activeElec = false;
            CheatToggles.commsSab = false; _activeComms = false;
            CheatToggles.mushSab = false; _activeMush = false;
            CheatToggles.unfixableLights = false;
        }

        GUILayout.FlexibleSpace();

        bool newKeepAll = GUILayout.Toggle(_keepEverythingSabotaged, " Keep Everything Sabotaged", GUIStylePreset.NormalToggle);

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

    private void DrawSabotageRow(string displayName, string toggleName, ref bool activeState, ref bool keepState, bool isAvailable)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(displayName, GUILayout.Width(140f));

        if (!isAvailable)
        {
            GUI.enabled = false;
        }

        bool gameVal = GetSabToggleValue(toggleName);

        // Sync UI state with game state, but ignore sync if we recently clicked
        if (_uiLockTimer <= 0f)
        {
            activeState = keepState || gameVal;
        }

        // Draw Active toggle
        bool newActive = GUILayout.Toggle(activeState, "Active", GUIStylePreset.NormalToggle, GUILayout.Width(80f));
        if (newActive != activeState)
        {
            _uiLockTimer = 1.0f; // Lock UI updates for 1 second to prevent flicker
            activeState = newActive;
            SetSabToggleValue(toggleName, newActive);

            // If user unchecks Active, also uncheck Keep
            if (!newActive && keepState)
            {
                keepState = false;
            }
        }

        // Draw Keep toggle
        bool newKeep = GUILayout.Toggle(keepState, "Keep", GUIStylePreset.NormalToggle, GUILayout.Width(80f));
        if (newKeep != keepState)
        {
            keepState = newKeep;
            if (newKeep && !activeState) // If turning on Keep, activate sabotage immediately
            {
                _uiLockTimer = 1.0f;
                activeState = true;
                SetSabToggleValue(toggleName, true);
            }
        }

        GUI.enabled = true;

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