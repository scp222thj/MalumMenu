using System.Collections.Generic;

namespace FabMenu;

public struct SubmenuInfo(string name, bool isExpanded, List<ToggleInfo> toggles)
{
	public string name = name;

	public bool isExpanded = isExpanded;

	public List<ToggleInfo> toggles = toggles;
}
