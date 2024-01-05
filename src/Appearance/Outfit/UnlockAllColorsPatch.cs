using HarmonyLib;
using Hazel;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.UpdateAvailableColors))]
public static class UnlockAllColors_PlayerTab_UpdateAvailableColors_Prefix
{
    //Prefix patch of PlayerTab.UpdateAvailableColors to always display all the colors as available
    public static bool Prefix(PlayerTab __instance)
    {
        if (!CheatToggles.unlockColors){
            return true; //Only works if CheatSettings.unlockColors is enabled
        }

        for (int i = 0; i < Palette.PlayerColors.Length; i++)
        {
            __instance.AvailableColors.Add(i);
        }

        return false;
    }
}

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.ClickEquip))]
public static class UnlockAllColors_PlayerTab_ClickEquip_Prefix
{
    //Prefix patch of PlayerTab.ClickEquip to equip any color regardless of its availability
    public static bool Prefix(PlayerTab __instance)
    {
        if (!CheatToggles.unlockColors){
            return true; //Only works if CheatSettings.unlockColors is enabled
        }

        //Save the color in the user's customization settings
        AmongUs.Data.DataManager.Player.Customization.Color = (byte)__instance.currentColor;

        if (__instance.HasLocalPlayer())
        {
            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected)
            {
                foreach (var sender in PlayerControl.AllPlayerControls)
                {
                    //Change the color of any other player using your chosen color, to avoid color duplicates
                    if(sender.CurrentOutfit.ColorId == (byte)__instance.currentColor){
                        foreach (var recipient in PlayerControl.AllPlayerControls)
                        {
                            MessageWriter senderColorWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient));
                            senderColorWriter.Write(PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId);
                            AmongUsClient.Instance.FinishRpcImmediately(senderColorWriter);
                        }
                    }

                    //Set the color in-game using RPC calls
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