using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sentry.Internal.Extensions;

namespace MalumMenu;
public static class MalumESP
{
    private static bool _freecamActive;
    private static bool _resolutionChangeNeeded;
    public static void SporeCloudVision(Mushroom mushroom)
    {
        if (CheatToggles.noShadows)
        {
            // Change the Z axis position of spore clouds as to make players appear above them

            mushroom.sporeMask.transform.position = new Vector3(mushroom.sporeMask.transform.position.x, mushroom.sporeMask.transform.position.y, -1);
            return;
        }

        // Normal Z axis position: 5f
        mushroom.sporeMask.transform.position = new Vector3(mushroom.sporeMask.transform.position.x, mushroom.sporeMask.transform.position.y, 5f);
    }

    public static bool IsFullbrightActive()
    {
        // Fullbright is automatically activated when being a ghost, zooming out, spectating other players, or "freecamming"
        // This is done to avoid issues with shadows

        return CheatToggles.noShadows || (PlayerControl.LocalPlayer?.Data && PlayerControl.LocalPlayer.Data.IsDead) || Camera.main.orthographicSize > 3f || Camera.main.gameObject.GetComponent<FollowerCamera>().Target != PlayerControl.LocalPlayer;
    }

    public static void ZoomOut(HudManager hudManager)
    {
        if (CheatToggles.zoomOut)
        {
            // Suspend zoomOut whenever a UI screen requires scrolling
            if (hudManager.Chat.IsOpenOrOpening || MatchInfoGuide.Instance.IsActive || PlayerCustomizationMenu.Instance ||
            (Utils.isLobby && (FriendsListUI.Instance.IsOpen || GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.gameObject.active || GameStartManager.Instance.RulesEditPanel))) return;

            _resolutionChangeNeeded = true;

            if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // Zoom out
            {

                // Both the main camera and the UI camera need to be adjusted

                Camera.main.orthographicSize++;
                hudManager.UICamera.orthographicSize++;

                // Utils.AdjustResolution() seems to be needed to properly sync the game's UI
                // after a change in orthographicSize

                Utils.AdjustResolution();

            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f )
            {
                // Zoom in
                if (!(Camera.main.orthographicSize > 3f)) return; // Never go below the default orthographicSize: 3f

                Camera.main.orthographicSize--;
                hudManager.UICamera.orthographicSize--;

                Utils.AdjustResolution();
            }
        }
        else
        {
            // orthographicSize is reset to default value: 3f
            Camera.main.orthographicSize = 3f;
            hudManager.UICamera.orthographicSize = 3f;

            // Utils.AdjustResolution() is invoked one last time to prevent issues with UI
            if (_resolutionChangeNeeded)
            {
                Utils.AdjustResolution();
                _resolutionChangeNeeded = false;
            }
        }
    }

    public static void MeetingNametags(MeetingHud meetingHud)
    {
        try
        {
            foreach (var playerState in meetingHud.playerStates)
            {
                // Fetch the NetworkedPlayerInfo of each playerState
                var data = GameData.Instance.GetPlayerById(playerState.PlayerId);

                if (data.IsNull() || data.Disconnected || data.Outfits[PlayerOutfitType.Default].IsNull()) continue;

                // Update the player's nametag appropriately
                playerState.NameText.text = Utils.GetNameTag(data, data.DefaultOutfit.PlayerName);

                // Move and resize the nametag to prevent it overlapping with colorblind text
                if (CheatToggles.seeRoles && CheatToggles.seePlayerInfo)
                {
                    playerState.NameText.transform.localPosition = new Vector3(0.33f, 0.08f, 0f);
                    playerState.NameText.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                }
                else if (CheatToggles.seeRoles || CheatToggles.seePlayerInfo)
                {
                    playerState.NameText.transform.localPosition = new Vector3(0.3384f, 0.1125f, -0.1f);
                    playerState.NameText.transform.localScale = new Vector3(0.9f, 1f, 1f);
                }
                else
                {
                    // Reset the position and scale of the nametag to default values (they're kinda weird but whatever)
                    playerState.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
                    playerState.NameText.transform.localScale = new Vector3(0.9f, 1f, 1f);
                }
            }
        } catch { }
    }

    public static void PlayerNametags(PlayerPhysics playerPhysics)
    {
        try
        {
            UpdateVentAppearance(playerPhysics.myPlayer);
        } catch { }
    }

    public static void RefreshVentOpacity()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            try { UpdateVentAppearance(player); } catch { }
        }
    }

    private static void UpdateVentAppearance(PlayerControl player)
    {
        if (player == null) return;

        if (player.cosmetics != null && player.Data != null)
        {
            player.cosmetics.SetName(Utils.GetNameTag(player.Data, player.CurrentOutfit.PlayerName));
        }

        // The vent visibility feature should only affect remote players.
        // Touching local vent rendering can interfere with vent travel controls.
        if (player.AmOwner) return;

        float ventOpacity = Mathf.Clamp(CheatToggles.ventPlayerOpacity, 0f, 100f) / 100f;

        if (!player.inVent || ventOpacity <= 0f)
        {
            if (player.inVent && ventOpacity <= 0f)
            {
                if (player.cosmetics?.nameText != null)
                {
                    player.cosmetics.nameText.enabled = false;
                    player.cosmetics.nameText.gameObject.SetActive(false);
                }

                if (player.cosmetics?.colorBlindText != null)
                {
                    player.cosmetics.colorBlindText.enabled = false;
                    player.cosmetics.colorBlindText.gameObject.SetActive(false);
                }
            }

            if (!player.inVent && player.Data != null && !player.Data.IsDead)
            {
                RestoreVentAppearance(player);

                if (player.cosmetics != null && player.cosmetics.nameText != null)
                {
                    if (CheatToggles.seeRoles && CheatToggles.seePlayerInfo)
                    {
                        player.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0.186f, 0f);
                    }
                    else if (CheatToggles.seeRoles || CheatToggles.seePlayerInfo)
                    {
                        player.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0.093f, 0f);
                    }
                    else
                    {
                        player.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0f, 0f);
                    }
                }
            }

            return;
        }

        // Keep the vented player visible while the vented state is rendered locally.
        player.Visible = true;

        if (player.cosmetics != null)
        {
            player.cosmetics.gameObject.SetActive(true);
        }

        // Apply the configured opacity to the cosmetic layer so body parts, hats, and pets share the same fade.
        ApplyVentOpacity(player, ventOpacity);

        // Show nametag
        if (player.cosmetics?.nameText != null)
        {
            player.cosmetics.nameText.gameObject.SetActive(true);
            player.cosmetics.nameText.enabled = true;
        }

        if (player.cosmetics?.colorBlindText != null)
        {
            player.cosmetics.colorBlindText.gameObject.SetActive(true);
            player.cosmetics.colorBlindText.enabled = true;
        }
    }

    private static void RestoreVentAppearance(PlayerControl player)
    {
        if (player == null) return;

        if (player.Collider != null) player.Collider.enabled = true;

        TrySetCosmeticsOpacity(player, 1f);

        if (player.cosmetics != null)
        {
            player.cosmetics.gameObject.SetActive(true);
        }

        foreach (var spriteRenderer in GetVentSpriteRenderers(player))
        {
            if (!Mathf.Approximately(spriteRenderer.color.a, 1f))
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            }
        }

        if (player.cosmetics?.nameText != null)
        {
            player.cosmetics.nameText.gameObject.SetActive(true);
            player.cosmetics.nameText.enabled = true;
        }

        if (player.cosmetics?.colorBlindText != null)
        {
            player.cosmetics.colorBlindText.gameObject.SetActive(true);
            player.cosmetics.colorBlindText.enabled = true;
        }
    }

    private static void ApplyVentOpacity(PlayerControl player, float opacity)
    {
        if (player == null) return;

        TrySetCosmeticsOpacity(player, opacity);

        foreach (var spriteRenderer in GetVentSpriteRenderers(player))
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
        }
    }

    private static IEnumerable<SpriteRenderer> GetVentSpriteRenderers(PlayerControl player)
    {
        if (player == null) yield break;

        var seenRenderers = new HashSet<int>();

        if (player.cosmetics != null && player.cosmetics.transform != null)
        {
            foreach (var spriteRenderer in player.cosmetics.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (spriteRenderer != null && seenRenderers.Add(spriteRenderer.GetInstanceID()))
                {
                    yield return spriteRenderer;
                }
            }
        }

        foreach (var spriteRenderer in player.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (spriteRenderer == null) continue;

            if (IsSafeFallbackRenderer(spriteRenderer.gameObject.name) && seenRenderers.Add(spriteRenderer.GetInstanceID()))
            {
                yield return spriteRenderer;
            }
        }
    }

    private static bool IsSafeFallbackRenderer(string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) return true;

        return objectName.IndexOf("camera", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               objectName.IndexOf("canvas", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               objectName.IndexOf("screen", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               objectName.IndexOf("rendertexture", System.StringComparison.OrdinalIgnoreCase) < 0 &&
               objectName.IndexOf("ui", System.StringComparison.OrdinalIgnoreCase) < 0;
    }

    private static bool TrySetCosmeticsOpacity(PlayerControl player, float opacity)
    {
        var cosmetics = player?.cosmetics;
        if (cosmetics == null) return false;

        var method = cosmetics.GetType().GetMethod("SetAlpha", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(float) }, null);
        if (method == null) return false;

        // Callers pass normalized alpha (0..1), matching the expected SetAlpha input.
        method.Invoke(cosmetics, new object[] { Mathf.Clamp01(opacity) });
        return true;
    }

    public static void ChatNametags(ChatBubble chatBubble)
    {
        try
        {
            // Update the player's nametag appropriately
            chatBubble.NameText.text = Utils.GetNameTag(chatBubble.playerInfo, chatBubble.NameText.text, true);

            // Adjust the chatBubble's size to the new nametag to prevent issues
            chatBubble.NameText.ForceMeshUpdate(true, true);
            chatBubble.Background.size = new Vector2(5.52f, 0.2f + chatBubble.NameText.GetNotDumbRenderedHeight() + chatBubble.TextArea.GetNotDumbRenderedHeight());
            chatBubble.MaskArea.size = chatBubble.Background.size - new Vector2(0f, 0.03f);

        } catch { }
    }

    public static void SeeGhostsCheat(PlayerPhysics playerPhysics)
    {
        try{

            if(playerPhysics.myPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                playerPhysics.myPlayer.Visible = CheatToggles.seeGhosts;
            }

        }catch{}
    }

    public static void FreecamCheat()
    {
        if (CheatToggles.freecam)
        {
            // Completely disable FollowerCamera
            if (!_freecamActive)
            {

                Camera.main.gameObject.GetComponent<FollowerCamera>().enabled = false;
                Camera.main.gameObject.GetComponent<FollowerCamera>().Target = null;

                _freecamActive = true;

            }

            // Prevent the player from moving while in freecam
            PlayerControl.LocalPlayer.moveable = false;

            // Get keyboard input
            var movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

            // Change the camera's position depending on the keyboard input
            // Speed: 10f
            Camera.main.transform.position = Camera.main.transform.position + movement * 10f * Time.deltaTime;

        }
        else
        {
            // Re-enable FollowerCamera & movement once freecam is disabled
            if (!_freecamActive) return;
            PlayerControl.LocalPlayer.moveable = true;
            Camera.main.gameObject.GetComponent<FollowerCamera>().enabled = true;
            Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            _freecamActive = false;
        }
    }
}
