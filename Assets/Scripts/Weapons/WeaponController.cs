using UnityEngine;
using Balla.Core;
using Balla;

public class WeaponController : BallaScript
{
    [SerializeField, ReadOnly] protected bool fireInput;

    public RangedWeapon2D weapon;

    [SerializeField] protected Entity entity;

    protected virtual void Start()
    {
        entity = GetComponent<Entity>();
    }

    protected override void Timestep()
    {
        weapon.fireInput = fireInput;
    }
}
