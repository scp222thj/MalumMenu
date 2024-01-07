using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Whisper_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to pick a target for whisper
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.whisper)
        {
            //Open player pick menu when CheatSettings.whisper is first enabled
            if (!isActive)
            {
                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null)
                {
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("whisper");
                }

                List<PlayerControl> playerList = new List<PlayerControl>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    playerList.Add(player);
                }

                //New player pick menu made for picking a whisper target
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() =>
                {
                    CheatToggles.enableCommands = true;
                    Utils.OpenChat();
                    HudManager.Instance.Chat.freeChatField.textArea.SetText("/whisper '" + Utils_PlayerPickMenu.targetPlayer.Data.DefaultOutfit.PlayerName + "' ");
                }));

                isActive = true;

            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null)
            {
                CheatToggles.whisper = false;
            }
        }
        else
        {
            //Deactivate cheat when it is disabled from the GUI
            if (isActive)
            {
                isActive = false;
            }
        }
    }
}