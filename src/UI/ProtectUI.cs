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
        if (!CheatToggles.showProtectMenu || !MenuUI.isGUIActive || MenuUI.isPanicked) return;

        GUI.backgroundColor = MenuUI.GetWindowColor(CheatToggles.rgbMode);

        _windowRect = GUI.Window(5, _windowRect, (GUI.WindowFunction)ProtectWindow, "Protect Players");
    }

    private void ProtectWindow(int windowID)
    {
        GUILayout.BeginVertical();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.Data || !player.Data.Role || string.IsNullOrEmpty(player.Data.PlayerName))
            {
                if (playersToProtect.Contains(player))  // Ensure to remove invalid players from the list
                {
                    playersToProtect.Remove(player);
                }

                continue;
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label($"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Color)}>{player.Data.PlayerName}</color>", GUILayout.Width(140f));

            if (player.protectedByGuardianId == -1)
            {
                GUILayout.Label("<color=#FF0000>Unprotected</color>", GUILayout.Width(135));
            }
            else
            {
                NetworkedPlayerInfo guardianInfo = GameData.Instance.GetPlayerById((byte)player.protectedByGuardianId);
                GUILayout.Label($"<color=#00FF00>Protected</color> by <color=#{ColorUtility.ToHtmlStringRGB(guardianInfo.Color)}>{guardianInfo._object.Data.PlayerName}</color>", GUILayout.Width(135));
            }

            if (GUILayout.Button("Protect", GUIStylePreset.NormalButton) && Utils.isHost && !Utils.isLobby)
            {
                PlayerControl.LocalPlayer.RpcProtectPlayer(player, player.cosmetics.ColorId);
            }

            var keepProtected = playersToProtect.Contains(player);
            keepProtected = GUILayout.Toggle(keepProtected, "Keep protected", GUIStylePreset.NormalToggle);

            if (keepProtected && !playersToProtect.Contains(player))
            {
                playersToProtect.Add(player);
            }
            else if (!keepProtected && playersToProtect.Contains(player))
            {
                playersToProtect.Remove(player);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Protect Everyone") && Utils.isHost && !Utils.isLobby)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                PlayerControl.LocalPlayer.RpcProtectPlayer(player, player.cosmetics.ColorId);
            }
        }

        GUILayout.FlexibleSpace();

        _keepEveryoneProtected = GUILayout.Toggle(_keepEveryoneProtected, "Keep Everyone Protected");

        if (_keepEveryoneProtected)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!playersToProtect.Contains(player))
                {
                    playersToProtect.Add(player);
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
