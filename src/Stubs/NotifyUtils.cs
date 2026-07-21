using UnityEngine;

namespace MalumMenu;

public static class NotifyUtils
{
    public static void Info(string message) => Debug.Log($"[MalumMenu] {message}");
    public static void Warning(string message) => Debug.LogWarning($"[MalumMenu] {message}");
    public static void Success(string message) => Debug.Log($"[MalumMenu] [SUCCESS] {message}");
    public static void Error(string message) => Debug.LogError($"[MalumMenu] {message}");
}
