using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class SwapOutfit_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to pick a player outfit to swap
    public static bool isActive;
    public static PlayerControl firstTarget = null;

    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.swapOutfit)
        {
            if (!isActive)
            {
                // Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("swapOutfit");
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

                // Two consecutive player pick menus for both targets
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    firstTarget = Utils_PlayerPickMenu.targetPlayerData.Object;

                    Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                    {
                        var HostData = AmongUsClient.Instance.GetHost();
                        if (HostData != null && !HostData.Character.Data.Disconnected)
                        {
                            Utils.CopyOutfit(firstTarget, Utils_PlayerPickMenu.targetPlayerData.Object);
                            Utils.CopyOutfit(Utils_PlayerPickMenu.targetPlayerData.Object, firstTarget);
                            firstTarget = null;
                            CheatToggles.swapOutfit = false;
                        }
                    }));
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.swapOutfit = false;
                firstTarget = null;
            }

        }
        else if (isActive)
        {
            isActive = false;
            firstTarget = null;
        }
    }
}