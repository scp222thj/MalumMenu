using UnityEngine;
using System.Collections.Generic;

namespace MalumMenu.UI
{
    internal static class OldUi
    {
        public static void Render(ref Rect windowRect, List<GroupInfo> groups, ref bool isDragging, GUIStyle submenuStyle)
        {
            
            windowRect.width = 300;
            if (!isDragging)
            {
                windowRect.height = CalculateWindowHeight(groups);
            }

            if (Event.current.type == EventType.MouseDrag) isDragging = true;
            if (Event.current.type == EventType.MouseUp) isDragging = false;

            ApplyColor();

            // Draw Window
            windowRect = GUI.Window(0, windowRect, (GUI.WindowFunction)((id) =>
            {
                DrawContent(groups, submenuStyle);
                GUI.DragWindow();
            }), "MalumMenu v" + MalumMenu.malumVersion);
        }

        private static void DrawContent(List<GroupInfo> groups, GUIStyle submenuButtonStyle)
        {
            
            int groupSpacing = 50;
            int toggleSpacing = 40;
            int submenuSpacing = 40;
            int currentY = 20;

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];

                // Group Button
                if (GUI.Button(new Rect(10, currentY, 280, 40), group.name))
                {
                    group.isExpanded = !group.isExpanded;
                    if (group.isExpanded) CloseOthers(groups, i);
                }
                currentY += groupSpacing;

                if (group.isExpanded)
                {
                    // Group Toggles
                    foreach (var toggle in group.toggles)
                    {
                        bool newState = GUI.Toggle(new Rect(20, currentY, 260, 30), toggle.getState(), toggle.label);
                        toggle.setState(newState);
                        currentY += toggleSpacing;
                    }

                    // Submenus
                    for (int j = 0; j < group.submenus.Count; j++)
                    {
                        var sub = group.submenus[j];
                        if (GUI.Button(new Rect(20, currentY, 260, 30), sub.name, submenuButtonStyle))
                        {
                            sub.isExpanded = !sub.isExpanded;
                            if (sub.isExpanded) CloseSubOthers(group, j);
                        }
                        currentY += submenuSpacing;

                        if (sub.isExpanded)
                        {
                            foreach (var st in sub.toggles)
                            {
                                bool sNew = GUI.Toggle(new Rect(30, currentY, 250, 30), st.getState(), st.label);
                                st.setState(sNew);
                                currentY += toggleSpacing;
                            }
                        }
                    }
                }
            }
        }

        private static int CalculateWindowHeight(List<GroupInfo> groups)
        {
            
            int totalHeight = 70;
            int groupHeight = 50;
            int toggleHeight = 40; 
            int submenuHeight = 40;

            foreach (GroupInfo group in groups)
            {
                totalHeight += groupHeight;
                if (group.isExpanded)
                {
                    totalHeight += group.toggles.Count * toggleHeight;
                    foreach (SubmenuInfo submenu in group.submenus)
                    {
                        totalHeight += submenuHeight;
                        if (submenu.isExpanded)
                        {
                            totalHeight += submenu.toggles.Count * toggleHeight;
                        }
                    }
                }
            }
            return totalHeight;
        }

        private static void CloseOthers(List<GroupInfo> groups, int active)
        {
            for (int i = 0; i < groups.Count; i++) if (i != active) groups[i].isExpanded = false;
        }

        private static void CloseSubOthers(GroupInfo group, int active)
        {
            for (int i = 0; i < group.submenus.Count; i++) if (i != active) group.submenus[i].isExpanded = false;
        }

        private static void ApplyColor()
        {
            //Just import color logic from Pr #410 once merged
            string hex = MalumMenu.menuHtmlColor.Value;
            if (ColorUtility.TryParseHtmlString(hex.StartsWith("#") ? hex : "#" + hex, out Color col))
                GUI.backgroundColor = col;
        }
    }
}