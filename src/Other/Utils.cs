using UnityEngine;
using AmongUs.Data;
using InnerNet;
using Il2CppSystem.Collections.Generic;
using System.IO;
using Hazel;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;

namespace MalumMenu;
public static class Utils
{
    //Adjusts HUD resolution
    //Used to fix UI problems when zooming out
    public static void adjustResolution() {
        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
    }

    //Useful for getting full lists of all the Among Us cosmetics IDs
    public static ReferenceDataManager referenceDataManager = DestroyableSingleton<ReferenceDataManager>.Instance;
    
    public static bool isShip => ShipStatus.Instance != null;
    public static bool isLobby => AmongUsClient.Instance.GameState == AmongUsClient.GameStates.Joined;
    public static bool isFreePlay => AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
    public static bool isPlayer => PlayerControl.LocalPlayer != null;
    public static bool isHost = AmongUsClient.Instance.AmHost;
    public static bool utilsOpenChat;
    
    //Completly randomize a player outfit using fake RPC calls
/*  public static void ShuffleOutfit(PlayerControl sender)
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter colorWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                MessageWriter nameWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetName, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                MessageWriter hatWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetHatStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                MessageWriter petWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetPetStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                MessageWriter visorWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetVisorStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                MessageWriter skinWriter = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetSkinStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));

                colorWriter.Write((byte)new System.Random().Next(18));
                nameWriter.Write(DestroyableSingleton<AccountManager>.Instance.GetRandomName());
                hatWriter.Write(referenceDataManager.Refdata.hats[new System.Random().Next(referenceDataManager.Refdata.hats.Count)].ProdId);
                petWriter.Write(referenceDataManager.Refdata.pets[new System.Random().Next(referenceDataManager.Refdata.pets.Count)].ProdId);
                visorWriter.Write(referenceDataManager.Refdata.visors[new System.Random().Next(referenceDataManager.Refdata.visors.Count)].ProdId);
                skinWriter.Write(referenceDataManager.Refdata.skins[new System.Random().Next(referenceDataManager.Refdata.skins.Count)].ProdId);

                AmongUsClient.Instance.FinishRpcImmediately(colorWriter);
                AmongUsClient.Instance.FinishRpcImmediately(nameWriter);
                AmongUsClient.Instance.FinishRpcImmediately(hatWriter);
                AmongUsClient.Instance.FinishRpcImmediately(petWriter);
                AmongUsClient.Instance.FinishRpcImmediately(visorWriter);
                AmongUsClient.Instance.FinishRpcImmediately(skinWriter);
            }
        }
    } */

    //Kill any player using RPC calls
    public static void MurderPlayer(PlayerControl target, MurderResultFlags result)
    {
        if (isFreePlay){

            PlayerControl.LocalPlayer.RpcMurderPlayer(target, true);
            return;
        
        }

        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.MurderPlayer, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                writer.WriteNetObject(target);
                writer.Write((int)result);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }

    //Report any body using RPC calls
    public static void ReportDeadBody(GameData.PlayerInfo playerData)
    {

        if (isFreePlay){

            PlayerControl.LocalPlayer.CmdReportDeadBody(playerData);
            return;
        
        }

        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.ReportDeadBody, SendOption.None, HostData.Id);
            writer.Write(playerData.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }


    //Complete all your tasks using RPC calls
    public static void CompleteAllTasks()
    {

        if (isFreePlay){

            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
            {
                if (!task.IsComplete){
                    PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
                }
            }
            return;
        
        }

        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
            {
                if (!task.IsComplete){

                    foreach (var item in PlayerControl.AllPlayerControls)
                    {
                        MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.CompleteTask, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                        messageWriter.WritePacked(task.Id);
                        AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
                    }

                }
            }
        }
    }

    //Open Chat UI
    public static void OpenChat()
    {
        if (!DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening){
            utilsOpenChat = true;
            DestroyableSingleton<HudManager>.Instance.Chat.chatScreen.SetActive(true);
            PlayerControl.LocalPlayer.NetTransform.Halt();
            DestroyableSingleton<HudManager>.Instance.Chat.StartCoroutine(DestroyableSingleton<HudManager>.Instance.Chat.CoOpen());
            if (DestroyableSingleton<FriendsListManager>.InstanceExists)
            {
                DestroyableSingleton<FriendsListManager>.Instance.SetFriendButtonColor(true);
            }
        }

    }

    //Close Chat UI
    public static void CloseChat()
    {
        utilsOpenChat = false;
        if (DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening){
            DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
        }

    }

    //Close Chat UI
    public static float getDistanceFrom(this PlayerControl source, PlayerControl target){
        
        Vector2 vector = target.GetTruePosition() - source.GetTruePosition();
		float magnitude = vector.magnitude;

        return magnitude;

    }

    //Gets current map ID
    public static byte getCurrentMapID()
    {
        //If playing the tutorial
        if (isFreePlay)
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

    //Get a UnityEngine.KeyCode from a string
    public static KeyCode stringToKeycode(string keyCodeStr){

        if(!string.IsNullOrEmpty(keyCodeStr)){ //Empty strings are automatically invalid

            try{
                
                //Case-insensitive parse of UnityEngine.KeyCode to check if string is validssss
                KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeStr, true);
                
                return keyCode;

            }catch{}
        
        }

        return KeyCode.Delete; //If string is invalid, return Delete as the default key
    }

    public static string getRoleName(GameData.PlayerInfo playerData)
    {
        var translatedRole = DestroyableSingleton<TranslationController>.Instance.GetString(playerData.Role.StringName, Il2CppSystem.Array.Empty<Il2CppSystem.Object>());
        if (translatedRole == "STRMISS")
        {
            StringNames @string;
            if (playerData.RoleWhenAlive.HasValue)
            {
                switch (playerData.RoleWhenAlive.Value)
                {
                    case RoleTypes.Crewmate:
                        @string = DestroyableSingleton<CrewmateRole>.Instance.StringName;
                        break;
                    case RoleTypes.Engineer:
                        @string = DestroyableSingleton<EngineerRole>.Instance.StringName;
                        break;
                    case RoleTypes.Scientist:
                        @string = DestroyableSingleton<ScientistRole>.Instance.StringName;
                        break;
                    case RoleTypes.Impostor:
                        @string = DestroyableSingleton<ImpostorRole>.Instance.StringName;
                        break;
                    case RoleTypes.Shapeshifter:
                        @string = DestroyableSingleton<ShapeshifterRole>.Instance.StringName;
                        break;
                    default:
                        @string = DestroyableSingleton<GuardianAngelRole>.Instance.StringName;
                        break;
                }
                translatedRole = DestroyableSingleton<TranslationController>.Instance.GetString(@string, Il2CppSystem.Array.Empty<Il2CppSystem.Object>());
            } else {
                translatedRole = "Ghost";
            }
        }
        return translatedRole;
    }

    //Get the appropriate name color for a player depending on if cheat is enabled (cheatVar)
    public static string getNameTag(PlayerControl player, string playerName, bool isChat = false){
        string nameTag = playerName;

        if (CheatToggles.seeRoles){

            if (isChat){
                nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Role.TeamColor)}><size=70%>{Utils.getRoleName(player.Data)}</size> {nameTag}</color>";
                return nameTag;
            }

            nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Role.TeamColor)}><size=70%>{getRoleName(player.Data)}</size>\r\n{nameTag}</color>";
        
        } else if (PlayerControl.LocalPlayer.Data.Role.NameColor == player.Data.Role.NameColor){

            nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(player.Data.Role.NameColor)}>{nameTag}</color>";

        }

        return nameTag;
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
        var popup = Object.Instantiate(DiscordManager.Instance.discordPopup, Camera.main!.transform);
        
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
    public static GameData.PlayerInfo targetPlayerData;
    public static Il2CppSystem.Action customAction;
    public static List<GameData.PlayerInfo> customPlayerList;

    //Open a custom menu to pick a player as a target
    public static void openPlayerPickMenu(List<GameData.PlayerInfo> playerList, Il2CppSystem.Action action)
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
                GameData.PlayerInfo playerData = list[i];
                int num = i % 3;
                int num2 = i / 3;
                ShapeshifterPanel shapeshifterPanel = UnityEngine.Object.Instantiate<ShapeshifterPanel>(__instance.PanelPrefab, __instance.transform);
                shapeshifterPanel.transform.localPosition = new Vector3(__instance.XStart + (float)num * __instance.XOffset, __instance.YStart + (float)num2 * __instance.YOffset, -1f);
                
                shapeshifterPanel.SetPlayer(i, playerData, (Il2CppSystem.Action) (() =>
                {
                    targetPlayerData = playerData; //Save targeted player

                    customAction.Invoke(); //Custom action set by openPlayerPickMenu

                    __instance.Close();
                }));

                if (playerData.Object != null){
                    shapeshifterPanel.NameText.text = Utils.getNameTag(playerData.Object, playerData.DefaultOutfit.PlayerName);
                }
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
    public static bool Prefix(ShapeshifterPanel __instance, int index, GameData.PlayerInfo playerInfo, Il2CppSystem.Action onShift)
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
            DataManager.Settings.Accessibility.OnColorBlindModeChanged += (Il2CppSystem.Action)__instance.SetColorblindText;
            __instance.SetColorblindText();

            return false; //Skip original method when active

        }

        return true; //Open normal shapeshifter menu if not active
    }
}