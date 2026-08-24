using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;
public static class MinimapHandler
{
    public static bool minimapActive;
    public static bool sabotageMapActive;
    public static List<HerePoint> herePoints = new List<HerePoint>();
    public static List<HerePoint> herePointsToRemove = new List<HerePoint>();

    public static bool IsCheatEnabled()
    {
        return (CheatToggles.mapCrew || CheatToggles.mapGhosts || CheatToggles.mapImps) && (!sabotageMapActive || CheatToggles.mapSabotage);
    }

    public static void RefreshHerePoints()
    {
        try
        {
            if (!MapBehaviour.Instance || !MapBehaviour.Instance.gameObject.activeInHierarchy)
            {
                DestroyHerePoints();
                minimapActive = false;
                return;
            }

            var cheatEnabled = IsCheatEnabled();
            minimapActive = cheatEnabled;

            if (!cheatEnabled)
            {
                DestroyHerePoints();
                return;
            }

            if (herePoints.Count == 0)
            {
                SpawnHerePoints(MapBehaviour.Instance);
            }

            foreach (var herePoint in herePoints)
            {
                HandleHerePoint(herePoint);
            }

            foreach (var herePoint in herePointsToRemove)
            {
                herePoints.Remove(herePoint);
            }

            herePointsToRemove.Clear();
        }
        catch { }
    }

    public static void DestroyHerePoints()
    {
        try
        {
            herePoints.ForEach(x => UnityEngine.Object.Destroy(x.sprite.gameObject));
            herePoints.Clear();
            herePointsToRemove.Clear();
        }
        catch { }
    }

    public static void SpawnHerePoints(MapBehaviour mapBehaviour)
    {
        try
        {
            DestroyHerePoints();

            var temp = new List<HerePoint>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.AmOwner)
                {
                    var herePoint = UnityEngine.Object.Instantiate(mapBehaviour.HerePoint, mapBehaviour.HerePoint.transform.parent);
                    temp.Add(new HerePoint(player, herePoint));
                }
            }

            herePoints = temp;
        }
        catch { }
    }

    public static void HandleHerePoint(HerePoint herePoint)
    {
        Color herePointColor = new Color();

        try // try-catch to fix issues caused by player disconnection
        {
            herePoint.sprite.gameObject.SetActive(false); // Initally make player icon invisible

            // Crewmate, alive
            if (CheatToggles.mapCrew && !herePoint.player.Data.Role.IsImpostor)
            {
                if (!herePoint.player.Data.IsDead)
                {
                    herePoint.sprite.gameObject.SetActive(true);
                    if (CheatToggles.colorBasedMap)
                    {
                        herePointColor = herePoint.player.Data.Color; // Color-Based Icon
                    }
                    else
                    {
                        herePointColor = herePoint.player.Data.Role.TeamColor; // Role-Based Icon
                    }
                }
            }
            // Impostor, alive
            else if (CheatToggles.mapImps && herePoint.player.Data.Role.IsImpostor)
            {
                if (!herePoint.player.Data.IsDead)
                {
                    herePoint.sprite.gameObject.SetActive(true);
                    if (CheatToggles.colorBasedMap)
                    {
                        herePointColor = herePoint.player.Data.Color; // Color-Based Icon
                    }
                    else
                    {
                        herePointColor = herePoint.player.Data.Role.TeamColor; // Role-Based Icon
                    }
                }
            }
            // Any Role, dead
            if (CheatToggles.mapGhosts && herePoint.player.Data.IsDead)
            {
                herePoint.sprite.gameObject.SetActive(true);
                if (CheatToggles.colorBasedMap)
                {
                    herePointColor = herePoint.player.Data.Color; // Color-Based Icon
                }
                else
                {
                    herePointColor = Palette.White;
                }
            }

            if (herePoint.sprite.gameObject.active)
            {
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
        }
        catch
        {
            // Remove icons that are causing problems
            UnityEngine.Object.Destroy(herePoint.sprite.gameObject);
            herePointsToRemove.Add(herePoint);
        }
    }
}
