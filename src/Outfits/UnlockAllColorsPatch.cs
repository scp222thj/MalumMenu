using HarmonyLib;
using Hazel;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.UpdateAvailableColors))]
public static class Passive_UpdateAvailableColorsPrefix
{
    //Prefix patch of EOSManager.IsFreechatAllowed to unlock freechat
    public static bool Prefix(PlayerTab __instance)
    {
        if (!CheatSettings.unlockColors){
            return true; //Only works if CheatSettings.unlockFeatures is enabled
        }

        for (int i = 0; i < Palette.PlayerColors.Length; i++)
        {
            __instance.AvailableColors.Add(i);
        }

        return false;
    }
}

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.ClickEquip))]
public static class Passive_ClickEquipPrefix
{
    //Prefix patch of EOSManager.IsFreechatAllowed to unlock freechat
    public static bool Prefix(PlayerTab __instance)
    {
        if (!CheatSettings.unlockColors){
            return true; //Only works if CheatSettings.unlockFeatures is enabled
        }

        AmongUs.Data.DataManager.Player.Customization.Color = (byte)__instance.currentColor;

        if (__instance.HasLocalPlayer())
        {
            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected){
                foreach (var sender in PlayerControl.AllPlayerControls)
                {
                    if(sender.CurrentOutfit.ColorId == (byte)__instance.currentColor){
                        foreach (var recipient in PlayerControl.AllPlayerControls)
                        {
                            MessageWriter senderColorWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient));
                            senderColorWriter.Write(PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId);
                            AmongUsClient.Instance.FinishRpcImmediately(senderColorWriter);
                        }
                    }

                    MessageWriter colorWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(sender));
                    colorWriter.Write((byte)__instance.currentColor);
                    AmongUsClient.Instance.FinishRpcImmediately(colorWriter);
                }
            }
        }

        string colorName = Palette.GetColorName(__instance.currentColor);
        Logger.GlobalInstance.Info("PlayerCustomizationMenu::Set player color " + colorName, null);
        
        return false;
    }
}