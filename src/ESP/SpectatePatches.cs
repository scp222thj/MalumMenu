using HarmonyLib;
using UnityEngine;
using System;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Spectate_PlayerPhysics_LateUpdate_Postfix
{
    // Postfix patch of PlayerPhysics.LateUpdate to open spectator menu
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.spectate)
        {
            // Open spectator menu when CheatSettings.spectate is first enabled
            if (!isActive)
            {
                // Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("spectate");
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

                // New player pick menu made for spectating
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(Utils_PlayerPickMenu.targetPlayerData.Object);
                }));

                isActive = true;

                PlayerControl.LocalPlayer.moveable = false; // Can't move while spectating

                CheatToggles.freeCam = false; // Disable incompatible cheats while spectating

            }

            // Deactivate cheat if menu is closed without spectating anyone
            if (Utils_PlayerPickMenu.playerpickMenu == null && Camera.main.gameObject.GetComponent<FollowerCamera>().Target == PlayerControl.LocalPlayer)
            {
                CheatToggles.spectate = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }
        else
        {
            // Deactivate cheat when it is disabled from the GUI
            if (isActive)
            {
                isActive = false;
                PlayerControl.LocalPlayer.moveable = true;
                Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            }
        }
    }
}