using UnityEngine;
using Sentry.Internal.Extensions;

namespace MalumMenu;
public static class MalumESP
{
    public static bool freecamActive;
    public static bool resolutionchangeNeeded;
    public static void sporeCloudVision(Mushroom mushroom)
    {
        if (CheatToggles.fullBright)
        {
            //Change the Z axis position of spore clouds as to make players appear above them

            mushroom.sporeMask.transform.position = new Vector3(mushroom.sporeMask.transform.position.x, mushroom.sporeMask.transform.position.y, -1);
            return;
        } 

        // Normal Z axis position: 5f
        mushroom.sporeMask.transform.position = new Vector3(mushroom.sporeMask.transform.position.x, mushroom.sporeMask.transform.position.y, 5f);
    }

    public static bool fullBrightActive()
    {
        // Fullbright is automatically activated when zooming out, spectating other players, or "freecamming"
        // This is done to avoid issues with shadows

        return CheatToggles.fullBright || Camera.main.orthographicSize > 3f || Camera.main.gameObject.GetComponent<FollowerCamera>().Target != PlayerControl.LocalPlayer;
    }

    public static void zoomOut(HudManager hudManager)
    {
        if(CheatToggles.zoomOut){
            
            resolutionchangeNeeded = true;

            if(Input.GetAxis("Mouse ScrollWheel") < 0f ){ // Zoom out
                
                //Both the main camera and the UI camera need to be adjusted

                Camera.main.orthographicSize++;
                hudManager.UICamera.orthographicSize++;

                // Utils.AdjustResolution() seems to be needed to properly sync the game's UI 
                // after a change in orthographicSize

                Utils.adjustResolution();
                
            } else if(Input.GetAxis("Mouse ScrollWheel") > 0f ){ // Zoom in
                if (Camera.main.orthographicSize > 3f){ // Never go below the default orthographicSize: 3f

                    Camera.main.orthographicSize--;
                    hudManager.UICamera.orthographicSize--;
                    
                    Utils.adjustResolution();
                }
            }
        } else {

            // orthographicSize is reset to default value: 3f
            Camera.main.orthographicSize = 3f;
            hudManager.UICamera.orthographicSize = 3f;

            // Utils.AdjustResolution() is invoked one last time to prevent issues with UI
            if (resolutionchangeNeeded){
                Utils.adjustResolution();
                resolutionchangeNeeded = false;
            }
        }
    }

    public static void meetingNametags(MeetingHud meetingHud)
    {
        try{
            foreach (PlayerVoteArea playerState in meetingHud.playerStates)
            {
                // Fetch the NetworkedPlayerInfo of each playerState
                NetworkedPlayerInfo data = GameData.Instance.GetPlayerById(playerState.TargetPlayerId);

                if (!data.IsNull() && !data.Disconnected && !data.Outfits[PlayerOutfitType.Default].IsNull())
                {
                    // Update the player's nametag appropriately
                    playerState.NameText.text = Utils.getNameTag(data, data.DefaultOutfit.PlayerName);
                }

            }
        }catch{}
    }

    public static void playerNametags(PlayerPhysics playerPhysics)
    {
        try{

            playerPhysics.myPlayer.cosmetics.SetName(Utils.getNameTag(playerPhysics.myPlayer.Data, playerPhysics.myPlayer.CurrentOutfit.PlayerName));
        
        }catch{}
    }

    public static void chatNametags(ChatBubble chatBubble)
    {
        try{

            // Update the player's nametag appropriately
            chatBubble.NameText.text = Utils.getNameTag(chatBubble.playerInfo, chatBubble.NameText.text, true);
            
            // Adjust the chatBubble's size to the new nametag to prevent issues
            chatBubble.NameText.ForceMeshUpdate(true, true);
            chatBubble.Background.size = new Vector2(5.52f, 0.2f + chatBubble.NameText.GetNotDumbRenderedHeight() + chatBubble.TextArea.GetNotDumbRenderedHeight());
            chatBubble.MaskArea.size = chatBubble.Background.size - new Vector2(0f, 0.03f);

        }catch{}
    }

    public static void seeGhostsCheat(PlayerPhysics playerPhysics)
    {
        try{

            if(playerPhysics.myPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsDead){
                playerPhysics.myPlayer.Visible = CheatToggles.seeGhosts;
            }    

        }catch{}
    }

    public static void freecamCheat()
    {
        if(CheatToggles.freecam){
            // Completly disable FollowerCamera
            if (!freecamActive){

                Camera.main.gameObject.GetComponent<FollowerCamera>().enabled = false;
                Camera.main.gameObject.GetComponent<FollowerCamera>().Target = null;

                freecamActive = true;

            }

            // Prevent the player from moving while in freecam
            PlayerControl.LocalPlayer.moveable = false;

            // Get keyboard input
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

            // Change the camera's position depending on the keyboard input
            // Speed: 10f
            Camera.main.transform.position = Camera.main.transform.position + movement * 10f * Time.deltaTime;
            
        }else{
            // Reenable FollowerCamera & movement once freecam is disabled
            if (freecamActive){

                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().enabled = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
                freecamActive = false;

            }
        }
    }
}