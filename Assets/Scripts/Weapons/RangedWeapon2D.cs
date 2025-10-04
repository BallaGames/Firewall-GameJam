using UnityEngine;
using Balla.Core;
using Balla;
using UnityEngine.VFX;


public class RangedWeapon2D : BallaScript
{
    public ProjectileData projectileData;

    public float roundsPerMinute;
    [ReadOnly] public float fireInterval;
    protected float waitTick;
    float fireWait;
    public bool autoFire;

    public Transform muzzle;

    public ParticleSystem muzzleVFX;
    public bool alwaysPlayVFXWhenFiring;
    bool playing;
    [SerializeField, ReadOnly] internal bool fireInput, lastFireInput;

    public Animator animator;
    public string fireAnimKey;

    private void OnValidate()
    {
        fireInterval = (1 / (roundsPerMinute / 60));
        waitTick = fireInterval * Delta;
    }

    protected override void Timestep()
    {
        waitTick = fireInterval * Delta;

        TryFire();

        if(muzzleVFX != null && alwaysPlayVFXWhenFiring && playing && !fireInput)
        {
            playing = false;
            muzzleVFX.Stop();
        }
        lastFireInput = fireInput;
    }

    protected virtual void TryFire()
    {
        if ((autoFire || !lastFireInput) && fireInput && fireWait >= fireInterval)
        {
            if (muzzleVFX != null && alwaysPlayVFXWhenFiring && !playing)
            {
                playing = true;
                muzzleVFX.Play();
            }
            FireWeapon();
        }
        else
        {
            fireWait += Delta;
        }
    }
    protected virtual void FireWeapon()
    {
        fireWait = 0;
        FireEffects();
        FireLogic();
    }
    protected virtual void FireEffects()
    {
        if (muzzleVFX != null && !alwaysPlayVFXWhenFiring)
        {
            muzzleVFX.Play();
        }
        if (animator)
        {
            animator.SetTrigger(fireAnimKey);
        }
    }
    protected virtual void FireLogic()
    {
        WeaponManager.Instance.ProjectileFired(this);

    }
}
