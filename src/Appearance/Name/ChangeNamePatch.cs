using HarmonyLib;
using System;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class ChangePlayerName_PlayerControl_RpcSendChat_Prefix
{
    public static PlayerControl changeNameTarget = null;
    // Prefix patch of PlayerControl.RpcSendChat to set a custom name to the player
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (!CheatToggles.changeName || changeNameTarget == null) return true; //Only works if CheatToggles.setPlayerName is enabled

        Utils.SetName(changeNameTarget, chatText);

        CheatToggles.changeName = false; // Disable the cheat toggle

        return false;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ChangePlayerName_PlayerControl_LateUpdate_Postfix
{
    public static bool isActive; // this is for the chat to close automatically later
    public static bool isMenuActive; // this is internally needed for menu
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.changeName)
        {
            if (!isActive && !isMenuActive)
            {
                CheatToggles.chatMimic = CheatToggles.spamChat = CheatToggles.changeNameAll = false; // these cheats do not work together

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
                    ChangePlayerName_PlayerControl_RpcSendChat_Prefix.changeNameTarget = Utils_PlayerPickMenu.targetPlayerData.Object; // send the target to the rpcsendchat detector
                    Utils.OpenChat(); // open the chat box
                    isActive = true;
                }));

                isMenuActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null && ChangePlayerName_PlayerControl_RpcSendChat_Prefix.changeNameTarget == null){
                CheatToggles.changeName = false;
            }
        }
        else
        {
            if (isActive)
            {
                if (!CheatToggles.chatMimic && !CheatToggles.spamChat && !CheatToggles.changeNameAll) Utils.CloseChat();
                ChangePlayerName_PlayerControl_RpcSendChat_Prefix.changeNameTarget = null;
                isActive = false;
            };

            if (isMenuActive)
            {
                isMenuActive = false;
            };
        };
    }
}
