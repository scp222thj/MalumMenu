using UnityEngine;
using System.Collections.Generic;

namespace MalumMenu;
public class MenuUI : MonoBehaviour
{

    public List<GroupInfo> groups = new List<GroupInfo>();

    private bool isDragging = false;
    private Rect windowRect = new Rect(10, 10, 300, 500);
    private bool isGUIActive = false;

    //Create all groups (buttons) and their toggles on start
    private void Start()
    {
        groups.Add(new GroupInfo("Player", false, new List<ToggleInfo>() {
            new ToggleInfo(" NoClip", () => CheatSettings.noClip, x => CheatSettings.noClip = x),
            new ToggleInfo(" Teleport", () => CheatSettings.teleport, x => CheatSettings.teleport = x),
            new ToggleInfo(" SpeedBoost", () => CheatSettings.speedBoost, x => CheatSettings.speedBoost = x),
            new ToggleInfo(" NoCooldowns", () => CheatSettings.noCooldowns, x => CheatSettings.noCooldowns = x)
        }));

        groups.Add(new GroupInfo("ESP", false, new List<ToggleInfo>() {
            new ToggleInfo(" SeeGhosts", () => CheatSettings.seeGhosts, x => CheatSettings.seeGhosts = x),
            new ToggleInfo(" SeeRoles", () => CheatSettings.seeRoles, x => CheatSettings.seeRoles = x),
            new ToggleInfo(" FullBright", () => CheatSettings.fullBright, x => CheatSettings.fullBright = x),
            new ToggleInfo(" ZoomOut", () => CheatSettings.zoomOut, x => CheatSettings.zoomOut = x),
            new ToggleInfo(" Spectate", () => CheatSettings.spectate, x => CheatSettings.spectate = x)
        }));

        groups.Add(new GroupInfo("Tracers", false, new List<ToggleInfo>() {
            new ToggleInfo(" Crewmates", () => CheatSettings.tracersCrew, x => CheatSettings.tracersCrew = x),
            new ToggleInfo(" Impostors", () => CheatSettings.tracersImps, x => CheatSettings.tracersImps = x),
            new ToggleInfo(" Ghosts", () => CheatSettings.tracersGhosts, x => CheatSettings.tracersGhosts = x),
            new ToggleInfo(" Dead Bodies", () => CheatSettings.tracersBodies, x => CheatSettings.tracersBodies = x),
            new ToggleInfo(" Color-based", () => CheatSettings.colorBasedTracers, x => CheatSettings.colorBasedTracers = x),
        }));

        groups.Add(new GroupInfo("Minimap", false, new List<ToggleInfo>() {
            new ToggleInfo(" Crewmates", () => CheatSettings.mapCrew, x => CheatSettings.mapCrew = x),
            new ToggleInfo(" Impostors", () => CheatSettings.mapImps, x => CheatSettings.mapImps = x),
            new ToggleInfo(" Ghosts", () => CheatSettings.mapGhosts, x => CheatSettings.mapGhosts = x),
            new ToggleInfo(" Color-based", () => CheatSettings.colorBasedMap, x => CheatSettings.colorBasedMap = x)
        }));

        groups.Add(new GroupInfo("Sabotage", false, new List<ToggleInfo>() {
            new ToggleInfo(" Blackout", () => CheatSettings.blackOut, x => CheatSettings.blackOut = x),
            new ToggleInfo(" Doors", () => CheatSettings.fullLockdown, x => CheatSettings.fullLockdown = x),
            new ToggleInfo(" Reactor", () => CheatSettings.reactorSab, x => CheatSettings.reactorSab = x),
            new ToggleInfo(" Oxygen", () => CheatSettings.oxygenSab, x => CheatSettings.oxygenSab = x),
            new ToggleInfo(" Electrical", () => CheatSettings.elecSab, x => CheatSettings.elecSab = x),
            new ToggleInfo(" Comms", () => CheatSettings.commsSab, x => CheatSettings.commsSab = x),
        }));
        
        groups.Add(new GroupInfo("Vents", false, new List<ToggleInfo>() {
            new ToggleInfo(" UseVents", () => CheatSettings.useVents, x => CheatSettings.useVents = x),
            new ToggleInfo(" KickVents", () => CheatSettings.kickVents, x => CheatSettings.kickVents = x),
            new ToggleInfo(" VentVision", () => CheatSettings.ventVision, x => CheatSettings.ventVision = x),
            new ToggleInfo(" WalkInVents", () => CheatSettings.walkVent, x => CheatSettings.walkVent = x)
        }));

        groups.Add(new GroupInfo("Meetings", false, new List<ToggleInfo>() {
            new ToggleInfo(" RevealVotes", () => CheatSettings.revealVotes, x => CheatSettings.revealVotes = x),
            new ToggleInfo(" CloseMeeting", () => CheatSettings.closeMeeting, x => CheatSettings.closeMeeting = x),
            new ToggleInfo(" CallMeeting", () => CheatSettings.callMeeting, x => CheatSettings.callMeeting = x)
        }));

        groups.Add(new GroupInfo("Host-Only", false, new List<ToggleInfo>() {
            new ToggleInfo(" Godmode", () => CheatSettings.godMode, x => CheatSettings.godMode = x),
            new ToggleInfo(" EvilVote", () => CheatSettings.evilVote, x => CheatSettings.evilVote = x),
            new ToggleInfo(" VoteImmune", () => CheatSettings.voteImmune, x => CheatSettings.voteImmune = x)
        }));

        groups.Add(new GroupInfo("Passive", false, new List<ToggleInfo>() {
            new ToggleInfo(" AlwaysChat", () => CheatSettings.alwaysChat, x => CheatSettings.alwaysChat = x),
            new ToggleInfo(" FreeCosmetics", () => CheatSettings.freeCosmetics, x => CheatSettings.freeCosmetics = x),
            new ToggleInfo(" AvoidPenalties", () => CheatSettings.avoidBans, x => CheatSettings.avoidBans = x),
            new ToggleInfo(" UnlockFeatures", () => CheatSettings.unlockFeatures, x => CheatSettings.unlockFeatures = x)
        }));
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            //Enable-disable GUI with DELETE key
            isGUIActive = !isGUIActive;

            //Also teleport the window to the mouse for immediate use
            Vector2 mousePosition = Input.mousePosition;
            windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        }

        //Passive cheats are always on to avoid problems
        CheatSettings.alwaysChat = CheatSettings.unlockFeatures = CheatSettings.freeCosmetics = CheatSettings.avoidBans = true;

        //Host-only cheats are turned off if LocalPlayer is not the game's host
        if(!isHostCheck.isHost){
            CheatSettings.voteImmune = CheatSettings.godMode = CheatSettings.evilVote = false;
        }

        //Some cheats only work if the ship is present, so they are turned off if it is not
        if(!isShipCheck.isShip){
            CheatSettings.blackOut = CheatSettings.spectate = CheatSettings.kickVents = CheatSettings.callMeeting = CheatSettings.closeMeeting = CheatSettings.reactorSab = CheatSettings.oxygenSab = CheatSettings.commsSab = false;
        }
    }

    public void OnGUI()
    {
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
        int toggleSpacing = 50;
        int currentYPosition = 20;

        //Add a button for each group
        for(int i = 0; i < groups.Count; i++)
        {
            GroupInfo group = groups[i];

            //Expand group when its button is pressed
            if (GUI.Button(new Rect(10, currentYPosition, 280, 40), group.name))
            {
                group.isExpanded = !group.isExpanded;
                groups[i] = group;
            }

            if(group.isExpanded) //Group expansion
            {
                CloseAllGroupsExcept(i); //Close all other groups to avoid problems

                //Show all the toggles in the group
                for(int j = 0; j < group.toggles.Count; j++)
                {
                    bool currentState = group.toggles[j].getState();
                    bool newState = GUI.Toggle(new Rect(10, currentYPosition + ((j + 1) * toggleSpacing), 280, 40), currentState, group.toggles[j].label);
                    if(newState != currentState)
                    {
                        group.toggles[j].setState(newState);
                    }
                }
                currentYPosition += group.toggles.Count * toggleSpacing; //Update currentYPosition for each toggle
            }

            currentYPosition += groupSpacing; //Update currentYPosition for each group
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
        int baseHeight = 70;
        int groupHeight = 50;
        int expandedGroupHeight = 0;
        int totalHeight = baseHeight;

        foreach(GroupInfo group in groups)
        {
            expandedGroupHeight = group.toggles.Count * 50;
            totalHeight += group.isExpanded ? expandedGroupHeight : groupHeight;
        }

        return totalHeight;
    }

    //Closes all expanded groups other than indexToKeepOpen
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

}
