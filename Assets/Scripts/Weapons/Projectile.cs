using UnityEngine;
using Balla.Core;
using Balla;
using System.Collections;

public class Projectile : BallaScript
{

    public ProjectileData data;

    public SpriteRenderer r;
    public Vector2 direction;
    public float speed;
    public Vector2 velocity;

    float lifeNorm;

    [ReadOnly] public bool expired;

    [SerializeField, ReadOnly] protected float lifetime;

    public delegate void OnProjectileHit();
    public OnProjectileHit onProjectileHit;

    protected float radius;

    public void Initialise(RangedWeapon weapon)
    {
        data = weapon.projectileData;
        direction = Quaternion.Euler(0, 0, Random.Range(-data.deviation, data.deviation)) * weapon.muzzle.right;
        lifetime = 0;
        speed = data.projectileSpeed;
        transform.position = weapon.muzzle.position;
        transform.right = weapon.muzzle.right;
        velocity = direction * speed;
        r.transform.localScale = data.defaultScale;
        expired = false;

        r.enabled = data.projectileSprite != null;
    }


    protected override void Timestep()
    {
        base.Timestep();
        if(lifetime < data.maxLife)
        {
            RaycastHit2D hit;
            if (data.useCircleCast)
            {
                hit = Physics2D.Raycast(transform.position, direction, speed * Delta,
                    WeaponManager.Instance.projectileMask);
            }
            else
            {
                radius = r.transform.localScale.y * data.projectileRadius;
                hit = Physics2D.CircleCast(transform.position, radius, direction, speed * Delta,
                    WeaponManager.Instance.projectileMask);
            }
            if (hit.collider != null)
            {
                Debug.DrawLine(transform.position, hit.point, Color.green, Time.fixedDeltaTime);
                transform.position = hit.point;
                transform.right = -hit.normal;

                ProjectileHit(hit);
            }
            else
            {
                Debug.DrawRay(transform.position, velocity * Delta, Color.red, Time.fixedDeltaTime);
                transform.right = direction;
                transform.position += (Vector3)velocity * Delta;
                lifetime += Delta;

                if (data.gravityScale != 0)
                {
                    velocity += Physics2D.gravity * Delta;
                }
            }
            lifeNorm = Mathf.InverseLerp(0, data.maxLife, lifetime);


            if (data.doLifeScaling)
            {
                r.transform.localScale = Vector3.one * Mathf.Lerp(data.scaleOverLife.x, data.scaleOverLife.y, data.scaleCurve.Evaluate(lifeNorm));
            }
            if (data.doDragOverLife)
            {
                velocity = direction * (speed - (speed * (data.dragLifeCurve.Evaluate(lifeNorm) * data.maxDrag)));
            }
        }
        else
        {
            if (!expired)
            {
                ProjectileExpired();
            }
        }
    }
    protected void ProjectileHit(RaycastHit2D hit)
    {
        onProjectileHit?.Invoke();

        if(hit.rigidbody && hit.rigidbody.TryGetComponent(out Entity e))
        {
            e.TakeDamage(Mathf.Lerp(data.maxDamage, data.minDamage, lifeNorm), data.burnAdd);
        }


        lifetime = data.maxLife;

    }
    protected void ProjectileExpired()
    {
        expired = true;
        StartCoroutine(ReturnToPool());
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(data.poolReturnTime);
        WeaponManager.Instance.projectilePool.Release(this);
    }
}
