using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionData", menuName = "Scriptable Objects/ExplosionData")]
public class ExplosionData : ScriptableObject
{
    public float explosionMaxSize = 5, explosionMinSize = 0.1f;
    public AnimationCurve explosionFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    public float damageCentre = 20, damageEdge = 1;
}
