using HarmonyLib;
using Hazel;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class MurderAll_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to kill all the players in a game
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.murderAll){

            //Kill all players by sending a successful MurderPlayer RPC call to all clients
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Utils.MurderPlayer(player, MurderResultFlags.Succeeded);
            }

            CheatToggles.murderAll = false;

        }
    }
}