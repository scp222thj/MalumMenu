using System;
using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace MalumMenu;

public static class ImmortalityService
{
    private const int VENT_ID = 50;
    private const float REASSERT_INTERVAL = 2f;
    private static float _lastReassert;
    private static int _blockedKills;

    private static bool IsInActiveGame()
    {
        try
        {
            return ShipStatus.Instance != null && PlayerControl.LocalPlayer != null && LobbyBehaviour.Instance == null;
        }
        catch { return false; }
    }

    private static void SendVent(VentilationSystem.Operation op)
    {
        try
        {
            if (IsInActiveGame())
                VentilationSystem.Update(op, VENT_ID);
        }
        catch { }
    }

    internal static void ToggleImmortality()
    {
        if (!IsInActiveGame()) { NotifyUtils.Warning("Immortality requires an active match"); return; }

        bool target = !CheatToggles.immortality;
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (lp != null && !lp.inVent)
            SendVent(target ? VentilationSystem.Operation.Enter : VentilationSystem.Operation.Exit);

        CheatToggles.immortality = target;
        if (target) { _blockedKills = 0; _lastReassert = Time.time; NotifyUtils.Success("Immortality ON"); }
        else NotifyUtils.Info("Immortality OFF");
    }

    internal static void UpdateImmortality()
    {
        if (!CheatToggles.immortality) return;
        float now = Time.time;
        if (now - _lastReassert < REASSERT_INTERVAL) return;
        _lastReassert = now;
        if (!IsInActiveGame()) return;
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (lp == null || lp.Data == null || lp.Data.IsDead || lp.inVent) return;
        SendVent(VentilationSystem.Operation.Enter);
    }

    [HarmonyPatch(typeof(VentilationSystem), nameof(VentilationSystem.Update))]
    internal static class BlockSendingUpdates
    {
        public static bool Prefix([HarmonyArgument(0)] VentilationSystem.Operation op, [HarmonyArgument(1)] int ventId)
        {
            if (ventId == VENT_ID) return true;
            if (!CheatToggles.immortality) return true;
            if (op != VentilationSystem.Operation.Enter && op != VentilationSystem.Operation.Exit && op != VentilationSystem.Operation.Move) return true;
            return false;
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    internal static class OnShipStatusCreate
    {
        public static void Postfix()
        {
            if (CheatToggles.immortality && IsInActiveGame())
                SendVent(VentilationSystem.Operation.Enter);
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
    internal static class OnShipStatusDestroyed
    {
        public static void Postfix()
        {
            if (CheatToggles.immortality)
            {
                CheatToggles.immortality = false;
                NotifyUtils.Info("Immortality OFF (game ended)");
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    internal static class OnMeetingEnd
    {
        public static void Postfix()
        {
            if (!CheatToggles.immortality || !IsInActiveGame()) return;
            PlayerControl lp = PlayerControl.LocalPlayer;
            if (lp == null || lp.Data == null || lp.Data.IsDead) return;
            SendVent(VentilationSystem.Operation.Enter);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    internal static class LocalKillBlocker
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target, [HarmonyArgument(1)] MurderResultFlags resultFlags)
        {
            if (!CheatToggles.immortality) return true;
            if (target == null || target != PlayerControl.LocalPlayer) return true;
            if (!resultFlags.HasFlag(MurderResultFlags.Succeeded)) return true;

            string who = "Someone";
            try { who = __instance?.Data?.PlayerName ?? "Someone"; } catch { }
            _blockedKills++;
            NotifyUtils.Warning($"Immortality blocked {who} (x{_blockedKills})");
            return false;
        }
    }

    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.SetEndpoint))]
    internal static class ForceDtls
    {
        public static void Prefix(ref bool dtls) { dtls = true; }
    }
}
