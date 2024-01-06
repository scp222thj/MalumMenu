using HarmonyLib;
using Hazel;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
public static class SpamChat_PlayerPhysics_RpcSendChat_Prefix
{
    public static string spamText;
    public static bool isActive;
    private static float lastChatTime = 0f;
    private static float chatDelay = 0.5f; // Delay between messages in seconds

    // Prefix patch of PlayerControl.RpcSendChat to set spamText based on the user's chat messages
    public static bool Prefix(string chatText, PlayerControl __instance)
    {
        if (CheatToggles.spamChat)
        {
            spamText = chatText;
            return false; // Skip the original method when the cheat is active
        }

        return true; // Send a normal chat message if the cheat is not active
    }

    //Keep flooding the chat with the user's spamText until the cheat is disabled
    //A short delay (chatDelay) is used to avoid being kicked for sending too many RPC calls too quickly
    public static void Update()
    {
        if (CheatToggles.spamChat){

            if (!isActive){

                CheatToggles.chatMimic = CheatToggles.setName = CheatToggles.setNameAll = false;

                Utils.OpenChat();

                isActive = true;

            }

            if (spamText != null && Time.time - lastChatTime >= chatDelay)
            {
                lastChatTime = Time.time;
                SendSpamChat();
            }
        
        }else{
            if (isActive){
                
                isActive = false;
            }
        }
    }

    private static void SendSpamChat()
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            //Sends a forged SendChat RPC call from each player to all players
            //This generates identical fake chat messages coming from all player, spamming the chat
            foreach (var sender in PlayerControl.AllPlayerControls)
            {
                foreach (var recipient in PlayerControl.AllPlayerControls)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SendChat, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient));
                    writer.Write(spamText);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class RPC_SpamChatPostfix
{
    public static void Postfix(PlayerPhysics __instance)
    {
        SpamChat_PlayerPhysics_RpcSendChat_Prefix.Update();
        if (!CheatToggles.spamChat)
        {
            SpamChat_PlayerPhysics_RpcSendChat_Prefix.spamText = null;
        }
    }
}
