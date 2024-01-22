using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ShapeshiftPlayer_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to shapeshift into any player
    public static bool isActive;
    public static PlayerControl target = null;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.shapeshiftPlayer)
        {
            if (!isActive)
            {
                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("shapeshiftPlayer");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                // All players are saved to playerList, including the localplayer
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    playerDataList.Add(player.Data);
                }

                // Two consecutive player pick menus, first for the shifter, second for the target of the shifter
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                {
                    target = Utils_PlayerPickMenu.targetPlayerData.Object;
                    
                    var HostData = AmongUsClient.Instance.GetHost();
                    if (HostData != null && !HostData.Character.Data.Disconnected)
                    {
                        if (CheatToggles.extraOptions){
                            Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action)(() =>
                            {
                                Utils.ShapeshiftPlayer(target, Utils_PlayerPickMenu.targetPlayerData.Object, true);
                                
                                target = null;
                                CheatToggles.shapeshiftPlayer = false;
                            }));
                        }else{
                            Utils.ShapeshiftPlayer(PlayerControl.LocalPlayer, target, !CheatToggles.noShapeshiftAnim);
                        }
                    }

                    target = null;
                    CheatToggles.shapeshiftPlayer = false;
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.shapeshiftPlayer = false;
            }
        }
        else if (isActive)
        {
            isActive = false;
            target = null;
        }
    }
}