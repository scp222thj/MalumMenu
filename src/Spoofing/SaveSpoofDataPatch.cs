using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System;
using BepInEx;
using System.IO;
using BepInEx.Configuration;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class RPC_SaveSpoofDataPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to kick any player
    public static bool isActive;
    public static ConfigFile userSpoofSaveFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "MM_SavedSpoofData.cfg"), true);
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.saveSpoofData){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatSettings.spectate = CheatSettings.kickPlayer = CheatSettings.callMeeting = CheatSettings.shapeshiftAll = CheatSettings.copyOutfit = CheatSettings.teleportPlayer = CheatSettings.murderPlayer = false;
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerList.Add(player);
                    }
                }

                //New player pick menu made for kicking players
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    var friendCode = userSpoofSaveFile.Bind($"SpoofData.{Utils_PlayerPickMenu.targetPlayer.Data.PlayerName}",
                                    "FriendCode",
                                    "");
                    friendCode.Value = AmongUsClient.Instance.GetClientFromCharacter(Utils_PlayerPickMenu.targetPlayer).FriendCode;

                    var puid = userSpoofSaveFile.Bind($"SpoofData.{Utils_PlayerPickMenu.targetPlayer.Data.PlayerName}",
                                    "ProductUserID",
                                    "");
                    puid.Value = AmongUsClient.Instance.GetClientFromCharacter(Utils_PlayerPickMenu.targetPlayer).ProductUserId;
                
                    Utils.showPopup($"\nSpoofing data for {Utils_PlayerPickMenu.targetPlayer.Data.PlayerName} has been saved in\n\nBepInEx/config/MM_SavedSpoofData.cfg");
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatSettings.saveSpoofData = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}