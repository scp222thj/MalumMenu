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
        // Fullbright is automatically activated when zooming out, spectating other players, or "freecamming"
        // This is done to avoid issues with shadows

        if (Camera.main == null) return false;
        var follower = Camera.main.gameObject.GetComponent<FollowerCamera>();
        return CheatToggles.noShadows || Camera.main.orthographicSize > 3f || (follower != null && follower.Target != PlayerControl.LocalPlayer);
    }

    public static void ZoomOut(HudManager hudManager)
    {
        if (CheatToggles.zoomOut)
        {
            if (hudManager.Chat.IsOpenOrOpening || PlayerCustomizationMenu.Instance || (Utils.isLobby && (FriendsListUI.Instance.IsOpen ||
                GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.gameObject.active || GameStartManager.Instance.RulesEditPanel))) return;

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
            if (CheatToggles.streamerMode) return;
            foreach (var playerState in meetingHud.playerStates)
            {
                // Fetch the NetworkedPlayerInfo of each playerState
                var data = GameData.Instance.GetPlayerById(playerState.TargetPlayerId);

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
            if (CheatToggles.streamerMode) return;

            var tag = Utils.GetNameTag(playerPhysics.myPlayer.Data, playerPhysics.myPlayer.CurrentOutfit.PlayerName);

            // Kill cooldown overlay: append remaining CD above impostors' heads (host only, has the data)
            if (CheatToggles.showKillCdOverlay && Utils.isHost && playerPhysics.myPlayer.Data.Role?.IsImpostor == true)
            {
                float cd = Mathf.CeilToInt(playerPhysics.myPlayer.killTimer);
                if (cd > 0f) tag = $"<size=70%><color=#ff4444>🗡 {cd}s</color></size>\n{tag}";
            }

            // Player notes suffix
            tag += PlayerNotesUI.GetNoteSuffix(playerPhysics.myPlayer.PlayerId);

            playerPhysics.myPlayer.cosmetics.SetName(tag);
            // Move the nameText up to prevent it overlapping with colorblind text
            if (CheatToggles.seeRoles && CheatToggles.seePlayerInfo)
            {
                playerPhysics.myPlayer.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0.186f, 0f);
            }
            else if (CheatToggles.seeRoles || CheatToggles.seePlayerInfo)
            {
                playerPhysics.myPlayer.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0.093f, 0f);
            }
            else
            {
                playerPhysics.myPlayer.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
        } catch { }
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
            if (Camera.main == null || PlayerControl.LocalPlayer == null) return;

            if (!_freecamActive)
            {
                var fc = Camera.main.gameObject.GetComponent<FollowerCamera>();
                if (fc != null) { fc.enabled = false; fc.Target = null; }
                _freecamActive = true;
            }

            PlayerControl.LocalPlayer.moveable = false;

            var movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
            Camera.main.transform.position = Camera.main.transform.position + movement * 10f * Time.deltaTime;

        }
        else
        {
            if (!_freecamActive) return;
            if (Camera.main != null && PlayerControl.LocalPlayer != null)
            {
                PlayerControl.LocalPlayer.moveable = true;
                var fc = Camera.main.gameObject.GetComponent<FollowerCamera>();
                if (fc != null) { fc.enabled = true; fc.SetTarget(PlayerControl.LocalPlayer); }
            }
            _freecamActive = false;
        }
    }
}
