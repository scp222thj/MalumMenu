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
        // Check if we are in-game. If not, leave the window empty (like DoorsUI does)
        bool isInGame = ShipStatus.Instance != null && !Utils.isLobby;
        if (!isInGame)
        {
            GUI.DragWindow();
            return;
        }

        GUILayout.BeginVertical();

        // We already know we are in game, so we just get the map ID
        byte mapId = Utils.GetCurrentMapID();

        bool canReactor = true;
        bool canComms = true;
        bool canOxygen = (mapId != 4 && mapId != 2 && mapId != 5);
        bool canElec = (mapId != 5);
        bool canMush = (mapId == 5);

        DrawSabotageRow("Reactor", "reactorSab", ref _keepReactor, canReactor);
        DrawSabotageRow("Oxygen", "oxygenSab", ref _keepOxygen, canOxygen);
        DrawSabotageRow("Lights", "elecSab", ref _keepElec, canElec);
        DrawSabotageRow("Comms", "commsSab", ref _keepComms, canComms);
        DrawSabotageRow("Mushroom Mixup", "mushSab", ref _keepMush, canMush);

        GUILayout.FlexibleSpace(); // Pushes buttons down

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Sabotage Everything", GUIStylePreset.NormalButton))
        {
            if (canReactor) CheatToggles.reactorSab = true;
            if (canComms) CheatToggles.commsSab = true;
            if (canOxygen) CheatToggles.oxygenSab = true;
            if (canElec) CheatToggles.elecSab = true;
            if (canMush) CheatToggles.mushSab = true;
        }

        if (GUILayout.Button("Repair All", GUIStylePreset.NormalButton))
        {
            // Turn off all keep states
            _keepEverythingSabotaged = false;
            _keepReactor = false;
            _keepOxygen = false;
            _keepComms = false;
            _keepElec = false;
            _keepMush = false;

            // Turn off all sabotages
            CheatToggles.reactorSab = false;
            CheatToggles.oxygenSab = false;
            CheatToggles.elecSab = false;
            CheatToggles.commsSab = false;
            CheatToggles.mushSab = false;
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

    private void DrawSabotageRow(string displayName, string toggleName, ref bool keepState, bool isAvailable)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(displayName, GUILayout.Width(140f));

        // Disable UI interaction if sabotage is not available on this map
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

        GUI.enabled = true; // Re-enable UI

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