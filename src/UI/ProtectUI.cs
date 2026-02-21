using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

public class ProtectUI : MonoBehaviour
{
    private Vector2 _scrollPosition = Vector2.zero;
    private Rect _windowRect = new(320, 10, 500, 300);
    public static List<PlayerControl> playersToProtect = new();
    private bool _keepEveryoneProtected;

    private void OnGUI()
    {
        if (!CheatToggles.showProtectMenu) return;

        if(ColorUtility.TryParseHtmlString(MalumMenu.menuHtmlColor.Value, out var configUIColor))
        {
            GUI.backgroundColor = configUIColor;
        }

        _windowRect = GUI.Window(5, _windowRect, (GUI.WindowFunction)ProtectWindow, "Protect Players");
    }

    private void ProtectWindow(int windowID)
    {
        GUILayout.BeginVertical();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        foreach (var pc in playersToProtect)
        {
            if (!pc.Data || !pc.Data.Role || string.IsNullOrEmpty(pc.Data.PlayerName))
            {
                playersToProtect.Remove(pc);  // Ensure to remove invalid players from the list
                break; // Exit the loop to avoid modifying the collection during iteration
            }
        }
        foreach (var pc in PlayerControl.AllPlayerControls)
        {
            if (!pc.Data || !pc.Data.Role || string.IsNullOrEmpty(pc.Data.PlayerName))
            {
                if (playersToProtect.Contains(pc))  // Ensure to remove invalid players from the list
                {
                    playersToProtect.Remove(pc);
                }
                continue;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label($"<color=#{ColorUtility.ToHtmlStringRGB(pc.Data.Color)}>{pc.Data.PlayerName}</color>", GUILayout.Width(140f));
            GUILayout.Label($"{(pc.protectedByGuardianId != -1 ? $"<color=#00FF00>Protected</color> by <color=#{ColorUtility.ToHtmlStringRGB(GameData.Instance.GetPlayerById((byte)pc.protectedByGuardianId).Color)}>{GameData.Instance.GetPlayerById((byte)pc.protectedByGuardianId)._object.Data.PlayerName}</color>" : "<color=#FF0000>Unprotected</color>")}", GUILayout.Width(135));
            if (GUILayout.Button("Protect", GUIStylePreset.NormalButton) && Utils.IsHost && !Utils.IsLobby)
            {
                PlayerControl.LocalPlayer.RpcProtectPlayer(pc, pc.cosmetics.ColorId);
            }

            var keepProtected = playersToProtect.Contains(pc);
            keepProtected = GUILayout.Toggle(keepProtected, "Keep protected", GUIStylePreset.NormalToggle);
            if (keepProtected && !playersToProtect.Contains(pc))
                playersToProtect.Add(pc);
            else if (!keepProtected && playersToProtect.Contains(pc))
                playersToProtect.Remove(pc);

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Protect Everyone") && Utils.IsHost && !Utils.IsLobby)
        {
            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                PlayerControl.LocalPlayer.RpcProtectPlayer(pc, pc.cosmetics.ColorId);
            }
        }

        GUILayout.FlexibleSpace();

        _keepEveryoneProtected = GUILayout.Toggle(_keepEveryoneProtected, "Keep Everyone Protected");
        if (_keepEveryoneProtected)
        {
            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                if (!playersToProtect.Contains(pc))
                {
                    playersToProtect.Add(pc);
                }
            }
        }
        else
        {
            if (PlayerControl.AllPlayerControls.Count == playersToProtect.Count)  // Only clear the list if all players were being kept protected
            {
                playersToProtect.Clear();
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.DragWindow();
    }
}
