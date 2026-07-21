using System;
using System.Collections.Generic;
using AmongUs.Data;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace MalumMenu;

public static class SpoofingService
{
    internal static bool EnableLevelSpoof
    {
        get => _enableLevelSpoof;
        set
        {
            _enableLevelSpoof = value;
            if (MalumMenu.spoofLevel != null)
            {
                MalumMenu.spoofLevel.Value = value ? SpoofedLevel.ToString() : "";
            }
        }
    }

    internal static bool EnablePlatformSpoof
    {
        get => _enablePlatformSpoof;
        set
        {
            _enablePlatformSpoof = value;
            if (MalumMenu.spoofPlatform != null)
            {
                MalumMenu.spoofPlatform.Value = value ? PlatformToString(SpoofedPlatform) : "";
            }
        }
    }

    internal static bool EnableFriendCodeSpoof
    {
        get => _enableFriendCodeSpoof;
        set => _enableFriendCodeSpoof = value;
    }

    internal static bool EnableShuffleName
    {
        get => _enableShuffleName;
        set => _enableShuffleName = value;
    }

    internal static bool EnableShuffleColor
    {
        get => _enableShuffleColor;
        set => _enableShuffleColor = value;
    }

    internal static bool EnableShuffleCosmetics
    {
        get => _enableShuffleCosmetics;
        set => _enableShuffleCosmetics = value;
    }

    internal static uint SpoofedLevel
    {
        get => _spoofedLevel;
        set => _spoofedLevel = value;
    }

    internal static Platforms SpoofedPlatform
    {
        get => _spoofedPlatform;
        set
        {
            _spoofedPlatform = value;
            _cachedPlatformName = null;
        }
    }

    internal static string SpoofedPlatformName { get; set; } = "";

    internal static string SpoofedFriendCode
    {
        get => _spoofedFriendCode;
        set => _spoofedFriendCode = value ?? "";
    }

    public static uint GetEffectiveLevel() => SpoofedLevel;

    internal static void SetLevel(uint level)
    {
        SpoofedLevel = level;
        ApplyLevelSpoof();
    }

    internal static void ApplyLevelSpoof()
    {
        if (!EnableLevelSpoof) return;
        try
        {
            if (DataManager.Player == null || DataManager.Player.Stats == null)
            {
                Debug.LogWarning("[SpoofingService] DataManager not available");
                return;
            }

            if (!_levelCached)
            {
                _originalLevel = DataManager.Player.Stats.Level;
                _levelCached = true;
                Debug.Log($"[SpoofingService] Level original: {_originalLevel}");
            }

            uint targetLevel = GetEffectiveLevel();
            uint currentLevel = DataManager.Player.Stats.Level;
            if (targetLevel > 0 && targetLevel != currentLevel)
            {
                DataManager.Player.Stats.Level = targetLevel;
                DataManager.Player.Save();
                Debug.Log($"[SpoofingService] Level local: {currentLevel} -> {targetLevel}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[SpoofingService] Level error: " + e.Message);
        }
    }

    internal static void RestoreLevel()
    {
        if (!_levelCached) return;
        try
        {
            if (DataManager.Player != null && DataManager.Player.Stats != null)
            {
                DataManager.Player.Stats.Level = _originalLevel;
                DataManager.Player.Save();
                Debug.Log($"[SpoofingService] Level restored: {_originalLevel}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[SpoofingService] Error restoring: " + e.Message);
        }
    }

    internal static void ApplyPlatformSpoof(PlatformSpecificData data)
    {
        if (!EnablePlatformSpoof || data == null) return;
        try
        {
            data.Platform = SpoofedPlatform;

            if (!string.IsNullOrEmpty(SpoofedPlatformName))
            {
                data.PlatformName = SpoofedPlatformName;
            }
            else
            {
                if (_cachedPlatformName == null || _cachedPlatformType != SpoofedPlatform)
                {
                    _cachedPlatformName = GetDefaultPlatformName(SpoofedPlatform);
                    _cachedPlatformType = SpoofedPlatform;
                }
                data.PlatformName = _cachedPlatformName;
            }

            switch (SpoofedPlatform)
            {
                case Platforms.StandaloneWin10:
                case Platforms.Xbox:
                    data.XboxPlatformId = GenerateFakePlatformId();
                    break;
                case Platforms.Playstation:
                    data.PsnPlatformId = GenerateFakePlatformId();
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[SpoofingService] Platform error: " + e.Message);
        }
    }

    private static string GetDefaultPlatformName(Platforms platform)
    {
        return platform switch
        {
            Platforms.StandaloneEpicPC or Platforms.StandaloneSteamPC or Platforms.StandaloneMac
                or Platforms.StandaloneWin10 or Platforms.StandaloneItch or Platforms.Xbox
                => $"Player{_rng.Next(1000, 99999)}",
            _ => "",
        };
    }

    private static ulong GenerateFakePlatformId()
    {
        byte[] buf = new byte[8];
        _rng.NextBytes(buf);
        return BitConverter.ToUInt64(buf, 0) & 281474976710655UL;
    }

    internal static void ApplyFriendCodeSpoof()
    {
        if (!EnableFriendCodeSpoof) return;
        try
        {
            if (!_friendCodeCached)
            {
                try { _originalFriendCode = DestroyableSingleton<EOSManager>.Instance.FriendCode ?? ""; }
                catch { }
                _friendCodeCached = true;
            }

            string code = !string.IsNullOrEmpty(SpoofedFriendCode) ? SpoofedFriendCode : GenerateRandomFriendCode();
            try { DestroyableSingleton<EOSManager>.Instance.FriendCode = code; }
            catch { }
            if (PlayerControl.LocalPlayer != null)
            {
                PlayerControl.LocalPlayer.FriendCode = code;
            }
            Debug.Log("[SpoofingService] FriendCode spoofed to: " + code);
        }
        catch (Exception e)
        {
            Debug.LogError("[SpoofingService] FriendCode error: " + e.Message);
        }
    }

    public static string GenerateRandomFriendCode()
    {
        string[] words = { "cosmic", "stellar", "nebula", "astral", "lunar", "solar", "vortex", "plasma", "photon", "quasar" };
        string[] suffixes = { "flux", "wave", "beam", "core", "node", "link", "zone", "pulse", "spark", "glow" };
        return $"{words[_rng.Next(words.Length)]}{suffixes[_rng.Next(suffixes.Length)]}#{_rng.Next(1000, 9999)}";
    }

    internal static void RestoreFriendCode()
    {
        if (!_friendCodeCached) return;
        try
        {
            try { DestroyableSingleton<EOSManager>.Instance.FriendCode = _originalFriendCode; }
            catch { }
            if (PlayerControl.LocalPlayer != null)
            {
                PlayerControl.LocalPlayer.FriendCode = _originalFriendCode;
            }
        }
        catch { }
    }

    private static void CacheOriginalIdentity()
    {
        if (_identityCached) return;
        try
        {
            PlayerControl p = PlayerControl.LocalPlayer;
            if (p == null || p.Data == null) return;
            _originalName = p.Data.PlayerName ?? "";
            _originalColor = (byte)p.Data.DefaultOutfit.ColorId;
            _originalHat = p.Data.DefaultOutfit.HatId ?? "";
            _originalSkin = p.Data.DefaultOutfit.SkinId ?? "";
            _originalPet = p.Data.DefaultOutfit.PetId ?? "";
            _originalVisor = p.Data.DefaultOutfit.VisorId ?? "";
            _identityCached = true;
        }
        catch { }
    }

    internal static void ApplyIdentityShuffle()
    {
        try
        {
            PlayerControl p = PlayerControl.LocalPlayer;
            if (p == null || p.Data == null) return;

            CacheOriginalIdentity();

            if (EnableShuffleColor) RandomizeColor(p);
            if (EnableShuffleCosmetics) RandomizeCosmetics(p);
        }
        catch (Exception e)
        {
            Debug.LogError("[SpoofingService] IdentityShuffle: " + e.Message);
        }
    }

    private static void RandomizeColor(PlayerControl p)
    {
        try
        {
            List<byte> available = GetAvailableColors(p);
            if (available.Count != 0)
            {
                p.CmdCheckColor(available[_rng.Next(available.Count)]);
            }
        }
        catch { }
    }

    private static List<byte> GetAvailableColors(PlayerControl local)
    {
        HashSet<byte> taken = new();
        try
        {
            if (GameData.Instance != null)
            {
                foreach (NetworkedPlayerInfo info in GameData.Instance.AllPlayers)
                {
                    if (info != null && !info.Disconnected && info.PlayerId != local.PlayerId)
                    {
                        taken.Add((byte)info.DefaultOutfit.ColorId);
                    }
                }
            }
        }
        catch { }

        int maxColors = 18;
        try { maxColors = Palette.PlayerColors.Length; }
        catch { }

        List<byte> result = new();
        for (byte i = 0; i < maxColors; i++)
        {
            if (!taken.Contains(i)) result.Add(i);
        }
        return result;
    }

    private static void RandomizeCosmetics(PlayerControl p)
    {
        try
        {
            HatManager hm = DestroyableSingleton<HatManager>.Instance;
            if (hm == null) return;

            Il2CppReferenceArray<HatData> hats = hm.allHats;
            if (hats != null && hats.Length > 0)
            {
                HatData hat = hats[_rng.Next(hats.Length)];
                if (hat != null && !string.IsNullOrEmpty(hat.ProdId))
                    p.RpcSetHat(hat.ProdId);
            }

            Il2CppReferenceArray<SkinData> skins = hm.allSkins;
            if (skins != null && skins.Length > 0)
            {
                SkinData skin = skins[_rng.Next(skins.Length)];
                if (skin != null && !string.IsNullOrEmpty(skin.ProdId))
                    p.RpcSetSkin(skin.ProdId);
            }

            Il2CppReferenceArray<PetData> pets = hm.allPets;
            if (pets != null && pets.Length > 0)
            {
                PetData pet = pets[_rng.Next(pets.Length)];
                if (pet != null && !string.IsNullOrEmpty(pet.ProdId))
                    p.RpcSetPet(pet.ProdId);
            }

            Il2CppReferenceArray<VisorData> visors = hm.allVisors;
            if (visors != null && visors.Length > 0)
            {
                VisorData visor = visors[_rng.Next(visors.Length)];
                if (visor != null && !string.IsNullOrEmpty(visor.ProdId))
                    p.RpcSetVisor(visor.ProdId);
            }
        }
        catch { }
    }

    internal static void RestoreIdentity()
    {
        if (!_identityCached) return;
        try
        {
            PlayerControl p = PlayerControl.LocalPlayer;
            if (p == null) return;
            p.CmdCheckName(_originalName);
            p.CmdCheckColor(_originalColor);
            p.RpcSetHat(_originalHat);
            p.RpcSetSkin(_originalSkin);
            p.RpcSetPet(_originalPet);
            p.RpcSetVisor(_originalVisor);
        }
        catch { }
    }

    internal static bool IsAnyShuffleEnabled() =>
        EnableShuffleName || EnableShuffleColor || EnableShuffleCosmetics;

    public static string PlatformToString(Platforms platform) => platform switch
    {
        Platforms.StandaloneEpicPC => "Epic",
        Platforms.StandaloneSteamPC => "Steam",
        Platforms.StandaloneMac => "Mac",
        Platforms.StandaloneWin10 => "Microsoft Store",
        Platforms.StandaloneItch => "Itch.io",
        Platforms.IPhone => "iPhone",
        Platforms.Android => "Android",
        Platforms.Switch => "Switch",
        Platforms.Xbox => "Xbox",
        Platforms.Playstation => "PlayStation",
        _ => platform.ToString(),
    };

    public static Platforms[] GetAllPlatforms() => new Platforms[]
    {
        Platforms.StandaloneEpicPC, Platforms.StandaloneSteamPC, Platforms.StandaloneMac,
        Platforms.StandaloneWin10, Platforms.StandaloneItch, Platforms.IPhone,
        Platforms.Android, Platforms.Switch, Platforms.Xbox, Platforms.Playstation,
    };

    public static string GetStatus()
    {
        string status = "";
        if (EnableLevelSpoof) status += $"Lv{GetEffectiveLevel()} ";
        if (EnablePlatformSpoof) status += PlatformToString(SpoofedPlatform) + " ";
        if (EnableFriendCodeSpoof) status += "FC ";
        if (IsAnyShuffleEnabled()) status += "Shuffle ";
        return string.IsNullOrEmpty(status) ? "OFF" : status.Trim();
    }

    internal static void DisableAll()
    {
        EnableLevelSpoof = false;
        EnablePlatformSpoof = false;
        EnableFriendCodeSpoof = false;
        EnableShuffleName = false;
        EnableShuffleColor = false;
        EnableShuffleCosmetics = false;
        RestoreLevel();
        RestoreFriendCode();
        RestoreIdentity();
    }

    internal static void MarkForReapplication()
    {
        ApplyLevelSpoof();
        if (EnableFriendCodeSpoof) ApplyFriendCodeSpoof();
    }

    public static int GetActiveCount()
    {
        int count = 0;
        if (EnableLevelSpoof) count++;
        if (EnablePlatformSpoof) count++;
        if (EnableFriendCodeSpoof) count++;
        if (IsAnyShuffleEnabled()) count++;
        return count;
    }

    private static bool _enableLevelSpoof;
    private static bool _enablePlatformSpoof;
    private static bool _enableFriendCodeSpoof;
    private static bool _enableShuffleName;
    private static bool _enableShuffleColor;
    private static bool _enableShuffleCosmetics;

    private static uint _spoofedLevel = 100;
    private static Platforms _spoofedPlatform = Platforms.StandaloneSteamPC;
    private static string _spoofedFriendCode = "";

    private static uint _originalLevel;
    private static bool _levelCached;
    private static string _originalFriendCode = "";
    private static bool _friendCodeCached;
    private static string _cachedPlatformName;
    private static Platforms _cachedPlatformType = Platforms.Unknown;

    private static readonly System.Random _rng = new();

    private static string _originalName = "";
    private static byte _originalColor;
    private static string _originalHat = "";
    private static string _originalSkin = "";
    private static string _originalPet = "";
    private static string _originalVisor = "";
    private static bool _identityCached;
}
