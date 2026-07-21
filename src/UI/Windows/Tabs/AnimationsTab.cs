using System;
using UnityEngine;

namespace MalumMenu;

public class AnimationsTab : ITab
{
    public string name => "Animations";

    private Vector2 _scrollPosition = Vector2.zero;

    public void Draw()
    {
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.Height(MenuUI.windowHeight - 70));

        GUILayout.BeginVertical(GUILayout.Width(MenuUI.windowWidth * 0.425f));

        DrawPlayerAnimations();
        GUILayout.Space(10);
        DrawCosmeticAnimations();
        GUILayout.Space(10);
        DrawSpecialEffects();
        GUILayout.Space(10);
        DrawTriggerAnimations();
        GUILayout.Space(10);
        DrawMisc();

        GUILayout.EndVertical();

        GUILayout.EndScrollView();
    }

    private void DrawPlayerAnimations()
    {
        GUILayout.Label("Player Animations", GUIStylePreset.TabSubtitle);

        DrawLoopToggle("Idle", "anim_idle", () => AnimationService.PlayIdle(), ref CheatToggles.loopIdle);
        DrawLoopToggle("Run", "anim_run", () => AnimationService.PlayRun(), ref CheatToggles.loopRun);
        DrawLoopToggle("Climb Up", "anim_climb_up", () => AnimationService.PlayClimbUp(), ref CheatToggles.loopClimbUp);
        DrawLoopToggle("Climb Down", "anim_climb_down", () => AnimationService.PlayClimbDown(), ref CheatToggles.loopClimbDown);
        DrawLoopToggle("Enter Vent", "anim_enter_vent", () => AnimationService.PlayEnterVent(), ref CheatToggles.loopEnterVent);
        DrawLoopToggle("Exit Vent", "anim_exit_vent", () => AnimationService.PlayExitVent(), ref CheatToggles.loopExitVent);
        DrawLoopToggle("Jump", "anim_jump", () => AnimationService.PlayJump(), ref CheatToggles.loopJump);
        DrawLoopToggle("Spawn", "anim_spawn", () => AnimationService.PlaySpawn(), ref CheatToggles.loopSpawn);
        DrawLoopToggle("Ghost Idle", "anim_ghost_idle", () => AnimationService.PlayGhostIdle(), ref CheatToggles.loopGhostIdle);
        DrawLoopToggle("Guardian Angel Idle", "anim_ga_idle", () => AnimationService.PlayGuardianAngelIdle(), ref CheatToggles.loopGuardianAngelIdle);
    }

    private void DrawCosmeticAnimations()
    {
        GUILayout.Label("Pet Animations", GUIStylePreset.TabSubtitle);

        DrawLoopToggle("Pet Idle", "anim_pet_idle", () => AnimationService.PlayPetIdle(), ref CheatToggles.loopPetIdle);
        DrawLoopToggle("Pet Walk", "anim_pet_walk", () => AnimationService.PlayPetWalk(), ref CheatToggles.loopPetWalk);
        DrawLoopToggle("Pet Scared", "anim_pet_scared", () => AnimationService.PlayPetScared(), ref CheatToggles.loopPetScared);
        DrawLoopToggle("Pet Mourn", "anim_pet_mourn", () => AnimationService.PlayPetMourn(), ref CheatToggles.loopPetMourn);
        DrawLoopToggle("Pet Sequence", "anim_pet_sequence", () => AnimationService.PlayPetSequence(), ref CheatToggles.loopPetSequence);

        GUILayout.Space(5);
        GUILayout.Label("Skin Animations", GUIStylePreset.TabSubtitle);

        DrawLoopToggle("Skin Idle", "anim_skin_idle", () => AnimationService.PlaySkinIdle(), ref CheatToggles.loopSkinIdle);
        DrawLoopToggle("Skin Jump", "anim_skin_jump", () => AnimationService.PlaySkinJump(), ref CheatToggles.loopSkinJump);
        DrawLoopToggle("Skin Climb Up", "anim_skin_climb", () => AnimationService.PlaySkinClimbUp(), ref CheatToggles.loopSkinClimbUp);
        DrawLoopToggle("Skin Climb Down", "anim_skin_climb_down", () => AnimationService.PlaySkinClimbDown(), ref CheatToggles.loopSkinClimbDown);
        DrawLoopToggle("Skin Spawn", "anim_skin_spawn", () => AnimationService.PlaySkinSpawn(), ref CheatToggles.loopSkinSpawn);
        DrawLoopToggle("Skin Ghost", "anim_skin_ghost", () => AnimationService.PlaySkinGhost(), ref CheatToggles.loopSkinGhost);

        GUILayout.Space(5);
        GUILayout.Label("Hat Animations", GUIStylePreset.TabSubtitle);

        DrawLoopToggle("Hat Climb", "anim_hat_climb", () => AnimationService.PlayHatClimb(), ref CheatToggles.loopHatClimb);
        DrawLoopToggle("Hat Floor", "anim_hat_floor", () => AnimationService.PlayHatFloor(), ref CheatToggles.loopHatFloor);
    }

    private void DrawSpecialEffects()
    {
        GUILayout.Label("Role Effects", GUIStylePreset.TabSubtitle);

        DrawLoopToggle("Shapeshift", "anim_shapeshift", () => AnimationService.PlayShapeshift(), ref CheatToggles.loopShapeshift);
        DrawLoopToggle("Vanish", "anim_vanish", () => AnimationService.PlayVanishCharge(), ref CheatToggles.loopVanish);
        DrawLoopToggle("Appear", "anim_appear", () => AnimationService.PlayAppearPoof(), ref CheatToggles.loopAppear);
        DrawLoopToggle("Protect Flash", "anim_protect_flash", () => AnimationService.PlayProtectFlash(), ref CheatToggles.loopProtectFlash);
        DrawLoopToggle("Protect Loop", "anim_protect_loop", () => AnimationService.PlayProtectLoop(), ref CheatToggles.loopProtectLoop);
    }

    private void DrawTriggerAnimations()
    {
        GUILayout.Label("One-Shot Effects", GUIStylePreset.TabSubtitle);

        DrawLoopToggle("Alert Flash", "anim_alert_flash", () => AnimationService.PlayAlertFlash(), ref CheatToggles.loopAlertFlash);
        DrawLoopToggle("Kill Blur", "anim_kill_blur", () => AnimationService.PlayKillBlur(), ref CheatToggles.loopKillBlur);
        DrawLoopToggle("Particle Burst", "anim_particles_burst", () => AnimationService.PlayParticleBurst(), ref CheatToggles.loopParticleBurst);
    }

    private void DrawMisc()
    {
        GUILayout.Label("Mini-Game Triggers", GUIStylePreset.TabSubtitle);

        CheatToggles.animShields = GUILayout.Toggle(CheatToggles.animShields, " Shields");
        CheatToggles.animAsteroids = GUILayout.Toggle(CheatToggles.animAsteroids, " Asteroids");
        CheatToggles.animEmptyGarbage = GUILayout.Toggle(CheatToggles.animEmptyGarbage, " Empty Garbage");
        CheatToggles.animMedScan = GUILayout.Toggle(CheatToggles.animMedScan, " Medbay Scan");
        CheatToggles.animCamsInUse = GUILayout.Toggle(CheatToggles.animCamsInUse, " Cams In Use");
        CheatToggles.moonWalk = GUILayout.Toggle(CheatToggles.moonWalk, " Moonwalk");
    }

    private void DrawLoopToggle(string label, string animId, Action play, ref bool toggle)
    {
        bool newState = GUILayout.Toggle(toggle, $" {label}");
        if (newState != toggle)
        {
            toggle = newState;
            if (newState) AnimationToggleService.Toggle(animId, play);
            else AnimationToggleService.Toggle(animId, null);
        }
    }
}
