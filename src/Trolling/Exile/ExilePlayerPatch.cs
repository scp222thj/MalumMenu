using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ExilePlayer_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to pick a player outfit to exile
    public static bool isActive;
    // preset locations for each map where the player is exiled to
    public static Vector2 GetExileLocation()
    {
        if (Utils.isLobby)
        {
            return new Vector2(-30f, -30f);
        }
        else
        {
            return Utils.getCurrentMapID() switch
            {
                0 => new Vector2(-27f, 3.3f), // Skeld
                1 => new Vector2(-11.4f, 8.2f), // Mira
                2 => new Vector2(42.6f, -19.9f), // Polus
                4 => new Vector2(-16.8f, -6.2f), // Airship
                5 => new Vector2(9.6f, 23.2f), // Fungle
                _ => throw new System.NotImplementedException(),
            };
        };
    }

    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.exilePlayer)
        {
            if (!isActive)
            {

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("exilePlayer");
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
                    Utils.TeleportPlayer(Utils_PlayerPickMenu.targetPlayerData.Object, GetExileLocation());
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.exilePlayer = false;
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