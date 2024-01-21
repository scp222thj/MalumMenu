using HarmonyLib;
using System;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class SetPlayerName_PlayerControl_RpcSendChat_Prefix
{
    public static PlayerControl setNameTarget = null;
    // Prefix patch of PlayerControl.RpcSendChat to set a custom name to the player
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (!CheatToggles.setPlayerName || setNameTarget == null) return true; //Only works if CheatToggles.setPlayerName is enabled

        Utils.SetName(setNameTarget, chatText);

        CheatToggles.setPlayerName = false; // Disable the cheat toggle

        return false;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class SetPlayerName_PlayerControl_LateUpdate_Postfix
{
    public static bool isActive; // this is for the chat to close automatically later
    public static bool isMenuActive; // this is internally needed for menu
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.setPlayerName)
        {
            if (!isActive && !isMenuActive)
            {
                CheatToggles.chatMimic = CheatToggles.spamChat = CheatToggles.setNameAll = false; // these cheats do not work together

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("setPlayerName");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                // All players are saved to playerList, including LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    playerDataList.Add(player.Data);
                }

                // New player pick menu made for teleporting
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    SetPlayerName_PlayerControl_RpcSendChat_Prefix.setNameTarget = Utils_PlayerPickMenu.targetPlayerData.Object; // send the target to the rpcsendchat detector
                    Utils.OpenChat(); // open the chat box
                    isActive = true;
                }));

                isMenuActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null && SetPlayerName_PlayerControl_RpcSendChat_Prefix.setNameTarget == null){
                CheatToggles.setPlayerName = false;
            }
        }
        else
        {
            if (isActive)
            {
                if (!CheatToggles.chatMimic && !CheatToggles.spamChat && !CheatToggles.setNameAll) Utils.CloseChat();
                SetPlayerName_PlayerControl_RpcSendChat_Prefix.setNameTarget = null;
                isActive = false;
            };

            if (isMenuActive)
            {
                isMenuActive = false;
            };
        };
    }
}
