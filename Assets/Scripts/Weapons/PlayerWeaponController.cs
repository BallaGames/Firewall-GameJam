using UnityEngine;

public class PlayerWeaponController : WeaponController
{
    protected override void Timestep()
    {
        fireInput = Input.attackInput;

        base.Timestep();
    }
}
