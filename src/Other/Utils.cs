using UnityEngine;
using System;
using AmongUs.Data;
using Il2CppSystem.Collections.Generic;
using System.IO;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace MalumMenu;
public static class Utils
{
    //Adjusts HUD resolution
    //Used to fix UI problems when zooming out
    public static void adjustResolution() {
        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
    }

    //Gets current map ID
    public static byte getCurrentMapID()
    {
        //If playing the tutorial
        if (DestroyableSingleton<TutorialManager>.InstanceExists)
	    {
            return (byte)AmongUsClient.Instance.TutorialMapId;

	    }else{
            //Works for all other games
            return GameOptionsManager.Instance.currentGameOptions.MapId;
        }
    }

    //Get SystemType of the room the player is currently in
    public static SystemTypes getCurrentRoom(){
        return HudManager.Instance.roomTracker.LastRoom.RoomId;
    }

    //Fancy colored ping text
    public static string getColoredPingText(int ping){

        if (ping < 100){ //Green for ping < 100

            return $"<color=#00ff00ff>\nPing: {ping} ms</color>";

        } else if (ping < 400){ //Yellow for 100 < ping < 400

            return $"<color=#ffff00ff>\nPing: {ping} ms</color>";

        } else{ //Red for ping > 400

            return $"<color=#ff0000ff>\nPing: {ping} ms</color>";
        }
    }

    //Get the appropriate name color for a player depending on if cheat is enabled (cheatVar)
    public static Color getColorName(bool cheatVar, GameData.PlayerInfo playerInfo){
        if (cheatVar){
                
            return playerInfo.Role.TeamColor; //Cheat vision

        }else if(PlayerControl.LocalPlayer.Data.Role.NameColor == playerInfo.Role.NameColor){

            return playerInfo.Role.NameColor; //Normal Impostor Vision

        }else {

            return Color.white; //Normal Crewmate Vision
        }
    }

    //Get ShapeshifterMenu prefab to instantiate it
    //Found here: https://github.com/AlchlcDvl/TownOfUsReworked/blob/9f3cede9d30bab2c11eb7c960007ab3979f09156/TownOfUsReworked/Custom/Menu.cs
    public static ShapeshifterMinigame getShapeshifterMenu()
    {
        var rolePrefab = RoleManager.Instance.AllRoles.First(r => r.Role == AmongUs.GameOptions.RoleTypes.Shapeshifter);
        return UnityEngine.Object.Instantiate(rolePrefab?.Cast<ShapeshifterRole>(), GameData.Instance.transform).ShapeshifterMenu;
    }

    //Show custom popup ingame
    //Found here: https://github.com/NuclearPowered/Reactor/blob/6eb0bf19c30733b78532dada41db068b2b247742/Reactor/Networking/Patches/HttpPatches.cs
    public static void showPopup(string text){
        var popup = UnityEngine.Object.Instantiate(DiscordManager.Instance.discordPopup, Camera.main!.transform);
        
        var background = popup.transform.Find("Background").GetComponent<SpriteRenderer>();
        var size = background.size;
        size.x *= 2.5f;
        background.size = size;

        popup.TextAreaTMP.fontSizeMin = 2;
        popup.Show(text);
    }

    //Load sprites and textures from manifest resources
    //Found here: https://github.com/Loonie-Toons/TOHE-Restored/blob/TOHE/Modules/Utils.cs
    public static Dictionary<string, Sprite> CachedSprites = new();
    public static Sprite LoadSprite(string path, float pixelsPerUnit = 1f)
    {
        try
        {
            if (CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;

            Texture2D texture = LoadTextureFromResources(path);
            sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;

            return CachedSprites[path + pixelsPerUnit] = sprite;
        }
        catch
        {
            Debug.LogError($"Failed to read Texture: {path}");
        }
        return null;
    }
    public static Texture2D LoadTextureFromResources(string path)
    {
        try
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            using MemoryStream ms = new();
            
            stream.CopyTo(ms);
            ImageConversion.LoadImage(texture, ms.ToArray(), false);
            return texture;
        }
        catch
        {
            Debug.LogError($"Failed to read Texture: {path}");
        }
        return null;
    }
}

[HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
public static class Utils_PlayerPickMenu
{
    public static ShapeshifterMinigame playerpickMenu;
    public static bool IsActive;
    public static PlayerControl targetPlayer;
    public static Action customAction;
    public static List<GameData.PlayerInfo> customPlayerList;

    //Open a custom menu to pick a player as a target
    public static void openPlayerPickMenu(List<GameData.PlayerInfo> playerList, Action action)
    {
        IsActive = true;
        customPlayerList = playerList;
        customAction = action;

        //The menu is based off the shapeshifting menu
        playerpickMenu = UnityEngine.Object.Instantiate<ShapeshifterMinigame>(Utils.getShapeshifterMenu());
			    
        playerpickMenu.transform.SetParent(Camera.main.transform, false);
		playerpickMenu.transform.localPosition = new Vector3(0f, 0f, -50f);
		playerpickMenu.Begin(null);
    }
    
    //Prefix patch of ShapeshifterMinigame.Begin to implement player pick menu logic
    public static bool Prefix(PlayerTask task, ShapeshifterMinigame __instance)
    {
        if (IsActive){ //Player pick menu logic

            //Custom player list set by openPlayerPickMenu
            List<GameData.PlayerInfo> list = customPlayerList;

            __instance.potentialVictims = new List<ShapeshifterPanel>();
            List<UiElement> list2 = new List<UiElement>();

            for (int i = 0; i < list.Count; i++)
            {
                GameData.PlayerInfo player = list[i];
                int num = i % 3;
                int num2 = i / 3;
                ShapeshifterPanel shapeshifterPanel = UnityEngine.Object.Instantiate<ShapeshifterPanel>(__instance.PanelPrefab, __instance.transform);
                shapeshifterPanel.transform.localPosition = new Vector3(__instance.XStart + (float)num * __instance.XOffset, __instance.YStart + (float)num2 * __instance.YOffset, -1f);
                
                shapeshifterPanel.SetPlayer(i, player, (Action) (() =>
                {
                    targetPlayer = player.Object; //Save targeted player

                    customAction.Invoke(); //Custom action set by openPlayerPickMenu

                    __instance.Close();
                }));

                shapeshifterPanel.NameText.color = Utils.getColorName(CheatSettings.seeRoles, player);
                __instance.potentialVictims.Add(shapeshifterPanel);
                list2.Add(shapeshifterPanel.Button);
            }

            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2, false);
            
            IsActive = false;

            return false; //Skip original method when active

        }

        return true; //Open normal shapeshifter menu if not active
    }
}   

[HarmonyPatch(typeof(ShapeshifterPanel), nameof(ShapeshifterPanel.SetPlayer))]
public static class Utils_PlayerPickMenu_ShapeshifterPanelSetPlayer
{
    //Prefix patch of ShapeshifterPanel.SetPlayer to allow usage of PlayerPickMenu in lobbies
    public static bool Prefix(ShapeshifterPanel __instance, int index, GameData.PlayerInfo playerInfo, Action onShift)
    {
        if (Utils_PlayerPickMenu.IsActive){ //Player pick menu logic

            __instance.shapeshift = onShift;
            __instance.PlayerIcon.SetFlipX(false);
            __instance.PlayerIcon.ToggleName(false);
            SpriteRenderer[] componentsInChildren = __instance.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].material.SetInt(PlayerMaterial.MaskLayer, index + 2);
            }
            __instance.PlayerIcon.SetMaskLayer(index + 2);
            __instance.PlayerIcon.UpdateFromEitherPlayerDataOrCache(playerInfo, PlayerOutfitType.Default, PlayerMaterial.MaskType.ComplexUI, false, null);
            __instance.LevelNumberText.text = ProgressionManager.FormatVisualLevel(playerInfo.PlayerLevel);
            
            //Skips using custom nameplates because they break the PlayerPickMenu in lobbies

            __instance.NameText.text = playerInfo.PlayerName;
            DataManager.Settings.Accessibility.OnColorBlindModeChanged += (Action)__instance.SetColorblindText;
            __instance.SetColorblindText();

            return false; //Skip original method when active

        }

        return true; //Open normal shapeshifter menu if not active
    }
}