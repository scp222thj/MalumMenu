using System;
using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

public class TaskAutomationController : MonoBehaviour
{
    private static readonly Func<Minigame, PlayerTask> GetTaskFromMinigame = CreateTaskGetter();

    private Minigame _minigame;
    private PlayerTask _task;
    private bool _active;
    private bool _auto;
    private bool _closingFromAuto;
    private float _start;
    private float _end;
    private float _duration;
    private int _mapId;
    private int _taskType;
    private int _taskId;
    private Minigame _lastSeenMinigame;
    private float _nextScanTime;

    private static Func<Minigame, PlayerTask> CreateTaskGetter()
    {
        var prop = AccessTools.PropertyGetter(typeof(Minigame), "MyTask");
        if (prop != null && typeof(PlayerTask).IsAssignableFrom(prop.ReturnType))
        {
            return (Func<Minigame, PlayerTask>)Delegate.CreateDelegate(typeof(Func<Minigame, PlayerTask>), null, prop);
        }

        var field = AccessTools.Field(typeof(Minigame), "MyTask") ?? AccessTools.Field(typeof(Minigame), "myTask");
        if (field != null && typeof(PlayerTask).IsAssignableFrom(field.FieldType))
        {
            return mg => (PlayerTask)field.GetValue(mg);
        }

        return _ => null;
    }

    public void OnMinigameBegin(Minigame minigame)
    {
        if (minigame == null) return;
        if (!Utils.isInGame) return;
        if (!Utils.isPlayer) return;

        var task = GetTaskFromMinigame(minigame);
        if (task == null)
        {
            if (CheatToggles.debugTaskAutomation)
            {
                MalumMenu.Log.LogInfo($"TaskAutomation: minigame={minigame.GetType().Name} has no MyTask");
            }
            return;
        }

        if (task.Owner == null || !task.Owner.AmOwner) return;
        if (task.IsComplete) return;

        var now = Time.realtimeSinceStartup;

        _minigame = minigame;
        _task = task;
        _active = true;
        _closingFromAuto = false;
        _start = now;
        _mapId = Utils.GetCurrentMapID();
        _taskType = task is NormalPlayerTask npt ? (int)npt.TaskType : -1;
        _taskId = (int)task.Id;

        if (!CheatToggles.autoTaskOnOpen)
        {
            _auto = false;
            _duration = 0f;
            _end = 0f;
            if (CheatToggles.debugTaskAutomation)
            {
                MalumMenu.Log.LogInfo($"TaskAutomation: tracking manual task map={_mapId} taskId={_taskId} taskType={_taskType} mg={minigame.GetType().Name}");
            }
            return;
        }

        var duration = MalumMenu.autoTaskDefaultSeconds.Value;

        if (CheatToggles.autoTaskUseBestTime && TaskTimeStore.TryGetBest(_mapId, _taskId, _taskType, out var best))
        {
            duration = best;
        }

        if (duration < 0.1f) duration = 0.1f;

        _auto = true;
        _duration = duration;
        _end = now + duration;

        if (CheatToggles.debugTaskAutomation)
        {
            MalumMenu.Log.LogInfo($"TaskAutomation: scheduled map={_mapId} taskId={_taskId} taskType={_taskType} duration={duration:0.00}s mg={minigame.GetType().Name}");
        }
    }

    public void OnMinigameClosePrefix(Minigame minigame)
    {
        if (!_active) return;
        if (minigame == null) return;
        if (!ReferenceEquals(minigame, _minigame)) return;

        if (_auto && !_closingFromAuto)
        {
            var now = Time.realtimeSinceStartup;
            if (now < _end)
            {
                Clear();
                return;
            }
        }
    }

    public void OnMinigameClosePostfix(Minigame minigame)
    {
        if (!_active) return;
        if (minigame == null) return;
        if (!ReferenceEquals(minigame, _minigame)) return;

        var now = Time.realtimeSinceStartup;

        if (!_auto && CheatToggles.recordTaskTimes && _task != null && _task.IsComplete)
        {
            var elapsed = now - _start;
            if (elapsed > 0.01f)
            {
                TaskTimeStore.Record(_mapId, _taskId, _taskType, elapsed);
            }
        }

        Clear();
    }

    private void Update()
    {
        if (!_active)
        {
            if (!Utils.isInGame) return;
            if (!Utils.isPlayer) return;
            if (!CheatToggles.autoTaskOnOpen && !CheatToggles.recordTaskTimes) return;

            var scanNow = Time.realtimeSinceStartup;
            if (scanNow < _nextScanTime) return;
            _nextScanTime = scanNow + 0.25f;

            Minigame mg = null;
            try
            {
                mg = UnityEngine.Object.FindObjectOfType<Minigame>();
            }
            catch
            {
                mg = null;
            }

            if (mg != null && mg.isActiveAndEnabled && !ReferenceEquals(mg, _lastSeenMinigame))
            {
                _lastSeenMinigame = mg;
                OnMinigameBegin(mg);
            }

            return;
        }

        if (_active && (_minigame == null || !_minigame))
        {
            if (CheatToggles.debugTaskAutomation)
            {
                MalumMenu.Log.LogInfo("TaskAutomation: minigame destroyed, clearing state");
            }
            Clear();
            return;
        }

        if (!_active) return;
        if (!_auto) return;
        if (_task == null || _minigame == null) { Clear(); return; }
        if (_task.IsComplete) { Clear(); return; }

        var now = Time.realtimeSinceStartup;
        if (now < _end) return;

        _closingFromAuto = true;
        try
        {
            if (CheatToggles.debugTaskAutomation)
            {
                MalumMenu.Log.LogInfo($"TaskAutomation: completing task map={_mapId} taskId={_taskId} taskType={_taskType} mg={_minigame.GetType().Name}");
            }

            TryLocalComplete(_task);
            Utils.CompleteTask(_task);
        }
        finally
        {
            try
            {
                _minigame.Close();
            }
            catch
            {
            }
            _closingFromAuto = false;
        }
    }

    private static void TryLocalComplete(PlayerTask task)
    {
        if (task == null) return;
        try
        {
            var m = AccessTools.Method(task.GetType(), "Complete");
            if (m != null && m.GetParameters().Length == 0)
            {
                m.Invoke(task, null);
            }
        }
        catch
        {
        }
    }

    private void Clear()
    {
        _minigame = null;
        _task = null;
        _active = false;
        _auto = false;
        _closingFromAuto = false;
        _start = 0f;
        _end = 0f;
        _duration = 0f;
        _mapId = 0;
        _taskType = -1;
        _taskId = 0;
        _nextScanTime = 0f;
    }
}
