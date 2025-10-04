using UnityEngine;
using Balla.Core;
using UnityEngine.Pool;


public class WeaponManager : BallaScript
{
    public static WeaponManager Instance;
    int projIndex;


    public int maxPoolSize, defaultPoolSize;
    public IObjectPool<Projectile2D> projectilePool;

    public LayerMask projectileMask;
    public Projectile2D projectilePrefab;
    private void Awake()
    {
        //Set up singleton.
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        //Initialise projectile pool
        projectilePool = new ObjectPool<Projectile2D>(CreateProjectile, TakeFromPool, ReturnToPool, OnDestroyPoolObject, true, defaultPoolSize, maxPoolSize);
        Debug.Log($"{projectilePool.CountInactive} projectiles spawned for pool...");
    }
    Projectile2D CreateProjectile()
    {
        projIndex++;
        Projectile2D p = Instantiate(projectilePrefab);
        p.gameObject.hideFlags = HideFlags.HideInHierarchy;
        return p;
    }
    void TakeFromPool(Projectile2D p)
    {
        p.gameObject.SetActive(true);
    }
    void ReturnToPool(Projectile2D p)
    {
        p.gameObject.SetActive(false);
    }
    void OnDestroyPoolObject(Projectile2D p)
    {
        Destroy(p.gameObject);
    }


    protected override void Timestep()
    {

    }

    public Projectile2D ProjectileFired(RangedWeapon2D rangedWeapon)
    {
        projectilePool.Get(out Projectile2D proj);
        proj.Initialise(rangedWeapon);

        return proj;
    }
}
