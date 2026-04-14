using System;
using AmongUs.Data;
using AmongUs.Data.Settings;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using TMPro;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(ShapeshifterPanel), "SetPlayer")]
public static class ShapeshifterPanel_SetPlayer
{
	public static bool Prefix(ShapeshifterPanel __instance, int index, NetworkedPlayerInfo playerInfo, Action onShift)
	{
		if (!PlayerPickMenu.IsActive)
		{
			return true;
		}
		__instance.shapeshift = onShift;
		__instance.PlayerIcon.SetFlipX(false);
		__instance.PlayerIcon.ToggleName(false);
		SpriteRenderer[] array = Il2CppArrayBase<SpriteRenderer>.op_Implicit(((Component)__instance).GetComponentsInChildren<SpriteRenderer>());
		for (int i = 0; i < array.Length; i++)
		{
			((Renderer)array[i]).material.SetInt(PlayerMaterial.MaskLayer, index + 2);
		}
		__instance.PlayerIcon.SetMaskLayer(index + 2);
		__instance.PlayerIcon.UpdateFromEitherPlayerDataOrCache(playerInfo, (PlayerOutfitType)0, (MaskType)2, false, (Action)null);
		((TMP_Text)__instance.LevelNumberText).text = ProgressionManager.FormatVisualLevel(playerInfo.PlayerLevel);
		((TMP_Text)__instance.NameText).text = playerInfo.PlayerName;
		AccessibilitySettingsData accessibility = DataManager.Settings.Accessibility;
		accessibility.OnColorBlindModeChanged += Action.op_Implicit((Action)__instance.SetColorblindText);
		__instance.SetColorblindText();
		return false;
	}
}
