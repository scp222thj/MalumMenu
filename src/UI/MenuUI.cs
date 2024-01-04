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
        new ToggleInfo(" NoClip", () => CheatToggles.noClip, x => CheatToggles.noClip = x),
        new ToggleInfo(" SpeedBoost", () => CheatToggles.speedBoost, x => CheatToggles.speedBoost = x),
        }, new List<SubmenuInfo> {
            new SubmenuInfo("NoCooldowns", false, new List<ToggleInfo>() {
                new ToggleInfo(" Shapeshifter", () => CheatToggles.noCooldowns_shapeshifter, x => CheatToggles.noCooldowns_shapeshifter = x),
                new ToggleInfo(" Engineer", () => CheatToggles.noCooldowns_engineer, x => CheatToggles.noCooldowns_engineer = x),
                new ToggleInfo(" Scientist", () => CheatToggles.noCooldowns_scientist, x => CheatToggles.noCooldowns_scientist = x)

            }),
            new SubmenuInfo("Teleport", false, new List<ToggleInfo>() {
                new ToggleInfo(" to Cursor", () => CheatToggles.teleportCursor, x => CheatToggles.teleportCursor = x),
                new ToggleInfo(" to Player", () => CheatToggles.teleportPlayer, x => CheatToggles.teleportPlayer = x)
            })
        }));

        groups.Add(new GroupInfo("ESP", false, new List<ToggleInfo>() {
            new ToggleInfo(" SeeGhosts", () => CheatToggles.seeGhosts, x => CheatToggles.seeGhosts = x),
            new ToggleInfo(" SeeRoles", () => CheatToggles.seeRoles, x => CheatToggles.seeRoles = x),
            new ToggleInfo(" FullBright", () => CheatToggles.fullBright, x => CheatToggles.fullBright = x),
            new ToggleInfo(" VentVision", () => CheatToggles.ventVision, x => CheatToggles.ventVision = x),
            new ToggleInfo(" RevealVotes", () => CheatToggles.revealVotes, x => CheatToggles.revealVotes = x),

        }, new List<SubmenuInfo> {
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

        groups.Add(new GroupInfo("RPC Exploit", false, new List<ToggleInfo>() {
            new ToggleInfo(" KickPlayer", () => CheatToggles.kickPlayer, x => CheatToggles.kickPlayer = x),
            new ToggleInfo(" ReportBody", () => CheatToggles.reportBody, x => CheatToggles.reportBody = x)
        }, new List<SubmenuInfo> {
            new SubmenuInfo("Murder", false, new List<ToggleInfo>() {
                new ToggleInfo(" MurderPlayer", () => CheatToggles.murderPlayer, x => CheatToggles.murderPlayer = x),
                new ToggleInfo(" MurderIRL", () => CheatToggles.murderIRL, x => CheatToggles.murderIRL = x),
                new ToggleInfo(" MurderAll", () => CheatToggles.murderAll, x => CheatToggles.murderAll = x),
            }),
        }));

        groups.Add(new GroupInfo("Appearance", false, new List<ToggleInfo>() {
            new ToggleInfo(" UnlockAllColors", () => CheatToggles.unlockColors, x => CheatToggles.unlockColors = x),
            new ToggleInfo(" MimicOutfit", () => CheatToggles.mimicOutfit, x => CheatToggles.mimicOutfit = x),
            new ToggleInfo(" ShuffleOutfit", () => CheatToggles.shuffleOutfit, x => CheatToggles.shuffleOutfit = x),
            new ToggleInfo(" All MimicOutfit", () => CheatToggles.mimicAllOutfits, x => CheatToggles.mimicAllOutfits = x),
            new ToggleInfo(" All ShuffleOutfit", () => CheatToggles.shuffleAllOutfits, x => CheatToggles.shuffleAllOutfits = x),
            new ToggleInfo(" Reset Outfit", () => CheatToggles.resetOutfit, x => CheatToggles.resetOutfit = x),
        }, new List<SubmenuInfo>()));

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

        groups.Add(new GroupInfo("Chat", false, new List<ToggleInfo>() {
            new ToggleInfo(" AlwaysChat", () => CheatToggles.alwaysChat, x => CheatToggles.alwaysChat = x),
            new ToggleInfo(" ChatMimic", () => CheatToggles.chatMimic, x => CheatToggles.chatMimic = x),
            new ToggleInfo(" SpamChat", () => CheatToggles.spamChat, x => CheatToggles.spamChat = x),
            new ToggleInfo(" ChatJailbreak", () => CheatToggles.chatJailbreak, x => CheatToggles.chatJailbreak = x)
        }, new List<SubmenuInfo>()));

        //Host-Only Cheats are temporarly disabled because of some bugs

        //groups.Add(new GroupInfo("Host-Only", false, new List<ToggleInfo>() {
        //    new ToggleInfo(" ImpostorHack", () => CheatSettings.impostorHack, x => CheatSettings.impostorHack = x),
        //    new ToggleInfo(" Godmode", () => CheatSettings.godMode, x => CheatSettings.godMode = x),
        //    new ToggleInfo(" EvilVote", () => CheatSettings.evilVote, x => CheatSettings.evilVote = x),
        //    new ToggleInfo(" VoteImmune", () => CheatSettings.voteImmune, x => CheatSettings.voteImmune = x)
        //}, new List<SubmenuInfo>()));

        groups.Add(new GroupInfo("Spoofing", false, new List<ToggleInfo>(){
            new ToggleInfo(" RandomName", () => CheatToggles.spoofRandomName, x => CheatToggles.spoofRandomName = x),
            new ToggleInfo(" RandomFriendCode", () => CheatToggles.spoofRandomFC, x => CheatToggles.spoofRandomFC = x),
        }, new List<SubmenuInfo> {
            new SubmenuInfo("Other", false, new List<ToggleInfo>() {
                new ToggleInfo(" Copy PlayerFriendCode", () => CheatToggles.copyPlayerFC, x => CheatToggles.copyPlayerFC = x),
                new ToggleInfo(" Copy PlayerPUID", () => CheatToggles.copyPlayerPUID, x => CheatToggles.copyPlayerPUID = x),
            }),
            new SubmenuInfo("Config", false, new List<ToggleInfo>() {
                new ToggleInfo(" Spoofed FriendCode", () => MalumMenu.spoofFriendCode.Value != "", (bool n) => { }),
                new ToggleInfo(" Spoofed PUID", () => MalumMenu.spoofPuid.Value != "",  (bool n) => { }),
            }),
        }));

        groups.Add(new GroupInfo("Passive", false, new List<ToggleInfo>() {
            new ToggleInfo(" FreeCosmetics", () => CheatToggles.freeCosmetics, x => CheatToggles.freeCosmetics = x),
            new ToggleInfo(" AvoidPenalties", () => CheatToggles.avoidBans, x => CheatToggles.avoidBans = x),
            new ToggleInfo(" UnlockFeatures", () => CheatToggles.unlockFeatures, x => CheatToggles.unlockFeatures = x),
        }, new List<SubmenuInfo>()));
    }

    private void Update(){
        if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), MalumMenu.menuKeybind.Value)))
        {
            //Enable-disable GUI with DELETE key
            isGUIActive = !isGUIActive;

            //Also teleport the window to the mouse for immediate use
            Vector2 mousePosition = Input.mousePosition;
            windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        }

        //Passive cheats are always on to avoid problems
        CheatToggles.unlockFeatures = CheatToggles.freeCosmetics = CheatToggles.avoidBans = true;

        //Host-only cheats are turned off if LocalPlayer is not the game's host
        if(!CheatChecks.isHost){
            CheatToggles.voteImmune = CheatToggles.godMode = CheatToggles.impostorHack = CheatToggles.evilVote = false;
        }

        //Some cheats only work if the ship is present, so they are turned off if it is not
        if(!CheatChecks.isShip){
            CheatToggles.unfixableLights = CheatToggles.kickVents = CheatToggles.reportBody = CheatToggles.closeMeeting = CheatToggles.reactorSab = CheatToggles.oxygenSab = CheatToggles.commsSab = CheatToggles.mushSab = CheatToggles.doorsSab = false;
        }

        if(!CheatChecks.isPlayer){
            CheatToggles.copyPlayerFC = CheatToggles.copyPlayerPUID = CheatToggles.spamChat = CheatToggles.chatMimic = CheatToggles.spectate = CheatToggles.freeCam = CheatToggles.kickPlayer = CheatToggles.murderPlayer = CheatToggles.mimicOutfit = CheatToggles.mimicAllOutfits = false;
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

            windowRect = GUI.Window(0, windowRect, (UnityEngine.GUI.WindowFunction)WindowFunction, "MalumMenu v" + MalumMenu.malumVersion);
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
