using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckMurder))]
public static class ImpostorCheats_PlayerControl_CmdCheckMurder_Postfix
{
    //Prefix patch of PlayerControl.CmdCheckMurder to bypass anticheat
    public static bool Prefix(PlayerControl __instance, PlayerControl target){

        __instance.isKilling = false;

		if (!__instance.Data.Role.IsValidTarget(target.Data))
		{
			return false;
		}

        //Protected players can only be killed if killAnyone is enabled
        if (target.protectedByGuardianId > -1 && !CheatToggles.killAnyone){
            Utils.MurderPlayer(target, MurderResultFlags.FailedProtected);
        }

		__instance.isKilling = true;

        //Use custom util to bypass anticheat
        Utils.MurderPlayer(target, MurderResultFlags.Succeeded);

        return false;

    }
}

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.FindClosestTarget))]
public static class infiniteKillDistance_LogicOptions_GetKillDistance_Postfix
{
    public static List<PlayerControl> outputList = new List<PlayerControl>();

    //Prefix patch of ImpostorRole.FindClosestTarget to allow for infinite kill reach
    public static bool Prefix(ImpostorRole __instance, ref PlayerControl __result){

        if (CheatToggles.killReach){

            outputList.Clear();

            //Gets a list of all "valid" kill targets, ordered by how close they are to me
            //Purposely doesn't take into account the target distance & colliders in the way
            Vector2 myPos = __instance.Player.GetTruePosition();
            Il2CppSystem.Collections.Generic.List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
            for (int i = 0; i < allPlayers.Count; i++)
            {
                GameData.PlayerInfo playerInfo = allPlayers[i];
                if (__instance.IsValidTarget(playerInfo))
                {
                    PlayerControl @object = playerInfo.Object;
                    if (@object && @object.Collider.enabled)
                    {
                        Vector2 vector = @object.GetTruePosition() - myPos;
                        float magnitude = vector.magnitude;
                        outputList.Add(@object);
                    }
                }
            }
            
            outputList = outputList.OrderBy(a => (a.GetTruePosition() - myPos).magnitude).ToList();
            
            if (outputList.Count <= 0)
            {
                return false;
            }

            __result = outputList[0]; //Return the closest kill target, regardless of where they are

            return false;

        }

        return true;

    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class zeroKillCd_PlayerControl_FixedUpdate_Postfix
{
    //Postfix patch of PlayerControl.FixedUpdate to remove kill cooldown
    public static void Postfix(PlayerControl __instance){

        if (__instance.AmOwner && CheatToggles.zeroKillCd && __instance.killTimer > 0f){
            __instance.SetKillTimer(0f);
        }

    }
}

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
public static class killJailbreak_ImpostorRole_IsValidTarget_Postfix
{
    //Prefix patch of ImpostorRole.IsValidTarget to allow forbidden kill targets for killAnyone cheat
    //Allows killing ghosts (with seeGhosts), impostors, players in vents, etc...
    public static bool Prefix(ImpostorRole __instance, GameData.PlayerInfo target, ref bool __result){

        if (!CheatToggles.killAnyone){
            return true;
        }

        __result = target != null && !target.Disconnected && (!target.IsDead || CheatToggles.seeGhosts) && target.PlayerId != __instance.Player.PlayerId && !(target.Object == null);
        return false;

    }
}

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.CanUse))]
public static class impostorTasks_ImpostorRole_CanUse_Prefix
{
    //Prefix patch of ImpostorRole.CanUse to do tasks as an impostor
    public static void Postfix(ImpostorRole __instance, Console usable, ref bool __result)
    {
        __result = !(usable != null) || usable.AllowImpostor || CheatToggles.impostorTasks;
    }
}