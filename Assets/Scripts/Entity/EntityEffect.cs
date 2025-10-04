using Balla.Core;
using UnityEngine;


public enum FollowType
{
    position,
    rotation,
    fullPose
}


/// <summary>
/// Handles playing particle effects on an entity
/// </summary>
public class EntityEffect : BallaScript
{
    public Transform target;
    public Vector3 localPos;
    public Quaternion localRot;

    public FollowType followType;

    ParticleSystem particle;

    public float lifetime;
    public EffectData effectData;
    public int count;
    public float interval;
    public bool pendingDelete;

    public bool useParticles;
    public bool _particlesActive;

    public void InitialiseEffect(EffectData effectData, bool additive)
    {
        lifetime = additive ? effectData.lifetime : (lifetime + effectData.lifetime);
    }

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void EffectLost()
    {
        pendingDelete = true;
        interval = 0;
    }

    public void AttachToTransform(Transform t, Vector3 pos, Quaternion rot)
    {
        target = transform;
        localPos = pos;
        localRot = rot;
    }
    protected override void AfterFrame()
    {
        switch (followType)
        {
            case FollowType.position:
                transform.position = target.TransformPoint(localPos);
                break;
            case FollowType.rotation:
                transform.rotation = target.rotation * localRot;
                break;
            case FollowType.fullPose:
                transform.SetPositionAndRotation(target.TransformPoint(localPos), target.rotation * localRot);
                break;
            default:
                break;
        }

        if(useParticles != _particlesActive)
        {
            _particlesActive = useParticles;
            if (useParticles)
            {
                particle.Play();
            }
            else
            {
                particle.Stop();
            }
        }
    }
    protected override void Timestep()
    {
        interval += Delta;
        if (!pendingDelete)
        {
            if (interval > effectData.particleInterval)
            {
                if (count < effectData.particleCount)
                {
                    count++;
                    particle.Play();
                    interval = 0;
                }
            }
        }
        else
        {
            if (interval > effectData.deleteTime)
            {
                Destroy(gameObject);
            }
        }
    }
}
