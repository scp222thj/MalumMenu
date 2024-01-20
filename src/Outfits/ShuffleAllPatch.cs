using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class Outfits_ShuffleAllPostfix
{
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatSettings.shuffleAllOutfits)
        {
            var HostData = AmongUsClient.Instance.GetHost();
            if (HostData != null && !HostData.Character.Data.Disconnected)
            {
                foreach (var sender in PlayerControl.AllPlayerControls)
                {
                    Utils.ShuffleOutfit(sender);
                }
            }

            CheatSettings.shuffleAllOutfits = false;
        }
    }
}
