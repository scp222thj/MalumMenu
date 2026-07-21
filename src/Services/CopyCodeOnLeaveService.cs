using UnityEngine;

namespace MalumMenu;

internal static class CopyCodeOnLeaveService
{
    private static string _lastJoinedCode = "";

    internal static void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return;
        _lastJoinedCode = code;
    }

    internal static void OnDisconnect()
    {
        try
        {
            if (CheatConfig.CopyCodeOnLeave != null && CheatConfig.CopyCodeOnLeave.Value)
            {
                string code = _lastJoinedCode;
                if (!string.IsNullOrWhiteSpace(code) && code.Length >= 6 && code != "AAAAAA")
                {
                    GUIUtility.systemCopyBuffer = code;
                    NotifyUtils.Info($"Lobby code copied: {code}");
                }
            }
        }
        catch { }
        finally { _lastJoinedCode = ""; }
    }
}
