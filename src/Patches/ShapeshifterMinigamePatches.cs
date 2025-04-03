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
        if (!PlayerPickMenu.IsActive)
            return true; // Open normal shapeshifter menu if not active

        // Use custom player list from PlayerPickMenu
        List<NetworkedPlayerInfo> list = PlayerPickMenu.customPlayerList;
        __instance.potentialVictims = new List<ShapeshifterPanel>();
        List<UiElement> buttons = new List<UiElement>();

        for (int i = 0; i < list.Count; i++)
        {
            NetworkedPlayerInfo playerData = list[i];
            int col = i % 3;
            int row = i / 3;

            ShapeshifterPanel panel = Object.Instantiate(__instance.PanelPrefab, __instance.transform);
            panel.transform.localPosition = new Vector3(__instance.XStart + col * __instance.XOffset, __instance.YStart + row * __instance.YOffset, -1f);

            panel.SetPlayer(i, playerData, (Il2CppSystem.Action)(() =>
            {
                PlayerPickMenu.targetPlayerData = playerData;
                PlayerPickMenu.customAction.Invoke();
                __instance.Close();
            }));

            if (playerData.Object != null)
            {
                panel.NameText.text = Utils.getNameTag(playerData, playerData.DefaultOutfit.PlayerName);
            }

            __instance.potentialVictims.Add(panel);
            buttons.Add(panel.Button);
        }

        ControllerManager.Instance.OpenOverlayMenu(
            __instance.name,
            __instance.BackButton,
            __instance.DefaultButtonSelected,
            buttons,
            false
        );

        PlayerPickMenu.IsActive = false;
        return false; // Skip original method when using PPM
    }
}

[HarmonyPatch(typeof(ShapeshifterPanel), nameof(ShapeshifterPanel.SetPlayer))]
public static class ShapeshifterPanel_SetPlayer
{
    // Prefix patch of ShapeshifterPanel.SetPlayer to allow usage of PlayerPickMenu in lobbies
    public static bool Prefix(ShapeshifterPanel __instance, int index, NetworkedPlayerInfo playerInfo, Il2CppSystem.Action onShift)
    {
        if (!PlayerPickMenu.IsActive)
            return true; // Use original if not in PPM

        __instance.shapeshift = onShift;
        __instance.PlayerIcon.SetFlipX(false);
        __instance.PlayerIcon.ToggleName(false);

        foreach (var renderer in __instance.GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.material.SetInt(PlayerMaterial.MaskLayer, index + 2);
        }

        __instance.PlayerIcon.SetMaskLayer(index + 2);
        __instance.PlayerIcon.UpdateFromEitherPlayerDataOrCache(
            playerInfo,
            PlayerOutfitType.Default,
            PlayerMaterial.MaskType.ComplexUI,
            false,
            null
        );

        __instance.LevelNumberText.text = ProgressionManager.FormatVisualLevel(playerInfo.PlayerLevel);
        __instance.NameText.text = playerInfo.PlayerName;

        DataManager.Settings.Accessibility.OnColorBlindModeChanged += (Il2CppSystem.Action)__instance.SetColorblindText;
        __instance.SetColorblindText();

        return false; // Skip original method when active
    }
}
