using UnityEngine;

namespace MalumMenu;

internal static class FrameRateCapService
{
    public const int MinFps = 15;
    public const int MaxFps = 1000;

    private static bool _lastEnabled;
    private static int _lastFps = -1;
    private static int _lastTarget = int.MinValue;
    private static int _lastVSync = int.MinValue;

    public static int ClampFps(int fps) => Mathf.Clamp(fps, MinFps, MaxFps);

    public static void ApplyIfNeeded(bool force = false)
    {
        try
        {
            bool enabled = CheatConfig.FrameRateCapEnabled != null && CheatConfig.FrameRateCapEnabled.Value;
            int fps = ClampFps(CheatConfig.MaxFrameRate != null ? CheatConfig.MaxFrameRate.Value : 60);

            if (!enabled)
            {
                if (_lastEnabled)
                    Application.targetFrameRate = -1;
                _lastEnabled = false;
                _lastFps = fps;
                _lastTarget = Application.targetFrameRate;
                _lastVSync = QualitySettings.vSyncCount;
            }
            else if (force || !_lastEnabled || _lastFps != fps
                     || Application.targetFrameRate != fps
                     || QualitySettings.vSyncCount != 0
                     || _lastTarget != Application.targetFrameRate
                     || _lastVSync != QualitySettings.vSyncCount)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = fps;
                _lastEnabled = true;
                _lastFps = fps;
                _lastTarget = Application.targetFrameRate;
                _lastVSync = QualitySettings.vSyncCount;
            }
        }
        catch { }
    }
}
