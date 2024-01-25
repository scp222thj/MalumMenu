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
    private void Start()
    {
        groups.Add(new GroupInfo("Player", false, new List<ToggleInfo>() {
            new ToggleInfo(" NoClip", () => CheatToggles.noClip, x => CheatToggles.noClip = x),
            new ToggleInfo(" SpeedBoost", () => CheatToggles.speedBoost, x => CheatToggles.speedBoost = x),
            new ToggleInfo(" ReportBody", () => CheatToggles.reportBody, x => CheatToggles.reportBody = x),
            }, new List<SubmenuInfo> {
            new SubmenuInfo("Murder", false, new List<ToggleInfo>() {
                new ToggleInfo(" MurderPlayer", () => CheatToggles.murderPlayer, x => CheatToggles.murderPlayer = x),
                new ToggleInfo(" MurderAll", () => CheatToggles.murderAll, x => CheatToggles.murderAll = x),
            }),
            new SubmenuInfo("Teleport", false, new List<ToggleInfo>() {
                new ToggleInfo(" to Cursor", () => CheatToggles.teleportCursor, x => CheatToggles.teleportCursor = x),
                new ToggleInfo(" to Player", () => CheatToggles.teleportPlayer, x => CheatToggles.teleportPlayer = x),
            }),
        }
        ));

        groups.Add(new GroupInfo("ESP", false, new List<ToggleInfo>() {
            new ToggleInfo(" SeeGhosts", () => CheatToggles.seeGhosts, x => CheatToggles.seeGhosts = x),
            new ToggleInfo(" FullBright", () => CheatToggles.fullBright, x => CheatToggles.fullBright = x),
            new ToggleInfo(" RevealVotes", () => CheatToggles.revealVotes, x => CheatToggles.revealVotes = x),
            new ToggleInfo(" AlwaysChat", () => CheatToggles.alwaysChat, x => CheatToggles.alwaysChat = x)
        }, new List<SubmenuInfo> {
            new SubmenuInfo("Nametags", false, new List<ToggleInfo>() {
                new ToggleInfo(" SeeRoles", () => CheatToggles.seeRoles, x => CheatToggles.seeRoles = x),
                new ToggleInfo(" VentVision", () => CheatToggles.ventVision, x => CheatToggles.ventVision = x)
            }),
            new SubmenuInfo("Camera", false, new List<ToggleInfo>() {
                new ToggleInfo(" ZoomOut", () => CheatToggles.zoomOut, x => CheatToggles.zoomOut = x),
                new ToggleInfo(" Spectate", () => CheatToggles.spectate, x => CheatToggles.spectate = x),
                new ToggleInfo(" Freecam", () => CheatToggles.freeCam, x => CheatToggles.freeCam = x)
            }),
            new SubmenuInfo("Tracers", false, new List<ToggleInfo>() {
                new ToggleInfo(" Crewmates", () => CheatToggles.tracersCrew, x => CheatToggles.tracersCrew = x),
                new ToggleInfo(" Impostors", () => CheatToggles.tracersImps, x => CheatToggles.tracersImps = x),
                new ToggleInfo(" Ghosts", () => CheatToggles.tracersGhosts, x => CheatToggles.tracersGhosts = x),
                new ToggleInfo(" Dead Bodies", () => CheatToggles.tracersBodies, x => CheatToggles.tracersBodies = x),
                new ToggleInfo(" Color-based", () => CheatToggles.colorBasedTracers, x => CheatToggles.colorBasedTracers = x),
            }),
            new SubmenuInfo("Minimap", false, new List<ToggleInfo>() {
                new ToggleInfo(" Crewmates", () => CheatToggles.mapCrew, x => CheatToggles.mapCrew = x),
                new ToggleInfo(" Impostors", () => CheatToggles.mapImps, x => CheatToggles.mapImps = x),
                new ToggleInfo(" Ghosts", () => CheatToggles.mapGhosts, x => CheatToggles.mapGhosts = x),
                new ToggleInfo(" Color-based", () => CheatToggles.colorBasedMap, x => CheatToggles.colorBasedMap = x)
            }),
        }));

        groups.Add(new GroupInfo("Roles", false, new List<ToggleInfo>() {
            new ToggleInfo(" ChangeRole", () => CheatToggles.changeRole, x => CheatToggles.changeRole = x),
        },
            new List<SubmenuInfo> {
                new SubmenuInfo("Crewmate", false, new List<ToggleInfo>() {
                    new ToggleInfo(" CompleteMyTasks", () => CheatToggles.completeMyTasks, x => CheatToggles.completeMyTasks = x)
                }),
                new SubmenuInfo("Impostor", false, new List<ToggleInfo>() {
                    new ToggleInfo(" KillAnyone", () => CheatToggles.killAnyone, x => CheatToggles.killAnyone = x),
                    new ToggleInfo(" NoKillCooldown", () => CheatToggles.zeroKillCd, x => CheatToggles.zeroKillCd = x),
                    new ToggleInfo(" KillReach", () => CheatToggles.killReach, x => CheatToggles.killReach = x),
                }),
                new SubmenuInfo("Shapeshifter", false, new List<ToggleInfo>() {
                    new ToggleInfo(" NoSsAnimation", () => CheatToggles.noShapeshiftAnim, x => CheatToggles.noShapeshiftAnim = x),
                    new ToggleInfo(" EndlessSsDuration", () => CheatToggles.endlessSsDuration, x => CheatToggles.endlessSsDuration = x),
                }),
                new SubmenuInfo("Engineer", false, new List<ToggleInfo>() {
                    new ToggleInfo(" EndlessVentTime", () => CheatToggles.endlessVentTime, x => CheatToggles.endlessVentTime = x),
                    new ToggleInfo(" NoVentCooldown", () => CheatToggles.noVentCooldown, x => CheatToggles.noVentCooldown = x),
                }),
                new SubmenuInfo("Scientist", false, new List<ToggleInfo>() {
                    new ToggleInfo(" EndlessBattery", () => CheatToggles.endlessBattery, x => CheatToggles.endlessBattery = x),
                    new ToggleInfo(" NoVitalsCooldown", () => CheatToggles.noVitalsCooldown, x => CheatToggles.noVitalsCooldown = x),
                }),
            }));

        groups.Add(new GroupInfo("Ship", false, new List<ToggleInfo> {
            new ToggleInfo(" UnfixableLights", () => CheatToggles.unfixableLights, x => CheatToggles.unfixableLights = x),
            new ToggleInfo(" CloseMeeting", () => CheatToggles.closeMeeting, x => CheatToggles.closeMeeting = x),
        }, new List<SubmenuInfo> {
            new SubmenuInfo("Sabotage", false, new List<ToggleInfo>() {
                new ToggleInfo(" Reactor", () => CheatToggles.reactorSab, x => CheatToggles.reactorSab = x),
                new ToggleInfo(" Oxygen", () => CheatToggles.oxygenSab, x => CheatToggles.oxygenSab = x),
                new ToggleInfo(" Lights", () => CheatToggles.elecSab, x => CheatToggles.elecSab = x),
                new ToggleInfo(" Comms", () => CheatToggles.commsSab, x => CheatToggles.commsSab = x),
                new ToggleInfo(" Doors", () => CheatToggles.doorsSab, x => CheatToggles.doorsSab = x),
                new ToggleInfo(" MushroomMixup", () => CheatToggles.mushSab, x => CheatToggles.mushSab = x),
            }),
            new SubmenuInfo("Vents", false, new List<ToggleInfo>() {
                new ToggleInfo(" UseVents", () => CheatToggles.useVents, x => CheatToggles.useVents = x),
                new ToggleInfo(" KickVents", () => CheatToggles.kickVents, x => CheatToggles.kickVents = x),
                new ToggleInfo(" WalkInVents", () => CheatToggles.walkVent, x => CheatToggles.walkVent = x)
            }),
        }));

        // Host-Only cheats are temporarly disabled because of some bugs

        // groups.Add(new GroupInfo("Host-Only", false, new List<ToggleInfo>() {
        //     new ToggleInfo(" ImpostorHack", () => CheatSettings.impostorHack, x => CheatSettings.impostorHack = x),
        //     new ToggleInfo(" Godmode", () => CheatSettings.godMode, x => CheatSettings.godMode = x),
        //     new ToggleInfo(" EvilVote", () => CheatSettings.evilVote, x => CheatSettings.evilVote = x),
        //     new ToggleInfo(" VoteImmune", () => CheatSettings.voteImmune, x => CheatSettings.voteImmune = x)
        // }, new List<SubmenuInfo>()));

        groups.Add(new GroupInfo("Spoofing", false, new List<ToggleInfo>(){
            new ToggleInfo(" RandomFriendCode", () => CheatToggles.spoofRandomFC, x => CheatToggles.spoofRandomFC = x),
        }, new List<SubmenuInfo> {
            new SubmenuInfo("Config", false, new List<ToggleInfo>() {
                new ToggleInfo(" Spoofed FriendCode", () => MalumMenu.spoofFriendCode.Value != "", (bool n) => { }),
                new ToggleInfo(" Spoofed Level", () => MalumMenu.spoofLevel.Value != "", (bool n) => { }),
            }),
        }));

        groups.Add(new GroupInfo("Console", false, new List<ToggleInfo>() {
            new ToggleInfo(" ConsoleUI", () => MalumMenu.consoleUI.isVisible, x => MalumMenu.consoleUI.isVisible = x),
        }, new List<SubmenuInfo>()));

        groups.Add(new GroupInfo("Passive", false, new List<ToggleInfo>() {
            new ToggleInfo(" FreeCosmetics", () => CheatToggles.freeCosmetics, x => CheatToggles.freeCosmetics = x),
            new ToggleInfo(" AvoidPenalties", () => CheatToggles.avoidBans, x => CheatToggles.avoidBans = x),
            new ToggleInfo(" UnlockFeatures", () => CheatToggles.unlockFeatures, x => CheatToggles.unlockFeatures = x),
        }, new List<SubmenuInfo>()));
    }

    private void Update()
    {
        if (Input.GetKeyDown(Utils.stringToKeycode(MalumMenu.menuKeybind.Value))) // Enable-disable GUI with config key, default DELETE
        {
            isGUIActive = !isGUIActive;

            // Teleport the window to the mouse for immediate use
            Vector2 mousePosition = Input.mousePosition;
            windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        }

        // Passive cheats are always on to avoid problems
        CheatToggles.unlockFeatures = CheatToggles.freeCosmetics = CheatToggles.avoidBans = true;

        if (!Utils.isPlayer)
        {
            CheatToggles.changeRole = CheatToggles.murderAll = CheatToggles.teleportCursor = CheatToggles.teleportPlayer = CheatToggles.spectate = CheatToggles.freeCam = CheatToggles.murderPlayer;
        }

        // Host-only cheats are turned off if LocalPlayer is not the game's host
        // if (!CheatChecks.isHost)
        // {
        //     CheatToggles.voteImmune = CheatToggles.godMode = CheatToggles.impostorHack = CheatToggles.evilVote = false;
        // }

        // Some cheats only work if the ship is present, so they are turned off if it is not
        if (!Utils.isShip)
        {
            CheatToggles.unfixableLights = CheatToggles.completeMyTasks = CheatToggles.kickVents = CheatToggles.reportBody = CheatToggles.closeMeeting = CheatToggles.reactorSab = CheatToggles.oxygenSab = CheatToggles.commsSab = CheatToggles.elecSab = CheatToggles.mushSab = CheatToggles.doorsSab = false;
            Sabotages_ShipStatus_FixedUpdate_Postfix.reactorSab = Sabotages_ShipStatus_FixedUpdate_Postfix.commsSab = Sabotages_ShipStatus_FixedUpdate_Postfix.elecSab = Sabotages_ShipStatus_FixedUpdate_Postfix.oxygenSab = UnfixableLights_ShipStatus_FixedUpdate_Postfix.isActive = false;
        }
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

        // Only change the window height while the user is not dragging it
        // Or else dragging breaks
        if (!isDragging)
        {
            int windowHeight = CalculateWindowHeight();
            windowRect.height = windowHeight;
        }

        windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)WindowFunction, "MalumMenu v" + MalumMenu.malumVersion);
    }

    // Handles window creation
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
                CloseAllGroupsExcept(groupId); //  Close all other groups when one is expanded
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

        GUI.DragWindow(); // Allows dragging the GUI window with mouse
    }


    // Dynamically calculate the window's height depending on the number of toggles & group expansion
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