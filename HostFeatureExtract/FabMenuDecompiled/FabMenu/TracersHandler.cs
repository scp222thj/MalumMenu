using UnityEngine;

namespace FabMenu;

public static class TracersHandler
{
	public static void drawPlayerTracer(PlayerPhysics playerPhysics)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Color color = Color.clear;
			if (!playerPhysics.myPlayer.Data.IsDead)
			{
				if (CheatToggles.tracersCrew && !playerPhysics.myPlayer.Data.Role.IsImpostor)
				{
					color = ((!CheatToggles.colorBasedTracers) ? playerPhysics.myPlayer.Data.Role.TeamColor : playerPhysics.myPlayer.Data.Color);
				}
				else if (CheatToggles.tracersImps && playerPhysics.myPlayer.Data.Role.IsImpostor)
				{
					color = ((!CheatToggles.colorBasedTracers) ? playerPhysics.myPlayer.Data.Role.TeamColor : playerPhysics.myPlayer.Data.Color);
				}
			}
			else if (CheatToggles.tracersGhosts)
			{
				color = ((!CheatToggles.colorBasedTracers) ? Palette.White : playerPhysics.myPlayer.Data.Color);
			}
			Utils.drawTracer(((Component)playerPhysics.myPlayer).gameObject, ((Component)PlayerControl.LocalPlayer).gameObject, color);
		}
		catch
		{
		}
	}

	public static void drawBodyTracer(DeadBody deadBody)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Color color = Color.clear;
		if (CheatToggles.tracersBodies)
		{
			color = ((!CheatToggles.colorBasedTracers) ? Color.yellow : GameData.Instance.GetPlayerById(deadBody.ParentId).Color);
		}
		Utils.drawTracer(((Component)deadBody).gameObject, ((Component)PlayerControl.LocalPlayer).gameObject, color);
	}
}
