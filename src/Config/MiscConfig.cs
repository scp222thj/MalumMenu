using BepInEx.Configuration;

namespace MalumMenu;

public static class MiscConfig
{
    public static ConfigEntry<bool> CustomNameEnabled { get; private set; }
    public static ConfigEntry<string> CustomNameText { get; private set; }
    public static ConfigEntry<bool> NameBold { get; private set; }
    public static ConfigEntry<bool> NameItalic { get; private set; }
    public static ConfigEntry<bool> NameUnderline { get; private set; }
    public static ConfigEntry<bool> NameStrikethrough { get; private set; }
    public static ConfigEntry<int> NameSizePercent { get; private set; }
    public static ConfigEntry<int> NameColorMode { get; private set; }
    public static ConfigEntry<string> NameColorHex { get; private set; }
    public static ConfigEntry<string> NameGradientStartHex { get; private set; }
    public static ConfigEntry<string> NameGradientEndHex { get; private set; }
    public static ConfigEntry<int> NameGradientStopCount { get; private set; }
    public static ConfigEntry<string> NamePulseColor1Hex { get; private set; }
    public static ConfigEntry<string> NamePulseColor2Hex { get; private set; }
    public static ConfigEntry<bool> NameNobrEnabled { get; private set; }
    public static ConfigEntry<bool> NameCspaceEnabled { get; private set; }
    public static ConfigEntry<int> NameCspace { get; private set; }
    public static ConfigEntry<bool> NameMspaceEnabled { get; private set; }
    public static ConfigEntry<int> NameMspace { get; private set; }
    public static ConfigEntry<bool> NameVoffsetEnabled { get; private set; }
    public static ConfigEntry<int> NameVoffset { get; private set; }
    public static ConfigEntry<bool> NameRotateEnabled { get; private set; }
    public static ConfigEntry<int> NameRotate { get; private set; }
    public static ConfigEntry<bool> NameFontEnabled { get; private set; }
    public static ConfigEntry<int> NameFontIndex { get; private set; }
    public static ConfigEntry<bool> ChatColorEnabled { get; private set; }
    public static ConfigEntry<string> ChatColorHex { get; private set; }
    public static ConfigEntry<bool> ChatDarkMode { get; private set; }
    public static ConfigEntry<bool> NameColorOverrideEnabled { get; private set; }
    public static ConfigEntry<string> NameColorOverrideHex { get; private set; }
    public static ConfigEntry<bool> HideMyName { get; private set; }
    public static ConfigEntry<bool> HideMyPet { get; private set; }
    public static ConfigEntry<bool> LocalColorOverrideEnabled { get; private set; }
    public static ConfigEntry<int> LocalColorOverrideId { get; private set; }
    public static ConfigEntry<string> TextEditorInput { get; private set; }

    public static readonly string[] FontNames = new string[]
    {
        "LegacyRuntime", "LiberationSans", "NotoSans", "Impact",
        "Arial", "Courier New", "Georgia", "Times New Roman",
        "Trebuchet MS", "Verdana", "Comic Sans MS", "Custom"
    };

    public static void Initialize(ConfigFile config)
    {
        CustomNameEnabled = config.Bind("Misc.Name", "CustomNameEnabled", false, "Enable custom name display");
        CustomNameText = config.Bind("Misc.Name", "CustomNameText", "", "Custom name text");
        NameBold = config.Bind("Misc.Name", "Bold", false, "Bold name");
        NameItalic = config.Bind("Misc.Name", "Italic", false, "Italic name");
        NameUnderline = config.Bind("Misc.Name", "Underline", false, "Underline name");
        NameStrikethrough = config.Bind("Misc.Name", "Strikethrough", false, "Strikethrough name");
        NameSizePercent = config.Bind("Misc.Name", "SizePercent", 100, "Name size percentage");
        NameColorMode = config.Bind("Misc.Name", "ColorMode", 0, "Name color mode: 0=none, 1=solid, 2=gradient, 3=rainbow, 4=pulse");
        NameColorHex = config.Bind("Misc.Name", "ColorHex", "FFFFFF", "Solid name color hex");
        NameGradientStartHex = config.Bind("Misc.Name", "GradientStartHex", "FF0000", "Gradient start color");
        NameGradientEndHex = config.Bind("Misc.Name", "GradientEndHex", "0000FF", "Gradient end color");
        NameGradientStopCount = config.Bind("Misc.Name", "GradientStopCount", 3, "Gradient stop count (2-3)");
        NamePulseColor1Hex = config.Bind("Misc.Name", "PulseColor1Hex", "FF0000", "Pulse color 1");
        NamePulseColor2Hex = config.Bind("Misc.Name", "PulseColor2Hex", "00FF00", "Pulse color 2");
        NameNobrEnabled = config.Bind("Misc.Name", "NobrEnabled", false, "Enable nobr tag");
        NameCspaceEnabled = config.Bind("Misc.Name", "CspaceEnabled", false, "Enable character spacing");
        NameCspace = config.Bind("Misc.Name", "Cspace", 0, "Character spacing value");
        NameMspaceEnabled = config.Bind("Misc.Name", "MspaceEnabled", false, "Enable margin spacing");
        NameMspace = config.Bind("Misc.Name", "Mspace", 0, "Margin spacing value");
        NameVoffsetEnabled = config.Bind("Misc.Name", "VoffsetEnabled", false, "Enable vertical offset");
        NameVoffset = config.Bind("Misc.Name", "Voffset", 0, "Vertical offset value");
        NameRotateEnabled = config.Bind("Misc.Name", "RotateEnabled", false, "Enable rotation");
        NameRotate = config.Bind("Misc.Name", "Rotate", 0, "Rotation angle");
        NameFontEnabled = config.Bind("Misc.Name", "FontEnabled", false, "Enable custom font");
        NameFontIndex = config.Bind("Misc.Name", "FontIndex", 10, "Font index");
        ChatColorEnabled = config.Bind("Misc.Chat", "ColorEnabled", false, "Enable chat color override");
        ChatColorHex = config.Bind("Misc.Chat", "ColorHex", "FFFFFF", "Chat color hex");
        ChatDarkMode = config.Bind("Misc.Chat", "DarkMode", false, "Enable chat dark mode");
        NameColorOverrideEnabled = config.Bind("Misc.Visual", "NameColorOverrideEnabled", false, "Override name color");
        NameColorOverrideHex = config.Bind("Misc.Visual", "NameColorOverrideHex", "FFFFFF", "Name color override hex");
        HideMyName = config.Bind("Misc.Visual", "HideMyName", false, "Hide own name");
        HideMyPet = config.Bind("Misc.Visual", "HideMyPet", false, "Hide own pet");
        LocalColorOverrideEnabled = config.Bind("Misc.Visual", "LocalColorOverrideEnabled", false, "Override color locally");
        LocalColorOverrideId = config.Bind("Misc.Visual", "LocalColorOverrideId", 0, "Local color override ID");
        TextEditorInput = config.Bind("Misc.Name", "TextEditorInput", "Text", "Text editor preview input");
    }
}
