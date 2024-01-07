using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class SetName_PlayerControl_LateUpdate_Postfix
{
    public static void Postfix(PlayerPhysics __instance)
    {
        if (CheatToggles.setName){
            CheatToggles.enableCommands = true;
            Utils.OpenChat();
            HudManager.Instance.Chat.freeChatField.textArea.SetText("/setname ");
            CheatToggles.setName = false;
        }
    }
}
