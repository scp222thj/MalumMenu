using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalumMenu;

public static class RecentlyLeftService
{
    private static readonly object _lock = new();
    private static readonly List<Entry> _entries = new(32);
    private const int MAX_ENTRIES = 20;
    private static int _stateVersion;

    public static void Track(string friendCode, string playerName)
    {
        if (string.IsNullOrWhiteSpace(friendCode))
            return;

        var fc = friendCode.Trim();
        lock (_lock)
        {
            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                if (string.Equals(_entries[i].FriendCode, fc, StringComparison.OrdinalIgnoreCase))
                    _entries.RemoveAt(i);
            }

            _entries.Add(new Entry
            {
                FriendCode = fc,
                PlayerName = (playerName ?? "").Trim(),
                LeftAt = Time.realtimeSinceStartup
            });

            while (_entries.Count > MAX_ENTRIES)
                _entries.RemoveAt(0);

            _stateVersion++;
        }
    }

    public static bool RemoveAt(int index)
    {
        lock (_lock)
        {
            if (index < 0 || index >= _entries.Count)
                return false;

            _entries.RemoveAt(index);
            _stateVersion++;
            return true;
        }
    }

    public static bool RemoveByFriendCode(string friendCode)
    {
        if (string.IsNullOrWhiteSpace(friendCode))
            return false;

        var fc = friendCode.Trim();
        lock (_lock)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (string.Equals(_entries[i].FriendCode, fc, StringComparison.OrdinalIgnoreCase))
                {
                    _entries.RemoveAt(i);
                    _stateVersion++;
                    return true;
                }
            }
            return false;
        }
    }

    public static void Clear()
    {
        lock (_lock)
        {
            if (_entries.Count != 0)
            {
                _entries.Clear();
                _stateVersion++;
            }
        }
    }

    public static List<Entry> ListDetailed()
    {
        lock (_lock)
        {
            var copy = new List<Entry>(_entries.Count);
            for (int i = _entries.Count - 1; i >= 0; i--)
                copy.Add(_entries[i]);
            return copy;
        }
    }

    public static int Count
    {
        get
        {
            lock (_lock)
                return _entries.Count;
        }
    }

    public static int GetStateHash()
    {
        lock (_lock)
            return _stateVersion;
    }

    public static Entry GetAt(int index)
    {
        lock (_lock)
        {
            int mapped = _entries.Count - 1 - index;
            if (mapped < 0 || mapped >= _entries.Count)
                return null;

            var e = _entries[mapped];
            return new Entry
            {
                FriendCode = e.FriendCode,
                PlayerName = e.PlayerName,
                LeftAt = e.LeftAt
            };
        }
    }

    public sealed class Entry
    {
        public string FriendCode;
        public string PlayerName;
        public float LeftAt;
    }
}
