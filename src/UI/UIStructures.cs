using System;
using System.Collections.Generic;

namespace MalumMenu
{
    public struct GroupInfo
    {
        public string name;
        public bool isExpanded;
        public List<ToggleInfo> toggles;

        public GroupInfo(string name, bool isExpanded, List<ToggleInfo> toggles)
        {
            this.name = name;
            this.isExpanded = isExpanded;
            this.toggles = toggles;
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
