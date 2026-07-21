using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

public static class RoleCheats
{
    public static readonly float MAX_SAFE_VALUE = 3600f;
    private static HudManager _cachedHud;

    private static HudManager CachedHud => _cachedHud ??= DestroyableSingleton<HudManager>.Instance;

    public static void EnableVentingForAll(HudManager hudManager)
    {
        if (hudManager == null) return;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (player?.Data?.Role != null && !player.Data.IsDead && !player.Data.Role.CanVent)
            {
                player.Data.Role.CanVent = true;
                if (player == PlayerControl.LocalPlayer && hudManager.ImpostorVentButton != null)
                    hudManager.ImpostorVentButton.gameObject.SetActive(true);
            }
        }
    }

    public static void UpdateAbilityButton()
    {
        var hud = CachedHud;
        var abilityButton = hud?.AbilityButton;
        if (abilityButton == null) return;
        try
        {
            abilityButton.SetCoolDown(0f, 1f);
            abilityButton.canInteract = true;
            abilityButton.enabled = true;
            if (abilityButton.graphic != null) abilityButton.graphic.color = Color.white;
        }
        catch { }
    }

    [HarmonyPatch(typeof(EngineerRole), nameof(EngineerRole.SetCooldown))]
    public static class EngineerSetCooldownPatch
    {
        public static bool Prefix() => !CheatToggles.noVentCooldown;
    }

    [HarmonyPatch(typeof(EngineerRole), nameof(EngineerRole.FixedUpdate))]
    public static class EngineerUpdatePatch
    {
        public static void Postfix(EngineerRole __instance)
        {
            try
            {
                if (!__instance.Player.AmOwner) return;
                if (CheatToggles.endlessVentTime) __instance.inVentTimeRemaining = 30f;
                if (CheatToggles.noVentCooldown)
                {
                    if (__instance.cooldownSecondsRemaining > 0f) __instance.cooldownSecondsRemaining = 0.01f;
                    UpdateAbilityButton();
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(ShapeshifterRole), nameof(ShapeshifterRole.FixedUpdate))]
    public static class ShapeshifterUpdatePatch
    {
        public static void Postfix(ShapeshifterRole __instance)
        {
            try
            {
                if (!__instance.Player.AmOwner) return;
                if (CheatToggles.endlessSsDuration) __instance.durationSecondsRemaining = MAX_SAFE_VALUE;
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(TrackerRole), nameof(TrackerRole.SetCooldown))]
    public static class TrackerSetCooldownPatch
    {
        public static void Postfix(TrackerRole __instance)
        {
            if (CheatToggles.noTrackingCooldown) __instance.cooldownSecondsRemaining = 0f;
        }
    }

    [HarmonyPatch(typeof(TrackerRole), nameof(TrackerRole.FixedUpdate))]
    public static class TrackerUpdatePatch
    {
        public static void Postfix(TrackerRole __instance)
        {
            try
            {
                if (!__instance.Player.AmOwner) return;
                if (CheatToggles.endlessTracking) __instance.durationSecondsRemaining = MAX_SAFE_VALUE;
                if (CheatToggles.noTrackingCooldown)
                {
                    if (__instance.cooldownSecondsRemaining > 0f) __instance.cooldownSecondsRemaining = 0.01f;
                    if (__instance.delaySecondsRemaining > 0f) __instance.delaySecondsRemaining = 0.01f;
                    UpdateAbilityButton();
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(ScientistRole), nameof(ScientistRole.Update))]
    public static class ScientistUpdatePatch
    {
        public static void Postfix(ScientistRole __instance)
        {
            try
            {
                if (!__instance.Player.AmOwner) return;
                if (CheatToggles.endlessBattery)
                {
                    __instance.currentCharge = MAX_SAFE_VALUE;
                    var hud = CachedHud;
                    var abilityButton = hud?.AbilityButton;
                    if (abilityButton != null) abilityButton.canInteract = true;
                }
                if (CheatToggles.noVitalsCooldown && __instance.currentCooldown > 0f)
                {
                    __instance.currentCooldown = 0.01f;
                    UpdateAbilityButton();
                }
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    private class VentCanUsePatch
    {
        private static bool Prefix(Vent __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
        {
            if (pc?.Object != PlayerControl.LocalPlayer) return true;
            if (!CheatToggles.allowVenting) return true;

            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                canUse = false;
                couldUse = false;
                __result = float.MaxValue;
                return false;
            }
            float ventDistance = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), __instance.transform.position);
            canUse = ventDistance < __instance.UsableDistance;
            couldUse = true;
            __result = ventDistance;
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), "get_CanMove")]
    public static class AlwaysMovePatch
    {
        private static void Postfix(PlayerControl __instance, ref bool __result)
        {
            if (__instance == PlayerControl.LocalPlayer && __instance.inVent && CheatToggles.allowVenting
                && ShipStatus.Instance != null && __instance.moveable && !__instance.shapeshifting
                && !MeetingHud.Instance && !ExileController.Instance && !Minigame.Instance)
            {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.CanUse))]
    internal static class ImpTaskCanUsePatch
    {
        public static void Postfix(IUsable usable, ref bool __result)
        {
            if (__result || !CheatToggles.impostorCanDoTasks) return;
            PlayerControl lp = PlayerControl.LocalPlayer;
            if (lp?.Data?.Role == null || !lp.Data.Role.IsImpostor) return;
            if (usable == null) return;
            Console console = null;
            try { console = usable.TryCast<Console>(); } catch { }
            if (console == null) return;
            __result = true;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcCompleteTask))]
    internal static class ImpTaskNoBroadcastPatch
    {
        public static bool Prefix(PlayerControl __instance)
        {
            return !CheatToggles.impostorCanDoTasks || __instance == null || __instance != PlayerControl.LocalPlayer
                || __instance.Data?.Role == null || !__instance.Data.Role.IsImpostor;
        }
    }
}
