using HarmonyLib;
using Hazel;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class ResetOutfit_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to reset the user's outfit to default
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.resetAppearance){

            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected)
            {
                foreach (var item in PlayerControl.AllPlayerControls)
                {
                    //To prevent duplicate colors, removes user's default color from any other player who is wearing it
                    if(item.CurrentOutfit.ColorId == AmongUs.Data.DataManager.Player.Customization.Color){
                        foreach (var recipient in PlayerControl.AllPlayerControls)
                        {
                            MessageWriter itemColorWriter = AmongUsClient.Instance.StartRpcImmediately(item.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient));
                            itemColorWriter.Write(PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId);
                            AmongUsClient.Instance.FinishRpcImmediately(itemColorWriter);
                        }
                    }

                    //Reset default color
                    MessageWriter colorWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                    colorWriter.Write(AmongUs.Data.DataManager.Player.Customization.Color);
                    AmongUsClient.Instance.FinishRpcImmediately(colorWriter);

                    //Reset default player name
                    MessageWriter nameWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetName, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                    nameWriter.Write(AmongUs.Data.DataManager.Player.Customization.Name);
                    AmongUsClient.Instance.FinishRpcImmediately(nameWriter);

                    //Reset default hat
                    MessageWriter hatWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetHatStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                    hatWriter.Write(AmongUs.Data.DataManager.Player.Customization.Hat);
                    AmongUsClient.Instance.FinishRpcImmediately(hatWriter);

                    //Reset default pet
                    MessageWriter petWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetPetStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                    petWriter.Write(AmongUs.Data.DataManager.Player.Customization.Pet);
                    AmongUsClient.Instance.FinishRpcImmediately(petWriter);

                    //Reset default visor
                    MessageWriter visorWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetVisorStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                    visorWriter.Write(AmongUs.Data.DataManager.Player.Customization.Visor);
                    AmongUsClient.Instance.FinishRpcImmediately(visorWriter);

                    //Reset default skin
                    MessageWriter skinWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetSkinStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                    skinWriter.Write(AmongUs.Data.DataManager.Player.Customization.Skin);
                    AmongUsClient.Instance.FinishRpcImmediately(skinWriter);
                }
            }

            CheatToggles.resetAppearance = false;

        }
    }
}