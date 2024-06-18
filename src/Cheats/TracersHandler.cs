using UnityEngine;

namespace MalumMenu;
public static class TracersHandler
{
    public static void drawPlayerTracer(PlayerPhysics playerPhysics){
        try{

            Color color = Color.clear; // All tracers are invisible by default

            if (!playerPhysics.myPlayer.Data.IsDead){
                if (CheatToggles.tracersCrew && !playerPhysics.myPlayer.Data.Role.IsImpostor){
                    if (CheatToggles.colorBasedTracers){
                        color = playerPhysics.myPlayer.Data.Color; // Color-Based Tracer
                    }else{
                        color = playerPhysics.myPlayer.Data.Role.TeamColor; // Team-Based Tracer
                    }
                }else if (CheatToggles.tracersImps && playerPhysics.myPlayer.Data.Role.IsImpostor){
                    if (CheatToggles.colorBasedTracers){
                        color = playerPhysics.myPlayer.Data.Color; // Color-Based Tracer
                    }else{
                        color = playerPhysics.myPlayer.Data.Role.TeamColor; // Team-Based Tracer
                    }
                }
            }else{
                if (CheatToggles.tracersGhosts){
                    if (CheatToggles.colorBasedTracers){
                        color = playerPhysics.myPlayer.Data.Color; // Color-Based Tracer
                    }else{
                        color = Palette.White; // Ghost Tracer (White)
                    }
                }
            }

            // Draw tracer between the player and LocalPlayer using the right color
            Utils.drawTracer(playerPhysics.myPlayer.gameObject, PlayerControl.LocalPlayer.gameObject, color);

        }catch{}
    }

    public static void drawBodyTracer(DeadBody deadBody){
        Color color = Color.clear; // All tracers are invisible by default

        if (CheatToggles.tracersBodies){
            if (CheatToggles.colorBasedTracers){

                // Fetch the dead body's PlayerInfo
                NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(deadBody.ParentId);

                color = playerById.Color; // Color-Based Tracer

            }else{

                color = Color.yellow; // Dead Body Tracer (Yellow)

            }
        }

        // Draw tracer between the dead body and LocalPlayer using the right color
        Utils.drawTracer(deadBody.gameObject, PlayerControl.LocalPlayer.gameObject, color);
    }
}