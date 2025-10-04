using Balla;
using Balla.Core;
using System.Collections.Generic;
using UnityEngine;

public class Entity : BallaScript
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected float currentHealth;
    public float CurrentHealth => currentHealth;
    public float HealthPercentage => currentHealth / maxHealth;

    public static ulong nextID;
    public ulong ID;

    public bool IsAlive => currentHealth > 0;

    public ParticleSystem burnParticle;
    [SerializeField] protected float heatThreshold, heatDecay, extinguishTime;
    [SerializeField] protected float burnInterval, burnDamage;
    [SerializeField, ReadOnly] protected float burnTimer, extinguishTimer;
    [SerializeField, ReadOnly] protected float currentHeat;
    [SerializeField, ReadOnly] protected bool burning;
    private void Start()
    {
        //Assign an entity ID. Might use this for tracking stuff, not sure yet.
        ID = nextID++;
        nextID++;

        currentHealth = maxHealth;
    }

    protected override void Timestep()
    {
        base.Timestep();
        CalculateBurn();
    }
    protected virtual void CalculateBurn()
    {
        if(currentHeat >= 1 && !burning)
        {
            extinguishTimer = 0;
            burnTimer = 0;
        }
        burning = currentHeat >= 1;
        if (burning)
        {
            burnTimer += Delta;
            if(burnTimer >= burnInterval)
            {
                TakeDamage(burnDamage);
                burnTimer %= burnInterval;
            }
            extinguishTimer += Delta;
            if(extinguishTimer >= extinguishTime)
            {
                currentHeat = 0;
                extinguishTimer = 0;
            }
        }
        else
        {
            currentHeat = Mathf.Clamp01(currentHeat - (heatDecay * Delta));
        }
        if(burnParticle.isPlaying != burning)
        {
            if (burning)
            {
                burnParticle.Play();
            }
            else
            {
                burnParticle.Stop();
            }
        }
    }

    public void TakeDamage(float damageTaken, float burnAdd = 0)
    {
        currentHealth-= damageTaken;
        currentHeat += burnAdd;
        if(currentHealth <= 0)
        {
            EntityDied();
        }
    }

    public virtual void EntityDied()
    {
        burnParticle.transform.SetParent(null, true);
        burnParticle.Stop();
        Destroy(burnParticle.gameObject, 8f);

        Destroy(gameObject);
    }
}
