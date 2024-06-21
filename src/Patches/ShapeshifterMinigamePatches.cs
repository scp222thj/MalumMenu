using AmongUs.Data;
using HarmonyLib;
using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace MalumMenu;

[HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
public static class ShapeshifterMinigame_Begin
{
    // Prefix patch of ShapeshifterMinigame.Begin to implement player pick menu logic
    public static bool Prefix(ShapeshifterMinigame __instance)
    {
        if (PlayerPickMenu.IsActive){ // Player Pick Menu logic

            // Custom player list set by openPlayerPickMenu
            List<NetworkedPlayerInfo> list = PlayerPickMenu.customPlayerList;

            __instance.potentialVictims = new List<ShapeshifterPanel>();
            List<UiElement> list2 = new List<UiElement>();

            for (int i = 0; i < list.Count; i++)
            {
                NetworkedPlayerInfo playerData = list[i];
                int num = i % 3;
                int num2 = i / 3;
                ShapeshifterPanel shapeshifterPanel = UnityEngine.Object.Instantiate<ShapeshifterPanel>(__instance.PanelPrefab, __instance.transform);
                shapeshifterPanel.transform.localPosition = new Vector3(__instance.XStart + (float)num * __instance.XOffset, __instance.YStart + (float)num2 * __instance.YOffset, -1f);
                
                shapeshifterPanel.SetPlayer(i, playerData, (Il2CppSystem.Action) (() =>
                {
                    PlayerPickMenu.targetPlayerData = playerData; // Save targeted player

                    PlayerPickMenu.customAction.Invoke(); // Custom action set by openPlayerPickMenu

                    __instance.Close();
                }));

                if (playerData.Object != null){
                    shapeshifterPanel.NameText.text = Utils.getNameTag(playerData, playerData.DefaultOutfit.PlayerName);
                }
                __instance.potentialVictims.Add(shapeshifterPanel);
                list2.Add(shapeshifterPanel.Button);
            }

            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2, false);
            
            PlayerPickMenu.IsActive = false;

            return false; // Skip original method when active

        }

        return true; // Open normal shapeshifter menu if not active
    }
}

[HarmonyPatch(typeof(ShapeshifterPanel), nameof(ShapeshifterPanel.SetPlayer))]
public static class ShapeshifterPanel_SetPlayer
{
    // Prefix patch of ShapeshifterPanel.SetPlayer to allow usage of PlayerPickMenu in lobbies
    public static bool Prefix(ShapeshifterPanel __instance, int index, NetworkedPlayerInfo playerInfo, Il2CppSystem.Action onShift)
    {
        if (PlayerPickMenu.IsActive){ // Player Pick Menu logic

            __instance.shapeshift = onShift;
            __instance.PlayerIcon.SetFlipX(false);
            __instance.PlayerIcon.ToggleName(false);
            SpriteRenderer[] componentsInChildren = __instance.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].material.SetInt(PlayerMaterial.MaskLayer, index + 2);
            }
            __instance.PlayerIcon.SetMaskLayer(index + 2);
            __instance.PlayerIcon.UpdateFromEitherPlayerDataOrCache(playerInfo, PlayerOutfitType.Default, PlayerMaterial.MaskType.ComplexUI, false, null);
            __instance.LevelNumberText.text = ProgressionManager.FormatVisualLevel(playerInfo.PlayerLevel);
            
            // Skips using custom nameplates because they break the PlayerPickMenu in lobbies

            __instance.NameText.text = playerInfo.PlayerName;
            DataManager.Settings.Accessibility.OnColorBlindModeChanged += (Il2CppSystem.Action)__instance.SetColorblindText;
            __instance.SetColorblindText();

            return false; // Skips original method when active

        }

        return true; // Open normal shapeshifter menu if not active
    }
}