using HarmonyLib;
using Hazel;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class RPC_ResetShapeshiftPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to murder any player
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.resetShapeshift){

            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected){

                foreach (var sender in PlayerControl.AllPlayerControls)
                {
                    foreach (var recipient in PlayerControl.AllPlayerControls)
                    {
                        if (sender.CurrentOutfitType == PlayerOutfitType.Shapeshifted){
                            MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.Shapeshift, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient));
                            messageWriter.WriteNetObject(sender);
                            if (sender.AmOwner){
                                messageWriter.Write(!CheatSettings.noCooldowns);
                            }else{
                                messageWriter.Write(true);
                            }
                            AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
                        }
                    }
                }

            }

            CheatSettings.resetShapeshift = false;

        }

        //Bugfix: The player is dwarfed after the disguise is reverted, so this is meant to reset their scale to the default value
        try{
            if (!PlayerControl.LocalPlayer.shapeshifting && PlayerControl.LocalPlayer.transform.localScale != PlayerControl.LocalPlayer.defaultCosmeticsScale){
                PlayerControl.LocalPlayer.cosmetics.SetScale(PlayerControl.LocalPlayer.MyPhysics.Animations.DefaultPlayerScale, PlayerControl.LocalPlayer.defaultCosmeticsScale);
            }
        }catch{}
                
    }
}