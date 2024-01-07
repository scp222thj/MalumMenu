using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class SetNameAll_PlayerControl_LateUpdate_Postfix
{
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.setNameAll)
        {
            CheatToggles.enableCommands = true;
            Utils.OpenChat();
            HudManager.Instance.Chat.freeChatField.textArea.SetText("/setnameall ");
            CheatToggles.setNameAll = false;
        }
    }
}