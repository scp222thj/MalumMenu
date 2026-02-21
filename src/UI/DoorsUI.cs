using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

public class DoorsUI : MonoBehaviour
{
    private Rect _windowRect = new(320, 10, 480, 270);
    private List<SystemTypes> doorsToSpamOpen = new();
    private List<SystemTypes> doorsToSpamClose = new();

    private void OnGUI()
    {
        if (!CheatToggles.showDoorsMenu) return;

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

        var map = (MapNames)Utils.getCurrentMapID();

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
                var spamClose = doorsToSpamClose.Contains(doorRoom);
                spamClose = GUILayout.Toggle(spamClose, "Spam Close", GUIStylePreset.NormalToggle);
                if (spamClose && !doorsToSpamClose.Contains(doorRoom))
                    doorsToSpamClose.Add(doorRoom);
                else if (!spamClose && doorsToSpamClose.Contains(doorRoom))
                    doorsToSpamClose.Remove(doorRoom);

                if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
                {
                    var spamOpen = doorsToSpamOpen.Contains(doorRoom);
                    spamOpen = GUILayout.Toggle(spamOpen, "Spam Open", GUIStylePreset.NormalToggle);
                    if (spamOpen && !doorsToSpamOpen.Contains(doorRoom))
                        doorsToSpamOpen.Add(doorRoom);
                    else if (!spamOpen && doorsToSpamOpen.Contains(doorRoom))
                        doorsToSpamOpen.Remove(doorRoom);
                }
            }
            else
            {
                // Clear spam lists if not host
                if (doorsToSpamClose.Count != 0 || doorsToSpamOpen.Count != 0)
                {
                    doorsToSpamClose.Clear();
                    doorsToSpamOpen.Clear();
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();
        GUILayout.Box("", GUIStylePreset.Separator, GUILayout.Height(1f), GUILayout.ExpandWidth(true));
        GUILayout.Box("", GUIStyle.none, GUILayout.Height(1f), GUILayout.ExpandWidth(true));

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

        // Spam Close selected doors
        foreach (var doorRoom in doorsToSpamClose)
        {
            DoorsHandler.CloseDoorsInRoom(doorRoom);
        }

        // Spam Open selected doors
        var map = (MapNames)Utils.getCurrentMapID();

        if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
        {
            foreach (var doorRoom in doorsToSpamOpen)
            {
                DoorsHandler.OpenDoorsInRoom(doorRoom);
            }
        }
    }
}
