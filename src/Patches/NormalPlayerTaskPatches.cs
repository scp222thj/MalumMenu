using HarmonyLib;

namespace MalumMenu;

[HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.Initialize))]
public static class NormalPlayerTask_Initialize
{
    /// <summary>
    /// Postfix patch of NormalPlayerTask.Initialize to create arrows for tasks that don't have them.
    /// </summary>
    /// <param name="__instance">The <c>NormalPlayerTask</c> instance.</param>
    public static void Postfix(NormalPlayerTask __instance)
    {
        // Set up arrow target for Airship UploadData task separately
        if (__instance.TaskType == TaskTypes.UploadData && __instance.taskStep == 0 && (MapNames)Utils.GetCurrentMapID() == MapNames.Airship)
        {
            var airshipTask = __instance.GetComponent<AirshipUploadTask>();
            var consolePositions = airshipTask.FindValidConsolesPositions();
            // AirshipUploadTask uses an Arrows[] array instead of the inherited Arrow field
            for (var i = 0; i < consolePositions.Count && i < airshipTask.Arrows.Length; i++)
            {
                // There are two already existing arrows, we just need to set the target of one of them at step 0
                airshipTask.Arrows[i].target = consolePositions[i];
            }
            airshipTask.LocationDirty = true;
            return;
        }

        ArrowHandler.EnsureArrowExists(__instance);

        if (!ArrowHandler.NeedsSpecialTarget(__instance))
        {
            __instance.UpdateArrowAndLocation();
        }
        else if (ArrowHandler.IsOwnedAndIncomplete(__instance))
        {
            ArrowHandler.SetArrowTargetForSpecialTasks(__instance);
        }
    }
}

[HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.FixedUpdate))]
public static class NormalPlayerTask_FixedUpdate
{
    /// <summary>
    /// Postfix patch of NormalPlayerTask.FixedUpdate to control arrow visibility.
    /// </summary>
    /// <param name="__instance">The <c>NormalPlayerTask</c> instance.</param>
    public static void Postfix(NormalPlayerTask __instance)
    {
        if (__instance.Arrow == null) return;

        if (!CheatToggles.showTaskArrows)
        {
            // Hide arrows if taskStep == 0 (vanilla behavior)
            if (__instance.taskStep == 0)
            {
                __instance.Arrow.gameObject.SetActive(false);
            }
            return;
        }

        if (ArrowHandler.IsOwnedAndIncomplete(__instance))
        {
            if (ArrowHandler.NeedsSpecialTarget(__instance))
            {
                ArrowHandler.SetArrowTargetForSpecialTasks(__instance);
            }

            __instance.Arrow.gameObject.SetActive(true);
        }
    }
}

[HarmonyPatch(typeof(AirshipUploadTask), nameof(AirshipUploadTask.FixedUpdate))]
public static class AirshipUploadTask_FixedUpdate_Patch
{
    /// <summary>
    /// Postfix patch of AirshipUploadTask.FixedUpdate to keep arrows visible when taskStep == 0 or Comms sabotage is active.
    /// </summary>
    /// <param name="__instance">The <c>AirshipUploadTask</c> instance.</param>
    public static void Postfix(AirshipUploadTask __instance)
    {
        // This patch is unfortunately necessary because AirshipUploadTask overrides NormalPlayerTask.FixedUpdate
        if (__instance.Arrows == null) return;

        if (!CheatToggles.showTaskArrows)
        {
            // Deactivate all arrows if taskStep == 0 (vanilla behavior)
            if (__instance.taskStep != 0) return;
            foreach (var arrow in __instance.Arrows)
            {
                arrow.gameObject.SetActive(false);
            }
            return;
        }

        var consolePositions = __instance.FindValidConsolesPositions();
        for (var i = 0; i < __instance.Arrows.Length; i++)
        {
            // Only activate arrows that correspond to valid console positions
            __instance.Arrows[i].gameObject.SetActive(i < consolePositions.Count && __instance.Owner != null && __instance.Owner.AmOwner);
        }
    }
}
