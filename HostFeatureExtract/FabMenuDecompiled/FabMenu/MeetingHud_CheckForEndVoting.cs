using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(MeetingHud), "CheckForEndVoting")]
public static class MeetingHud_CheckForEndVoting
{
	public static bool Prefix(MeetingHud __instance)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (!CheatToggles.voteImmune)
		{
			return true;
		}
		if (!((IEnumerable<PlayerVoteArea>)__instance.playerStates).All((PlayerVoteArea ps) => ps.AmDead || ps.DidVote))
		{
			return true;
		}
		bool tie = default(bool);
		KeyValuePair<byte, int> max = Extensions.MaxPair(__instance.CalculateVotes(), ref tie);
		NetworkedPlayerInfo val = ((IEnumerable<NetworkedPlayerInfo>)GameData.Instance.AllPlayers.ToArray()).FirstOrDefault((NetworkedPlayerInfo v) => !tie && v.PlayerId == max.Key);
		if ((Object)(object)val != (Object)null && (Object)(object)val == (Object)(object)PlayerControl.LocalPlayer.Data)
		{
			val = null;
		}
		VoterState[] array = (VoterState[])(object)new VoterState[((Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates).Length];
		for (int num = 0; num < ((Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates).Length; num++)
		{
			PlayerVoteArea val2 = ((Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates)[num];
			array[num] = new VoterState
			{
				VoterId = val2.TargetPlayerId,
				VotedForId = val2.VotedFor
			};
		}
		__instance.RpcVotingComplete(Il2CppStructArray<VoterState>.op_Implicit(array), val, tie);
		return false;
	}
}
