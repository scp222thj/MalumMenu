using System;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace FabMenu;

public class DoorsUI : MonoBehaviour
{
	private Rect _windowRect = new Rect(320f, 10f, 530f, 280f);

	private GUIStyle _separatorStyle;

	private GUIStyle _normalButtonStyle;

	private GUIStyle _normalToggleStyle;

	private List<SystemTypes> doorsToSpamOpen = new List<SystemTypes>();

	private List<SystemTypes> doorsToSpamClose = new List<SystemTypes>();

	private void OnGUI()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0064: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (CheatToggles.showDoorsMenu)
		{
			if (_separatorStyle == null)
			{
				GUIStyle val = new GUIStyle(GUI.skin.box);
				val.normal.background = Texture2D.whiteTexture;
				val.margin = new RectOffset
				{
					top = 4,
					bottom = 4
				};
				val.padding = new RectOffset();
				val.border = new RectOffset();
				_separatorStyle = val;
			}
			if (_normalButtonStyle == null)
			{
				_normalButtonStyle = new GUIStyle(GUI.skin.button)
				{
					fontSize = 13
				};
			}
			if (_normalToggleStyle == null)
			{
				_normalToggleStyle = new GUIStyle(GUI.skin.toggle)
				{
					fontSize = 13
				};
			}
			Color backgroundColor = default(Color);
			if (ColorUtility.TryParseHtmlString(FabMenu.menuHtmlColor.Value, ref backgroundColor))
			{
				GUI.backgroundColor = backgroundColor;
			}
			_windowRect = GUI.Window(2, _windowRect, WindowFunction.op_Implicit((Action<int>)DoorsWindow), "Doors");
		}
	}

	private unsafe void DoorsWindow(int windowID)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Invalid comparison between Unknown and I4
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Invalid comparison between Unknown and I4
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Invalid comparison between Unknown and I4
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Invalid comparison between Unknown and I4
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Invalid comparison between Unknown and I4
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Invalid comparison between Unknown and I4
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Invalid comparison between Unknown and I4
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Invalid comparison between Unknown and I4
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		if (!Utils.isShip)
		{
			GUI.DragWindow();
			return;
		}
		MapNames val = (MapNames)Utils.getCurrentMapID();
		if ((int)val == 1)
		{
			GUI.DragWindow();
			return;
		}
		GUILayout.BeginVertical((Il2CppReferenceArray<GUILayoutOption>)null);
		bool flag;
		foreach (SystemTypes doorRoom in DoorsHandler.GetDoorRooms())
		{
			GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
			GUILayout.Label(((object)(*(SystemTypes*)(&doorRoom))/*cast due to .constrained prefix*/).ToString() ?? "", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(120f) });
			GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
			GUILayout.Label("Status: " + DoorsHandler.GetStatusOfDoorsInRoom(doorRoom, colorize: true), (Il2CppReferenceArray<GUILayoutOption>)null);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Close", _normalButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f) }))
			{
				DoorsHandler.CloseDoorsOfRoom(doorRoom);
			}
			flag = (((int)val == 2 || val - 4 <= 1) ? true : false);
			if (flag && GUILayout.Button("Open", _normalButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f) }))
			{
				DoorsHandler.OpenDoorsOfRoom(doorRoom);
			}
			if (Utils.isHost)
			{
				bool flag2 = doorsToSpamClose.Contains(doorRoom);
				flag2 = GUILayout.Toggle(flag2, "Spam Close", _normalToggleStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
				if (flag2 && !doorsToSpamClose.Contains(doorRoom))
				{
					doorsToSpamClose.Add(doorRoom);
				}
				else if (!flag2 && doorsToSpamClose.Contains(doorRoom))
				{
					doorsToSpamClose.Remove(doorRoom);
				}
				if (((int)val == 2 || val - 4 <= 1) ? true : false)
				{
					bool flag3 = doorsToSpamOpen.Contains(doorRoom);
					flag3 = GUILayout.Toggle(flag3, "Spam Open", _normalToggleStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
					if (flag3 && !doorsToSpamOpen.Contains(doorRoom))
					{
						doorsToSpamOpen.Add(doorRoom);
					}
					else if (!flag3 && doorsToSpamOpen.Contains(doorRoom))
					{
						doorsToSpamOpen.Remove(doorRoom);
					}
				}
			}
			else if (doorsToSpamClose.Count != 0 || doorsToSpamOpen.Count != 0)
			{
				doorsToSpamClose.Clear();
				doorsToSpamOpen.Clear();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
		}
		GUILayout.FlexibleSpace();
		GUILayout.Box("", _separatorStyle, (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			GUILayout.Height(1f),
			GUILayout.ExpandWidth(true)
		});
		GUILayout.Box("", GUIStyle.none, (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			GUILayout.Height(1f),
			GUILayout.ExpandWidth(true)
		});
		GUILayout.BeginHorizontal((Il2CppReferenceArray<GUILayoutOption>)null);
		if (GUILayout.Button("Close All", _normalButtonStyle, Array.Empty<GUILayoutOption>()))
		{
			CheatToggles.closeAllDoors = true;
		}
		flag = (((int)val == 2 || val - 4 <= 1) ? true : false);
		if (flag && GUILayout.Button("Open All", _normalButtonStyle, Array.Empty<GUILayoutOption>()))
		{
			CheatToggles.openAllDoors = true;
		}
		GUILayout.FlexibleSpace();
		if (Utils.isHost)
		{
			CheatToggles.spamCloseAllDoors = GUILayout.Toggle(CheatToggles.spamCloseAllDoors, "Spam Close All", _normalToggleStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
			if (((int)val == 2 || val - 4 <= 1) ? true : false)
			{
				CheatToggles.spamOpenAllDoors = GUILayout.Toggle(CheatToggles.spamOpenAllDoors, "Spam Open All", _normalToggleStyle, (Il2CppReferenceArray<GUILayoutOption>)null);
			}
		}
		else
		{
			CheatToggles.spamCloseAllDoors = (CheatToggles.spamOpenAllDoors = false);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUI.DragWindow();
	}

	public void Update()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!Utils.isShip)
		{
			return;
		}
		Enumerator<SystemTypes> enumerator = doorsToSpamClose.GetEnumerator();
		while (enumerator.MoveNext())
		{
			DoorsHandler.CloseDoorsOfRoom(enumerator.Current);
		}
		MapNames val = (MapNames)Utils.getCurrentMapID();
		if (((int)val == 2 || val - 4 <= 1) ? true : false)
		{
			enumerator = doorsToSpamOpen.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DoorsHandler.OpenDoorsOfRoom(enumerator.Current);
			}
		}
	}
}
