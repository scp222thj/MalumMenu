using BepInEx.Configuration;

public sealed class PluginSettings
{
    public ConfigEntry<string> MenuKeybind;
    public ConfigEntry<string> MenuHtmlColor;
    public ConfigEntry<bool> MenuOpenOnMouse;
    public ConfigEntry<bool> MenuKeepSubwindowsOpen;
    public ConfigEntry<string> SpoofLevel;
    public ConfigEntry<string> SpoofPlatform;
    public ConfigEntry<bool> SpoofDeviceId;
    public ConfigEntry<bool> NoTelemetry;
    public ConfigEntry<string> GuestFriendCode;
    public ConfigEntry<bool> GuestMode;
    public ConfigEntry<bool> AutoLoadProfile;
    public ConfigEntry<string> ConfigEditor;

    public void Bind(ConfigFile configFile)
    {
        MenuKeybind = configFile.Bind(
            "MalumMenu.GUI",
            "Keybind",
            "Delete",
            "The keyboard key used to toggle the GUI on and off. List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html"
        );

        MenuHtmlColor = configFile.Bind(
            "MalumMenu.GUI",
            "Color",
            "",
            "A custom color for your MalumMenu GUI. Supports html color codes"
        );

        MenuOpenOnMouse = configFile.Bind(
            "MalumMenu.GUI",
            "OpenOnMouse",
            false,
            "When enabled, the MalumMenu GUI will always be opened at the current mouse position"
        );

        MenuKeepSubwindowsOpen = configFile.Bind(
            "MalumMenu.GUI",
            "KeepSubwindowsOpen",
            false,
            "When enabled, closing the MalumMenu GUI will not automatically close its subwindows"
        );

        AutoLoadProfile = configFile.Bind(
            "MalumMenu.Profile",
            "AutoLoadProfile",
            false,
            "When enabled, your saved keybind and toggle profile will be automatically loaded at game startup"
        );

        ConfigEditor = configFile.Bind(
            "MalumMenu.Config",
            "ConfigEditor",
            "notepad.exe",
            "The program used to open the config file when using the Open Config toggle. Can be any executable, but using a text editor is recommended"
        );

        // GuestMode config settings are commented out as the cheats are broken in latest updates

        // GuestMode = configFile.Bind(
        //    "MalumMenu.GuestMode",
        //    "GuestMode",
        //    false,
        //    "When enabled, a new guest account will generate every time you start the game, allowing you to bypass account bans and PUID detection"
        // );

        // GuestFriendCode = configFile.Bind(
        //    "MalumMenu.GuestMode",
        //    "FriendName",
        //    "",
        //    "The username that will be used when setting a friend code for your guest account. IMPORTANT: Can only be used with GuestMode, needs to be ≤ 10 characters, and cannot include special characters/discriminator (#1234)"
        // );

        SpoofLevel = configFile.Bind(
            "MalumMenu.Spoofing",
            "Level",
            "",
            "A custom player level to display to others in online games to hide your actual platform. IMPORTANT: Custom levels can only be within 1 and 100001. Decimal numbers will not work"
        );

        SpoofPlatform = configFile.Bind(
            "MalumMenu.Spoofing",
            "Platform",
            "",
            "A custom gaming platform to display to others in online lobbies to hide your actual platform. List of supported platforms: https://skeld.js.org/enums/_skeldjs_constant.Platform.html"
        );

        SpoofDeviceId = configFile.Bind(
            "MalumMenu.Privacy",
            "HideDeviceId",
            true,
            "When enabled, it will hide your unique deviceId from Among Us, which could potentially help bypass hardware bans in the future"
        );

        NoTelemetry = configFile.Bind(
            "MalumMenu.Privacy",
            "NoTelemetry",
            true,
            "When enabled, it will stop Among Us from collecting analytics of your games and sending them to Innersloth using Unity Analytics"
        );
    }
}
