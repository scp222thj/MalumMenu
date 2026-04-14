using System;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(ShapeshifterMinigame), "Begin")]
public static class ShapeshifterMinigame_Begin
{
	public static bool Prefix(ShapeshifterMinigame __instance)
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		if (!PlayerPickMenu.IsActive)
		{
			return true;
		}
		List<NetworkedPlayerInfo> customPlayerList = PlayerPickMenu.customPlayerList;
		__instance.potentialVictims = new List<ShapeshifterPanel>();
		List<UiElement> val = new List<UiElement>();
		for (int i = 0; i < customPlayerList.Count; i++)
		{
			NetworkedPlayerInfo playerData = customPlayerList[i];
			int num = i % 3;
			int num2 = i / 3;
			ShapeshifterPanel val2 = Object.Instantiate<ShapeshifterPanel>(__instance.PanelPrefab, ((Component)__instance).transform);
			((Component)val2).transform.localPosition = new Vector3(__instance.XStart + (float)num * __instance.XOffset, __instance.YStart + (float)num2 * __instance.YOffset, -1f);
			val2.SetPlayer(i, playerData, Action.op_Implicit((Action)delegate
			{
				PlayerPickMenu.targetPlayerData = playerData;
				PlayerPickMenu.customAction.Invoke();
				((Minigame)__instance).Close();
			}));
			if ((Object)(object)playerData.Object != (Object)null)
			{
				((TMP_Text)val2.NameText).text = Utils.GetNameTag(playerData, playerData.DefaultOutfit.PlayerName);
				if (CheatToggles.seeRoles && CheatToggles.showPlayerInfo)
				{
					val2.NameText.transform.localPosition = new Vector3(0.33f, 0.08f, 0f);
					val2.NameText.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
				}
				else if (CheatToggles.seeRoles || CheatToggles.showPlayerInfo)
				{
					val2.NameText.transform.localPosition = new Vector3(0.3384f, 0.1125f, -0.1f);
					val2.NameText.transform.localScale = new Vector3(0.9f, 1f, 1f);
				}
				else
				{
					val2.NameText.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
					val2.NameText.transform.localScale = new Vector3(0.9f, 1f, 1f);
				}
			}
			__instance.potentialVictims.Add(val2);
			val.Add((UiElement)(object)val2.Button);
		}
		ControllerManager.Instance.OpenOverlayMenu(((Object)__instance).name, __instance.BackButton, __instance.DefaultButtonSelected, val, false);
		PlayerPickMenu.IsActive = false;
		return false;
	}
}
