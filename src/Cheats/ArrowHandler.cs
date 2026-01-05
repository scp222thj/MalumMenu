using UnityEngine;
using System.Linq;

namespace MalumMenu;

public static class ArrowHandler
{
    // Cache for an arrow template GameObject to clone from
    private static GameObject _cachedArrowTemplate;

    /// <summary>
    /// Determines if a task is owned by the local player and incomplete.
    /// </summary>
    /// <param name="task">The task to check.</param>
    /// <returns>True if the task is owned and incomplete, false otherwise.</returns>
    public static bool IsOwnedAndIncomplete(NormalPlayerTask task)
    {
        if (task.Owner == null || !task.Owner.AmOwner) return false;

        return !task.IsComplete;
    }

    /// <summary>
    /// Searches task prefabs in ShipStatus for arrow GameObjects to cache for cloning.
    /// </summary>
    private static void CacheArrowFromShipStatus()
    {
        if (_cachedArrowTemplate != null) return;

        NormalPlayerTask[][] allTasksArrays = [ShipStatus.Instance.CommonTasks, ShipStatus.Instance.LongTasks, ShipStatus.Instance.ShortTasks];
        foreach (var tasks in allTasksArrays) // I tried using ShipStatus.Instance.GetAllTasks() but couldn't get it to work
        {
            foreach (var task in tasks)
            {
                if (task.Arrow != null)
                {
                    // This code always clones the Arrow from FixWiring (except on Fungle), but the user might be playing on a custom map
                    // that doesn't have FixWiring. So we loop through all tasks instead.
                    _cachedArrowTemplate = task.Arrow.gameObject;
                    Debug.Log($"Cached Arrow gameObject from ShipStatus for task {task.TaskType}");
                    return;
                }
                Debug.Log($"No Arrow gameObject found on task {task.TaskType}");
            }
        }
    }

    /// <summary>
    /// Creates a new ArrowBehaviour for a task that doesn't have one. Clones from an existing task's arrow if available.
    /// </summary>
    /// <param name="task">The task to create an arrow for.</param>
    /// <returns>The newly created ArrowBehaviour.</returns>
    public static ArrowBehaviour CreateArrowForTask(NormalPlayerTask task)
    {
        // Make sure we have arrow assets cached from ShipStatus prefabs
        CacheArrowFromShipStatus();

        Debug.Log($"Creating task arrow by cloning cached template for task {task.TaskType}");

        // Parent to task's transform so it gets destroyed with the task
        var arrowObj = Object.Instantiate(_cachedArrowTemplate, task.transform, false);
        arrowObj.name = "MalumArrow";

        return arrowObj.GetComponent<ArrowBehaviour>();
    }

    /// <summary>
    /// Ensures a task has an arrow, creating one if necessary.
    /// </summary>
    /// <param name="task">The task to ensure has an arrow.</param>
    public static void EnsureArrowExists(NormalPlayerTask task)
    {
        // Only create arrows for owned, incomplete tasks that don't already have one
        if (!IsOwnedAndIncomplete(task) || task.Arrow != null) return;

        task.Arrow = CreateArrowForTask(task);
    }

    /// <summary>
    /// Checks if a task needs special handling for arrow target setting.
    /// Some tasks like ReplaceParts have logic that assumes taskStep > 0.
    /// </summary>
    /// <param name="task">The task to check.</param>
    /// <returns>True if the task needs special target handling, false otherwise.</returns>
    public static bool NeedsSpecialTarget(NormalPlayerTask task)
    {
        return task.TaskType is TaskTypes.AlignEngineOutput or TaskTypes.ReplaceParts or TaskTypes.RoastMarshmallow or TaskTypes.StartFans or TaskTypes.PickUpTowels;
    }

    /// <summary>
    /// Sets the arrow target and StartAt room for a given task and console.
    /// </summary>
    /// <param name="task">The task to set the arrow for.</param>
    /// <param name="targetConsole">The console to point the arrow to.</param>
    private static void SetArrowTarget(NormalPlayerTask task, Console targetConsole)
    {
        if (targetConsole == null) return;
        task.Arrow.target = targetConsole.transform.position;
        task.StartAt = targetConsole.Room;
    }

    /// <summary>
    /// Manually sets arrow target for tasks that have special logic at taskStep == 0.
    /// </summary>
    /// <param name="task">The task to set the arrow for.</param>
    public static void SetArrowTargetForSpecialTasks(NormalPlayerTask task)
    {
        if (task.Arrow == null) return;

        switch (task.TaskType)
        {
            case TaskTypes.AlignEngineOutput when task.TaskStep == 0:
            {
                // AlignEngineOutput: At step 0, point to the correct console (without this it always points to the wrong one for some reason)
                Il2CppSystem.Collections.Generic.List<Console> consoles = task.FindConsoles();
                if (consoles is { Count: > 0 })
                {
                    SetArrowTarget(task, consoles[0]);
                }

                break;
            }
            case TaskTypes.ReplaceParts when task.taskStep == 0:
            {
                // ReplaceParts: At step 0, point to the part collection console
                Il2CppSystem.Collections.Generic.List<Console> consoles = NormalPlayerTask.PickRandomConsoles(0, TaskTypes.ReplaceParts);
                if (consoles is { Count: > 0 })
                {
                    var firstConsole = consoles.ToArray().FirstOrDefault(c => c.ConsoleId == task.Data[0]);
                    SetArrowTarget(task, firstConsole);
                }

                break;
            }
            case TaskTypes.RoastMarshmallow when task.taskStep == 0:
            {
                // RoastMarshmallow: At step 0, point to the stick collection console
                Il2CppSystem.Collections.Generic.List<Console> consoles = NormalPlayerTask.PickRandomConsoles(0, TaskTypes.RoastMarshmallow);

                if (consoles is { Count: > 0 })
                {
                    var stickConsole = consoles.ToArray().FirstOrDefault(c => c.ConsoleId == task.Data[0]);
                    SetArrowTarget(task, stickConsole);
                }

                break;
            }
            case TaskTypes.StartFans when task.taskStep == 0:
            {
                // StartFans: At step 0, point to the first panel (ConsoleId == 0) where you view the code
                var targetConsole = task.FindSpecialConsole((Il2CppSystem.Func<Console, bool>)((Console c) => task.ValidConsole(c) && c.ConsoleId == 0));
                SetArrowTarget(task, targetConsole);

                break;
            }
            case TaskTypes.PickUpTowels when task.TaskStep == 0:
            {
                // PickUpTowels: At step 0, find any valid towel location
                var targetConsole = task.FindSpecialConsole((Il2CppSystem.Func<Console, bool>)((Console c) => task.ValidConsole(c)));
                SetArrowTarget(task, targetConsole);

                break;
            }
        }
    }
}
