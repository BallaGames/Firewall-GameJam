using Balla.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "BurningEffect", menuName = "Scriptable Objects/BurningEffect")]
public class BurningEffect : EffectData
{
    public float burnThreshold;
    public float burnDecay;
    public float burnIncrease;

    public float damagePerTick = 1f;
    public override void UpdateEffect(Entity entity, EntityEffect effect)
    {

    }
}
