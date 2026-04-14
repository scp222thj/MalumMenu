using System;
using System.Text;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace FabMenu
{
    public enum ModificationMode
    {
        None,
        Name,
        Color
    }

    public static class PlayerModifier
    {
        private static ModificationMode currentMode = ModificationMode.None;
        private static NetworkedPlayerInfo selectedPlayer;
        private static string newPlayerName = string.Empty;
        private static int newColorId;

        public static ModificationMode CurrentMode => currentMode;
        public static bool IsModifying => currentMode != ModificationMode.None;
        public static NetworkedPlayerInfo SelectedPlayer => selectedPlayer;
        public static string NewPlayerName => newPlayerName;
        public static int NewColorId => newColorId;

        public static void EnterNameModificationMode()
        {
            currentMode = ModificationMode.Name;
            selectedPlayer = null;
            newPlayerName = string.Empty;
        }

        public static void EnterColorModificationMode()
        {
            currentMode = ModificationMode.Color;
            selectedPlayer = null;
            newColorId = 0;
        }

        public static void ExitModificationMode()
        {
            currentMode = ModificationMode.None;
            selectedPlayer = null;
            newPlayerName = string.Empty;
            newColorId = 0;
        }

        public static void SelectPlayer(NetworkedPlayerInfo playerData)
        {
            selectedPlayer = playerData;
            if (selectedPlayer == null)
            {
                return;
            }

            if (currentMode == ModificationMode.Name)
            {
                newPlayerName = playerData.PlayerName;
            }
            else if (currentMode == ModificationMode.Color)
            {
                newColorId = playerData.DefaultOutfit.ColorId;
            }
        }

        public static void ApplyNameChange()
        {
            if (selectedPlayer == null || string.IsNullOrEmpty(newPlayerName))
            {
                return;
            }

            string text = newPlayerName;
            if (text.Length > 10)
            {
                text = text.Substring(0, 10);
            }

            if (selectedPlayer.Object != null)
            {
                try
                {
                    selectedPlayer.Object.cosmetics.SetName(text);
                }
                catch
                {
                }
            }

            try
            {
                if (Utils.isLocalGame || Utils.isFreePlay)
                {
                    string value = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
                    string message = $"[FABMOD]|NAME|{selectedPlayer.PlayerId}|{value}";
                    PlayerControl.LocalPlayer.RpcSendChat(message);
                }
            }
            catch
            {
            }

            selectedPlayer = null;
            newPlayerName = string.Empty;
        }

        public static void ApplyColorChange()
        {
            if (selectedPlayer == null)
            {
                return;
            }

            int colorIndex = Mathf.Clamp(newColorId, 0, ((Il2CppArrayBase<Color32>)(object)Palette.PlayerColors).Length - 1);
            if (selectedPlayer.Object != null)
            {
                try
                {
                    selectedPlayer.Object.SetColor(colorIndex);
                }
                catch
                {
                }
            }

            try
            {
                if (Utils.isLocalGame || Utils.isFreePlay)
                {
                    string message = $"[FABMOD]|COLOR|{selectedPlayer.PlayerId}|{colorIndex}";
                    PlayerControl.LocalPlayer.RpcSendChat(message);
                }
            }
            catch
            {
            }

            selectedPlayer = null;
            newColorId = 0;
        }

        public static void ChangePlayerName(NetworkedPlayerInfo playerData, string newName)
        {
            if (!Utils.isHost || playerData == null || string.IsNullOrEmpty(newName))
            {
                return;
            }

            if (newName.Length > 10)
            {
                newName = newName.Substring(0, 10);
            }

            PlayerOutfit defaultOutfit = playerData.DefaultOutfit;
            defaultOutfit.PlayerName = newName;
            playerData.Outfits[(PlayerOutfitType)0] = defaultOutfit;

            if (playerData.Object != null)
            {
                playerData.Object.cosmetics.SetName(newName);
            }
        }

        public static void ChangePlayerColor(NetworkedPlayerInfo playerData, int colorId)
        {
            if (!Utils.isHost || playerData == null)
            {
                return;
            }

            colorId = Mathf.Clamp(colorId, 0, ((Il2CppArrayBase<Color32>)(object)Palette.PlayerColors).Length - 1);
            PlayerOutfit defaultOutfit = playerData.DefaultOutfit;
            defaultOutfit.ColorId = colorId;
            playerData.Outfits[(PlayerOutfitType)0] = defaultOutfit;

            if (playerData.Object != null)
            {
                playerData.Object.SetColor(colorId);
            }
        }

        public static void UpdateNewName(string name)
        {
            newPlayerName = name;
        }

        public static void UpdateNewColorId(int colorId)
        {
            newColorId = colorId;
        }
    }
}
