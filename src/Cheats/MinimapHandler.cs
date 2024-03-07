using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;
public static class MinimapHandler
{
    public static bool minimapActive;
    public static List<HerePoint> herePoints = new List<HerePoint>();
    public static List<HerePoint> herePointsToRemove = new List<HerePoint>();
    
    public static bool isCheatEnabled(){
        return CheatToggles.mapCrew || CheatToggles.mapGhosts || CheatToggles.mapImps;
    }

    public static void handleHerePoint(HerePoint herePoint){

        Color herePointColor = new Color();

        try{ // try-catch to fix issues caused by player disconnection

            herePoint.sprite.gameObject.SetActive(false); // Initally make player icon invisible

            // Crewmate, alive
            if (CheatToggles.mapCrew && !herePoint.player.Data.Role.IsImpostor){
                if (!herePoint.player.Data.IsDead){
                    herePoint.sprite.gameObject.SetActive(true);
                    if (CheatToggles.colorBasedMap){
                        herePointColor = herePoint.player.Data.Color; // Color-Based Icon
                    }else{
                        herePointColor = herePoint.player.Data.Role.TeamColor; // Role-Based Icon
                    }
                }

            // Impostor, alive
            } else if (CheatToggles.mapImps && herePoint.player.Data.Role.IsImpostor){
                if (!herePoint.player.Data.IsDead){
                    herePoint.sprite.gameObject.SetActive(true);
                    if (CheatToggles.colorBasedMap){
                        herePointColor = herePoint.player.Data.Color; // Color-Based Icon
                    }else{
                        herePointColor = herePoint.player.Data.Role.TeamColor; // Role-Based Icon
                    }
                }
            }

            // Any Role, dead
            if (CheatToggles.mapGhosts && herePoint.player.Data.IsDead){
                herePoint.sprite.gameObject.SetActive(true);
                if (CheatToggles.colorBasedMap){
                    herePointColor = herePoint.player.Data.Color; // Color-Based Icon
                }else{
                    herePointColor = Palette.White;
                }
            }
        

            if (herePoint.sprite.gameObject.active){

                // Set the right colors for active herePoint icons
                herePoint.sprite.material.SetColor(PlayerMaterial.BackColor, herePointColor);
                herePoint.sprite.material.SetColor(PlayerMaterial.BodyColor, herePointColor);
                herePoint.sprite.material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);	

                // Sync the position of active herePoint icons with their players
                var vector = herePoint.player.transform.position;
                vector /= ShipStatus.Instance.MapScale;
                vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                vector.z = -1f;
                herePoint.sprite.transform.localPosition = vector;
            }

        }catch{

            // Remove icons that are causing problems
            Object.Destroy(herePoint.sprite.gameObject);
            herePointsToRemove.Add(herePoint);

        }
    }
}