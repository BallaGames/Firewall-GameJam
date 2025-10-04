using UnityEngine;

public class Flamethrower : RangedWeapon
{
    public float flameRange;
    public int flameDamage;
    public EffectData flameEffectData;

    public float minFlameRange, maxFlameRange;
    public float flamePower;
    public float flameDecay;
    public float flameRampSpeed;
    public float flameSpread;

    public int flameRays;
    RaycastHit2D[] flameHits;

    private void Start()
    {
        flameHits = new RaycastHit2D[flameRays];
    }

    protected override void TryFire()
    {
        flamePower = Mathf.MoveTowards(flamePower, fireInput ? 1 : 0, fireInput ? flameRampSpeed : flameDecay);
        if(fireInput)
        {
            FireWeapon();
        }

    }

    protected override void FireLogic()
    {
        for (int i = 0; i < flameRays; i++)
        {
            float angle = Random.Range(-flameSpread, flameSpread);
            float distance = Mathf.Lerp(minFlameRange, maxFlameRange, flamePower);
            flameHits[i] = Physics2D.Raycast(muzzle.position, Quaternion.Euler(0, 0, angle) * transform.right, distance, WeaponManager.Instance.projectileMask);
            if (flameHits[i].collider != null)
            {
                Debug.DrawLine(muzzle.position, flameHits[i].point);
            }
            else
            {
                Debug.DrawRay(muzzle.position, Quaternion.Euler(0, 0, angle) * transform.right * distance);
            }
            RaycastHit2D hit = flameHits[i];
            if (hit.rigidbody && hit.rigidbody.TryGetComponent(out Entity hitEnt))
            {
                hitEnt.TakeDamage(flameDamage);
            }
        }
    }
    protected override void FireEffects()
    {
        base.FireEffects();
    }
}
