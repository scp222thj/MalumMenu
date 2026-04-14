using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using InnerNet;
using UnityEngine;

namespace FabMenu;

public static class ArrowHandler
{
	private static GameObject _cachedArrowTemplate;

	public static bool IsOwnedAndIncomplete(NormalPlayerTask task)
	{
		if ((Object)(object)((PlayerTask)task).Owner == (Object)null || !((InnerNetObject)((PlayerTask)task).Owner).AmOwner)
		{
			return false;
		}
		return !((PlayerTask)task).IsComplete;
	}

	private static void CacheArrowFromShipStatus()
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_cachedArrowTemplate != (Object)null)
		{
			return;
		}
		NormalPlayerTask[][] array = new NormalPlayerTask[3][]
		{
			Il2CppArrayBase<NormalPlayerTask>.op_Implicit((Il2CppArrayBase<NormalPlayerTask>)(object)ShipStatus.Instance.CommonTasks),
			Il2CppArrayBase<NormalPlayerTask>.op_Implicit((Il2CppArrayBase<NormalPlayerTask>)(object)ShipStatus.Instance.LongTasks),
			Il2CppArrayBase<NormalPlayerTask>.op_Implicit((Il2CppArrayBase<NormalPlayerTask>)(object)ShipStatus.Instance.ShortTasks)
		};
		foreach (NormalPlayerTask[] array2 in array)
		{
			foreach (NormalPlayerTask val in array2)
			{
				if ((Object)(object)val.Arrow != (Object)null)
				{
					_cachedArrowTemplate = ((Component)val.Arrow).gameObject;
					Debug.Log(Object.op_Implicit($"Cached Arrow gameObject from ShipStatus for task {((PlayerTask)val).TaskType}"));
					return;
				}
				Debug.Log(Object.op_Implicit($"No Arrow gameObject found on task {((PlayerTask)val).TaskType}"));
			}
		}
	}

	public static ArrowBehaviour CreateArrowForTask(NormalPlayerTask task)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		CacheArrowFromShipStatus();
		Debug.Log(Object.op_Implicit($"Creating task arrow by cloning cached template for task {((PlayerTask)task).TaskType}"));
		GameObject obj = Object.Instantiate<GameObject>(_cachedArrowTemplate, ((Component)task).transform, false);
		((Object)obj).name = "MalumArrow";
		return obj.GetComponent<ArrowBehaviour>();
	}

	public static void EnsureArrowExists(NormalPlayerTask task)
	{
		if (IsOwnedAndIncomplete(task) && !((Object)(object)task.Arrow != (Object)null))
		{
			task.Arrow = CreateArrowForTask(task);
		}
	}

	public static bool NeedsSpecialTarget(NormalPlayerTask task)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		TaskTypes taskType = ((PlayerTask)task).TaskType;
		if ((int)taskType <= 53)
		{
			if ((int)taskType == 11 || (int)taskType == 53)
			{
				goto IL_0027;
			}
		}
		else if ((int)taskType == 55 || (int)taskType == 66 || (int)taskType == 73)
		{
			goto IL_0027;
		}
		return false;
		IL_0027:
		return true;
	}

	private static void SetArrowTarget(NormalPlayerTask task, Console targetConsole)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)targetConsole == (Object)null))
		{
			task.Arrow.target = ((Component)targetConsole).transform.position;
			((PlayerTask)task).StartAt = targetConsole.Room;
		}
	}

	public static void SetArrowTargetForSpecialTasks(NormalPlayerTask task)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		if ((Object)(object)task.Arrow == (Object)null)
		{
			return;
		}
		TaskTypes taskType = ((PlayerTask)task).TaskType;
		if ((int)taskType <= 53)
		{
			if ((int)taskType != 11)
			{
				if ((int)taskType == 53 && ((PlayerTask)task).TaskStep == 0)
				{
					Console targetConsole = ((PlayerTask)task).FindSpecialConsole(Func<Console, bool>.op_Implicit((Func<Console, bool>)((Console c) => ((PlayerTask)task).ValidConsole(c))));
					SetArrowTarget(task, targetConsole);
				}
			}
			else if (((PlayerTask)task).TaskStep == 0)
			{
				List<Console> val = ((PlayerTask)task).FindConsoles();
				if (val != null && val.Count > 0)
				{
					SetArrowTarget(task, val[0]);
				}
			}
		}
		else if ((int)taskType != 55)
		{
			if ((int)taskType != 66)
			{
				if ((int)taskType != 73 || task.taskStep != 0)
				{
					return;
				}
				List<Console> val2 = NormalPlayerTask.PickRandomConsoles(0, (TaskTypes)73);
				if (val2 != null && val2.Count > 0)
				{
					Console targetConsole2 = ((IEnumerable<Console>)val2.ToArray()).FirstOrDefault((Console c) => c.ConsoleId == ((Il2CppArrayBase<byte>)(object)task.Data)[0]);
					SetArrowTarget(task, targetConsole2);
				}
			}
			else
			{
				if (task.taskStep != 0)
				{
					return;
				}
				List<Console> val3 = NormalPlayerTask.PickRandomConsoles(0, (TaskTypes)66);
				if (val3 != null && val3.Count > 0)
				{
					Console targetConsole3 = ((IEnumerable<Console>)val3.ToArray()).FirstOrDefault((Console c) => c.ConsoleId == ((Il2CppArrayBase<byte>)(object)task.Data)[0]);
					SetArrowTarget(task, targetConsole3);
				}
			}
		}
		else if (task.taskStep == 0)
		{
			Console targetConsole4 = ((PlayerTask)task).FindSpecialConsole(Func<Console, bool>.op_Implicit((Func<Console, bool>)((Console c) => ((PlayerTask)task).ValidConsole(c) && c.ConsoleId == 0)));
			SetArrowTarget(task, targetConsole4);
		}
	}
}
