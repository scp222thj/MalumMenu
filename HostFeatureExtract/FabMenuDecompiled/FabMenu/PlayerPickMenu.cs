using AmongUs.GameOptions;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Sentry.Internal.Extensions;
using UnityEngine;

namespace FabMenu;

public static class PlayerPickMenu
{
	public static ShapeshifterMinigame playerpickMenu;

	public static bool IsActive;

	public static NetworkedPlayerInfo targetPlayerData;

	public static Action customAction;

	public static List<NetworkedPlayerInfo> customPlayerList;

	public static ShapeshifterMinigame getShapeshifterMenu()
	{
		RoleBehaviour behaviourByRoleType = Utils.getBehaviourByRoleType((RoleTypes)5);
		return Object.Instantiate<ShapeshifterRole>((behaviourByRoleType != null) ? ((Il2CppObjectBase)behaviourByRoleType).Cast<ShapeshifterRole>() : null, ((Component)GameData.Instance).transform).ShapeshifterMenu;
	}

	public static void openPlayerPickMenu(List<NetworkedPlayerInfo> playerList, Action action)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		IsActive = true;
		customPlayerList = playerList;
		customAction = action;
		playerpickMenu = Object.Instantiate<ShapeshifterMinigame>(getShapeshifterMenu());
		((Component)playerpickMenu).transform.SetParent(((Component)Camera.main).transform, false);
		((Component)playerpickMenu).transform.localPosition = new Vector3(0f, 0f, -50f);
		((Minigame)playerpickMenu).Begin((PlayerTask)null);
	}

	public static NetworkedPlayerInfo customPPMChoice(string name, PlayerOutfit outfit, RoleBehaviour role = null)
	{
		NetworkedPlayerInfo val = Object.Instantiate<NetworkedPlayerInfo>(GameData.Instance.PlayerInfoPrefab);
		outfit.PlayerName = name;
		val.Outfits[(PlayerOutfitType)0] = outfit;
		if (!MiscExtensions.IsNull((Object)(object)role))
		{
			val.Role = role;
		}
		return val;
	}
}
