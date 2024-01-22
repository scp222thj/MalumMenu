using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ExileAll_PlayerPhysics_LateUpdate_Postfix
{
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
        if (CheatToggles.exileAll)
        {
            List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

            //All players are saved to playerList
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Utils.TeleportPlayer(player, GetExileLocation());
            };

            CheatToggles.exileAll = false;
        }
    }
}