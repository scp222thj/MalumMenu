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
        if (task == null) return;

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
        if (!_active) return;
        if (!_auto) return;
        if (_task == null || _minigame == null) { Clear(); return; }
        if (_task.IsComplete) { Clear(); return; }

        var now = Time.realtimeSinceStartup;
        if (now < _end) return;

        _closingFromAuto = true;
        try
        {
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
    }
}
