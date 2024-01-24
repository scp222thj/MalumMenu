using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ReportBody_PlayerPhysics_LateUpdate_Postfix
{
    // Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to call a meeting by reporting any body
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.reportBody)
        {
            if (!isActive)
            {
                // Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("reportBody");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                // All players are saved to playerList apart from dead ones
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!player.Data.IsDead)
                    {
                        playerDataList.Add(player.Data);
                    }
                }

                // New player pick menu to choose any body (alive or dead) to report
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    Utils.ReportDeadBody(Utils_PlayerPickMenu.targetPlayerData);
                }));

                isActive = true;
            }

            // Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.reportBody = false;
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