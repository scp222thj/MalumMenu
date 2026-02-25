using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

public class DoorsUI : MonoBehaviour
{
    private Rect _windowRect = new(320, 10, 480, 270);
    private List<SystemTypes> _doorsToSpamOpen = new();
    private List<SystemTypes> _doorsToSpamClose = new();

    private void OnGUI()
    {
        if (!CheatToggles.showDoorsMenu || !MenuUI.isGUIActive || MenuUI.isPanicked) return;

        if (ColorUtility.TryParseHtmlString(MalumMenu.menuHtmlColor.Value, out var configUIColor))
        {
            GUI.backgroundColor = configUIColor;
        }

        _windowRect = GUI.Window(2, _windowRect, (GUI.WindowFunction)DoorsWindow, "Doors");
    }

    private void DoorsWindow(int windowID)
    {
        if (!Utils.isShip)
        {
            GUI.DragWindow();
            return;
        }

        var map = (MapNames)Utils.GetCurrentMapID();

        if (map is MapNames.MiraHQ)
        {
            GUI.DragWindow();
            return;
        }

        GUILayout.BeginVertical();

        foreach (var doorRoom in DoorsHandler.GetRoomsWithDoors())
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{doorRoom.ToString()}", GUILayout.Width(110f));

            GUILayout.BeginHorizontal();

            GUILayout.Label($"{DoorsHandler.GetStatusOfDoorsInRoom(doorRoom, true)}");

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close", GUIStylePreset.NormalButton, GUILayout.Width(50f)))
            {
                DoorsHandler.CloseDoorsInRoom(doorRoom);
            }

            if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
            {
                if (GUILayout.Button("Open", GUIStylePreset.NormalButton, GUILayout.Width(50f)))
                {
                    DoorsHandler.OpenDoorsInRoom(doorRoom);
                }
            }

            if (Utils.isHost)
            {
                var spamClose = _doorsToSpamClose.Contains(doorRoom);
                spamClose = GUILayout.Toggle(spamClose, "Spam Close", GUIStylePreset.NormalToggle);

                if (spamClose && !_doorsToSpamClose.Contains(doorRoom))
                {
                    _doorsToSpamClose.Add(doorRoom);
                }
                else if (!spamClose && _doorsToSpamClose.Contains(doorRoom))
                {
                    _doorsToSpamClose.Remove(doorRoom);
                }

                if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
                {
                    var spamOpen = _doorsToSpamOpen.Contains(doorRoom);
                    spamOpen = GUILayout.Toggle(spamOpen, "Spam Open", GUIStylePreset.NormalToggle);

                    if (spamOpen && !_doorsToSpamOpen.Contains(doorRoom))
                    {
                        _doorsToSpamOpen.Add(doorRoom);
                    }
                    else if (!spamOpen && _doorsToSpamOpen.Contains(doorRoom))
                    {
                        _doorsToSpamOpen.Remove(doorRoom);
                    }
                }
            }
            else
            {
                // Clear spam lists if not host
                if (_doorsToSpamClose.Count != 0 || _doorsToSpamOpen.Count != 0)
                {
                    _doorsToSpamClose.Clear();
                    _doorsToSpamOpen.Clear();
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();

        GUILayout.Box("", GUIStylePreset.Separator, GUILayout.Height(1f), GUILayout.ExpandWidth(true));
        GUILayout.Space(1f);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Close All", GUIStylePreset.NormalButton))
        {
            CheatToggles.closeAllDoors = true;
        }

        if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
        {
            if (GUILayout.Button("Open All", GUIStylePreset.NormalButton))
            {
                CheatToggles.openAllDoors = true;
            }
        }

        GUILayout.FlexibleSpace();

        if (Utils.isHost)
        {
            CheatToggles.spamCloseAllDoors = GUILayout.Toggle(CheatToggles.spamCloseAllDoors, "Spam Close All", GUIStylePreset.NormalToggle);

            if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
            {
                CheatToggles.spamOpenAllDoors = GUILayout.Toggle(CheatToggles.spamOpenAllDoors, "Spam Open All", GUIStylePreset.NormalToggle);
            }
        }
        else
        {
            CheatToggles.spamCloseAllDoors = CheatToggles.spamOpenAllDoors = false;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUI.DragWindow();
    }

    public void Update()
    {
        if (!Utils.isShip) return;

        // Spam close selected doors
        foreach (var doorRoom in _doorsToSpamClose)
        {
            DoorsHandler.CloseDoorsInRoom(doorRoom);
        }

        // Spam open selected doors
        var map = (MapNames)Utils.GetCurrentMapID();

        if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
        {
            foreach (var doorRoom in _doorsToSpamOpen)
            {
                DoorsHandler.OpenDoorsInRoom(doorRoom);
            }
        }
    }
}
