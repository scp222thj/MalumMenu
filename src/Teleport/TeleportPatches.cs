using HarmonyLib;
using UnityEngine;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class TeleportCursor_PlayerPhysics_LateUpdate_Postfix
{
    // Postfix patch of PlayerPhysics.LateUpdate to teleport to cursor position on right-click
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.teleportCursor)
        {
            if (Input.GetMouseButtonDown(1)) // Register right-click
            {
                // Snap the client to the screen point from the mouse position
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class TeleportToPlayer_PlayerPhysics_LateUpdate_Postfix
{
    // Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to teleport yourself to any player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.teleportPlayer)
        {
            if (!isActive)
            {
                // Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("teleportPlayer");
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

                //New player pick menu made for teleporting yourself to any player's position
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Utils_PlayerPickMenu.targetPlayerData.Object.transform.position);
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.teleportPlayer = false;
            }
        }
        else if (isActive)
        {
            isActive = false;
        }
    }
}