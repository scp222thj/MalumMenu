using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

public static class MinimapHandler
{
    public static bool minimapActive;
    public static List<HerePoint> herePoints = new();
    public static List<HerePoint> herePointsToRemove = new();

    public static bool isCheatEnabled()
    {
        return CheatToggles.mapCrew || CheatToggles.mapGhosts || CheatToggles.mapImps;
    }

    public static void handleHerePoint(HerePoint herePoint)
    {
        if (herePoint?.sprite == null || herePoint.player?.Data == null)
        {
            return;
        }

        try
        {
            Color herePointColor = Color.clear;
            var playerData = herePoint.player.Data;
            var sprite = herePoint.sprite;
            sprite.gameObject.SetActive(false); // Initially hidden

            bool isImpostor = playerData.Role.IsImpostor;
            bool isDead = playerData.IsDead;

            // Alive Crewmate
            if (CheatToggles.mapCrew && !isImpostor && !isDead)
            {
                sprite.gameObject.SetActive(true);
                herePointColor = CheatToggles.colorBasedMap ? playerData.Color : playerData.Role.TeamColor;
            }
            // Alive Impostor
            else if (CheatToggles.mapImps && isImpostor && !isDead)
            {
                sprite.gameObject.SetActive(true);
                herePointColor = CheatToggles.colorBasedMap ? playerData.Color : playerData.Role.TeamColor;
            }
            // Any Dead Role
            else if (CheatToggles.mapGhosts && isDead)
            {
                sprite.gameObject.SetActive(true);
                herePointColor = CheatToggles.colorBasedMap ? playerData.Color : Palette.White;
            }

            if (sprite.gameObject.activeSelf)
            {
                // Update sprite color
                sprite.material.SetColor(PlayerMaterial.BackColor, herePointColor);
                sprite.material.SetColor(PlayerMaterial.BodyColor, herePointColor);
                sprite.material.SetColor(PlayerMaterial.VisorColor, Palette.VisorColor);

                // Update sprite position relative to map scale
                Vector3 position = herePoint.player.transform.position;
                position /= ShipStatus.Instance.MapScale;
                position.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                position.z = -1f;

                sprite.transform.localPosition = position;
            }
        }
        catch
        {
            // Clean up bugged icons
            if (herePoint?.sprite?.gameObject != null)
            {
                Object.Destroy(herePoint.sprite.gameObject);
            }

            if (herePoint != null)
            {
                herePointsToRemove.Add(herePoint);
            }
        }
    }
}
