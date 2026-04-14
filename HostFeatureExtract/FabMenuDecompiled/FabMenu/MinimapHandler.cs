using System.Collections.Generic;
using UnityEngine;

namespace FabMenu;

public static class MinimapHandler
{
	public static bool minimapActive;

	public static List<HerePoint> herePoints = new List<HerePoint>();

	public static List<HerePoint> herePointsToRemove = new List<HerePoint>();

	public static bool isCheatEnabled()
	{
		if (!CheatToggles.mapCrew && !CheatToggles.mapGhosts)
		{
			return CheatToggles.mapImps;
		}
		return true;
	}

	public static void handleHerePoint(HerePoint herePoint)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		Color val = default(Color);
		try
		{
			((Component)herePoint.sprite).gameObject.SetActive(false);
			if (CheatToggles.mapCrew && !herePoint.player.Data.Role.IsImpostor)
			{
				if (!herePoint.player.Data.IsDead)
				{
					((Component)herePoint.sprite).gameObject.SetActive(true);
					val = ((!CheatToggles.colorBasedMap) ? herePoint.player.Data.Role.TeamColor : herePoint.player.Data.Color);
				}
			}
			else if (CheatToggles.mapImps && herePoint.player.Data.Role.IsImpostor && !herePoint.player.Data.IsDead)
			{
				((Component)herePoint.sprite).gameObject.SetActive(true);
				val = ((!CheatToggles.colorBasedMap) ? herePoint.player.Data.Role.TeamColor : herePoint.player.Data.Color);
			}
			if (CheatToggles.mapGhosts && herePoint.player.Data.IsDead)
			{
				((Component)herePoint.sprite).gameObject.SetActive(true);
				val = ((!CheatToggles.colorBasedMap) ? Palette.White : herePoint.player.Data.Color);
			}
			if (((Component)herePoint.sprite).gameObject.active)
			{
				((Renderer)herePoint.sprite).material.SetColor(PlayerMaterial.BackColor, val);
				((Renderer)herePoint.sprite).material.SetColor(PlayerMaterial.BodyColor, val);
				((Renderer)herePoint.sprite).material.SetColor(PlayerMaterial.VisorColor, Color32.op_Implicit(Palette.VisorColor));
				Vector3 position = ((Component)herePoint.player).transform.position;
				position /= ShipStatus.Instance.MapScale;
				position.x *= Mathf.Sign(((Component)ShipStatus.Instance).transform.localScale.x);
				position.z = -1f;
				((Component)herePoint.sprite).transform.localPosition = position;
			}
		}
		catch
		{
			Object.Destroy((Object)(object)((Component)herePoint.sprite).gameObject);
			herePointsToRemove.Add(herePoint);
		}
	}
}
