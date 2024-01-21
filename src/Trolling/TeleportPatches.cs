using HarmonyLib;
using UnityEngine;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class TeleportCursor_PlayerPhysics_LateUpdate_Postfix
{
    public static bool previousTeleportPlayerCursorState = false;
    public static bool previousTeleportAllCursorState = false;
    public static PlayerControl playerTeleportTarget = null;
    public static bool isActive;

    // Postfix patch of PlayerPhysics.LateUpdate to teleport players to cursor position on right-click
    public static void Postfix(PlayerPhysics __instance)
    {
        //Code to make sure only one of these cheats is enabled at a time
        if (CheatToggles.teleportAllCursor && !previousTeleportAllCursorState)
        {
            CheatToggles.teleportPlayerCursor = false;
            previousTeleportAllCursorState = true;
            previousTeleportPlayerCursorState = false;
        }
        else if (CheatToggles.teleportPlayerCursor && !previousTeleportPlayerCursorState)
        {
            CheatToggles.teleportAllCursor = false;
            previousTeleportPlayerCursorState = true;
            previousTeleportAllCursorState = false;
        }

        if (CheatToggles.teleportPlayerCursor)
        {
            //Open player pick menu when CheatSettings.teleportPlayerCursor is first enabled
            if (!isActive)
            {
                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("teleportPlayerCursor");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList apart, including LocalPlayer because this replaced teleportMeCursor
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    playerDataList.Add(player.Data);
                }

                //New player pick menu made for picking a Player Teleport target
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    playerTeleportTarget = Utils_PlayerPickMenu.targetPlayerData.Object;
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed and the target is gone
            if (Utils_PlayerPickMenu.playerpickMenu == null && playerTeleportTarget == null)
            {
                CheatToggles.teleportPlayerCursor = false;
            }
        }
        else
        {
            //Deactivate cheat when it is disabled from the GUI
            if (isActive)
            {
                isActive = false;

                playerTeleportTarget = null;
            }
        }

        if (Input.GetMouseButtonDown(1)) // Register right-click
        {
            if (CheatToggles.teleportPlayerCursor && playerTeleportTarget != null) // Teleport LocalPlayer to cursor position
            {
                Utils.TeleportPlayer(playerTeleportTarget, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            else if (CheatToggles.teleportAllCursor) //Teleport all players to cursor position
            {
                foreach (var item in PlayerControl.AllPlayerControls)
                {
                    Utils.TeleportPlayer(item, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
        }
    }
}


[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class TeleportMePlayer_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to teleport to player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.teleportMePlayer)
        {

            if (!isActive)
            {

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("teleportMePlayer");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!player.AmOwner)
                    {
                        playerDataList.Add(player.Data);
                    }
                }

                //New player pick menu made for teleporting
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    Utils.TeleportPlayer(PlayerControl.LocalPlayer, Utils_PlayerPickMenu.targetPlayerData.Object.transform.position);

                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.teleportMePlayer = false;
            }

        }
        else
        {
            if (isActive)
            {
                isActive = false;
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class TeleportPlayerMe_PlayerPhysics_LateUpdate_Postfix
{
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.teleportPlayerMe)
        {
            if (!isActive)
            {
                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("teleportPlayerMe");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                // All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!player.AmOwner)
                    {
                        playerDataList.Add(player.Data);
                    }
                }

                // New player pick menu made for teleporting
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    Utils.TeleportPlayer(Utils_PlayerPickMenu.targetPlayerData.Object, PlayerControl.LocalPlayer.transform.position);
                }));

                isActive = true;
            }

            // Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null) CheatToggles.teleportPlayerMe = false;

        }
        else if (isActive)
        {
            isActive = false;
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class TeleportAllPlayer_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to teleport all players to one player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.teleportAllPlayer)
        {

            if (!isActive)
            {

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("teleportAllPlayer");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList, including LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    playerDataList.Add(player.Data);
                }

                // New player pick menu made for teleporting
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    foreach (var item in PlayerControl.AllPlayerControls)
                    {
                        if (item.PlayerId != Utils_PlayerPickMenu.targetPlayerData.PlayerId)
                        {
                            Utils.TeleportPlayer(item, Utils_PlayerPickMenu.targetPlayerData.Object.transform.position);
                        }
                    }

                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.teleportAllPlayer = false;
            }

        }
        else if (isActive)
        {

            isActive = false;

        }
    }
}