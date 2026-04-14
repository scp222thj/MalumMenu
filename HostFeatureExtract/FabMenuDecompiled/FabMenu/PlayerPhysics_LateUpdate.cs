using System;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(PlayerPhysics), "LateUpdate")]
public static class PlayerPhysics_LateUpdate
{
	public static void Postfix(PlayerPhysics __instance)
	{
		MalumESP.PlayerNametags(__instance);
		MalumESP.seeGhostsCheat(__instance);
		MalumCheats.noClipCheat();
		MalumCheats.ReviveCheat();
		MalumCheats.killAllCheat();
		MalumCheats.killAllCrewCheat();
		MalumCheats.spamKillAllCheat();
		MalumCheats.killAllImpsCheat();
		MalumCheats.forceStartGameCheat();
		MalumCheats.teleportCursorCheat();
		MalumCheats.completeMyTasksCheat();
		MalumCheats.AnimationCheat();
		MalumCheats.ScanCheat();
		MalumPPMCheats.ejectPlayerPPM();
		MalumPPMCheats.spectatePPM();
		MalumPPMCheats.killPlayerPPM();
		MalumPPMCheats.telekillPlayerPPM();
		MalumPPMCheats.teleportPlayerPPM();
		MalumPPMCheats.ProtectPlayerPPM();
		MalumPPMCheats.changeRolePPM();
		MalumPPMCheats.modifyPlayerColorPPM();
		TracersHandler.drawPlayerTracer(__instance);
		GameObject[] array = Il2CppArrayBase<GameObject>.op_Implicit((Il2CppArrayBase<GameObject>)(object)GameObject.FindGameObjectsWithTag("DeadBody"));
		for (int i = 0; i < array.Length; i++)
		{
			DeadBody component = array[i].GetComponent<DeadBody>();
			if (Object.op_Implicit((Object)(object)component) && !component.Reported)
			{
				TracersHandler.drawBodyTracer(component);
			}
		}
		try
		{
			if (CheatToggles.invertControls)
			{
				PlayerControl.LocalPlayer.MyPhysics.Speed = 0f - Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.Speed);
				PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = 0f - Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed);
			}
			else
			{
				PlayerControl.LocalPlayer.MyPhysics.Speed = Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.Speed);
				PlayerControl.LocalPlayer.MyPhysics.GhostSpeed = Mathf.Abs(PlayerControl.LocalPlayer.MyPhysics.GhostSpeed);
			}
		}
		catch (NullReferenceException)
		{
		}
	}
}
