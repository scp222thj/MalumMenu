using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BepInEx;
using UnityEngine;

namespace MalumMenu;

public static class CrewBanListService
{
    public static string FilePath => Path.Combine(GetGameRootSafe(), "MalumMenu", "banlist.txt");

    private static string GetGameRootSafe()
    {
        try
        {
            string p = Paths.GameRootPath;
            if (!string.IsNullOrWhiteSpace(p)) return p;
        }
        catch { }

        try
        {
            string p2 = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrWhiteSpace(p2)) return p2;
        }
        catch { }

        try
        {
            string p3 = Environment.CurrentDirectory;
            if (!string.IsNullOrWhiteSpace(p3)) return p3;
        }
        catch { }

        return ".";
    }

    public static void EnsureLoaded()
    {
        if (_loaded) return;

        lock (_lock)
        {
            if (_loaded) return;

            string path = FilePath;
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                _bans.Clear();
                _names.Clear();
                _reasons.Clear();

                if (File.Exists(path))
                {
                    foreach (string text in File.ReadAllLines(path))
                    {
                        string line = text?.Trim();
                        if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                        {
                            string[] parts = line.Split(',', StringSplitOptions.None);
                            string fc = parts[0].Trim();
                            if (!string.IsNullOrEmpty(fc))
                            {
                                _bans.Add(fc);
                                if (parts.Length >= 2)
                                    _names[fc] = parts[1].Trim();
                                if (parts.Length >= 3)
                                    _reasons[fc] = parts[2].Trim();
                            }
                        }
                    }
                }
                else
                {
                    File.WriteAllText(path,
                        "# MalumMenu BanList — persistent friend-code ban list" + Environment.NewLine +
                        "# Format: friendCode, playerName, reason" + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[CrewBanList] Load error: " + e);
            }

            _loaded = true;
        }
    }

    public static bool IsBanned(string friendCode)
    {
        if (string.IsNullOrWhiteSpace(friendCode))
            return false;

        EnsureLoaded();
        lock (_lock)
        {
            return _bans.Contains(friendCode.Trim());
        }
    }

    public static bool Add(string friendCode, string playerName = null, string reason = null)
    {
        if (string.IsNullOrWhiteSpace(friendCode))
            return false;

        EnsureLoaded();
        lock (_lock)
        {
            string fc = friendCode.Trim();
            if (!_bans.Add(fc))
                return false;

            if (!string.IsNullOrEmpty(playerName))
                _names[fc] = playerName.Trim();
            if (!string.IsNullOrEmpty(reason))
                _reasons[fc] = reason.Trim();

            PersistUnlocked();
            return true;
        }
    }

    public static bool Remove(string friendCode)
    {
        if (string.IsNullOrWhiteSpace(friendCode))
            return false;

        EnsureLoaded();
        lock (_lock)
        {
            string fc = friendCode.Trim();
            if (!_bans.Remove(fc))
                return false;

            _names.Remove(fc);
            _reasons.Remove(fc);
            PersistUnlocked();
            return true;
        }
    }

    public static int RemoveByName(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            return 0;

        EnsureLoaded();
        lock (_lock)
        {
            string pn = playerName.Trim();
            List<string> toRemove = new();

            foreach (var kv in _names)
            {
                if (string.Equals(kv.Value, pn, StringComparison.OrdinalIgnoreCase))
                    toRemove.Add(kv.Key);
            }

            foreach (string fc in toRemove)
            {
                _bans.Remove(fc);
                _names.Remove(fc);
                _reasons.Remove(fc);
            }

            if (toRemove.Count > 0)
                PersistUnlocked();

            return toRemove.Count;
        }
    }

    public static List<string> List()
    {
        EnsureLoaded();
        lock (_lock)
        {
            List<string> result = new(_bans);
            result.Sort((a, b) => string.Compare(a, b, StringComparison.OrdinalIgnoreCase));
            return result;
        }
    }

    public static List<KeyValuePair<string, string>> ListDetailed()
    {
        EnsureLoaded();
        lock (_lock)
        {
            List<KeyValuePair<string, string>> result = new(_bans.Count);
            foreach (string fc in _bans)
            {
                _names.TryGetValue(fc, out string name);
                result.Add(new KeyValuePair<string, string>(fc, name ?? ""));
            }
            result.Sort((a, b) => string.Compare(a.Key, b.Key, StringComparison.OrdinalIgnoreCase));
            return result;
        }
    }

    public static int Count
    {
        get
        {
            EnsureLoaded();
            lock (_lock)
            {
                return _bans.Count;
            }
        }
    }

    private static void PersistUnlocked()
    {
        string path = FilePath;
        try
        {
            StringBuilder sb = new();
            sb.AppendLine("# MalumMenu BanList — persistent friend-code ban list");
            sb.AppendLine("# Format: friendCode, playerName, reason");

            foreach (string fc in _bans)
            {
                _names.TryGetValue(fc, out string name);
                _reasons.TryGetValue(fc, out string reason);
                sb.Append(fc);
                sb.Append(',');
                sb.Append(name ?? "");
                sb.Append(',');
                sb.Append(reason ?? "");
                sb.AppendLine();
            }

            AtomicWrite(path, sb.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError("[CrewBanList] Persist error: " + e);
        }
    }

    private static void AtomicWrite(string path, string content)
    {
        string tmp = path + ".tmp." + Guid.NewGuid().ToString("N")[..8];
        try
        {
            File.WriteAllText(tmp, content, new UTF8Encoding(false));
            if (File.Exists(path))
            {
                try
                {
                    File.Replace(tmp, path, null);
                    return;
                }
                catch { }

                try { File.Delete(path); } catch { }
            }
            File.Move(tmp, path);
        }
        finally
        {
            try
            {
                if (File.Exists(tmp))
                    File.Delete(tmp);
            }
            catch { }
        }
    }

    private static readonly object _lock = new();
    private static readonly HashSet<string> _bans = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string> _names = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string> _reasons = new(StringComparer.OrdinalIgnoreCase);
    private static bool _loaded;
}
