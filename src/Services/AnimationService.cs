using System;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace MalumMenu;

internal static class AnimationService
{
    private static PlayerControl L => PlayerControl.LocalPlayer;

    private static PlayerAnimations A
    {
        get
        {
            PlayerControl i = L;
            if (i == null || i.MyPhysics == null)
                return null;
            return i.MyPhysics.Animations;
        }
    }

    private static void StartCo(Il2CppSystem.Collections.IEnumerator co)
    {
        try
        {
            PlayerControl i = L;
            if (i != null && co != null)
                i.MyPhysics.StartCoroutine(co);
        }
        catch { }
    }

    private static void PlayRole(Func<RoleManager, RoleEffectAnimation> pick, float duration = 0f)
    {
        try
        {
            PlayerControl i = L;
            if (i == null) return;

            RoleManager rm = DestroyableSingleton<RoleManager>.Instance;
            if (rm == null) return;

            RoleEffectAnimation clip = pick(rm);
            if (clip == null) return;

            RoleEffectAnimation inst = UnityEngine.Object.Instantiate(clip, i.transform);
            bool flipX = i.cosmetics != null && i.cosmetics.FlipX;

            if (duration > 0f)
                inst.Play(i, null, flipX, RoleEffectAnimation.SoundType.Local, duration, true, -0.05f);
            else
                inst.Play(i, null, flipX, RoleEffectAnimation.SoundType.Local, 0f, true, 0f);
        }
        catch { }
    }

    internal static void PlayIdle()
    {
        try
        {
            PlayerAnimations a = A;
            if (a != null) a.PlayIdleAnimation();
        }
        catch { }
    }

    internal static void PlayRun()
    {
        try
        {
            PlayerAnimations a = A;
            if (a != null) a.PlayRunAnimation();
        }
        catch { }
    }

    internal static void PlayClimbUp()
    {
        try
        {
            PlayerAnimations a = A;
            if (a != null) a.PlayClimbAnimation(false);
        }
        catch { }
    }

    internal static void PlayClimbDown()
    {
        try
        {
            PlayerAnimations a = A;
            if (a != null) a.PlayClimbAnimation(true);
        }
        catch { }
    }

    internal static void PlayEnterVent()
    {
        try
        {
            PlayerAnimations a = A;
            if (a != null) StartCo(a.CoPlayEnterVentAnimation(0));
        }
        catch { }
    }

    internal static void PlayExitVent()
    {
        try
        {
            PlayerAnimations a = A;
            if (a != null) StartCo(a.CoPlayExitVentAnimation());
        }
        catch { }
    }

    internal static void PlayJump()
    {
        try
        {
            PlayerAnimations a = A;
            if (a != null) StartCo(a.CoPlayJumpAnimation());
        }
        catch { }
    }

    internal static void PlaySpawn()
    {
        try
        {
            PlayerControl i = L;
            if (i == null) return;

            PlayerAnimations a = A;
            if (a != null) StartCo(a.CoPlaySpawnAnimation(i.cosmetics != null && i.cosmetics.FlipX));
        }
        catch { }
    }

    internal static void PlayScannerOn()
    {
        try
        {
            PlayerControl i = L;
            if (i == null) return;

            PlayerAnimations a = A;
            if (a != null) a.PlayScanner(true, false, i.cosmetics != null && i.cosmetics.FlipX);
        }
        catch { }
    }

    internal static void PlayScannerOff()
    {
        try
        {
            PlayerAnimations a = A;
            if (a != null) a.PlayScanner(false, false, false);
        }
        catch { }
    }

    internal static void PlayGhostIdle()
    {
        try
        {
            PlayerAnimations a = A;
            if (a != null) a.PlayGhostIdleAnimation();
        }
        catch { }
    }

    internal static void PlayGuardianAngelIdle()
    {
        try
        {
            PlayerAnimations a = A;
            if (a != null) a.PlayGuardianAngelIdleAnimation();
        }
        catch { }
    }

    internal static void PlayShapeshift()
    {
        PlayRole(rm => rm.shapeshiftAnim, 0f);
    }

    internal static void PlayVanishCharge()
    {
        PlayRole(rm => rm.vanish_ChargeAnim, 0.5f);
    }

    internal static void PlayVanishPoof()
    {
        PlayRole(rm => rm.vanish_PoofAnim, 0f);
    }

    internal static void PlayAppearPoof()
    {
        PlayRole(rm => rm.appear_PoofAnim, 0f);
    }

    internal static void PlayProtectFlash()
    {
        PlayRole(rm => rm.protectAnim, 0f);
    }

    internal static void PlayProtectLoop()
    {
        PlayRole(rm => rm.protectLoopAnim, 5f);
    }

    internal static void PlayPetSequence()
    {
        try
        {
            PlayerControl i = L;
            if (i == null || i.MyPhysics == null) return;

            Vector2 pos = i.transform.position;
            Vector2 petPos = pos + new Vector2(0.5f, 0f);
            i.MyPhysics.PetPet(pos, petPos);
        }
        catch { }
    }

    private static PetBehaviour Pet
    {
        get
        {
            try
            {
                PlayerControl l = L;
                if (l == null) return null;
                CosmeticsLayer cosmetics = l.cosmetics;
                return cosmetics?.currentPet;
            }
            catch { return null; }
        }
    }

    internal static void PlayPetIdle()
    {
        try
        {
            PetBehaviour p = Pet;
            if (p != null) p.SetIdle();
        }
        catch { }
    }

    internal static void PlayPetWalk()
    {
        try
        {
            PetBehaviour p = Pet;
            if (p != null) p.StartWalkAnim();
        }
        catch { }
    }

    internal static void PlayPetScared()
    {
        try
        {
            PetBehaviour p = Pet;
            if (p != null) p.SetScared();
        }
        catch { }
    }

    internal static void PlayPetMourn()
    {
        try
        {
            PetBehaviour p = Pet;
            if (p != null) p.SetMourning();
        }
        catch { }
    }

    private static SkinLayer Skin
    {
        get
        {
            try
            {
                PlayerControl l = L;
                if (l == null) return null;
                CosmeticsLayer cosmetics = l.cosmetics;
                return cosmetics?.skin;
            }
            catch { return null; }
        }
    }

    internal static void PlaySkinIdle()
    {
        try
        {
            PlayerControl i = L;
            SkinLayer s = Skin;
            if (s != null && i != null && i.cosmetics != null)
                s.SetIdle(i.cosmetics.FlipX);
        }
        catch { }
    }

    internal static void PlaySkinJump()
    {
        try
        {
            PlayerControl i = L;
            SkinLayer s = Skin;
            if (s != null && i != null && i.cosmetics != null)
                s.SetJump(i.cosmetics.FlipX);
        }
        catch { }
    }

    internal static void PlaySkinClimbUp()
    {
        try
        {
            SkinLayer skin = Skin;
            if (skin != null) skin.SetClimb(false);
        }
        catch { }
    }

    internal static void PlaySkinClimbDown()
    {
        try
        {
            SkinLayer skin = Skin;
            if (skin != null) skin.SetClimb(true);
        }
        catch { }
    }

    internal static void PlaySkinSpawn()
    {
        try
        {
            PlayerControl i = L;
            SkinLayer s = Skin;
            if (s != null && i != null && i.cosmetics != null)
                s.SetSpawn(i.cosmetics.FlipX, 0f);
        }
        catch { }
    }

    internal static void PlaySkinGhost()
    {
        try
        {
            SkinLayer skin = Skin;
            if (skin != null) skin.SetGhost();
        }
        catch { }
    }

    internal static void PlayHatClimb()
    {
        try
        {
            PlayerControl l = L;
            if (l == null) return;
            CosmeticsLayer cosmetics = l.cosmetics;
            if (cosmetics == null) return;
            HatParent hat = cosmetics.hat;
            if (hat != null) hat.SetClimbAnim();
        }
        catch { }
    }

    internal static void PlayHatFloor()
    {
        try
        {
            PlayerControl l = L;
            if (l == null) return;
            CosmeticsLayer cosmetics = l.cosmetics;
            if (cosmetics == null) return;
            HatParent hat = cosmetics.hat;
            if (hat != null) hat.SetFloorAnim();
        }
        catch { }
    }

    internal static void PlayAlertFlash()
    {
        try
        {
            HudManager hm = DestroyableSingleton<HudManager>.Instance;
            if (hm == null || hm.AlertFlash == null) return;
            Animator anim = hm.AlertFlash.animator;
            if (anim != null) anim.SetTrigger("OnFlash");
        }
        catch { }
    }

    private static AnimationClip _cachedBlurAnim;

    private static AnimationClip FindBlurAnim()
    {
        if (_cachedBlurAnim != null)
            return _cachedBlurAnim;

        try
        {
            PlayerControl i = L;
            if (i != null && i.KillAnimations != null)
            {
                for (int j = 0; j < i.KillAnimations.Count; j++)
                {
                    KillAnimation ka = i.KillAnimations[j];
                    if (ka != null && ka.BlurAnim != null)
                    {
                        _cachedBlurAnim = ka.BlurAnim;
                        return _cachedBlurAnim;
                    }
                }
            }

            var all = Resources.FindObjectsOfTypeAll(Il2CppType.Of<KillAnimation>());
            if (all != null)
            {
                for (int k = 0; k < all.Length; k++)
                {
                    UnityEngine.Object obj = all[k];
                    KillAnimation ka2 = obj != null ? obj.TryCast<KillAnimation>() : null;
                    if (ka2 != null && ka2.BlurAnim != null)
                    {
                        _cachedBlurAnim = ka2.BlurAnim;
                        return _cachedBlurAnim;
                    }
                }
            }
        }
        catch { }

        return null;
    }

    internal static void PlayKillBlur()
    {
        try
        {
            PlayerControl i = L;
            if (i == null || i.MyPhysics == null) return;
            PlayerAnimations anims = i.MyPhysics.Animations;
            if (anims == null) return;
            AnimationClip clip = FindBlurAnim();
            if (clip != null) StartCo(anims.CoPlayCustomAnimation(clip));
        }
        catch { }
    }

    private static MushroomMixupScreenTint _cachedTint;

    private static MushroomMixupScreenTint FindMushroomTint()
    {
        if (_cachedTint != null)
            return _cachedTint;

        try
        {
            HudManager hm = DestroyableSingleton<HudManager>.Instance;
            if (hm != null)
            {
                MushroomMixupScreenTint t = hm.GetComponentInChildren<MushroomMixupScreenTint>(true);
                if (t != null)
                {
                    _cachedTint = t;
                    return t;
                }
            }

            ShipStatus ss = ShipStatus.Instance;
            if (ss != null)
            {
                MushroomMixupScreenTint t2 = ss.GetComponentInChildren<MushroomMixupScreenTint>(true);
                if (t2 != null)
                {
                    _cachedTint = t2;
                    return t2;
                }
            }

            var all = Resources.FindObjectsOfTypeAll(Il2CppType.Of<MushroomMixupScreenTint>());
            if (all != null && all.Length > 0)
            {
                for (int i = 0; i < all.Length; i++)
                {
                    UnityEngine.Object obj = all[i];
                    MushroomMixupScreenTint t3 = obj != null ? obj.TryCast<MushroomMixupScreenTint>() : null;
                    if (t3 != null)
                    {
                        _cachedTint = t3;
                        return _cachedTint;
                    }
                }
            }
        }
        catch { }

        return null;
    }

    internal static void PlayMushroomTintIn()
    {
        try
        {
            MushroomMixupScreenTint t = FindMushroomTint();
            if (t != null) t.Activate();
        }
        catch { }
    }

    internal static void PlayMushroomTintOut()
    {
        try
        {
            MushroomMixupScreenTint t = FindMushroomTint();
            if (t != null) t.Deactivate();
        }
        catch { }
    }

    private static PlayerParticles _cachedParticles;

    internal static void PlayParticleBurst()
    {
        try
        {
            PlayerControl i = L;
            if (i == null) return;

            PlayerParticles parts = _cachedParticles;
            if (parts == null)
            {
                parts = UnityEngine.Object.FindObjectOfType<PlayerParticles>();
                if (parts == null)
                {
                    var all = Resources.FindObjectsOfTypeAll(Il2CppType.Of<PlayerParticles>());
                    if (all != null)
                    {
                        for (int j = 0; j < all.Length; j++)
                        {
                            UnityEngine.Object obj = all[j];
                            PlayerParticles p = obj != null ? obj.TryCast<PlayerParticles>() : null;
                            if (p != null)
                            {
                                parts = p;
                                break;
                            }
                        }
                    }
                }

                if (parts != null)
                    _cachedParticles = parts;
            }

            if (parts == null || parts.pool == null) return;

            for (int k = 0; k < 8; k++)
            {
                PoolableBehavior p2 = parts.pool.Get<PoolableBehavior>();
                if (p2 == null) break;
                p2.transform.position = i.transform.position + new Vector3(
                    UnityEngine.Random.Range(-0.5f, 0.5f),
                    UnityEngine.Random.Range(-0.5f, 0.5f), 0f);
                p2.gameObject.SetActive(true);
            }
        }
        catch { }
    }

    private static AudioClip _cachedDeadSound;

    private static AudioClip FindDeadSound()
    {
        if (_cachedDeadSound != null)
            return _cachedDeadSound;

        try
        {
            var all = Resources.FindObjectsOfTypeAll(Il2CppType.Of<MeetingIntroAnimation>());
            if (all != null)
            {
                for (int i = 0; i < all.Length; i++)
                {
                    UnityEngine.Object obj = all[i];
                    MeetingIntroAnimation j = obj != null ? obj.TryCast<MeetingIntroAnimation>() : null;
                    if (j != null && j.PlayerDeadSound != null)
                    {
                        _cachedDeadSound = j.PlayerDeadSound;
                        return _cachedDeadSound;
                    }
                }
            }
        }
        catch { }

        return null;
    }

    private static AudioClip _cachedTextSound;

    private static AudioClip FindTextSound()
    {
        if (_cachedTextSound != null)
            return _cachedTextSound;

        try
        {
            var all = Resources.FindObjectsOfTypeAll(Il2CppType.Of<ExileController>());
            if (all != null)
            {
                for (int i = 0; i < all.Length; i++)
                {
                    UnityEngine.Object obj = all[i];
                    ExileController e = obj != null ? obj.TryCast<ExileController>() : null;
                    if (e != null && e.TextSound != null)
                    {
                        _cachedTextSound = e.TextSound;
                        return _cachedTextSound;
                    }
                }
            }
        }
        catch { }

        return null;
    }

    internal static void PlayMeetingSlam()
    {
        try
        {
            AudioClip clip = FindDeadSound();
            if (clip == null) return;
            SoundManager instance = SoundManager.Instance;
            if (instance != null) instance.PlaySound(clip, false, 0.7f, null);
        }
        catch { }
    }

    internal static void PlayEjectTextSfx()
    {
        try
        {
            AudioClip clip = FindTextSound();
            if (clip == null) return;
            SoundManager instance = SoundManager.Instance;
            if (instance != null) instance.PlaySoundImmediate(clip, false, 0.8f, 1f, null);
        }
        catch { }
    }
}
