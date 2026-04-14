using System.Collections.Generic;

namespace FabMenu;

public struct GroupInfo(string name, bool isExpanded, List<ToggleInfo> toggles, List<SubmenuInfo> submenus)
{
	public string name = name;

	public bool isExpanded = isExpanded;

	public List<ToggleInfo> toggles = toggles;

	public List<SubmenuInfo> submenus = submenus;
}
