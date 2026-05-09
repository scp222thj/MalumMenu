using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MalumMenu;

public static class TaskTimeStore
{
    private static readonly Dictionary<ulong, float> BestSeconds = new();
    private static bool _loaded;

    private static string FilePath => Path.Combine(BepInEx.Paths.ConfigPath, "MalumTaskTimes.txt");

    private static ulong MakeKey(int mapId, int taskId, int taskType)
    {
        var type = taskType;
        if (type < 0) type = 0xFFFF;
        return ((ulong)(uint)mapId << 32) | ((ulong)(uint)taskId << 16) | (ushort)type;
    }

    public static void Load()
    {
        if (_loaded) return;
        _loaded = true;

        BestSeconds.Clear();
        if (!File.Exists(FilePath)) return;

        using var reader = new StreamReader(FilePath);
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            line = line.Trim();
            if (line.StartsWith("#")) continue;

            var parts = line.Split('|');
            if (parts.Length != 3 && parts.Length != 4) continue;

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var mapId)) continue;
            var taskId = 0;
            var taskType = -1;
            var secondsIndex = 2;

            if (parts.Length == 4)
            {
                if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out taskId)) continue;
                if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out taskType)) continue;
                secondsIndex = 3;
            }
            else
            {
                if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out taskType)) continue;
            }

            if (!float.TryParse(parts[secondsIndex], NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds)) continue;
            if (seconds <= 0f) continue;

            var key = MakeKey(mapId, taskId, taskType);
            BestSeconds[key] = seconds;
        }
    }

    public static bool TryGetBest(int mapId, int taskId, int taskType, out float seconds)
    {
        Load();
        return BestSeconds.TryGetValue(MakeKey(mapId, taskId, taskType), out seconds);
    }

    public static void Record(int mapId, int taskId, int taskType, float seconds)
    {
        Load();
        if (seconds <= 0f) return;

        var key = MakeKey(mapId, taskId, taskType);
        if (BestSeconds.TryGetValue(key, out var existing))
        {
            if (seconds >= existing) return;
        }

        BestSeconds[key] = seconds;
        Save();
    }

    public static void Clear()
    {
        Load();
        BestSeconds.Clear();
        Save();
    }

    private static void Save()
    {
        using var writer = new StreamWriter(FilePath);
        writer.WriteLine("# MapId|TaskId|TaskType|BestSeconds");
        foreach (var kvp in BestSeconds)
        {
            var mapId = (int)(kvp.Key >> 32);
            var taskId = (int)((kvp.Key >> 16) & 0xFFFF);
            var taskType = (int)(kvp.Key & 0xFFFF);
            if (taskType == 0xFFFF) taskType = -1;
            writer.WriteLine($"{mapId.ToString(CultureInfo.InvariantCulture)}|{taskId.ToString(CultureInfo.InvariantCulture)}|{taskType.ToString(CultureInfo.InvariantCulture)}|{kvp.Value.ToString("0.000", CultureInfo.InvariantCulture)}");
        }
    }
}
