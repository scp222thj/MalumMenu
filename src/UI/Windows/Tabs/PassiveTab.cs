using UnityEngine;

namespace MalumMenu;

public class PassiveTab : ITab
{
    public string name => "Passive";

    private uint _spoofedLevel = 100;
    private int _selectedPlatformIndex = 1;
    private string[] _platformNames = null;
    private bool _initialized;

    private void EnsureInitialized()
    {
        if (_initialized) return;
        _initialized = true;

        var platforms = SpoofingService.GetAllPlatforms();
        _platformNames = new string[platforms.Length];
        for (int i = 0; i < platforms.Length; i++)
        {
            _platformNames[i] = SpoofingService.PlatformToString(platforms[i]);
        }

        uint currentLevel = SpoofingService.SpoofedLevel;
        _spoofedLevel = currentLevel > 0 ? currentLevel : 100;

        Platforms currentPlatform = SpoofingService.SpoofedPlatform;
        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i] == currentPlatform)
            {
                _selectedPlatformIndex = i;
                break;
            }
        }
    }

    private Vector2 _scrollPosition = Vector2.zero;

    public void Draw()
    {
        EnsureInitialized();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Height(MenuUI.windowHeight - 70));

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawGeneral();
        DrawSpoofing();

        GUILayout.EndVertical();

        GUILayout.EndScrollView();
    }

    private void DrawGeneral()
    {
        CheatToggles.freeCosmetics = GUILayout.Toggle(CheatToggles.freeCosmetics, " Free Cosmetics");

        CheatToggles.avoidPenalties = GUILayout.Toggle(CheatToggles.avoidPenalties, " Avoid Penalties");

        CheatToggles.unlockFeatures = GUILayout.Toggle(CheatToggles.unlockFeatures, " Unlock Extra Features");

        CheatToggles.copyLobbyCodeOnDisconnect = GUILayout.Toggle(CheatToggles.copyLobbyCodeOnDisconnect, " Copy Lobby Code on Disconnect");

        CheatToggles.spoofAprilFoolsDate = GUILayout.Toggle(CheatToggles.spoofAprilFoolsDate, " Spoof Date to April 1st");

        CheatToggles.impostorCanDoTasks = GUILayout.Toggle(CheatToggles.impostorCanDoTasks, " Impostor Can Do Tasks");

        bool newImmortality = GUILayout.Toggle(CheatToggles.immortality, " Immortality");
        if (newImmortality != CheatToggles.immortality)
        {
            ImmortalityService.ToggleImmortality();
        }
    }

    private void DrawSpoofing()
    {
        bool newSpoofLevel = GUILayout.Toggle(CheatToggles.spoofLevel, " Spoof Level");
        if (newSpoofLevel != CheatToggles.spoofLevel)
        {
            CheatToggles.spoofLevel = newSpoofLevel;
            SpoofingService.EnableLevelSpoof = newSpoofLevel;
            if (newSpoofLevel)
            {
                SpoofingService.SpoofedLevel = _spoofedLevel;
            }
        }
        if (CheatToggles.spoofLevel)
        {
            GUILayout.Label($"  Level: {(uint)_spoofedLevel}", GUILayout.Width(200));
            float newLevel = GUILayout.HorizontalSlider((float)_spoofedLevel, 0f, 1000f, GUILayout.Width(250f));
            uint snapped = (uint)(Mathf.Round(newLevel / 10f) * 10f);
            if (snapped != _spoofedLevel)
            {
                _spoofedLevel = snapped;
                SpoofingService.SpoofedLevel = _spoofedLevel;
            }
        }

        bool newSpoofPlatform = GUILayout.Toggle(CheatToggles.spoofPlatform, " Spoof Platform");
        if (newSpoofPlatform != CheatToggles.spoofPlatform)
        {
            CheatToggles.spoofPlatform = newSpoofPlatform;
            SpoofingService.EnablePlatformSpoof = newSpoofPlatform;
            if (newSpoofPlatform)
            {
                var platforms = SpoofingService.GetAllPlatforms();
                if (_selectedPlatformIndex >= 0 && _selectedPlatformIndex < platforms.Length)
                {
                    SpoofingService.SpoofedPlatform = platforms[_selectedPlatformIndex];
                }
            }
        }
        if (CheatToggles.spoofPlatform)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("  Platform:", GUILayout.Width(80));
            if (GUILayout.Button("<", GUILayout.Width(30)))
            {
                _selectedPlatformIndex = (_selectedPlatformIndex - 1 + _platformNames.Length) % _platformNames.Length;
                SpoofingService.SpoofedPlatform = SpoofingService.GetAllPlatforms()[_selectedPlatformIndex];
            }
            GUILayout.Label(_platformNames[_selectedPlatformIndex], GUILayout.Width(120));
            if (GUILayout.Button(">", GUILayout.Width(30)))
            {
                _selectedPlatformIndex = (_selectedPlatformIndex + 1) % _platformNames.Length;
                SpoofingService.SpoofedPlatform = SpoofingService.GetAllPlatforms()[_selectedPlatformIndex];
            }
            GUILayout.EndHorizontal();
        }

        bool newSpoofFriendCode = GUILayout.Toggle(CheatToggles.spoofFriendCode, " Spoof Friend Code");
        if (newSpoofFriendCode != CheatToggles.spoofFriendCode)
        {
            CheatToggles.spoofFriendCode = newSpoofFriendCode;
            SpoofingService.EnableFriendCodeSpoof = newSpoofFriendCode;
            if (newSpoofFriendCode)
            {
                SpoofingService.SpoofedFriendCode = MalumMenu.spoofFriendCode?.Value ?? "";
            }
        }

        bool newShuffleColor = GUILayout.Toggle(CheatToggles.shuffleColor, " Shuffle Color");
        if (newShuffleColor != CheatToggles.shuffleColor)
        {
            CheatToggles.shuffleColor = newShuffleColor;
            SpoofingService.EnableShuffleColor = newShuffleColor;
        }

        bool newShuffleCosmetics = GUILayout.Toggle(CheatToggles.shuffleCosmetics, " Shuffle Cosmetics");
        if (newShuffleCosmetics != CheatToggles.shuffleCosmetics)
        {
            CheatToggles.shuffleCosmetics = newShuffleCosmetics;
            SpoofingService.EnableShuffleCosmetics = newShuffleCosmetics;
        }
    }
}
