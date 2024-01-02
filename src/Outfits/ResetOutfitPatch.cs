using HarmonyLib;
using Hazel;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class RPC_ResetOutfitPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to murder any player
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.resetOutfit){

            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected){
                foreach (var sender in PlayerControl.AllPlayerControls)
                {
                    if(sender.CurrentOutfit.ColorId == AmongUs.Data.DataManager.Player.Customization.Color){
                        foreach (var recipient in PlayerControl.AllPlayerControls)
                        {
                            MessageWriter senderColorWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient));
                            senderColorWriter.Write(PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId);
                            AmongUsClient.Instance.FinishRpcImmediately(senderColorWriter);
                        }
                    }

                    MessageWriter colorWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(sender));
                    colorWriter.Write(AmongUs.Data.DataManager.Player.Customization.Color);
                    AmongUsClient.Instance.FinishRpcImmediately(colorWriter);

                    if(sender.CurrentOutfit.PlayerName == AmongUs.Data.DataManager.Player.Customization.Name){
                        foreach (var recipient in PlayerControl.AllPlayerControls)
                        {
                            MessageWriter senderColorWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient));
                            senderColorWriter.Write(PlayerControl.LocalPlayer.Data.DefaultOutfit.PlayerName);
                            AmongUsClient.Instance.FinishRpcImmediately(senderColorWriter);
                        }
                    }

                    MessageWriter nameWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(sender));
                    nameWriter.Write(AmongUs.Data.DataManager.Player.Customization.Name);
                    AmongUsClient.Instance.FinishRpcImmediately(nameWriter);

                    MessageWriter hatWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetHatStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(sender));
                    hatWriter.Write(AmongUs.Data.DataManager.Player.Customization.Hat);
                    AmongUsClient.Instance.FinishRpcImmediately(hatWriter);

                    MessageWriter petWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetPetStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(sender));
                    petWriter.Write(AmongUs.Data.DataManager.Player.Customization.Pet);
                    AmongUsClient.Instance.FinishRpcImmediately(petWriter);

                    MessageWriter visorWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetVisorStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(sender));
                    visorWriter.Write(AmongUs.Data.DataManager.Player.Customization.Visor);
                    AmongUsClient.Instance.FinishRpcImmediately(visorWriter);

                    MessageWriter skinWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.SetSkinStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(sender));
                    skinWriter.Write(AmongUs.Data.DataManager.Player.Customization.Skin);
                    AmongUsClient.Instance.FinishRpcImmediately(skinWriter);
                }
            }

            CheatSettings.resetOutfit = false;

        }
    }
}