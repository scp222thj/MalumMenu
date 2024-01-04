using HarmonyLib;
using Hazel;
using Il2CppSystem.Collections.Generic;
using System;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class MurderIRL {
    // Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to murder any player
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance) {
        if (CheatToggles.murderPlayer) {
            if (!isActive) {
                List<PlayerControl> playerList = new List<PlayerControl>();

                foreach (var player in PlayerControl.AllPlayerControls) playerList.Add(player);
    
                Utils_PlayerPickMenu.openPlayerPickMenu(playerList, (Action) (() => {
                    Utils.MurderPlayerInRealLife(Utils_PlayerPickMenu.targetPlayer, Utils_PlayerPickMenu.targetPlayer);
                }));

                isActive = true;
            };

            if (Utils_PlayerPickMenu.playerpickMenu == null) CheatToggles.murderPlayer = false;
        } else if (isActive){
            isActive = false;
        }
    }
}
