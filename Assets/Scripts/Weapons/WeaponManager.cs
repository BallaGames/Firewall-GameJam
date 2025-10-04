using UnityEngine;
using Balla.Core;
using UnityEngine.Pool;


public class WeaponManager : BallaScript
{
    public static WeaponManager Instance;
    int projIndex;


    public int maxPoolSize, defaultPoolSize;
    public IObjectPool<Projectile> projectilePool;

    public LayerMask projectileMask;
    public Projectile projectilePrefab;
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
        projectilePool = new ObjectPool<Projectile>(CreateProjectile, TakeFromPool, ReturnToPool, OnDestroyPoolObject, true, defaultPoolSize, maxPoolSize);
        Debug.Log($"{projectilePool.CountInactive} projectiles spawned for pool...");
    }
    Projectile CreateProjectile()
    {
        projIndex++;
        Projectile p = Instantiate(projectilePrefab);
        p.gameObject.hideFlags = HideFlags.HideInHierarchy;
        return p;
    }
    void TakeFromPool(Projectile p)
    {
        p.gameObject.SetActive(true);
    }
    void ReturnToPool(Projectile p)
    {
        p.gameObject.SetActive(false);
    }
    void OnDestroyPoolObject(Projectile p)
    {
        Destroy(p.gameObject);
    }


    protected override void Timestep()
    {

    }

    public Projectile ProjectileFired(RangedWeapon rangedWeapon)
    {
        projectilePool.Get(out Projectile proj);
        proj.Initialise(rangedWeapon);

        return proj;
    }
}
