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

    //Create all groups (buttons) and their toggles on start
    private void Start()
    {
        groups.Add(new GroupInfo("Player", false, new List<ToggleInfo>() {
        new ToggleInfo(" NoClip", () => CheatSettings.noClip, x => CheatSettings.noClip = x),
        new ToggleInfo(" SpeedBoost", () => CheatSettings.speedBoost, x => CheatSettings.speedBoost = x),
        new ToggleInfo(" NoCooldowns", () => CheatSettings.noCooldowns, x => CheatSettings.noCooldowns = x)
        }, new List<SubmenuInfo> {
            new SubmenuInfo("Teleport", false, new List<ToggleInfo>() {
                new ToggleInfo(" to Cursor", () => CheatSettings.teleportCursor, x => CheatSettings.teleportCursor = x),
                new ToggleInfo(" to Player", () => CheatSettings.teleportPlayer, x => CheatSettings.teleportPlayer = x)
            })
        }));

        groups.Add(new GroupInfo("ESP", false, new List<ToggleInfo>() {
            new ToggleInfo(" SeeGhosts", () => CheatSettings.seeGhosts, x => CheatSettings.seeGhosts = x),
            new ToggleInfo(" SeeRoles", () => CheatSettings.seeRoles, x => CheatSettings.seeRoles = x),
            new ToggleInfo(" FullBright", () => CheatSettings.fullBright, x => CheatSettings.fullBright = x)
        }, new List<SubmenuInfo> {
            new SubmenuInfo("Camera", false, new List<ToggleInfo>() {
                new ToggleInfo(" ZoomOut", () => CheatSettings.zoomOut, x => CheatSettings.zoomOut = x),
                new ToggleInfo(" Spectate", () => CheatSettings.spectate, x => CheatSettings.spectate = x),
                new ToggleInfo(" Freecam", () => CheatSettings.freeCam, x => CheatSettings.freeCam = x)
            }),
            new SubmenuInfo("Tracers", false, new List<ToggleInfo>() {
                new ToggleInfo(" Crewmates", () => CheatSettings.tracersCrew, x => CheatSettings.tracersCrew = x),
                new ToggleInfo(" Impostors", () => CheatSettings.tracersImps, x => CheatSettings.tracersImps = x),
                new ToggleInfo(" Ghosts", () => CheatSettings.tracersGhosts, x => CheatSettings.tracersGhosts = x),
                new ToggleInfo(" Dead Bodies", () => CheatSettings.tracersBodies, x => CheatSettings.tracersBodies = x),
                new ToggleInfo(" Color-based", () => CheatSettings.colorBasedTracers, x => CheatSettings.colorBasedTracers = x),
            }),
            new SubmenuInfo("Minimap", false, new List<ToggleInfo>() {
                new ToggleInfo(" Crewmates", () => CheatSettings.mapCrew, x => CheatSettings.mapCrew = x),
                new ToggleInfo(" Impostors", () => CheatSettings.mapImps, x => CheatSettings.mapImps = x),
                new ToggleInfo(" Ghosts", () => CheatSettings.mapGhosts, x => CheatSettings.mapGhosts = x),
                new ToggleInfo(" Color-based", () => CheatSettings.colorBasedMap, x => CheatSettings.colorBasedMap = x)
            }),
        }));

        groups.Add(new GroupInfo("RPC Exploit", false, new List<ToggleInfo>() {
            new ToggleInfo(" KickPlayer", () => CheatSettings.kickPlayer, x => CheatSettings.kickPlayer = x),
        }, new List<SubmenuInfo> {
            new SubmenuInfo("Outfits", false, new List<ToggleInfo>() {
                new ToggleInfo(" ShuffleOutfit", () => CheatSettings.shuffleOutfit, x => CheatSettings.shuffleOutfit = x),
                new ToggleInfo(" ShuffleAllOutfits", () => CheatSettings.shuffleAllOutfits, x => CheatSettings.shuffleAllOutfits = x),
                new ToggleInfo(" CopyOutfit", () => CheatSettings.copyOutfit, x => CheatSettings.copyOutfit = x),
                new ToggleInfo(" ResetMyOutfit", () => CheatSettings.resetOutfit, x => CheatSettings.resetOutfit = x),
                new ToggleInfo(" UnlockAllColors", () => CheatSettings.unlockColors, x => CheatSettings.unlockColors = x),
            }),
            new SubmenuInfo("Shapeshift", false, new List<ToggleInfo>() {
                new ToggleInfo(" ShapeshiftAll", () => CheatSettings.shapeshiftAll, x => CheatSettings.shapeshiftAll = x),
                new ToggleInfo(" ResetShapeshifters", () => CheatSettings.resetShapeshift, x => CheatSettings.resetShapeshift = x),
            }),
            new SubmenuInfo("Murder", false, new List<ToggleInfo>() {
                new ToggleInfo(" MurderPlayer", () => CheatSettings.murderPlayer, x => CheatSettings.murderPlayer = x),
                new ToggleInfo(" MurderAll", () => CheatSettings.murderAll, x => CheatSettings.murderAll = x),
            }),
        }));

        groups.Add(new GroupInfo("Ship", false, new List<ToggleInfo>(), new List<SubmenuInfo> {
            new SubmenuInfo("Sabotage", false, new List<ToggleInfo>() {
                new ToggleInfo(" Blackout", () => CheatSettings.blackOut, x => CheatSettings.blackOut = x),
                new ToggleInfo(" Doors", () => CheatSettings.fullLockdown, x => CheatSettings.fullLockdown = x),
                new ToggleInfo(" Reactor", () => CheatSettings.reactorSab, x => CheatSettings.reactorSab = x),
                new ToggleInfo(" Oxygen", () => CheatSettings.oxygenSab, x => CheatSettings.oxygenSab = x),
                new ToggleInfo(" Electrical", () => CheatSettings.elecSab, x => CheatSettings.elecSab = x),
                new ToggleInfo(" Comms", () => CheatSettings.commsSab, x => CheatSettings.commsSab = x)
            }),
            new SubmenuInfo("Mushrooms", false, new List<ToggleInfo>() {
                new ToggleInfo(" MushroomMixup", () => CheatSettings.mushSab, x => CheatSettings.mushSab = x),
                new ToggleInfo(" SporesTrigger", () => CheatSettings.mushSpore, x => CheatSettings.mushSpore = x),
                new ToggleInfo(" SporeCloudVision", () => CheatSettings.sporeVision, x => CheatSettings.sporeVision = x),
            }),
            new SubmenuInfo("Vents", false, new List<ToggleInfo>() {
                new ToggleInfo(" UseVents", () => CheatSettings.useVents, x => CheatSettings.useVents = x),
                new ToggleInfo(" KickVents", () => CheatSettings.kickVents, x => CheatSettings.kickVents = x),
                new ToggleInfo(" VentVision", () => CheatSettings.ventVision, x => CheatSettings.ventVision = x),
                new ToggleInfo(" WalkInVents", () => CheatSettings.walkVent, x => CheatSettings.walkVent = x)
            }),
            new SubmenuInfo("Meetings", false, new List<ToggleInfo>() {
                new ToggleInfo(" RevealVotes", () => CheatSettings.revealVotes, x => CheatSettings.revealVotes = x),
                new ToggleInfo(" CloseMeeting", () => CheatSettings.closeMeeting, x => CheatSettings.closeMeeting = x),
                new ToggleInfo(" CallMeeting", () => CheatSettings.callMeeting, x => CheatSettings.callMeeting = x)
            }),
        }));

        groups.Add(new GroupInfo("Chat", false, new List<ToggleInfo>() {
            new ToggleInfo(" AlwaysChat", () => CheatSettings.alwaysChat, x => CheatSettings.alwaysChat = x),
            new ToggleInfo(" SpamChat", () => CheatSettings.spamChat, x => CheatSettings.spamChat = x),
            new ToggleInfo(" ChatJailbreak", () => CheatSettings.chatJailbreak, x => CheatSettings.chatJailbreak = x)
        }, new List<SubmenuInfo>()));

        groups.Add(new GroupInfo("Host-Only", false, new List<ToggleInfo>() {
            new ToggleInfo(" ImpostorHack", () => CheatSettings.impostorHack, x => CheatSettings.impostorHack = x),
            new ToggleInfo(" Godmode", () => CheatSettings.godMode, x => CheatSettings.godMode = x),
            new ToggleInfo(" EvilVote", () => CheatSettings.evilVote, x => CheatSettings.evilVote = x),
            new ToggleInfo(" VoteImmune", () => CheatSettings.voteImmune, x => CheatSettings.voteImmune = x)
        }, new List<SubmenuInfo>()));

        groups.Add(new GroupInfo("Passive", false, new List<ToggleInfo>() {
            new ToggleInfo(" FreeCosmetics", () => CheatSettings.freeCosmetics, x => CheatSettings.freeCosmetics = x),
            new ToggleInfo(" AvoidPenalties", () => CheatSettings.avoidBans, x => CheatSettings.avoidBans = x),
            new ToggleInfo(" UnlockFeatures", () => CheatSettings.unlockFeatures, x => CheatSettings.unlockFeatures = x),
            new ToggleInfo(" CustomUserData", () => CheatSettings.customUserData, x => CheatSettings.customUserData = x)
        }, new List<SubmenuInfo>()));
    }

    private void Update(){
        if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), MalumPlugin.menuKeybind.Value)))
        {
            //Enable-disable GUI with DELETE key
            isGUIActive = !isGUIActive;

            //Also teleport the window to the mouse for immediate use
            Vector2 mousePosition = Input.mousePosition;
            windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        }

        //Passive cheats are always on to avoid problems
        CheatSettings.unlockFeatures = CheatSettings.customUserData = CheatSettings.freeCosmetics = CheatSettings.avoidBans = true;

        //Host-only cheats are turned off if LocalPlayer is not the game's host
        if(!isHostCheck.isHost){
            CheatSettings.voteImmune = CheatSettings.godMode = CheatSettings.impostorHack = CheatSettings.evilVote = false;
        }

        //Some cheats only work if the ship is present, so they are turned off if it is not
        if(!isShipCheck.isShip){
            CheatSettings.blackOut = CheatSettings.kickVents = CheatSettings.callMeeting = CheatSettings.closeMeeting = CheatSettings.reactorSab = CheatSettings.oxygenSab = CheatSettings.commsSab = CheatSettings.mushSab = CheatSettings.fullLockdown = CheatSettings.mushSpore = false;
        }
    }

    public void OnGUI()
    {
        if (submenuButtonStyle == null)
        {
            submenuButtonStyle = new GUIStyle(GUI.skin.button);
            submenuButtonStyle.normal.textColor = Color.white;
            submenuButtonStyle.fontSize = 18;
            submenuButtonStyle.normal.background = Texture2D.grayTexture;
            submenuButtonStyle.normal.background.Apply();
        }

        //If GUI is enabled render the window
        if (isGUIActive)
        {
            GUI.skin.toggle.fontSize = 20;
            GUI.skin.button.fontSize = 20;

            //Only change the window height while the user is not dragging it
            //Or else dragging breaks
            if (!isDragging)
            {
                int windowHeight = CalculateWindowHeight();
                windowRect.height = windowHeight;
            }

            windowRect = GUI.Window(0, windowRect, (UnityEngine.GUI.WindowFunction)WindowFunction, "MalumMenu v" + MalumPlugin.malumVersion);
        }
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


    //Dynamically calculate the window's height depending on
    //The number of toggles & group expansion
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
