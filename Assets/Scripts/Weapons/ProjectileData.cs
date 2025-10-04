using Balla;
using UnityEngine;

[System.Serializable]
public struct ProjectileEffect
{
    public EffectData effect;
    public bool additive;
    [Range(0, 1)] public float chance;
}



[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public float projectileSpeed = 20f;
    public float minDamage = 5f, maxDamage = 25f;
    public float maxLife = 3f;

    public Vector3 defaultScale = Vector3.one;
    public bool useCircleCast;
    public float projectileRadius;

    public float gravityScale = 1;

    public float poolReturnTime = 5;
    public float deviation = 5f;

    public Sprite projectileSprite;


    public bool doDragOverLife;
    [ReadOnly] public float speedAtDeath;
    [Range(0, 1)] public float maxDrag;
    public AnimationCurve dragLifeCurve;

    public bool doLifeScaling;
    public Vector2 scaleOverLife;
    public AnimationCurve scaleCurve;

    public float burnAdd;

    private void OnValidate()
    {
        speedAtDeath = projectileSpeed - (projectileSpeed * dragLifeCurve.Evaluate(1) * maxDrag);
    }
}
