using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(AuthManager), nameof(AuthManager.CoConnect))]
public static class AuthManager_CoConnect_FriendCode_Spoof_Patch
{
    public static void Prefix()
    {
        if (SpoofingService.EnableFriendCodeSpoof)
        {
            SpoofingService.ApplyFriendCodeSpoof();
            Debug.Log("[SpoofingService] FriendCode spoof applied before AuthManager.CoConnect");
        }
    }
}
