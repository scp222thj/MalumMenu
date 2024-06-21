using UnityEngine;
using Il2CppSystem.Collections.Generic;
using Sentry.Internal.Extensions;

namespace MalumMenu;
public static class PlayerPickMenu
{
    public static ShapeshifterMinigame playerpickMenu;
    public static bool IsActive;
    public static NetworkedPlayerInfo targetPlayerData;
    public static Il2CppSystem.Action customAction;
    public static List<NetworkedPlayerInfo> customPlayerList;

    // Get ShapeshifterMenu prefab to instantiate it
    // Found here: https://github.com/AlchlcDvl/TownOfUsReworked/blob/9f3cede9d30bab2c11eb7c960007ab3979f09156/TownOfUsReworked/Custom/Menu.cs
    public static ShapeshifterMinigame getShapeshifterMenu()
    {
        var rolePrefab = Utils.getBehaviourByRoleType(AmongUs.GameOptions.RoleTypes.Shapeshifter);
        return Object.Instantiate(rolePrefab?.Cast<ShapeshifterRole>(), GameData.Instance.transform).ShapeshifterMenu;
    }

    // Open a PlayerPickMenu to pick a specific player to target
    public static void openPlayerPickMenu(List<NetworkedPlayerInfo> playerList, Il2CppSystem.Action action)
    {
        IsActive = true;
        customPlayerList = playerList;
        customAction = action;

        //The menu is based off the shapeshifting menu
        playerpickMenu = Object.Instantiate(getShapeshifterMenu());
			    
        playerpickMenu.transform.SetParent(Camera.main.transform, false);
		playerpickMenu.transform.localPosition = new Vector3(0f, 0f, -50f);
		playerpickMenu.Begin(null);
    }

    // Returns a custom NetworkedPlayerInfo that can be used as a PPM choice
    public static NetworkedPlayerInfo customPPMChoice(string name, NetworkedPlayerInfo.PlayerOutfit outfit, RoleBehaviour role = null)
    {
        NetworkedPlayerInfo customChoice = Object.Instantiate<NetworkedPlayerInfo>(GameData.Instance.PlayerInfoPrefab);

        outfit.PlayerName = name;

        customChoice.Outfits[PlayerOutfitType.Default] = outfit;

        if (!role.IsNull()){
            customChoice.Role = role;
        }

        return customChoice;
    }
}   