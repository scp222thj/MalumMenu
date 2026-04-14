using System;
using System.Globalization;
using System.Linq;
using System.Text;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace FabMenu
{
    [HarmonyPatch(typeof(ChatController), "AddChat")]
    public static class ChatController_AddChat
    {
        public static bool Prefix(PlayerControl sourcePlayer, string chatText, bool censor, ChatController __instance)
        {
            if (string.IsNullOrEmpty(chatText) || !chatText.StartsWith("[FABMOD]"))
            {
                return true;
            }

            try
            {
                string[] parts = chatText.Split('|');
                if (parts.Length < 4)
                {
                    return false;
                }

                string messageType = parts[1];
                if (!int.TryParse(parts[2], out int targetId))
                {
                    return false;
                }

                string payload = parts[3];
                PlayerControl targetPlayer = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == targetId);
                if (targetPlayer == null || targetPlayer.Data == null || targetPlayer.Data.Disconnected)
                {
                    return false;
                }

                if (messageType == "NAME")
                {
                    try
                    {
                        string decodedName = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
                        if (decodedName.Length > 10)
                        {
                            decodedName = decodedName.Substring(0, 10);
                        }

                        PlayerOutfit defaultOutfit = targetPlayer.Data.DefaultOutfit;
                        defaultOutfit.PlayerName = decodedName;
                        targetPlayer.Data.Outfits[(PlayerOutfitType)0] = defaultOutfit;

                        if (targetPlayer != null && targetPlayer.cosmetics != null)
                        {
                            targetPlayer.cosmetics.SetName(decodedName);
                        }
                    }
                    catch
                    {
                    }
                }
                else if (messageType == "COLOR")
                {
                    try
                    {
                        if (int.TryParse(payload, out int colorId))
                        {
                            colorId = Mathf.Clamp(colorId, 0, ((Il2CppArrayBase<Color32>)(object)Palette.PlayerColors).Length - 1);
                            PlayerOutfit defaultOutfit = targetPlayer.Data.DefaultOutfit;
                            defaultOutfit.ColorId = colorId;
                            targetPlayer.Data.Outfits[(PlayerOutfitType)0] = defaultOutfit;

                            if (targetPlayer != null)
                            {
                                targetPlayer.SetColor(colorId);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            return false;
        }
    }
}
