using UnityEngine;
using InnerNet;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using System.IO;
using Hazel;
using System.Reflection;
using AmongUs.GameOptions;
using Sentry.Internal.Extensions;
using Il2CppSystem.Net.NetworkInformation;

namespace MalumMenu;
public static class Utils
{
    //Useful for getting full lists of all the Among Us cosmetics IDs
    public static ReferenceDataManager referenceDataManager = DestroyableSingleton<ReferenceDataManager>.Instance;
    public static bool isShip => ShipStatus.Instance;
    public static bool isLobby => AmongUsClient.Instance && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined && !isFreePlay;
    public static bool isOnlineGame => AmongUsClient.Instance && AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame;
    public static bool isLocalGame => AmongUsClient.Instance && AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
    public static bool isFreePlay => AmongUsClient.Instance && AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
    public static bool isPlayer => PlayerControl.LocalPlayer;
    public static bool isHost = AmongUsClient.Instance && AmongUsClient.Instance.AmHost;
    public static bool isInGame => AmongUsClient.Instance && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started && isPlayer;
    public static bool isMeeting => MeetingHud.Instance;
    public static bool isMeetingVoting => isMeeting && MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted;
    public static bool isMeetingProceeding => isMeeting && MeetingHud.Instance.state is MeetingHud.VoteStates.Proceeding;
    public static bool isExiling => ExileController.Instance && !(AirshipIsActive && SpawnInMinigame.Instance.isActiveAndEnabled);
    public static bool isNormalGame => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal;
    public static bool isHideNSeek => GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek;
    public static bool SkeldIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Skeld;
    public static bool MiraHQIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Mira;
    public static bool PolusIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Polus;
    public static bool DleksIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Dleks;
    public static bool AirshipIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Airship;
    public static bool FungleIsActive => (MapNames)GameOptionsManager.Instance.CurrentGameOptions.MapId == MapNames.Fungle;

    //Get ClientData by PlayerControl
    public static ClientData getClientByPlayer(PlayerControl player)
    {
        try
        {
            var client = AmongUsClient.Instance.allClients.ToArray().FirstOrDefault(cd => cd.Character.PlayerId == player.PlayerId);
            return client;
        }
        catch
        {
            return null;
        }
    }

    // Get ClientData.Id by PlayerControl
    public static int getClientIdByPlayer(PlayerControl player)
    {
        if (player == null) return -1;
        var client = getClientByPlayer(player);
        return client == null ? -1 : client.Id;
    }

    // Check if player is currently vanished
    public static bool isVanished(NetworkedPlayerInfo playerInfo)
    {
        PhantomRole phantomRole = playerInfo.Role as PhantomRole;

        if (phantomRole != null){
            return phantomRole.fading || phantomRole.isInvisible;
        }

        return false;
    }

    // Custom isValidTarget method for cheats
    public static bool isValidTarget(NetworkedPlayerInfo target)
    {
        bool killAnyoneRequirements = !(target == null) && !target.Disconnected && target.Object.Visible && target.PlayerId != PlayerControl.LocalPlayer.PlayerId && !(target.Role == null) && !(target.Object == null);

        bool fullRequirements = killAnyoneRequirements && !target.IsDead && !target.Object.inVent && !target.Object.inMovingPlat && target.Role.CanBeKilled;

        if (CheatToggles.killAnyone){
            return killAnyoneRequirements;
        }

        return fullRequirements;
        
    }

    // Adjusts HUD resolution
    // Used to fix UI problems when zooming out
    public static void adjustResolution() {
        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
    }

    // Get RoleBehaviour from a RoleType
    public static RoleBehaviour getBehaviourByRoleType(RoleTypes roleType) {
        return RoleManager.Instance.AllRoles.First(r => r.Role == roleType);
    }

    // Kill any player using RPC calls
    public static void murderPlayer(PlayerControl target, MurderResultFlags result)
    {
        if (isFreePlay){

            PlayerControl.LocalPlayer.MurderPlayer(target, MurderResultFlags.Succeeded);
            return;
        
        }

        foreach (var item in PlayerControl.AllPlayerControls)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.MurderPlayer, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(item));
            writer.WriteNetObject(target);
            writer.Write((int)result);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }

    // Report bodies using RPC calls
    public static void reportDeadBody(NetworkedPlayerInfo playerData)
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


    // Complete all of LocalPlayer's tasks using RPC calls
    public static void completeMyTasks()
    {

        if (isFreePlay){

            foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
            {
                PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
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

    // Open Chat UI
    public static void openChat()
    {
        if (!DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening){
            DestroyableSingleton<HudManager>.Instance.Chat.chatScreen.SetActive(true);
            PlayerControl.LocalPlayer.NetTransform.Halt();
            DestroyableSingleton<HudManager>.Instance.Chat.StartCoroutine(DestroyableSingleton<HudManager>.Instance.Chat.CoOpen());
            if (DestroyableSingleton<FriendsListManager>.InstanceExists)
            {
                DestroyableSingleton<FriendsListManager>.Instance.SetFriendButtonColor(true);
            }
        }

    }

    // Draw a tracer line between two 2 GameObjects
    public static void drawTracer(GameObject sourceObject, GameObject targetObject, Color color)
    {
        LineRenderer lineRenderer;

        lineRenderer = sourceObject.GetComponent<LineRenderer>();

        if(!lineRenderer){
            lineRenderer = sourceObject.AddComponent<LineRenderer>();
        }

        lineRenderer.SetVertexCount(2);
        lineRenderer.SetWidth(0.02F, 0.02F);

        // I just picked an already existing material from the game
        Material material = DestroyableSingleton<HatManager>.Instance.PlayerMaterial;

        lineRenderer.material = material;
        lineRenderer.SetColors(color, color);
                
        lineRenderer.SetPosition(0, sourceObject.transform.position);
        lineRenderer.SetPosition(1, targetObject.transform.position);
    }

    // Return if the ChatUI should be active or not
    public static bool chatUiActive()
    {
        try{
            return CheatToggles.alwaysChat || MeetingHud.Instance || !ShipStatus.Instance || PlayerControl.LocalPlayer.Data.IsDead;
        }catch{
            return false;
        }
    }

    // Close Chat UI
    public static void closeChat()
    {
        if (DestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening){
            DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
        }

    }

    // Get the distance between two players as a float
    public static float getDistanceFrom(PlayerControl target, PlayerControl source = null){
        
        if (source.IsNull()){
            source = PlayerControl.LocalPlayer;
        }

        Vector2 vector = target.GetTruePosition() - source.GetTruePosition();
		float magnitude = vector.magnitude;

        return magnitude;

    }

    // Returns a list of all the players in the game ordered from closest to farthest (from LocalPlayer by default)
    public static System.Collections.Generic.List<PlayerControl> getPlayersSortedByDistance(PlayerControl source = null){
        
        if (source.IsNull()){
            source = PlayerControl.LocalPlayer;
        }

        System.Collections.Generic.List<PlayerControl> outputList = new System.Collections.Generic.List<PlayerControl>();

        outputList.Clear();

        List<NetworkedPlayerInfo> allPlayers = GameData.Instance.AllPlayers;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerControl player = allPlayers[i].Object;
            if (player)
            {
                outputList.Add(player);
            }
        }
        
        outputList = outputList.OrderBy(target => getDistanceFrom(target, source)).ToList();
        
        if (outputList.Count <= 0)
        {
            return null;
        }

        return outputList;

    }

    // Gets current map ID
    public static byte getCurrentMapID()
    {
        // If playing the tutorial
        if (isFreePlay)
	    {
            return (byte)AmongUsClient.Instance.TutorialMapId;

	    }else{
            // Works for local/online games
            return GameOptionsManager.Instance.currentGameOptions.MapId;
        }
    }

    // Get SystemType of the room the player is currently in
    public static SystemTypes getCurrentRoom(){
        return HudManager.Instance.roomTracker.LastRoom.RoomId;
    }

    // Fancy colored ping text
    public static string getColoredPingText(int ping){

        if (ping <= 100){ // Green for ping < 100

            return $"<color=#00ff00ff>PING: {ping} ms</color>";

        } else if (ping < 400){ // Yellow for 100 < ping < 400

            return $"<color=#ffff00ff>PING: {ping} ms</color>";

        } else{ // Red for ping > 400

            return $"<color=#ff0000ff>PING: {ping} ms</color>";
        }
    }

    // Get a UnityEngine.KeyCode from a string
    public static KeyCode stringToKeycode(string keyCodeStr){

        if(!string.IsNullOrEmpty(keyCodeStr)){ // Empty strings are automatically invalid

            try{
                
                // Case-insensitive parse of UnityEngine.KeyCode to check if string is validssss
                KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeStr, true);
                
                return keyCode;

            }catch{}
        
        }

        return KeyCode.Delete; // If string is invalid, return Delete as the default key
    }

    // Get a platform type from a string
    public static bool stringToPlatformType(string platformStr, out Platforms? platform){

        if(!string.IsNullOrEmpty(platformStr)){ // Empty strings are automatically invalid

            try{
                
                // Case-insensitive parse of Platforms from string (if it valid)
                platform = (Platforms)System.Enum.Parse(typeof(Platforms), platformStr, true);
                
                return true; // If platform type is valid, return false

            }catch{}
        
        }

        platform = null;
        return false; // If platform type is invalid, return false
    }

    // Get the string name for a chosen player's role
    // String are automatically translated
    public static string getRoleName(NetworkedPlayerInfo playerData)
    {
        var translatedRole = DestroyableSingleton<TranslationController>.Instance.GetString(playerData.Role.StringName, Il2CppSystem.Array.Empty<Il2CppSystem.Object>());
        if (translatedRole == "STRMISS")
        {
            if (playerData.RoleWhenAlive.HasValue)
            {
                translatedRole = DestroyableSingleton<TranslationController>.Instance.GetString(getBehaviourByRoleType(playerData.RoleWhenAlive.Value).StringName, Il2CppSystem.Array.Empty<Il2CppSystem.Object>());
            } else {
                translatedRole = "Ghost";
            }
        }
        return translatedRole;
    }

    // Get the appropriate nametag for a player (seeRoles cheat)
    public static string getNameTag(NetworkedPlayerInfo playerInfo, string playerName, bool isChat = false){
        string nameTag = playerName;

        if (!playerInfo.Role.IsNull() && !playerInfo.IsNull() && !playerInfo.Disconnected && !playerInfo.Object.CurrentOutfit.IsNull()){

            if (CheatToggles.seeRoles){

                if (isChat){
                    nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(playerInfo.Role.TeamColor)}><size=70%>{Utils.getRoleName(playerInfo)}</size> {nameTag}</color>";
                    return nameTag;
                }

                nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(playerInfo.Role.TeamColor)}><size=70%>{getRoleName(playerInfo)}</size>\r\n{nameTag}</color>";
            
            } else if (PlayerControl.LocalPlayer.Data.Role.NameColor == playerInfo.Role.NameColor){

                if (isChat){
                    return nameTag;
                }

                nameTag = $"<color=#{ColorUtility.ToHtmlStringRGB(playerInfo.Role.NameColor)}>{nameTag}</color>";

            }
        }

        return nameTag;
    }

    // Show custom popup ingame
    // Found here: https://github.com/NuclearPowered/Reactor/blob/6eb0bf19c30733b78532dada41db068b2b247742/Reactor/Networking/Patches/HttpPatches.cs
    public static void showPopup(string text){
        var popup = Object.Instantiate(DiscordManager.Instance.discordPopup, Camera.main!.transform);
        
        var background = popup.transform.Find("Background").GetComponent<SpriteRenderer>();
        var size = background.size;
        size.x *= 2.5f;
        background.size = size;

        popup.TextAreaTMP.fontSizeMin = 2;
        popup.Show(text);
    }

    // Load sprites and textures from manifest resources
    // Found here: https://github.com/Loonie-Toons/TOHE-Restored/blob/TOHE/Modules/Utils.cs
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

