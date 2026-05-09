using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch]
public static class Minigame_Begin_TaskAutomation
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        var asm = typeof(Minigame).Assembly;
        foreach (var t in asm.GetTypes())
        {
            if (t == null) continue;
            if (!typeof(Minigame).IsAssignableFrom(t)) continue;

            var methods = AccessTools.GetDeclaredMethods(t);
            foreach (var m in methods)
            {
                if (m == null) continue;
                if (m.Name != "Begin") continue;
                yield return m;
            }
        }
    }

    public static void Postfix(Minigame __instance)
    {
        if (MalumMenu.taskAutomationController == null) return;
        MalumMenu.taskAutomationController.OnMinigameBegin(__instance);
    }
}

[HarmonyPatch]
public static class Minigame_Close_TaskAutomation
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        var asm = typeof(Minigame).Assembly;
        foreach (var t in asm.GetTypes())
        {
            if (t == null) continue;
            if (!typeof(Minigame).IsAssignableFrom(t)) continue;

            var methods = AccessTools.GetDeclaredMethods(t);
            foreach (var m in methods)
            {
                if (m == null) continue;
                if (m.Name != "Close") continue;
                yield return m;
            }
        }
    }

    public static void Prefix(Minigame __instance)
    {
        if (MalumMenu.taskAutomationController == null) return;
        MalumMenu.taskAutomationController.OnMinigameClosePrefix(__instance);
    }

    public static void Postfix(Minigame __instance)
    {
        if (MalumMenu.taskAutomationController == null) return;
        MalumMenu.taskAutomationController.OnMinigameClosePostfix(__instance);
    }
}
