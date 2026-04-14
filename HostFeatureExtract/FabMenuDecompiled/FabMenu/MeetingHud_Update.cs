using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(MeetingHud), "Update")]
public static class MeetingHud_Update
{
	public static List<int> votedPlayers = new List<int>();

	public static void Prefix(MeetingHud __instance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)__instance.state >= 4)
		{
			return;
		}
		foreach (PlayerVoteArea item in (Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates)
		{
			if (!Object.op_Implicit((Object)(object)item))
			{
				continue;
			}
			NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(item.TargetPlayerId);
			if (!((Object)(object)playerById != (Object)null) || playerById.Disconnected || item.VotedFor == PlayerVoteArea.HasNotVoted || item.VotedFor == PlayerVoteArea.MissedVote || item.VotedFor == PlayerVoteArea.DeadVote || votedPlayers.Contains(item.TargetPlayerId))
			{
				continue;
			}
			votedPlayers.Add(item.TargetPlayerId);
			if (item.VotedFor != PlayerVoteArea.SkippedVote)
			{
				foreach (PlayerVoteArea item2 in (Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates)
				{
					if (item2.TargetPlayerId == item.VotedFor)
					{
						__instance.BloopAVoteIcon(playerById, 0, ((Component)item2).transform);
						break;
					}
				}
			}
			else if (Object.op_Implicit((Object)(object)__instance.SkippedVoting))
			{
				__instance.BloopAVoteIcon(playerById, 0, __instance.SkippedVoting.transform);
			}
		}
		foreach (PlayerVoteArea item3 in (Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates)
		{
			if (!Object.op_Implicit((Object)(object)item3))
			{
				continue;
			}
			VoteSpreader component = ((Component)((Component)item3).transform).GetComponent<VoteSpreader>();
			if (Object.op_Implicit((Object)(object)component))
			{
				Enumerator<SpriteRenderer> enumerator3 = component.Votes.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					((Component)enumerator3.Current).gameObject.SetActive(CheatToggles.revealVotes);
				}
			}
		}
		if (Object.op_Implicit((Object)(object)__instance.SkippedVoting))
		{
			__instance.SkippedVoting.SetActive(CheatToggles.revealVotes);
		}
	}

	public static void Postfix(MeetingHud __instance)
	{
		MalumESP.MeetingNametags(__instance);
		PlayerControl.LocalPlayer.onLadder = false;
	}
}
