using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Text;
using UnityEngine;

namespace FabMenu;

public class TasksUI : MonoBehaviour
{
	private Vector2 _scrollPosition = Vector2.zero;

	private Rect _windowRect = new Rect(320f, 10f, 500f, 300f);

	private GUIStyle _playerHeaderStyle;

	private GUIStyle _normalButtonStyle;

	private StringBuilder _tasksString = new StringBuilder();

	private readonly Dictionary<string, bool> _expandedPlayers = new Dictionary<string, bool>();

	private void OnGUI()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.showTasksMenu)
		{
			if (_playerHeaderStyle == null)
			{
				_playerHeaderStyle = new GUIStyle(GUI.skin.button)
				{
					fontSize = 18,
					alignment = (TextAnchor)3
				};
			}
			if (_normalButtonStyle == null)
			{
				_normalButtonStyle = new GUIStyle(GUI.skin.button)
				{
					fontSize = 13
				};
			}
			Color backgroundColor = default(Color);
			if (ColorUtility.TryParseHtmlString(FabMenu.menuHtmlColor.Value, ref backgroundColor))
			{
				GUI.backgroundColor = backgroundColor;
			}
			_windowRect = GUI.Window(3, _windowRect, WindowFunction.op_Implicit((Action<int>)TasksWindow), "Tasks");
		}
	}

	private void TasksWindow(int windowID)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Invalid comparison between Unknown and I4
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Invalid comparison between Unknown and I4
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Invalid comparison between Unknown and I4
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Invalid comparison between Unknown and I4
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Invalid comparison between Unknown and I4
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Invalid comparison between Unknown and I4
		GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, Array.Empty<GUILayoutOption>());
		Enumerator<PlayerControl> enumerator = PlayerControl.AllPlayerControls.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PlayerControl current = enumerator.Current;
			if (!Object.op_Implicit((Object)(object)current.Data) || !Object.op_Implicit((Object)(object)current.Data.Role) || string.IsNullOrEmpty(current.Data.PlayerName))
			{
				continue;
			}
			GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
			string name = ((Object)current).name;
			_expandedPlayers.TryGetValue(name, out var value);
			string value2 = (value ? "▼" : "▶");
			int num = current.myTasks.Count;
			int value3 = ((IEnumerable<PlayerTask>)current.myTasks.ToArray()).Count((PlayerTask t) => t.IsComplete);
			if ((Object)(object)current == (Object)(object)PlayerControl.LocalPlayer && current.Data.IsDead)
			{
				num--;
			}
			if ((Object)(object)current == (Object)(object)PlayerControl.LocalPlayer && Utils.isAnySabotageActive)
			{
				num--;
			}
			if ((Object)(object)current == (Object)(object)PlayerControl.LocalPlayer && current.Data.Role.IsImpostor)
			{
				num--;
			}
			if (GUILayout.Button($"{value2} [{value3}/{num}] <color=#{ColorUtility.ToHtmlStringRGB(current.Data.Color)}>{name}</color>", _playerHeaderStyle, Array.Empty<GUILayoutOption>()))
			{
				_expandedPlayers[name] = !value;
				value = !value;
			}
			if (value)
			{
				GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
				GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
				Enumerator<PlayerTask> enumerator2 = current.myTasks.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					PlayerTask current2 = enumerator2.Current;
					TaskTypes taskType = current2.TaskType;
					if ((int)taskType <= 20)
					{
						if (taskType - 16 <= 1 || taskType - 19 <= 1)
						{
							goto IL_0247;
						}
					}
					else if ((int)taskType == 35 || (int)taskType == 59 || (int)taskType == 78)
					{
						goto IL_0247;
					}
					bool flag = false;
					goto IL_024f;
					IL_024f:
					if (flag)
					{
						continue;
					}
					_tasksString.Clear();
					current2.AppendTaskText(_tasksString);
					string text = ((Object)_tasksString).ToString();
					if (!text.Contains("You're dead") && !text.Contains("Sabotage and kill"))
					{
						GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
						GUILayout.Label(text.Replace("\n", "").Replace("</color>", "").Replace("<color=#00DD00FF>", "")
							.Replace("<color=#FFFF00FF>", ""), (Il2CppReferenceArray<GUILayoutOption>)null);
						GUILayout.FlexibleSpace();
						if (current2.IsComplete)
						{
							GUILayout.Label("<color=#00ff00>✔ Complete</color>", (Il2CppReferenceArray<GUILayoutOption>)null);
						}
						else if ((Object)(object)current == (Object)(object)PlayerControl.LocalPlayer && GUILayout.Button("Complete", _normalButtonStyle, Array.Empty<GUILayoutOption>()))
						{
							Utils.completeTask(current2);
						}
						GUILayout.EndHorizontal();
					}
					continue;
					IL_0247:
					flag = true;
					goto IL_024f;
				}
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndScrollView();
		if (GUILayout.Button("Complete My Tasks", _normalButtonStyle, Array.Empty<GUILayoutOption>()))
		{
			CheatToggles.completeMyTasks = true;
		}
		GUILayout.EndVertical();
		GUI.DragWindow();
	}
}
