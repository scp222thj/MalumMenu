using UnityEngine;
using Il2CppSystem.Collections.Generic;
using System.Linq;

namespace MalumMenu;
public static class PlayerPickMenu
{
    public static ShapeshifterMinigame playerpickMenu;
    public static bool IsActive;
    public static GameData.PlayerInfo targetPlayerData;
    public static Il2CppSystem.Action customAction;
    public static List<GameData.PlayerInfo> customPlayerList;

    //Get ShapeshifterMenu prefab to instantiate it
    //Found here: https://github.com/AlchlcDvl/TownOfUsReworked/blob/9f3cede9d30bab2c11eb7c960007ab3979f09156/TownOfUsReworked/Custom/Menu.cs
    public static ShapeshifterMinigame getShapeshifterMenu()
    {
        var rolePrefab = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Shapeshifter);
        return Object.Instantiate(rolePrefab?.Cast<ShapeshifterRole>(), GameData.Instance.transform).ShapeshifterMenu;
    }

    //Open a custom menu to pick a player as a target
    public static void openPlayerPickMenu(List<GameData.PlayerInfo> playerList, Il2CppSystem.Action action)
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
}   