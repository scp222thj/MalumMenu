using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BepInEx;
using UnityEngine.SceneManagement;

namespace MalumMenu;

public class MenuUI : MonoBehaviour
{
    public static int windowHeight = 550;
    public static int windowWidth = 700;
    public static Rect windowRect;

    public static bool isGUIActive = false;
    private List<ITab> _tabs = new();
    private int _selectedTab;
    public static float hue;
    private static string _lastDrawError = string.Empty;
    private static int _lastDumpFrame = -100000;

    private void Start()
    {
        // Add all tabs on start
        _tabs.Add(new MovementTab());
        _tabs.Add(new ESPTab());
        _tabs.Add(new RolesTab());
        _tabs.Add(new ShipTab());
        _tabs.Add(new ChatTab());
        _tabs.Add(new AnimationsTab());
        _tabs.Add(new ConsoleTab());
        _tabs.Add(new HostOnlyTab());
        _tabs.Add(new PassiveTab());
        _tabs.Add(new ModesTab());
        _tabs.Add(new ConfigTab());
        // _tabs.Add(new OverloadTab());

        // Instantiate 2D area of MenuUI
        windowRect = new(
            Screen.width / 2f - windowWidth / 2f,
            Screen.height / 2f - windowHeight / 2f,
            windowWidth,
            windowHeight
        );
    }

    public void InitStyles()
    {
        if (GUI.skin == null)
        {
            return;
        }

        GUI.skin.toggle.fontSize = GUI.skin.button.fontSize = GUI.skin.label.fontSize = 15;
        UIHelpers.EnsureSkinFont();
        UIHelpers.EnsureSkinTextColors();
    }

    private void Update()
    {
        try
        {
            UpdateCheats();
        }
        catch (Exception ex)
        {
            WiniLogListener.Record(nameof(MenuUI) + ".Update", ex);
        }
    }

    private void UpdateCheats()
    {
        if (Input.GetKeyDown(Utils.StringToKeycode(MalumMenu.menuKeybind.Value)))
        {
            // Enable or disable GUI with DELETE key
            isGUIActive = !isGUIActive;

            if (MalumMenu.menuOpenOnMouse.Value)
            {
                // Teleport the window to the mouse for immediate use
                Vector2 mousePosition = Input.mousePosition;
                windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
            }
        }

        if (CheatToggles.rgbMode)
        {
            hue += Time.deltaTime * 0.3f; // Adjust speed of color change, higher multiplier = faster
            if (hue > 1f) hue -= 1f; // Loop hue back to 0 when it exceeds 1
        }

        if (CheatToggles.stealthMode != MalumMenu.inStealthMode)
        {
            MalumMenu.inStealthMode = CheatToggles.stealthMode;

            Scene scene = SceneManager.GetActiveScene();

            if (scene.name == "MainMenu" || scene.name == "MatchMaking")
            {
                SceneManager.LoadScene(scene.name);
            }
        }

        if (CheatToggles.panicMode) Utils.Panic();

        if (ModManager.InstanceExists)
        {
            var stamp = ModManager.Instance.ModStamp;
            if (stamp) stamp.enabled = !(MalumMenu.inStealthMode || MalumMenu.isPanicked);
        }

        if (CheatToggles.openConfig)
        {
            Utils.OpenConfigFile();
            CheatToggles.openConfig = false;
        }

        if (CheatToggles.reloadConfig)
        {
            MalumMenu.Plugin.Config.Reload();
            CheatToggles.reloadConfig = false;
        }

        if (CheatToggles.saveProfile)
        {
            CheatToggles.saveProfile = false; // Disable first to avoid saving it to profile
            CheatToggles.SaveTogglesToProfile();
        }

        if (CheatToggles.loadProfile)
        {
            CheatToggles.LoadTogglesFromProfile();
            CheatToggles.loadProfile = false;
        }

        // Some cheats only work if the LocalPlayer exists, so they are turned off if it does not
        if(!Utils.isPlayer)
        {
            CheatToggles.setFakeRole = false;
            CheatToggles.setFakeAlive = false;
            CheatToggles.killAll = false;
            CheatToggles.telekillPlayer = false;
            CheatToggles.killAllCrew = false;
            CheatToggles.killAllImps = false;
            CheatToggles.teleportPlayer = false;
            CheatToggles.spectate = false;
            CheatToggles.freecam = false;
            CheatToggles.killPlayer = false;
            CheatToggles.callMeeting = false;

            if (CheatToggles.runOverload)
            {
                OverloadUI.StopOverload();
            }
        }

        // Some cheats only work if the ship exists, so they are turned off if it does not
        if(!Utils.isShip)
        {
            CheatToggles.sabotageMap = false;
            CheatToggles.unfixableLights = false;
            CheatToggles.completeMyTasks = false;
            CheatToggles.kickVents = false;
            CheatToggles.reportBody = false;
            CheatToggles.closeMeeting = false;
            CheatToggles.reactorSab = false;
            CheatToggles.oxygenSab = false;
            CheatToggles.commsSab = false;
            CheatToggles.elecSab = false;
            CheatToggles.mushSab = false;
            CheatToggles.closeAllDoors = false;
            CheatToggles.openAllDoors = false;
            CheatToggles.spamCloseAllDoors = false;
            CheatToggles.spamOpenAllDoors = false;
            CheatToggles.mushSpore = false;

            MalumCheats.StopShipAnimCheats();
        }

        if(!Utils.isHost && !Utils.isFreePlay)
        {
            CheatToggles.killAll = false;
            CheatToggles.telekillPlayer = false;
            CheatToggles.killAllCrew = false;
            CheatToggles.killAllImps = false;
            CheatToggles.killPlayer = false;
            CheatToggles.ejectPlayer = false;
            CheatToggles.noKillCd = false;
            CheatToggles.killAnyone = false;
            CheatToggles.killVanished = false;
            CheatToggles.forceStartGame = false;
            CheatToggles.skipMeeting = false;
            CheatToggles.voteImmune = false;
            CheatToggles.noGameEnd = false;
            CheatToggles.showProtectMenu = false;
            CheatToggles.showRolesMenu = false;
            CheatToggles.noOptionsLimits = false;
        }

        // Some cheats only work if in a meeting, so they are turned off if it does not
        if (!Utils.isMeeting)
        {
            CheatToggles.skipMeeting = false;
            CheatToggles.ejectPlayer = false;
        }
    }

    public void OnGUI()
    {
        if (!isGUIActive || MalumMenu.isPanicked) return;

        InitStyles();

        Color savedBackground = GUI.backgroundColor;
        Color savedContent = GUI.contentColor;
        Color savedColor = GUI.color;

        UIHelpers.ApplyUIColor();
        GUIStylePreset.RefreshButtonTextColors();

        Color appliedBackground = GUI.backgroundColor;
        Color appliedContent = GUI.contentColor;
        Color appliedColor = GUI.color;

        windowRect = GUI.Window((int)WindowId.MenuUI, windowRect, (GUI.WindowFunction)WindowFunction, "MalumMenu v" + MalumMenu.malumVersion);

        GUI.backgroundColor = savedBackground;
        GUI.contentColor = savedContent;
        GUI.color = savedColor;

        WriteGuiDebugLog(appliedBackground, appliedContent, appliedColor);
    }

    public void WindowFunction(int windowID)
    {
        GUILayout.BeginHorizontal();

        // Left tab selector (15% width)
        GUILayout.BeginVertical(GUILayout.Width(windowWidth * 0.15f));
        for (var i = 0; i < _tabs.Count; i++)
        {
            Color standardBackground = GUI.backgroundColor;
            Color standardContent = GUI.contentColor;
            Color standardColor = GUI.color;

            if (_selectedTab == i)
            {
                GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            }

            GUIStylePreset.RefreshButtonTextColors();

            if (GUILayout.Button(_tabs[i].name, GUIStylePreset.TabButton, GUILayout.Height(35)))
                _selectedTab = i;

            GUI.backgroundColor = standardBackground;
            GUI.contentColor = standardContent;
            GUI.color = standardColor;

        }
        GUILayout.EndVertical();

        // Vertical separator line + invisible space to create gap between the tab selector and the content
        GUILayout.Box("", GUIStylePreset.Separator, GUILayout.Width(1f), GUILayout.ExpandHeight(true));
        GUILayout.Space(10f);

        // Right tab content and controls (85% width)
        GUILayout.BeginVertical(GUILayout.Width(windowWidth * 0.85f));

        // Tab-specific content
        if (_selectedTab >= 0 && _selectedTab < _tabs.Count)
        {
            GUILayout.Label(_tabs[_selectedTab].name, GUIStylePreset.TabTitle);
            try
            {
                _tabs[_selectedTab].Draw();
            }
            catch (Exception ex)
            {
                _lastDrawError = ex.ToString();
            }
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUI.DragWindow();
    }

    private void WriteGuiDebugLog(Color appliedBackground, Color appliedContent, Color appliedColor)
    {
        try
        {
            bool forced = false;
            try
            {
                forced = Input.GetKeyDown(KeyCode.F9);
            }
            catch
            {
                forced = false;
            }

            int frame = 0;
            try
            {
                frame = Time.frameCount;
            }
            catch
            {
                frame = 0;
            }

            if (!forced && frame - _lastDumpFrame < 600)
            {
                return;
            }

            _lastDumpFrame = frame;

            StringBuilder output = new StringBuilder();
            output.AppendLine("Timestamp: " + DateTime.Now.ToString("O"));
            output.AppendLine("Frame: " + frame);
            output.AppendLine("Forced: " + forced);

            try
            {
                output.AppendLine("ApplicationVersion: " + Application.version);
            }
            catch (Exception ex)
            {
                output.AppendLine("ApplicationVersionError: " + ex.GetType().FullName);
            }

            try
            {
                output.AppendLine("Screen: " + Screen.width + "x" + Screen.height + " fullscreen=" + Screen.fullScreen);
            }
            catch (Exception ex)
            {
                output.AppendLine("ScreenError: " + ex.GetType().FullName);
            }

            output.AppendLine("IsGUIActive: " + isGUIActive);
            output.AppendLine("SelectedTab: " + _selectedTab);

            try
            {
                StringBuilder names = new StringBuilder();
                for (int i = 0; i < _tabs.Count; i++)
                {
                    if (i > 0)
                    {
                        names.Append(",");
                    }

                    names.Append(_tabs[i].name);
                }

                output.AppendLine("Tabs: " + names);
            }
            catch (Exception ex)
            {
                output.AppendLine("TabsError: " + ex.GetType().FullName);
            }

            output.AppendLine("WindowRect: " + windowRect.ToString());

            try
            {
                output.AppendLine("ConfigColor: [" + MalumMenu.menuHtmlColor.Value + "] rgbMode=" + CheatToggles.rgbMode + " hue=" + hue);
            }
            catch (Exception ex)
            {
                output.AppendLine("ConfigError: " + ex.GetType().FullName);
            }

            output.AppendLine("AppliedBackground: " + appliedBackground.ToString());
            output.AppendLine("AppliedContent: " + appliedContent.ToString());
            output.AppendLine("AppliedColor: " + appliedColor.ToString());

            try
            {
                output.AppendLine("LiveBackground: " + GUI.backgroundColor.ToString());
                output.AppendLine("LiveContent: " + GUI.contentColor.ToString());
                output.AppendLine("LiveColor: " + GUI.color.ToString());
                output.AppendLine("GUIEnabled: " + GUI.enabled);
            }
            catch (Exception ex)
            {
                output.AppendLine("LiveGUIError: " + ex.GetType().FullName);
            }

            try
            {
                var skin = GUI.skin;
                output.AppendLine("SkinNull: " + (skin == null));

                if (skin != null)
                {
                    output.AppendLine("SkinName: " + skin.name);
                    output.AppendLine("SkinFont: " + DescribeFont(skin.font));
                    output.AppendLine("SkinLabel: " + DescribeStyle(skin.label));
                    output.AppendLine("SkinButton: " + DescribeStyle(skin.button));
                    output.AppendLine("SkinToggle: " + DescribeStyle(skin.toggle));
                    output.AppendLine("SkinWindow: " + DescribeStyle(skin.window));
                    output.AppendLine("SkinBox: " + DescribeStyle(skin.box));
                }
            }
            catch (Exception ex)
            {
                output.AppendLine("SkinError: " + ex);
            }

            try
            {
                output.AppendLine("FallbackFont: " + DescribeFont(GUIStylePreset.FallbackFont));
                output.AppendLine("TabButton: " + DescribeStyle(GUIStylePreset.TabButton));
                output.AppendLine("TabTitle: " + DescribeStyle(GUIStylePreset.TabTitle));
                output.AppendLine("TabSubtitle: " + DescribeStyle(GUIStylePreset.TabSubtitle));
                output.AppendLine("NormalButton: " + DescribeStyle(GUIStylePreset.NormalButton));
                output.AppendLine("NormalToggle: " + DescribeStyle(GUIStylePreset.NormalToggle));
            }
            catch (Exception ex)
            {
                output.AppendLine("PresetError: " + ex);
            }

            try
            {
                var installed = Font.GetOSInstalledFontNames();
                output.AppendLine("OSFontCount: " + installed.Length);
                int shown = installed.Length < 60 ? installed.Length : 60;
                StringBuilder listed = new StringBuilder();
                for (int i = 0; i < shown; i++)
                {
                    if (i > 0)
                    {
                        listed.Append(" | ");
                    }

                    listed.Append(installed[i]);
                }

                output.AppendLine("OSFonts: " + listed);
            }
            catch (Exception ex)
            {
                output.AppendLine("OSFontsError: " + ex.GetType().FullName);
            }

            try
            {
                var fallback = GUIStylePreset.FallbackFont;
                output.AppendLine("FallbackHasA: " + (fallback != null && fallback.HasCharacter('A')));
                output.AppendLine("FallbackHasg: " + (fallback != null && fallback.HasCharacter('g')));
                output.AppendLine("FallbackHas0: " + (fallback != null && fallback.HasCharacter('0')));
            }
            catch (Exception ex)
            {
                output.AppendLine("FallbackCheckError: " + ex.GetType().FullName);
            }

            if (string.IsNullOrEmpty(_lastDrawError))
            {
                output.AppendLine("LastDrawError: none");
            }
            else
            {
                output.AppendLine("LastDrawError: " + _lastDrawError);
            }

            string gameRoot = string.Empty;
            try
            {
                gameRoot = Paths.GameRootPath;
                output.AppendLine("GameRoot: " + gameRoot);
            }
            catch (Exception ex)
            {
                output.AppendLine("GameRootError: " + ex.GetType().FullName);
            }

            string path = string.Empty;
            try
            {
                path = Path.Combine(gameRoot, "MalumMenu-GUI-debug.log");
            }
            catch
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "MalumMenu-GUI-debug.log");
            }

            output.AppendLine("LogPath: " + path);
            File.WriteAllText(path, output.ToString());
        }
        catch
        {
        }
    }

    private static string DescribeFont(Font font)
    {
        try
        {
            if (font == null)
            {
                return "null";
            }

            return font.name + " dynamic=" + font.dynamic;
        }
        catch (Exception ex)
        {
            return "error:" + ex.GetType().FullName;
        }
    }

    private static string DescribeStyle(GUIStyle style)
    {
        try
        {
            if (style == null)
            {
                return "null";
            }

            string fontPart = DescribeFont(style.font);
            Color text = style.normal.textColor;
            bool hasBackground = style.normal.background != null;
            return "fontSize=" + style.fontSize + " fontStyle=" + style.fontStyle + " font=" + fontPart + " textRGBA=" + text.r + "," + text.g + "," + text.b + "," + text.a + " hasBackground=" + hasBackground;
        }
        catch (Exception ex)
        {
            return "error:" + ex.GetType().FullName;
        }
    }
}
