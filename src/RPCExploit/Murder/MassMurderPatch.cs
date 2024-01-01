using HarmonyLib;
using Hazel;
using InnerNet;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class RPC_MassMurderPostfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to murder any player
    public static void Postfix(PlayerPhysics __instance){
        if (CheatSettings.massMurder){

            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected){
                foreach (var sender in PlayerControl.AllPlayerControls)
                {
                    foreach (var recipient in PlayerControl.AllPlayerControls)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.MurderPlayer, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient));
                        writer.WriteNetObject(sender);
                        writer.Write((int)MurderResultFlags.Succeeded);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }

            CheatSettings.massMurder = false;

        }
                
    }
}