using UnityEngine;

public enum EffectType
{
    instant = 0,
    overTime = 1,
    threshold = 2,
}


/// <summary>
/// Data container for an effect applied to an entity. Can either be applied over time, when a certain threshold is reached, or once when the effect is gained.
/// <b></b>Also handles the application of effects to an entity. This class does nothing on its own.
/// </summary>
[CreateAssetMenu(fileName = "EffectData", menuName = "Scriptable Objects/EffectData")]
public class EffectData : ScriptableObject
{
    public uint effectID;
    public EffectType effectType;

    public string effectKey;

    [Tooltip("Does the particle system play as a loop, rather than playing multiple times, like a bursting effect.\nThis will always apply if the effect type is Instant.")]
    public bool particleLoops;
    [Tooltip("If 'Particle Loops' is false, this determines how many times the particle will play.\nThis is ignored if the effect type is Instant")]
    public float particleInterval;
    [Tooltip("How many times the particle system should be played")]
    public int particleCount;
    [Tooltip("The entity effect to be instantiated under the target")]
    public EntityEffect effectParticles;

    public float deleteTime;
    public float lifetime;

    public bool useBuildup;
    public float buildupAdded;

    public float effectInterval;

    public virtual void EffectAdded(Entity entity, EntityEffect effect)
    {

    }
    
    public virtual void EffectRemoved(Entity entity, EntityEffect effect)
    {

    }

    /// <summary>
    /// called by the entity when the specified interval on this effect has elapsed.
    /// </summary>
    /// <param name="entity"></param>
    public virtual void UpdateEffect(Entity entity, EntityEffect effect)
    {
        
    }
}
