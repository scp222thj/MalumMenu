using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

public class DoorsUI : MonoBehaviour
{
    public static bool isVisible = false;
    private Rect _windowRect = new(320, 10, 530, 280);
    private GUIStyle _separatorStyle;
    private List<SystemTypes> doorsToSpamOpen = new();
    private List<SystemTypes> doorsToSpamClose = new();

    private void OnGUI()
    {
        if (!isVisible) return;

        _separatorStyle ??= new GUIStyle(GUI.skin.box)
        {
            normal = { background = Texture2D.whiteTexture },
            margin = new RectOffset { top = 4, bottom = 4 },
            padding = new RectOffset(),
            border = new RectOffset()
        };

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

        foreach (var doorRoom in DoorsHandler.GetDoorRooms())
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{doorRoom.ToString()}", GUILayout.Width(120f));

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Status: {DoorsHandler.GetStatusOfDoorsInRoom(doorRoom)}");

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close", GUILayout.Width(50f)))
            {
                DoorsHandler.CloseDoorsOfRoom(doorRoom);
            }

            if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
            {
                if (GUILayout.Button("Open", GUILayout.Width(50f)))
                {
                    DoorsHandler.OpenDoorsOfRoom(doorRoom);
                }
            }

            var spamClose = doorsToSpamClose.Contains(doorRoom);
            spamClose = GUILayout.Toggle(spamClose, "Spam Close");
            if (spamClose && !doorsToSpamClose.Contains(doorRoom))
                doorsToSpamClose.Add(doorRoom);
            else if (!spamClose && doorsToSpamClose.Contains(doorRoom))
                doorsToSpamClose.Remove(doorRoom);

            if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
            {
                var spamOpen = doorsToSpamOpen.Contains(doorRoom);
                spamOpen = GUILayout.Toggle(spamOpen, "Spam Open");
                if (spamOpen && !doorsToSpamOpen.Contains(doorRoom))
                    doorsToSpamOpen.Add(doorRoom);
                else if (!spamOpen && doorsToSpamOpen.Contains(doorRoom))
                    doorsToSpamOpen.Remove(doorRoom);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();
        GUILayout.Box("", _separatorStyle, GUILayout.Height(1f), GUILayout.ExpandWidth(true));
        GUILayout.Box("", GUIStyle.none, GUILayout.Height(1f), GUILayout.ExpandWidth(true));

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Close All"))
        {
            CheatToggles.closeAllDoors = true;
        }

        if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
        {
            if (GUILayout.Button("Open All"))
            {
                CheatToggles.openAllDoors = true;
            }
        }

        GUILayout.FlexibleSpace();

        CheatToggles.spamCloseAllDoors = GUILayout.Toggle(CheatToggles.spamCloseAllDoors, "Spam Close All");

        if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
        {
            CheatToggles.spamOpenAllDoors = GUILayout.Toggle(CheatToggles.spamOpenAllDoors, "Spam Open All");
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
            DoorsHandler.CloseDoorsOfRoom(doorRoom);
        }

        // Spam Open selected doors
        var map = (MapNames)Utils.getCurrentMapID();

        if (map is MapNames.Polus or MapNames.Airship or MapNames.Fungle)
        {
            foreach (var doorRoom in doorsToSpamOpen)
            {
                DoorsHandler.OpenDoorsOfRoom(doorRoom);
            }
        }
    }
}
