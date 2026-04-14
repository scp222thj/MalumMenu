using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(PlayerControl), "Start")]
public static class PlayerControl_StartPatch
{
	public static void Postfix(PlayerControl __instance)
	{
		try
		{
			if (((InnerNetObject)__instance).AmOwner && !Utils.isHost && (Object)(object)PlayerControl.LocalPlayer != (Object)null)
			{
				try
				{
					PlayerControl.LocalPlayer.RpcSendChat("[FABMOD]|REQ|-1|");
					return;
				}
				catch
				{
					return;
				}
			}
		}
		catch
		{
		}
	}
}
