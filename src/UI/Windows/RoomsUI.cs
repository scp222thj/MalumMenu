using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

public class RoomsUI : MonoBehaviour
{
    public static int windowHeight = 270;
    public static int windowWidth = 480;
    private Rect _windowRect;

    private Vector2 _scrollPosition = Vector2.zero;

    private void Start()
    {
        _windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    private void OnGUI()
    {
        if (!CheatToggles.showRoomsMenu || !(MenuUI.isGUIActive || MalumMenu.menuKeepSubwindowsOpen.Value) || MalumMenu.isPanicked) return;

        UIHelpers.ApplyUIColor();

        _windowRect = GUI.Window((int)WindowId.RoomsUI, _windowRect, (GUI.WindowFunction)RoomsWindow, "Rooms");
    }

    private void RoomsWindow(int windowID)
    {
        if (!Utils.isShip)
        {
            GUI.DragWindow();
            return;
        }

        GUILayout.BeginVertical();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.ExpandHeight(true));

        foreach (var room in RoomsHandler.GetValidRooms())
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{room.ToString()}", GUILayout.Width(110f));

            GUILayout.BeginHorizontal();

            GUILayout.Label("<color=#FFFF00>Available</color>");

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Teleport", GUIStylePreset.NormalButton, GUILayout.Width(80f)))
            {
                RoomsHandler.TeleportToRoom(room);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        GUILayout.Box("", GUIStylePreset.Separator, GUILayout.Height(1f), GUILayout.ExpandWidth(true));
        GUILayout.Space(1f);

        GUILayout.BeginHorizontal();

        var currentMap = (MapNames)Utils.GetCurrentMapID();
        GUILayout.Label($"Current Map: {currentMap.ToString()}", GUIStylePreset.NormalToggle);

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUI.DragWindow();
    }
}
