using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Outfits_ShufflePostfix
{
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatSettings.shuffleOutfit)
        {
            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected)
            {
                Utils.ShuffleOutfit(PlayerControl.LocalPlayer);
            }

            CheatSettings.shuffleOutfit = false;
        }
    }
}
