using System;

namespace FabMenu;

public struct ToggleInfo(string label, Func<bool> getState, Action<bool> setState)
{
	public string label = label;

	public Func<bool> getState = getState;

	public Action<bool> setState = setState;
}
