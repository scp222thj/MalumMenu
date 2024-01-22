using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class MimicOutfit_PlayerPhysics_LateUpdate_Postfix
{
    public static bool isActive;
    public static PlayerControl targetPlayer = null;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.mimicOutfit)
        {
            if (!isActive)
            {
                // Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("mimicOutfit");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                // All players are saved to playerList
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    playerDataList.Add(player.Data);
                }

                // Two consecutive player pick menus, first for the mimicker, second for the target of the mimic
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    targetPlayer = Utils_PlayerPickMenu.targetPlayerData.Object;

                    Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                    {
                        var HostData = AmongUsClient.Instance.GetHost();
                        if (HostData != null && !HostData.Character.Data.Disconnected)
                        {
                            Utils.CopyOutfit(targetPlayer, Utils_PlayerPickMenu.targetPlayerData.Object);
                            targetPlayer = null;
                            CheatToggles.mimicOutfit = false;
                        }
                    }));
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.mimicOutfit = false;
                targetPlayer = null;
            }
        }
        else if (isActive)
        {
            isActive = false;
            targetPlayer = null;
        }
    }
}