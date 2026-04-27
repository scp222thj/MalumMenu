using UnityEngine;

namespace MalumMenu;

/// <summary>
/// Lightweight private message state manager.
/// Does NOT hold any IL2CPP object references to avoid dangling pointer crashes.
/// Only stores the target player's ID (a byte) and name (a managed string).
/// The notification banner is drawn inside MenuUI.OnGUI to avoid a separate MonoBehaviour.
/// </summary>
public static class PrivateMessageState
{
    public static bool isPrivateMode;
    public static byte targetPlayerId;
    public static string targetName = "";

    /// <summary>
    /// Activates private message mode for the specified target.
    /// Only stores safe primitive values, never IL2CPP object references.
    /// </summary>
    public static void Activate(byte playerId, string playerName)
    {
        targetPlayerId = playerId;
        targetName = playerName;
        isPrivateMode = true;

        // Open the in-game chat so the user can type their message there
        Utils.OpenChat();
    }

    /// <summary>
    /// Deactivates private message mode.
    /// </summary>
    public static void Cancel()
    {
        isPrivateMode = false;
        targetPlayerId = 0;
        targetName = "";
    }

    /// <summary>
    /// Gets the target PlayerControl by looking up the stored player ID at call time.
    /// This avoids holding stale IL2CPP references.
    /// Returns null if the player is no longer valid.
    /// </summary>
    public static PlayerControl GetTargetPlayer()
    {
        if (!isPrivateMode) return null;

        try
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player != null && player.Data != null && player.PlayerId == targetPlayerId)
                {
                    return player;
                }
            }
        }
        catch { }

        return null;
    }

    /// <summary>
    /// Draws the notification banner. Called from MenuUI.OnGUI() to avoid
    /// needing a separate MonoBehaviour.
    /// </summary>
    public static void DrawNotification()
    {
        if (!isPrivateMode) return;

        float bannerWidth = 400f;
        float bannerHeight = 30f;
        Rect bannerRect = new Rect(
            Screen.width / 2f - bannerWidth / 2f,
            8f,
            bannerWidth,
            bannerHeight
        );

        GUI.Box(bannerRect, $"  PM MODE -> {targetName}  |  Type in chat & send  |  ESC = cancel");

        // Allow cancelling with Escape key
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            Cancel();
            Event.current.Use();
        }
    }
}
