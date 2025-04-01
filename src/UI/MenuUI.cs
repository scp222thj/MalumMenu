using UnityEngine;
using System.Collections.Generic;

namespace MalumMenu;

    public class MenuUI : MonoBehaviour    
{

        public List<GroupInfo> groups = new List<GroupInfo>();
        private bool isDragging = false;
        private Rect windowRect = new Rect(10, 10, 300, 500);
        private bool isGUIActive = false;
        private GUIStyle submenuButtonStyle;

    // Create all groups (buttons) and their toggles on start
        private readonly string[] supportedLanguages = new[] { "en", "ko", "ru", "zh-Hans", "zh-Hant", "es" };
        private string lastLanguage;

        private void Start()
        {
            lastLanguage = Localization.CurrentLanguage;
            BuildMenuGroups();
        }

        private void Update()
        {
            if (Input.GetKeyDown(Utils.stringToKeycode(MalumMenu.menuKeybind.Value)))
            {
                isGUIActive = !isGUIActive;
                Vector2 mousePosition = Input.mousePosition;
                windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
            }

            CheatToggles.unlockFeatures = CheatToggles.freeCosmetics = CheatToggles.avoidBans = true;

            if (!Utils.isPlayer)
            {
                CheatToggles.changeRole = CheatToggles.killAll = CheatToggles.telekillPlayer = CheatToggles.killAllCrew =
                    CheatToggles.killAllImps = CheatToggles.teleportCursor = CheatToggles.teleportPlayer =
                    CheatToggles.spectate = CheatToggles.freecam = CheatToggles.killPlayer = false;
            }

            if (!Utils.isHost && !Utils.isFreePlay)
            {
                CheatToggles.killAll = CheatToggles.telekillPlayer = CheatToggles.killAllCrew = CheatToggles.killAllImps =
                    CheatToggles.killPlayer = CheatToggles.zeroKillCd = CheatToggles.killAnyone = CheatToggles.killVanished = false;
            }

            if (!Utils.isShip)
            {
                CheatToggles.unfixableLights = CheatToggles.completeMyTasks = CheatToggles.kickVents = CheatToggles.reportBody =
                    CheatToggles.closeMeeting = CheatToggles.reactorSab = CheatToggles.oxygenSab = CheatToggles.commsSab =
                    CheatToggles.elecSab = CheatToggles.mushSab = CheatToggles.doorsSab = false;
            }

            // Rebuild menu if language has changed
            if (Localization.CurrentLanguage != lastLanguage)
            {
                bool wasGUIActive = isGUIActive;
                isGUIActive = false; // Temporarily disable UI
                BuildMenuGroups();
                lastLanguage = Localization.CurrentLanguage;
                isGUIActive = wasGUIActive;
            }
        }

        private void BuildMenuGroups()
        {
            groups.Clear();

            // Language Selector
            groups.Add(new GroupInfo(Localization.Translate("Language"), false, new List<ToggleInfo>(), new List<SubmenuInfo>()));
            foreach (string lang in supportedLanguages)
            {
                groups[0].toggles.Add(new ToggleInfo(lang, () => Localization.CurrentLanguage == lang, x =>
                {
                    if (x)
                    {
                        Localization.CurrentLanguage = lang;
                        MalumMenu.selectedLanguage.Value = lang; // Save to config
                    }
                }));
        }

        // Player
        groups.Add(new GroupInfo(Localization.Translate("Player"), false, new List<ToggleInfo>
            {
                new ToggleInfo(Localization.Translate(" NoClip"), () => CheatToggles.noClip, x => CheatToggles.noClip = x),
                new ToggleInfo(Localization.Translate(" SpeedHack"), () => CheatToggles.speedBoost, x => CheatToggles.speedBoost = x),
            }, new List<SubmenuInfo> {
                new SubmenuInfo(Localization.Translate("Teleport"), false, new List<ToggleInfo>() {
                    new ToggleInfo(Localization.Translate(" to Cursor"), () => CheatToggles.teleportCursor, x => CheatToggles.teleportCursor = x),
                    new ToggleInfo(Localization.Translate(" to Player"), () => CheatToggles.teleportPlayer, x => CheatToggles.teleportPlayer = x),
                }),
            }));

            // ESP
            groups.Add(new GroupInfo(Localization.Translate("ESP"), false, new List<ToggleInfo>
            {
                new ToggleInfo(Localization.Translate("See Roles"), () => CheatToggles.seeRoles, x => CheatToggles.seeRoles = x),
                new ToggleInfo(Localization.Translate("See Ghosts"), () => CheatToggles.seeGhosts, x => CheatToggles.seeGhosts = x),
                new ToggleInfo(Localization.Translate("No Shadows"), () => CheatToggles.fullBright, x => CheatToggles.fullBright = x),
                new ToggleInfo(Localization.Translate("Reveal Votes"), () => CheatToggles.revealVotes, x => CheatToggles.revealVotes = x),
            }, new List<SubmenuInfo>
            {
                new SubmenuInfo(Localization.Translate("Camera"), false, new List<ToggleInfo>
                {
                    new ToggleInfo(Localization.Translate("Zoom Out"), () => CheatToggles.zoomOut, x => CheatToggles.zoomOut = x),
                    new ToggleInfo(Localization.Translate("Spectate"), () => CheatToggles.spectate, x => CheatToggles.spectate = x),
                    new ToggleInfo(Localization.Translate("Freecam"), () => CheatToggles.freecam, x => CheatToggles.freecam = x),
                }),
                new SubmenuInfo(Localization.Translate("Tracers"), false, new List<ToggleInfo>() {
                    new ToggleInfo(Localization.Translate(" Crewmates"), () => CheatToggles.tracersCrew, x => CheatToggles.tracersCrew = x),
                    new ToggleInfo(Localization.Translate(" Impostors"), () => CheatToggles.tracersImps, x => CheatToggles.tracersImps = x),
                    new ToggleInfo(Localization.Translate(" Ghosts"), () => CheatToggles.tracersGhosts, x => CheatToggles.tracersGhosts = x),
                    new ToggleInfo(Localization.Translate(" Dead Bodies"), () => CheatToggles.tracersBodies, x => CheatToggles.tracersBodies = x),
                    new ToggleInfo(Localization.Translate(" Color-based"), () => CheatToggles.colorBasedTracers, x => CheatToggles.colorBasedTracers = x),
                }),
                new SubmenuInfo(Localization.Translate("Minimap"), false, new List<ToggleInfo>() {
                    new ToggleInfo(Localization.Translate(" Crewmates"), () => CheatToggles.mapCrew, x => CheatToggles.mapCrew = x),
                    new ToggleInfo(Localization.Translate(" Impostors"), () => CheatToggles.mapImps, x => CheatToggles.mapImps = x),
                    new ToggleInfo(Localization.Translate(" Ghosts"), () => CheatToggles.mapGhosts, x => CheatToggles.mapGhosts = x),
                    new ToggleInfo(Localization.Translate(" Color-based"), () => CheatToggles.colorBasedMap, x => CheatToggles.colorBasedMap = x),
                }),
            }));

            // Roles
            groups.Add(new GroupInfo(Localization.Translate("Roles"), false, new List<ToggleInfo>() {
                new ToggleInfo(Localization.Translate(" Set Fake Role"), () => CheatToggles.changeRole, x => CheatToggles.changeRole = x),
            }, new List<SubmenuInfo> {
                new SubmenuInfo(Localization.Translate("Impostor"), false, new List<ToggleInfo>() {
                    new ToggleInfo(Localization.Translate(" Kill Reach"), () => CheatToggles.killReach, x => CheatToggles.killReach = x),
                }),
                new SubmenuInfo(Localization.Translate("Shapeshifter"), false, new List<ToggleInfo>() {
                    new ToggleInfo(Localization.Translate(" No Ss Animation"), () => CheatToggles.noShapeshiftAnim, x => CheatToggles.noShapeshiftAnim = x),
                    new ToggleInfo(Localization.Translate(" Endless Ss Duration"), () => CheatToggles.endlessSsDuration, x => CheatToggles.endlessSsDuration = x),
                }),
                new SubmenuInfo(Localization.Translate("Crewmate"), false, new List<ToggleInfo>() {
                    new ToggleInfo(Localization.Translate(" Complete My Tasks"), () => CheatToggles.completeMyTasks, x => CheatToggles.completeMyTasks = x),
                }),
                new SubmenuInfo(Localization.Translate("Tracker"), false, new List<ToggleInfo>() {
                    new ToggleInfo(Localization.Translate(" Endless Tracking"), () => CheatToggles.endlessTracking, x => CheatToggles.endlessTracking = x),
                    new ToggleInfo(Localization.Translate(" No Track Delay"), () => CheatToggles.noTrackingDelay, x => CheatToggles.noTrackingDelay = x),
                    new ToggleInfo(Localization.Translate(" No Track Cooldown"), () => CheatToggles.noTrackingCooldown, x => CheatToggles.noTrackingCooldown = x),
                }),
                new SubmenuInfo(Localization.Translate("Engineer"), false, new List<ToggleInfo>() {
                    new ToggleInfo(Localization.Translate(" Endless Vent Time"), () => CheatToggles.endlessVentTime, x => CheatToggles.endlessVentTime = x),
                    new ToggleInfo(Localization.Translate(" No Vent Cooldown"), () => CheatToggles.noVentCooldown, x => CheatToggles.noVentCooldown = x),
                }),
                new SubmenuInfo(Localization.Translate("Scientist"), false, new List<ToggleInfo>() {
                    new ToggleInfo(Localization.Translate(" Endless Battery"), () => CheatToggles.endlessBattery, x => CheatToggles.endlessBattery = x),
                    new ToggleInfo(Localization.Translate(" No Vitals Cooldown"), () => CheatToggles.noVitalsCooldown, x => CheatToggles.noVitalsCooldown = x),
                }),
            }));

            // Ship
            groups.Add(new GroupInfo(Localization.Translate("Ship"), false, new List<ToggleInfo>
            {
                new ToggleInfo(Localization.Translate("Unfixable Lights"), () => CheatToggles.unfixableLights, x => CheatToggles.unfixableLights = x),
                new ToggleInfo(Localization.Translate("Report Body"), () => CheatToggles.reportBody, x => CheatToggles.reportBody = x),
                new ToggleInfo(Localization.Translate("Close Meeting"), () => CheatToggles.closeMeeting, x => CheatToggles.closeMeeting = x),
            }, new List<SubmenuInfo>
            {
                new SubmenuInfo(Localization.Translate("Sabotage"), false, new List<ToggleInfo>
                {
                    new ToggleInfo(Localization.Translate("Reactor"), () => CheatToggles.reactorSab, x => CheatToggles.reactorSab = x),
                    new ToggleInfo(Localization.Translate("Oxygen"), () => CheatToggles.oxygenSab, x => CheatToggles.oxygenSab = x),
                    new ToggleInfo(Localization.Translate("Lights"), () => CheatToggles.elecSab, x => CheatToggles.elecSab = x),
                    new ToggleInfo(Localization.Translate("Comms"), () => CheatToggles.commsSab, x => CheatToggles.commsSab = x),
                    new ToggleInfo(Localization.Translate("Doors"), () => CheatToggles.doorsSab, x => CheatToggles.doorsSab = x),
                    new ToggleInfo(Localization.Translate("MushroomMixup"), () => CheatToggles.mushSab, x => CheatToggles.mushSab = x)
                }),
                new SubmenuInfo(Localization.Translate("Vents"), false, new List<ToggleInfo>() {
                    new ToggleInfo(Localization.Translate(" Unlock Vents"), () => CheatToggles.useVents, x => CheatToggles.useVents = x),
                    new ToggleInfo(Localization.Translate(" Kick All From Vents"), () => CheatToggles.kickVents, x => CheatToggles.kickVents = x),
                    new ToggleInfo(Localization.Translate(" Walk In Vents"), () => CheatToggles.walkVent, x => CheatToggles.walkVent = x)
                }),
            }));

            // Chat
            groups.Add(new GroupInfo(Localization.Translate("Chat"), false, new List<ToggleInfo>() {
                new ToggleInfo(Localization.Translate(" Enable Chat"), () => CheatToggles.alwaysChat, x => CheatToggles.alwaysChat = x),
                new ToggleInfo(Localization.Translate(" Unlock Textbox"), () => CheatToggles.chatJailbreak, x => CheatToggles.chatJailbreak = x)
            }, new List<SubmenuInfo>()));

        // Host-Only cheats are temporarily disabled because of some bugs

        //groups.Add(new GroupInfo(Localization.Translate("Host-Only", false, new List<ToggleInfo>() {
        //    new ToggleInfo(Localization.Translate(" ImpostorHack", () => CheatSettings.impostorHack, x => CheatSettings.impostorHack = x),
        //    new ToggleInfo(Localization.Translate(" Godmode", () => CheatSettings.godMode, x => CheatSettings.godMode = x),
        //    new ToggleInfo(Localization.Translate(" EvilVote", () => CheatSettings.evilVote, x => CheatSettings.evilVote = x),
        //    new ToggleInfo(Localization.Translate(" VoteImmune", () => CheatSettings.voteImmune, x => CheatSettings.voteImmune = x)
        //}, new List<SubmenuInfo>()));

        // Console is temporarily disabled until we implement some features for it

        //groups.Add(new GroupInfo(Localization.Translate("Console", false, new List<ToggleInfo>() {
        //    new ToggleInfo(Localization.Translate(" ConsoleUI", () => MalumMenu.consoleUI.isVisible, x => MalumMenu.consoleUI.isVisible = x),
        //}, new List<SubmenuInfo>()));

        groups.Add(new GroupInfo(Localization.Translate("Host-Only"), false, new List<ToggleInfo>
            {
                new ToggleInfo(Localization.Translate("Kill While Vanished"), () => CheatToggles.killVanished, x => CheatToggles.killVanished = x),
                new ToggleInfo(Localization.Translate("Kill Anyone"), () => CheatToggles.killAnyone, x => CheatToggles.killAnyone = x),
                new ToggleInfo(Localization.Translate("No Kill Cooldown"), () => CheatToggles.zeroKillCd, x => CheatToggles.zeroKillCd = x),
            }, new List<SubmenuInfo>
            {
                new SubmenuInfo(Localization.Translate("Murder"), false, new List<ToggleInfo>
                {
                    new ToggleInfo(Localization.Translate("Kill Player"), () => CheatToggles.killPlayer, x => CheatToggles.killPlayer = x),
                    new ToggleInfo(Localization.Translate("Kill All Crewmates"), () => CheatToggles.killAllCrew, x => CheatToggles.killAllCrew = x),
                    new ToggleInfo(Localization.Translate("Kill All Impostors"), () => CheatToggles.killAllImps, x => CheatToggles.killAllImps = x),
                    new ToggleInfo(Localization.Translate("Kill Everyone"), () => CheatToggles.killAll, x => CheatToggles.killAll = x),
                })
            }));

            // Passive
            groups.Add(new GroupInfo(Localization.Translate("Passive"), false, new List<ToggleInfo>() {
                new ToggleInfo(Localization.Translate(" Free Cosmetics"), () => CheatToggles.freeCosmetics, x => CheatToggles.freeCosmetics = x),
                new ToggleInfo(Localization.Translate(" Avoid Penalties"), () => CheatToggles.avoidBans, x => CheatToggles.avoidBans = x),
                new ToggleInfo(Localization.Translate(" Unlock Extra Features"), () => CheatToggles.unlockFeatures, x => CheatToggles.unlockFeatures = x),
            }, new List<SubmenuInfo>()));

        }

        public void OnGUI()
        {
            if (!isGUIActive) return;

            if (submenuButtonStyle == null)
        {
            submenuButtonStyle = new GUIStyle(GUI.skin.button);

            submenuButtonStyle.normal.textColor = Color.white;

            submenuButtonStyle.fontSize = 18;
            GUI.skin.toggle.fontSize = GUI.skin.button.fontSize = 20;

            submenuButtonStyle.normal.background = Texture2D.grayTexture;
            submenuButtonStyle.normal.background.Apply();
        }

        //Only change the window height while the user is not dragging it
        //Or else dragging breaks
        if (!isDragging)
        {
            int windowHeight = CalculateWindowHeight();
            windowRect.height = windowHeight;
        }

        Color uiColor;

        string configHtmlColor = MalumMenu.menuHtmlColor.Value;

        if (!ColorUtility.TryParseHtmlString(configHtmlColor, out uiColor))
        {
            if (!configHtmlColor.StartsWith("#"))
            {
                if (ColorUtility.TryParseHtmlString("#" + configHtmlColor, out uiColor))
                {
                    GUI.backgroundColor = uiColor;
                }
            }
        }
        else
        {
            GUI.backgroundColor = uiColor;
        }

        windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)WindowFunction, "MalumMenu v" + MalumMenu.malumVersion);
    }

    public void WindowFunction(int windowID)
    {
        int groupSpacing = 50;
            int toggleSpacing = 40;
            int submenuSpacing = 40;
            int currentYPosition = 20;

            for (int groupId = 0; groupId < groups.Count; groupId++)
            {
                GroupInfo group = groups[groupId];

                if (GUI.Button(new Rect(10, currentYPosition, 280, 40), group.name))
                {
                    group.isExpanded = !group.isExpanded;
                    groups[groupId] = group;
                CloseAllGroupsExcept(groupId); // Close all other groups when one is expanded
            }
            currentYPosition += groupSpacing;

                if (group.isExpanded)
                {
                // Render direct toggles for the group
                foreach (var toggle in group.toggles)
                    {
                        bool currentState = toggle.getState();
                        bool newState = GUI.Toggle(new Rect(20, currentYPosition, 260, 30), currentState, toggle.label);
                    if (newState != currentState)
                    {
                        toggle.setState(newState);
                    }
                        currentYPosition += toggleSpacing;
                    }

                    for (int submenuId = 0; submenuId < group.submenus.Count; submenuId++)
                    {
                        var submenu = group.submenus[submenuId];

                    // Add a button for each submenu and toggle its expansion state when clicked
                    if (GUI.Button(new Rect(20, currentYPosition, 260, 30), submenu.name, submenuButtonStyle))
                    {
                        submenu.isExpanded = !submenu.isExpanded;
                        group.submenus[submenuId] = submenu;
                        if (submenu.isExpanded)
                        {
                            CloseAllSubmenusExcept(group, submenuId);
                        }
                    }
                        currentYPosition += submenuSpacing;

                        if (submenu.isExpanded)
                        {
                        // Show all the toggles in the expanded submenu
                        foreach (var toggle in submenu.toggles)
                            {
                                bool currentState = toggle.getState();
                                bool newState = GUI.Toggle(new Rect(30, currentYPosition, 250, 30), currentState, toggle.label);
                            if (newState != currentState)
                            {
                                toggle.setState(newState);
                            }
                                currentYPosition += toggleSpacing;
                            }
                        }
                    }
                }
            }

        if (Event.current.type == EventType.MouseDrag)
        {
            isDragging = true;
        }

        if (Event.current.type == EventType.MouseUp)
        {
            isDragging = false;
        }

        GUI.DragWindow(); //Allows dragging the GUI window with mouse
    }


    // Dynamically calculate the window's height depending on
    // The number of toggles & group expansion
    private int CalculateWindowHeight()
        {
        int totalHeight = 70; // Base height for the window
        int groupHeight = 50; // Height for each group title
        int toggleHeight = 30; // Height for each toggle
        int submenuHeight = 40; // Height for each submenu title

        foreach (GroupInfo group in groups)
        {
            totalHeight += groupHeight; // Always add height for the group title

            if (group.isExpanded)
            {
                totalHeight += group.toggles.Count * toggleHeight; // Add height for toggles in the group

                foreach (SubmenuInfo submenu in group.submenus)
                {
                    totalHeight += submenuHeight; // Always add height for the submenu title

                    if (submenu.isExpanded)
                    {
                        totalHeight += submenu.toggles.Count * toggleHeight; // Add height for toggles in the expanded submenu
                    }
                }
            }
        }

        return totalHeight;
    }


    // Closes all expanded groups other than indexToKeepOpen

    private void CloseAllGroupsExcept(int indexToKeepOpen)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            if (i != indexToKeepOpen)
            {
                GroupInfo group = groups[i];
                group.isExpanded = false;
                groups[i] = group;
            }
        }
    }

    private void CloseAllSubmenusExcept(GroupInfo group, int submenuIndexToKeepOpen)
    {
        for (int i = 0; i < group.submenus.Count; i++)
        {
            if (i != submenuIndexToKeepOpen)
            {
                var submenu = group.submenus[i];
                submenu.isExpanded = false;
                group.submenus[i] = submenu;
            }
        }
    }

}
