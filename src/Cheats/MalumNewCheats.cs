using Il2CppSystem.Collections.Generic;
using System;
using UnityEngine;
using Hazel;

namespace MalumMenu;

/// <summary>
/// Contains implementations for the new cheat features:
/// - Set All Same Color (Host-Only): Sets all players to the same selected color
/// - Send Private Message (Any-One): Sends a chat message visible only to selected players
/// </summary>
public static class MalumNewCheats
{
    private static bool _setAllSameColorActive;
    private static bool _sendPrivateMessageActive;

    // Among Us color names matching the color IDs
    public static readonly string[] ColorNames = new string[]
    {
        "Red", "Blue", "Green", "Pink", "Orange",
        "Yellow", "Black", "White", "Purple", "Brown",
        "Cyan", "Lime", "Maroon", "Rose", "Banana",
        "Gray", "Tan", "Coral", "Olive", "Turquoise"
    };

    /// <summary>
    /// Host-Only: Opens a PPM to select a color, then sets ALL players to that color.
    /// Uses RPC SetColor (0x08) sent from each player's PlayerControl NetId.
    /// Only the host can do this because SetColor is a host-authority RPC.
    /// </summary>
    public static void SetAllSameColorPPM()
    {
        if (CheatToggles.setAllSameColor)
        {
            if (!_setAllSameColorActive)
            {
                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("setAllSameColor");
                }

                List<NetworkedPlayerInfo> colorChoices = new List<NetworkedPlayerInfo>();

                // Create a PPM entry for each available color
                for (int i = 0; i < ColorNames.Length && i < Palette.PlayerColors.Count; i++)
                {
                    var outfit = new NetworkedPlayerInfo.PlayerOutfit()
                    {
                        ColorId = i
                    };
                    colorChoices.Add(PlayerPickMenu.CustomPPMChoice(ColorNames[i], outfit));
                }

                // Player pick menu to choose a color and set all players to it
                PlayerPickMenu.OpenPlayerPickMenu(colorChoices, (Action)(() =>
                {
                    int colorId = PlayerPickMenu.targetPlayerData.DefaultOutfit.ColorId;

                    // Set every player's color to the selected color using RPC
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        // Use RpcSetColor which is the proper host-authority method
                        player.RpcSetColor((byte)colorId);
                    }
                }));

                _setAllSameColorActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.setAllSameColor = false;
            }
        }
        else if (_setAllSameColorActive)
        {
            _setAllSameColorActive = false;
        }
    }

    /// <summary>
    /// Any-One: Opens a PPM to select a target player, then activates private message mode.
    /// Only stores the target's player ID (byte) and name (string) — never IL2CPP object references.
    /// After selection, the in-game chat opens and the next message sent is intercepted
    /// by the ChatController patch and sent as a targeted RPC.
    /// </summary>
    public static void SendPrivateMessagePPM()
    {
        if (CheatToggles.sendPrivateMessage)
        {
            if (!_sendPrivateMessageActive)
            {
                // Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null)
                {
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("sendPrivateMessage");
                }

                List<NetworkedPlayerInfo> playerDataList = new List<NetworkedPlayerInfo>();

                // Add all players except LocalPlayer to the selection
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!player.AmOwner && player.Data != null && !player.Data.Disconnected)
                    {
                        playerDataList.Add(player.Data);
                    }
                }

                // Player pick menu to choose the target for the private message
                // IMPORTANT: Only extract primitive values from the PPM result, never store the IL2CPP object
                PlayerPickMenu.OpenPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    byte playerId = PlayerPickMenu.targetPlayerData.PlayerId;
                    string playerName = PlayerPickMenu.targetPlayerData.PlayerName;

                    // Activate private mode with safe primitive values only
                    PrivateMessageState.Activate(playerId, playerName);
                }));

                _sendPrivateMessageActive = true;
            }

            // Deactivate cheat if menu is closed
            if (PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.sendPrivateMessage = false;
            }
        }
        else if (_sendPrivateMessageActive)
        {
            _sendPrivateMessageActive = false;
        }
    }

    /// <summary>
    /// Sends a chat message that only the specified target player can see.
    /// Looks up the player fresh by ID to avoid dangling IL2CPP pointers.
    /// Uses StartRpcImmediately with the target's client ID to send a targeted SendChat RPC.
    /// </summary>
    public static void SendPrivateChat(string message, byte targetPlayerId, string targetName)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        // Look up the target player fresh by ID — never use cached IL2CPP references
        PlayerControl targetPlayer = null;
        try
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player != null && player.PlayerId == targetPlayerId)
                {
                    targetPlayer = player;
                    break;
                }
            }
        }
        catch { return; }

        if (targetPlayer == null) return;

        try
        {
            // Get the target's client ID
            int targetClientId = AmongUsClient.Instance.GetClientIdFromCharacter(targetPlayer);

            // Send SendChat RPC (0x0d) only to the targeted player
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                (byte)RpcCalls.SendChat,
                SendOption.Reliable,
                targetClientId
            );
            writer.Write(message);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            // Also show it locally in our own chat (so we can see what we sent)
            if (DestroyableSingleton<HudManager>.Instance?.Chat != null)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                    PlayerControl.LocalPlayer,
                    $"<color=#ff69b4>[PM > {targetName}]</color> {message}",
                    false
                );
            }
        }
        catch { }
    }
}
