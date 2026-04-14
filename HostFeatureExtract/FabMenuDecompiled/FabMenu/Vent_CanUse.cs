using HarmonyLib;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(Vent), "CanUse")]
public static class Vent_CanUse
{
	public static void Postfix(Vent __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)PlayerControl.LocalPlayer) && Object.op_Implicit((Object)(object)PlayerControl.LocalPlayer.Data) && !PlayerControl.LocalPlayer.Data.Role.CanVent && !PlayerControl.LocalPlayer.Data.IsDead && CheatToggles.useVents)
		{
			PlayerControl val = pc.Object;
			Bounds bounds = val.Collider.bounds;
			Vector3 center = ((Bounds)(ref bounds)).center;
			Vector3 position = ((Component)__instance).transform.position;
			float num = Vector2.Distance(Vector2.op_Implicit(center), Vector2.op_Implicit(position));
			canUse = num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(val.Collider, Vector2.op_Implicit(center), Vector2.op_Implicit(position), Constants.ShipOnlyMask, false);
			couldUse = true;
			__result = num;
		}
	}
}
