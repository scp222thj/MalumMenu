using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace FabMenu;

[HarmonyPatch(typeof(MeetingHud), "PopulateResults")]
public static class MeetingHud_PopulateResults
{
	public static void Prefix(MeetingHud __instance)
	{
		foreach (PlayerVoteArea item in (Il2CppArrayBase<PlayerVoteArea>)(object)__instance.playerStates)
		{
			if (!Object.op_Implicit((Object)(object)item))
			{
				continue;
			}
			VoteSpreader component = ((Component)((Component)item).transform).GetComponent<VoteSpreader>();
			if (Object.op_Implicit((Object)(object)component) && component.Votes.Count != 0)
			{
				Enumerator<SpriteRenderer> enumerator2 = component.Votes.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Object.DestroyImmediate((Object)(object)enumerator2.Current);
				}
				component.Votes.Clear();
			}
		}
		if (Object.op_Implicit((Object)(object)__instance.SkippedVoting))
		{
			VoteSpreader component2 = ((Component)__instance.SkippedVoting.transform).GetComponent<VoteSpreader>();
			Enumerator<SpriteRenderer> enumerator2 = component2.Votes.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				Object.DestroyImmediate((Object)(object)enumerator2.Current);
			}
			component2.Votes.Clear();
		}
		MeetingHud_Update.votedPlayers.Clear();
	}
}
