using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace MalumMenu;

internal static class AnimationToggleService
{
    private class LoopEntry
    {
        public Action Play;
        public float Interval;
        public float NextFire;
    }

    private static readonly Dictionary<string, LoopEntry> _active = new();
    private static readonly Dictionary<string, float> _intervals = BuildIntervals();
    private static readonly List<string> _toFireBuffer = new(8);

    private static Dictionary<string, float> BuildIntervals() => new()
    {
        { "anim_idle", 2f },
        { "anim_run", 2f },
        { "anim_climb_up", 2f },
        { "anim_climb_down", 2f },
        { "anim_enter_vent", 1.5f },
        { "anim_exit_vent", 1.5f },
        { "anim_jump", 1f },
        { "anim_spawn", 1.5f },
        { "anim_scanner_on", 0.8f },
        { "anim_ghost_idle", 2f },
        { "anim_ga_idle", 2f },
        { "anim_shapeshift", 1.5f },
        { "anim_vanish", 1.5f },
        { "anim_vanish_poof", 1.5f },
        { "anim_appear", 1.5f },
        { "anim_protect_flash", 1.5f },
        { "anim_protect_loop", 5.2f },
        { "anim_pet_sequence", 3f },
        { "anim_pet_idle", 2.5f },
        { "anim_pet_walk", 2.5f },
        { "anim_pet_scared", 2.5f },
        { "anim_pet_mourn", 2.5f },
        { "anim_skin_idle", 2f },
        { "anim_skin_jump", 1f },
        { "anim_skin_climb", 2f },
        { "anim_skin_climb_down", 2f },
        { "anim_skin_spawn", 2f },
        { "anim_skin_ghost", 2f },
        { "anim_hat_climb", 2.5f },
        { "anim_hat_floor", 2.5f },
        { "anim_alert_flash", 1.2f },
        { "anim_kill_blur", 1f },
        { "anim_particles_burst", 1f },
    };

    internal static bool IsActive(string id) => !string.IsNullOrEmpty(id) && _active.ContainsKey(id);
    internal static int ActiveCount => _active.Count;

    internal static void Toggle(string id, Action play)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (play == null) { _active.Remove(id); return; }
        if (_active.ContainsKey(id)) { _active.Remove(id); return; }

        float interval = _intervals.TryGetValue(id, out float iv) ? iv : 1.5f;
        _active[id] = new LoopEntry { Play = play, Interval = interval, NextFire = Time.time };
    }

    internal static void ClearAll() => _active.Clear();

    internal static void Tick()
    {
        if (_active.Count == 0) return;
        float now = Time.time;
        _toFireBuffer.Clear();
        foreach (var kv in _active)
        {
            if (now >= kv.Value.NextFire) _toFireBuffer.Add(kv.Key);
        }
        foreach (string id in _toFireBuffer)
        {
            if (_active.TryGetValue(id, out var entry))
            {
                try { entry.Play?.Invoke(); } catch { }
                entry.NextFire = now + entry.Interval;
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class TickPatch
    {
        public static void Postfix()
        {
            try { Tick(); } catch { }
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    public static class ForceAnimPatch
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            try
            {
                if (_active.Count == 0) return;
                if (__instance == null || __instance.myPlayer == null || !__instance.AmOwner) return;

                PlayerAnimations anims = __instance.Animations;
                if (anims == null) return;

                if (_active.ContainsKey("anim_idle"))
                {
                    if (anims.IsPlayingRunAnimation() || anims.IsPlayingClimbAnimation())
                        anims.PlayIdleAnimation();
                }
                else if (_active.ContainsKey("anim_run"))
                {
                    if (!anims.IsPlayingRunAnimation() && !anims.IsPlayingClimbAnimation()
                        && !anims.IsPlayingEnterVentAnimation() && !anims.IsPlayingSpawnAnimation())
                        anims.PlayRunAnimation();
                }
                else if (_active.ContainsKey("anim_climb_up"))
                {
                    if (!anims.IsPlayingClimbAnimation()) anims.PlayClimbAnimation(false);
                }
                else if (_active.ContainsKey("anim_climb_down"))
                {
                    if (!anims.IsPlayingClimbAnimation()) anims.PlayClimbAnimation(true);
                }
                else if (_active.ContainsKey("anim_ghost_idle"))
                {
                    if (!anims.IsPlayingGhostIdleAnimation()) anims.PlayGhostIdleAnimation();
                }
                else if (_active.ContainsKey("anim_ga_idle"))
                {
                    if (!anims.IsPlayingGuardianAngelIdleAnimation()) anims.PlayGuardianAngelIdleAnimation();
                }
            }
            catch { }
        }
    }
}
