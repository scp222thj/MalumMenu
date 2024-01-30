using UnityEngine;
using System;
using Sentry.Internal.Extensions;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;
public static class MalumESP
{
    public static bool spectateActive;
    public static bool freecamActive;
    public static void sporeCloudVision(Mushroom mushroom)
    {
        if (CheatToggles.fullBright)
        {
            mushroom.sporeMask.transform.position = new Vector3(mushroom.sporeMask.transform.position.x, mushroom.sporeMask.transform.position.y, -1);
            return;
        } 

        mushroom.sporeMask.transform.position = new Vector3(mushroom.sporeMask.transform.position.x, mushroom.sporeMask.transform.position.y, 5f);
    }

    public static void meetingNametags(MeetingHud meetingHud)
    {
        try{
            foreach (PlayerVoteArea playerState in meetingHud.playerStates)
            {
                //Fetching the GameData.PlayerInfo of each playerState to get the player's role
                GameData.PlayerInfo data = GameData.Instance.GetPlayerById(playerState.TargetPlayerId);

                if (!data.IsNull() && !data.Disconnected && !data.Outfits[PlayerOutfitType.Default].IsNull())
                {
                    //Get appropriate name color depending on if CheatSettings.seeRoles is enabled
                    playerState.NameText.text = Utils.getNameTag(data.Object, data.DefaultOutfit.PlayerName);
                }

            }
        }catch{}
    }

    public static void playerNametags(PlayerPhysics playerPhysics)
    {
        try{
            if (!playerPhysics.myPlayer.Data.IsNull() && !playerPhysics.myPlayer.Data.Disconnected && !playerPhysics.myPlayer.CurrentOutfit.IsNull())
            {
                playerPhysics.myPlayer.cosmetics.SetName(Utils.getNameTag(playerPhysics.myPlayer, playerPhysics.myPlayer.CurrentOutfit.PlayerName));
                
                if (playerPhysics.myPlayer.inVent){
                    playerPhysics.myPlayer.cosmetics.nameText.gameObject.SetActive(CheatToggles.ventVision);
                }
            }
        }catch{}
    }

    public static void chatNametags(ChatBubble chatBubble)
    {
        try{
            chatBubble.NameText.text = Utils.getNameTag(chatBubble.playerInfo.Object, chatBubble.NameText.text, true);
            chatBubble.NameText.ForceMeshUpdate(true, true);
            chatBubble.Background.size = new Vector2(5.52f, 0.2f + chatBubble.NameText.GetNotDumbRenderedHeight() + chatBubble.TextArea.GetNotDumbRenderedHeight());
            chatBubble.MaskArea.size = chatBubble.Background.size - new Vector2(0f, 0.03f);
        }catch{}
    }


    public static void spectateCheat()
    {
        if (CheatToggles.spectate){

            //Open spectator menu when CheatSettings.spectate is first enabled
            if (!spectateActive){

                //Close any player pick menus already open & their cheats
                if (PlayerPickMenu.playerpickMenu != null){
                    PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("spectate");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerDataList.Add(player.Data);
                    }
                }

                //New player pick menu made for spectating
                PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerPickMenu.targetPlayerData.Object);
                }));

                spectateActive = true;

                PlayerControl.LocalPlayer.moveable = false; //Can't move while spectating

                CheatToggles.freecam = false; //Disable incompatible cheats while spectating

            }

            //Deactivate cheat if menu is closed without spectating anyone
            if (PlayerPickMenu.playerpickMenu == null && Camera.main.gameObject.GetComponent<FollowerCamera>().Target == PlayerControl.LocalPlayer){
                CheatToggles.spectate = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }else{
            //Deactivate cheat when it is disabled from the GUI
            if (spectateActive){
                spectateActive = false;
                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            }
        }
    }

    public static void freecamCheat()
    {
        if(CheatToggles.freecam){
            //Disable FollowerCamera & prevent the player from moving while in freecam
            if (!freecamActive){

                Camera.main.gameObject.GetComponent<FollowerCamera>().enabled = false;
                Camera.main.gameObject.GetComponent<FollowerCamera>().Target = null;

                freecamActive = true;

            }

            PlayerControl.LocalPlayer.moveable = false;

            //Get keyboard input & turn it into movement for the camera
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

            //Change the camera's position depending on the input
            //Speed: 10f
            Camera.main.transform.position = Camera.main.transform.position + movement * 10f * Time.deltaTime;
            
        }else{
            if (freecamActive){

                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().enabled = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
                freecamActive = false;

            }
        }
    }
}