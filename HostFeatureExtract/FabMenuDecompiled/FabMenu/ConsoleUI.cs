using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace FabMenu;

public class ConsoleUI : MonoBehaviour
{
	public bool isVisible;

	private Vector2 scrollPosition = Vector2.zero;

	private static List<string> logEntries = new List<string>();

	private const int MaxLogEntries = 100;

	private Rect windowRect = new Rect(320f, 10f, 500f, 300f);

	private GUIStyle logStyle;

	public void Log(string message)
	{
		if (logEntries.Count >= 100)
		{
			logEntries.RemoveAt(0);
		}
		logEntries.Add(message);
		scrollPosition.y = float.MaxValue;
	}

	private void OnGUI()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (isVisible)
		{
			if (logStyle == null)
			{
				logStyle = new GUIStyle(GUI.skin.label)
				{
					fontSize = 20
				};
			}
			Color backgroundColor = default(Color);
			if (ColorUtility.TryParseHtmlString(FabMenu.menuHtmlColor.Value, ref backgroundColor))
			{
				GUI.backgroundColor = backgroundColor;
			}
			windowRect = GUI.Window(1, windowRect, WindowFunction.op_Implicit((Action<int>)ConsoleWindow), "MalumConsole");
		}
	}

	private void ConsoleWindow(int windowID)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, Array.Empty<GUILayoutOption>());
		Enumerator<string> enumerator = logEntries.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GUILayout.Label(enumerator.Current, logStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUI.DragWindow();
	}
}
