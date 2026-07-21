using UnityEngine;

namespace MalumMenu;

public static class HostOutboundBudget
{
    public const int CAP = 28;

    private static float _windowStart;
    private static int _windowBytes;

    public static void Record(int bytes)
    {
        if (bytes <= 0)
            return;

        float now = Time.time;
        if (now - _windowStart > 1f)
        {
            _windowStart = now;
            _windowBytes = bytes;
        }
        else
        {
            _windowBytes += bytes;
        }
    }

    public static bool IsSaturated() => GetUtilization() >= 0.8f;

    public static float GetUtilization()
    {
        Trim();
        return (float)_windowBytes / CAP;
    }

    private static void Trim()
    {
        float now = Time.time;
        if (now - _windowStart > 1f)
        {
            _windowStart = now;
            _windowBytes = 0;
        }
    }
}
