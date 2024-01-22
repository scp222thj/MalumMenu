using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class RestorePlayer_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to pick a player outfit to restore
    public static bool isActive;

    // preset locations for each map where the player is restored to, usually spawn on the map
    public static Vector2 GetRestoreLocation()
    {
        if (Utils.isLobby)
        {
            return new Vector2(-0.2f, 1.3f);
        }
        else
        {
            return Utils.getCurrentMapID() switch
            { // from: https://github.com/0xDrMoe/TownofHost-Enhanced/blob/main/Patches/RandomSpawnPatch.cs#L87
                0 => new Vector2 (-1.0f, 3.0f), // Skeld
                1 => new Vector2 (-4.5f, 2.0f), // Mira
                2 => new Vector2 (16.7f, -3.0f), // Polus
                4 => new Vector2 (15.5f, 0.0f), // Airship (main hall)
                5 => new Vector2(-9.8f, 3.4f), // Fungle
                _ => throw new System.NotImplementedException(),
            };
        };
    }

    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.restorePlayer)
        {
            if (!isActive)
            {

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("restorePlayer");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    playerDataList.Add(player.Data);
                }

                // New player pick menu made for exiling
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    Utils.TeleportPlayer(Utils_PlayerPickMenu.targetPlayerData.Object, GetRestoreLocation());
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.restorePlayer = false;
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