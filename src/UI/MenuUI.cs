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
        groups.Add(new GroupInfo("玩家", false, new List<ToggleInfo>() {
            new ToggleInfo(" 穿墙", () => CheatToggles.noClip, x => CheatToggles.noClip = x),
            new ToggleInfo(" 速度3x", () => CheatToggles.speedBoost, x => CheatToggles.speedBoost = x),
            }, new List<SubmenuInfo> {
            new SubmenuInfo("击杀", false, new List<ToggleInfo>() {
                new ToggleInfo(" 击杀指定玩家", () => CheatToggles.murderPlayer, x => CheatToggles.murderPlayer = x),
                new ToggleInfo(" 击杀所有玩家", () => CheatToggles.murderAll, x => CheatToggles.murderAll = x),
                new ToggleInfo(" 持续击杀", () => CheatToggles.ContinuousKillingAll, x => CheatToggles.ContinuousKillingAll = x),
                new ToggleInfo(" 持续击杀指定玩家", () => CheatToggles.chixumurderPlayer, x => CheatToggles.chixumurderPlayer = x),
            }),
            new SubmenuInfo("传送", false, new List<ToggleInfo>() {
                new ToggleInfo(" 到 鼠标", () => CheatToggles.teleportCursor, x => CheatToggles.teleportCursor = x),
                new ToggleInfo(" 到 指定玩家", () => CheatToggles.teleportPlayer, x => CheatToggles.teleportPlayer = x),
            }),
        }
        ));

        groups.Add(new GroupInfo("系统级别外挂", false, new List<ToggleInfo>() {
            new ToggleInfo(" 见鬼啦！", () => CheatToggles.seeGhosts, x => CheatToggles.seeGhosts = x),
            new ToggleInfo(" 明亮！", () => CheatToggles.fullBright, x => CheatToggles.fullBright = x),
            new ToggleInfo(" 投票可视", () => CheatToggles.revealVotes, x => CheatToggles.revealVotes = x),
            new ToggleInfo(" 总是聊天", () => CheatToggles.alwaysChat, x => CheatToggles.alwaysChat = x)
        }, new List<SubmenuInfo> {
            new SubmenuInfo("名字标签", false, new List<ToggleInfo>() {
                new ToggleInfo(" 职业", () => CheatToggles.seeRoles, x => CheatToggles.seeRoles = x),
                new ToggleInfo(" 管道", () => CheatToggles.ventVision, x => CheatToggles.ventVision = x)
            }),
            new SubmenuInfo("视角", false, new List<ToggleInfo>() {
                new ToggleInfo(" 缩放", () => CheatToggles.zoomOut, x => CheatToggles.zoomOut = x),
                new ToggleInfo(" 观战", () => CheatToggles.spectate, x => CheatToggles.spectate = x),
                new ToggleInfo(" 自由", () => CheatToggles.freecam, x => CheatToggles.freecam = x)
            }),
            new SubmenuInfo("追踪", false, new List<ToggleInfo>() {
                new ToggleInfo(" 船员", () => CheatToggles.tracersCrew, x => CheatToggles.tracersCrew = x),
                new ToggleInfo(" 内鬼", () => CheatToggles.tracersImps, x => CheatToggles.tracersImps = x),
                new ToggleInfo(" 鬼魂", () => CheatToggles.tracersGhosts, x => CheatToggles.tracersGhosts = x),
                new ToggleInfo(" 尸体", () => CheatToggles.tracersBodies, x => CheatToggles.tracersBodies = x),
                new ToggleInfo(" 颜色显示", () => CheatToggles.colorBasedTracers, x => CheatToggles.colorBasedTracers = x),
            }),
            new SubmenuInfo("小地图标志", false, new List<ToggleInfo>() {
                new ToggleInfo(" 船员", () => CheatToggles.mapCrew, x => CheatToggles.mapCrew = x),
                new ToggleInfo(" 内鬼", () => CheatToggles.mapImps, x => CheatToggles.mapImps = x),
                new ToggleInfo(" 鬼魂", () => CheatToggles.mapGhosts, x => CheatToggles.mapGhosts = x),
                new ToggleInfo(" 颜色显示", () => CheatToggles.colorBasedMap, x => CheatToggles.colorBasedMap = x)
            }),
        }));

        groups.Add(new GroupInfo("职业", false, new List<ToggleInfo>() {
            new ToggleInfo(" 修改职业", () => CheatToggles.changeRole, x => CheatToggles.changeRole = x),
        }, 
            new List<SubmenuInfo> {
                new SubmenuInfo("船员", false, new List<ToggleInfo>() {
                    new ToggleInfo(" 完成所有任务", () => CheatToggles.completeMyTasks, x => CheatToggles.completeMyTasks = x)
                }),
                new SubmenuInfo("内鬼", false, new List<ToggleInfo>() {
                    new ToggleInfo(" 可击杀所有人", () => CheatToggles.killAnyone, x => CheatToggles.killAnyone = x),
                    new ToggleInfo(" 无CD", () => CheatToggles.zeroKillCd, x => CheatToggles.zeroKillCd = x),
                    new ToggleInfo(" 无击杀范围", () => CheatToggles.killReach, x => CheatToggles.killReach = x),
                }),
                new SubmenuInfo("变形", false, new List<ToggleInfo>() {
                    new ToggleInfo(" 没有蛋壳动画", () => CheatToggles.noShapeshiftAnim, x => CheatToggles.noShapeshiftAnim = x),
                    new ToggleInfo(" 无限持续", () => CheatToggles.endlessSsDuration, x => CheatToggles.endlessSsDuration = x),
                }),
                new SubmenuInfo("工程师", false, new List<ToggleInfo>() {
                    new ToggleInfo(" 无限管道", () => CheatToggles.endlessVentTime, x => CheatToggles.endlessVentTime = x),
                    new ToggleInfo(" 无管道CD", () => CheatToggles.noVentCooldown, x => CheatToggles.noVentCooldown = x),
                }),
                new SubmenuInfo("科学家", false, new List<ToggleInfo>() {
                    new ToggleInfo(" 无尽电池", () => CheatToggles.endlessBattery, x => CheatToggles.endlessBattery = x),
                    new ToggleInfo(" 无电池冷却", () => CheatToggles.noVitalsCooldown, x => CheatToggles.noVitalsCooldown = x),
                }),
            }));

        groups.Add(new GroupInfo("船", false, new List<ToggleInfo> {
            new ToggleInfo(" 无法修复的灯", () => CheatToggles.unfixableLights, x => CheatToggles.unfixableLights = x),
            new ToggleInfo(" 报告尸体", () => CheatToggles.reportBody, x => CheatToggles.reportBody = x),
            new ToggleInfo(" 关闭会议界面", () => CheatToggles.closeMeeting, x => CheatToggles.closeMeeting = x),
        }, new List<SubmenuInfo> {
            new SubmenuInfo("破坏", false, new List<ToggleInfo>() {
                new ToggleInfo(" 反应堆", () => CheatToggles.reactorSab, x => CheatToggles.reactorSab = x),
                new ToggleInfo(" 氧气", () => CheatToggles.oxygenSab, x => CheatToggles.oxygenSab = x),
                new ToggleInfo(" 灯", () => CheatToggles.elecSab, x => CheatToggles.elecSab = x),
                new ToggleInfo(" 通讯", () => CheatToggles.commsSab, x => CheatToggles.commsSab = x),
                new ToggleInfo(" 门", () => CheatToggles.doorsSab, x => CheatToggles.doorsSab = x),
                new ToggleInfo(" 蘑菇", () => CheatToggles.mushSab, x => CheatToggles.mushSab = x),
            }),
            new SubmenuInfo("管道", false, new List<ToggleInfo>() {
                new ToggleInfo(" 可使用管道", () => CheatToggles.useVents, x => CheatToggles.useVents = x),
                new ToggleInfo(" 释放管道", () => CheatToggles.kickVents, x => CheatToggles.kickVents = x),
                new ToggleInfo(" 在管道内行动", () => CheatToggles.walkVent, x => CheatToggles.walkVent = x)
            }),
        }));

        // Host-Only cheats are temporarly disabled because of some bugs

        //groups.Add(new GroupInfo("Host-Only", false, new List<ToggleInfo>() {
        //    new ToggleInfo(" ImpostorHack", () => CheatSettings.impostorHack, x => CheatSettings.impostorHack = x),
        //    new ToggleInfo(" Godmode", () => CheatSettings.godMode, x => CheatSettings.godMode = x),
        //    new ToggleInfo(" EvilVote", () => CheatSettings.evilVote, x => CheatSettings.evilVote = x),
        //    new ToggleInfo(" VoteImmune", () => CheatSettings.voteImmune, x => CheatSettings.voteImmune = x)
        //}, new List<SubmenuInfo>()));

        // Console is temporarly disabled until we implement some features for it

        //groups.Add(new GroupInfo("Console", false, new List<ToggleInfo>() {
        //    new ToggleInfo(" ConsoleUI", () => MalumMenu.consoleUI.isVisible, x => MalumMenu.consoleUI.isVisible = x),
        //}, new List<SubmenuInfo>()));

        groups.Add(new GroupInfo("系统级别", false, new List<ToggleInfo>() {
            new ToggleInfo(" 免费皮肤", () => CheatToggles.freeCosmetics, x => CheatToggles.freeCosmetics = x),
            new ToggleInfo(" 避免惩罚", () => CheatToggles.avoidBans, x => CheatToggles.avoidBans = x),
            new ToggleInfo(" 解锁功能", () => CheatToggles.unlockFeatures, x => CheatToggles.unlockFeatures = x),
        }, new List<SubmenuInfo>()));
    }

    private void Update(){

        if (Input.GetKeyDown(Utils.stringToKeycode(MalumMenu.menuKeybind.Value)))
        {
            //Enable-disable GUI with DELETE key
            isGUIActive = !isGUIActive;

            //Also teleport the window to the mouse for immediate use
            Vector2 mousePosition = Input.mousePosition;
            windowRect.position = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        }

        //Passive cheats are always on to avoid problems
        CheatToggles.unlockFeatures = CheatToggles.freeCosmetics = CheatToggles.avoidBans = true;

        if(!Utils.isPlayer){
            CheatToggles.changeRole = CheatToggles.murderAll = CheatToggles.teleportCursor = CheatToggles.teleportPlayer = CheatToggles.spectate = CheatToggles.freecam = CheatToggles.murderPlayer;
        }

        //Host-only cheats are turned off if LocalPlayer is not the game's host
        //if(!CheatChecks.isHost){
        //    CheatToggles.voteImmune = CheatToggles.godMode = CheatToggles.impostorHack = CheatToggles.evilVote = false;
        //}

        //Some cheats only work if the ship is present, so they are turned off if it is not
        if(!Utils.isShip){
            CheatToggles.unfixableLights = CheatToggles.completeMyTasks = CheatToggles.kickVents = CheatToggles.reportBody = CheatToggles.closeMeeting = CheatToggles.reactorSab = CheatToggles.oxygenSab = CheatToggles.commsSab = CheatToggles.elecSab = CheatToggles.mushSab = CheatToggles.doorsSab = false;
            //Sabotages_ShipStatus_FixedUpdate_Postfix.reactorSab = Sabotages_ShipStatus_FixedUpdate_Postfix.commsSab = Sabotages_ShipStatus_FixedUpdate_Postfix.elecSab = Sabotages_ShipStatus_FixedUpdate_Postfix.oxygenSab = UnfixableLights_ShipStatus_FixedUpdate_Postfix.isActive = false;
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

        //Only change the window height while the user is not dragging it
        //Or else dragging breaks
        if (!isDragging)
        {
            int windowHeight = CalculateWindowHeight();
            windowRect.height = windowHeight;
        }

        Color uiColor;

        string configHtmlColor = MalumMenu.menuHtmlColor.Value;

        if (!configHtmlColor.StartsWith("#"))
        {
            configHtmlColor = "#" + configHtmlColor;
        }

        if(ColorUtility.TryParseHtmlString(configHtmlColor, out uiColor)){
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
