using UnityEngine;
using System.Linq;

namespace MalumMenu;

public static class ArrowHandler
{
    // Cache for an arrow template GameObject to clone from
    private static GameObject _cachedArrowTemplate;

    // Determines if a task is owned by LocalPlayer and incomplete
    public static bool IsOwnedAndIncomplete(NormalPlayerTask task)
    {
        if (task.Owner == null || !task.Owner.AmOwner) return false;

        return !task.IsComplete;
    }

    // Searches through task prefabs in ShipStatus to cache first arrow GameObject found
    private static void CacheArrowFromShipStatus()
    {
        if (_cachedArrowTemplate != null) return;

        NormalPlayerTask[][] allTasksArrays = new NormalPlayerTask[][]
        {
            ShipStatus.Instance.CommonTasks,
            ShipStatus.Instance.LongTasks,
            ShipStatus.Instance.ShortTasks
        };

        foreach (var tasks in allTasksArrays) 
        {
            foreach (var task in tasks)
            {
                if (task.Arrow != null)
                {
                    _cachedArrowTemplate = task.Arrow.gameObject;
                    MalumMenu.Log.LogInfo($"Cached Arrow.gameObject for task {task.TaskType}");
                    return;
                }
                MalumMenu.Log.LogInfo($"No Arrow.gameObject found for task {task.TaskType}");
            }
        }
    }

    // Creates a new ArrowBehaviour for a task that doesn't have one
    public static ArrowBehaviour CreateArrowForTask(NormalPlayerTask task)
    {
        // Cache an arrow GameObject from ShipStatus task prefabs if its missing
        CacheArrowFromShipStatus();

        // Set task.transform as parent of arrowObj so it gets destroyed with the task
        var arrowObj = Object.Instantiate(_cachedArrowTemplate, task.transform, false);
        
        return arrowObj.GetComponent<ArrowBehaviour>();
    }

    // Ensures a task has an arrow, creating one if necessary
    public static void EnsureArrowExists(NormalPlayerTask task)
    {
        // Only create arrows for owned, incomplete tasks that don't already have one
        if (!IsOwnedAndIncomplete(task) || task.Arrow != null) return;

        task.Arrow = CreateArrowForTask(task);
    }

    // Checks if a task needs special handling for arrow target setting
    // Some tasks like ReplaceParts have logic that assumes taskStep > 0
    public static bool NeedsSpecialTarget(NormalPlayerTask task)
    {
        return task.TaskType is TaskTypes.AlignEngineOutput or TaskTypes.ReplaceParts or TaskTypes.RoastMarshmallow or TaskTypes.StartFans or TaskTypes.PickUpTowels;
    }

    // Sets the arrow target and StartAt room for a given task and console
    private static void SetArrowTarget(NormalPlayerTask task, Console targetConsole)
    {
        if (targetConsole == null) return;
        
        task.Arrow.target = targetConsole.transform.position;
        task.StartAt = targetConsole.Room;
    }

    // Sets the arrow target for tasks that have special logic at TaskStep == 0
    // Targets each special task with case-specific logic
    public static void SetArrowTargetForSpecialTasks(NormalPlayerTask task)
    {
        if (task.Arrow == null) return;

        switch (task.TaskType)
        {
            // AlignEngineOutput: At step 0, always point arrow to the currently relevant console (Upper Engine panel)
            case TaskTypes.AlignEngineOutput when task.TaskStep == 0:
            {
                Il2CppSystem.Collections.Generic.List<Console> consoles = task.FindConsoles();
                
                if (consoles is { Count: > 0 })
                {
                    SetArrowTarget(task, consoles[0]);
                }

                break;
            }
            // ReplaceParts: At step 0, always point arrow to the currently relevant console (Collect Parts panel)
            case TaskTypes.ReplaceParts when task.taskStep == 0:
            {
                Il2CppSystem.Collections.Generic.List<Console> consoles = NormalPlayerTask.PickRandomConsoles(0, TaskTypes.ReplaceParts);
                
                if (consoles is { Count: > 0 })
                {
                    var firstConsole = consoles.ToArray().FirstOrDefault(c => c.ConsoleId == task.Data[0]);
                    SetArrowTarget(task, firstConsole);
                }

                break;
            }
            // RoastMarshmallow: At step 0, always point arrow to the currently relevant console (Collect Stick panel)
            case TaskTypes.RoastMarshmallow when task.taskStep == 0:
            {
                Il2CppSystem.Collections.Generic.List<Console> consoles = NormalPlayerTask.PickRandomConsoles(0, TaskTypes.RoastMarshmallow);

                if (consoles is { Count: > 0 })
                {
                    var stickConsole = consoles.ToArray().FirstOrDefault(c => c.ConsoleId == task.Data[0]);
                    SetArrowTarget(task, stickConsole);
                }

                break;
            }
            // StartFans: At step 0, always point arrow to the currently relevant console (Reveal Code panel)
            case TaskTypes.StartFans when task.taskStep == 0:
            {
                var targetConsole = task.FindSpecialConsole((Il2CppSystem.Func<Console, bool>)((Console c) => task.ValidConsole(c) && c.ConsoleId == 0));
                SetArrowTarget(task, targetConsole);

                break;
            }
            // PickUpTowels: At step 0, always point arrow to any valid towel location
            case TaskTypes.PickUpTowels when task.TaskStep == 0:
            {
                var targetConsole = task.FindSpecialConsole((Il2CppSystem.Func<Console, bool>)((Console c) => task.ValidConsole(c)));
                SetArrowTarget(task, targetConsole);

                break;
            }
        }
    }
}
