using UnityEngine;
using System;
using AmongUs.Data;
using InnerNet;
using Il2CppSystem.Collections.Generic;
using System.IO;
using Hazel;
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

    //Useful for getting full lists of all the Among Us cosmetics IDs
    public static ReferenceDataManager referenceDataManager = DestroyableSingleton<ReferenceDataManager>.Instance;
    
    public static bool utilsOpenChat;
    
    //Completly randomize a player outfit using fake RPC calls
    public static void ShuffleOutfit(PlayerControl sender)
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
    }

    //Kill any player using fake RPC calls
    public static void MurderPlayer(PlayerControl source, PlayerControl target)
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(source.NetId, (byte)RpcCalls.MurderPlayer, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                writer.WriteNetObject(target);
                writer.Write((int)MurderResultFlags.Succeeded);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }

    //Kill any player using fake RPC calls
    public static void SetRole(PlayerControl player, AmongUs.GameOptions.RoleTypes role)
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected){
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)RpcCalls.SetRole, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                messageWriter.Write((ushort)role);
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
            }
        }
    }

    //Make any player shapeshift into any other player using fake RPC calls
    public static void ShapeshiftPlayer(PlayerControl source, PlayerControl target, bool shouldAnimate)
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(source.NetId, (byte)RpcCalls.Shapeshift, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                messageWriter.WriteNetObject(target);
                messageWriter.Write(shouldAnimate);
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
            }
        }
    }

    //Make any player teleport anywhere using fake RPC calls
    public static void TeleportPlayer(PlayerControl player, Vector2 position)
    {
        //if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId){
        //    player.NetTransform.RpcSnapTo(position);
        //}
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            foreach (PlayerControl item in PlayerControl.AllPlayerControls){
                MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(player.NetTransform.NetId, (byte)RpcCalls.SnapTo, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                NetHelpers.WriteVector2(position, messageWriter);
                messageWriter.Write(player.NetTransform.lastSequenceId + 100U);
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
            }
        }
    }

    //Make any player copy any other player's outfit using fake RPC calls
    public static void CopyOutfit(PlayerControl source, PlayerControl target)
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter colorWriter = AmongUsClient.Instance.StartRpcImmediately(source.NetId, (byte)RpcCalls.SetColor, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                MessageWriter nameWriter = AmongUsClient.Instance.StartRpcImmediately(source.NetId, (byte)RpcCalls.SetName, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                MessageWriter hatWriter = AmongUsClient.Instance.StartRpcImmediately(source.NetId, (byte)RpcCalls.SetHatStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                MessageWriter petWriter = AmongUsClient.Instance.StartRpcImmediately(source.NetId, (byte)RpcCalls.SetPetStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                MessageWriter visorWriter = AmongUsClient.Instance.StartRpcImmediately(source.NetId, (byte)RpcCalls.SetVisorStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                MessageWriter skinWriter = AmongUsClient.Instance.StartRpcImmediately(source.NetId, (byte)RpcCalls.SetSkinStr, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));

                colorWriter.Write(target.Data.DefaultOutfit.ColorId);
                nameWriter.Write(target.Data.DefaultOutfit.PlayerName);
                hatWriter.Write(target.Data.DefaultOutfit.HatId);
                petWriter.Write(target.Data.DefaultOutfit.PetId);
                visorWriter.Write(target.Data.DefaultOutfit.VisorId);
                skinWriter.Write(target.Data.DefaultOutfit.SkinId);

                AmongUsClient.Instance.FinishRpcImmediately(colorWriter);
                AmongUsClient.Instance.FinishRpcImmediately(nameWriter);
                AmongUsClient.Instance.FinishRpcImmediately(hatWriter);
                AmongUsClient.Instance.FinishRpcImmediately(petWriter);
                AmongUsClient.Instance.FinishRpcImmediately(visorWriter);
                AmongUsClient.Instance.FinishRpcImmediately(skinWriter);
            }
        }

    }

    //Change any player's name using fake RPC calls
    public static void SetName(PlayerControl target, string name)
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            foreach (var item in PlayerControl.AllPlayerControls)
            {
                MessageWriter nameWriter = AmongUsClient.Instance.StartRpcImmediately(target.NetId, (byte)RpcCalls.SetName, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
                
                nameWriter.Write(name);
                
                AmongUsClient.Instance.FinishRpcImmediately(nameWriter);
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
    public static List<PlayerControl> customPlayerList;

    //Open a custom menu to pick a player as a target
    public static void openPlayerPickMenu(List<PlayerControl> playerList, Action action)
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
            List<PlayerControl> list = customPlayerList;

            __instance.potentialVictims = new List<ShapeshifterPanel>();
            List<UiElement> list2 = new List<UiElement>();

            for (int i = 0; i < list.Count; i++)
            {
                PlayerControl player = list[i];
                int num = i % 3;
                int num2 = i / 3;
                ShapeshifterPanel shapeshifterPanel = UnityEngine.Object.Instantiate<ShapeshifterPanel>(__instance.PanelPrefab, __instance.transform);
                shapeshifterPanel.transform.localPosition = new Vector3(__instance.XStart + (float)num * __instance.XOffset, __instance.YStart + (float)num2 * __instance.YOffset, -1f);
                
                shapeshifterPanel.SetPlayer(i, player.Data, (Action) (() =>
                {
                    targetPlayer = player; //Save targeted player

                    customAction.Invoke(); //Custom action set by openPlayerPickMenu

                    __instance.Close();
                }));

                shapeshifterPanel.NameText.color = Utils.getColorName(CheatToggles.seeRoles, player.Data);
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

//Some useful cheat checks that I use in MenuUI.cs
[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class CheatChecks
{
    public static bool isShip;
    public static bool isPlayer;
    public static bool isHost;

    public static void Postfix(PlayerPhysics __instance)
    {
        isShip = ShipStatus.Instance != null;
        isPlayer = PlayerControl.LocalPlayer != null;
        isHost = AmongUsClient.Instance.AmHost;
    }
}