using System;
using System.Collections.Generic;

namespace MalumMenu
{
    public struct SubmenuInfo
    {
        public string name;
        public bool isExpanded;
        public List<ToggleInfo> toggles;

        public SubmenuInfo(string name, bool isExpanded, List<ToggleInfo> toggles)
        {
            this.name = name;
            this.isExpanded = isExpanded;
            this.toggles = toggles;
        }
    }

    public struct GroupInfo
    {
        public string name;
        public bool isExpanded;
        public List<ToggleInfo> toggles;
        public List<SubmenuInfo> submenus;

        public GroupInfo(string name, bool isExpanded, List<ToggleInfo> toggles, List<SubmenuInfo> submenus)
        {
            this.name = name;
            this.isExpanded = isExpanded;
            this.toggles = toggles;
            this.submenus = submenus;
        }
    }

    public struct ToggleInfo
    {
        public string label;
        public Func<bool> getState;
        public Action<bool> setState;

        public ToggleInfo(string label, Func<bool> getState, Action<bool> setState)
        {
            this.label = label;
            this.getState = getState;
            this.setState = setState;
        }
    }
}
