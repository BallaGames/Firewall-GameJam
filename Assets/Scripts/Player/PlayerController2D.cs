using UnityEngine;
using Balla.Core;
using Balla;
using UnityEngine.InputSystem;
using Balla.Utils;
using JetBrains.Annotations;

public class PlayerController2D : BallaScript
{
    public bool usingGamepad;
    public Vector3 aimTarget;
    public Transform aimDirector;
    public Transform aimRoot;
    public Transform weaponScaleRoot;
    [SerializeField, ReadOnly] internal Vector3 dir;
    [SerializeField, ReadOnly] internal float angle;
    [SerializeField, ReadOnly] bool aimingOnLeft;

    public Rigidbody2D rb;

    [ReadOnly] public float maxSpeed;
    public float moveForce;
    public float moveDamping;

    public float dashDelayTimer, dashSpeed;
    float dashDelay;

    [SerializeField] protected Entity entity;

    public Spring2D[] springs;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();       
    }
    private void OnValidate()
    {
        maxSpeed = moveForce / moveDamping;
    }

    protected override void AfterFrame()
    {
        base.AfterFrame();
        if(aimRoot != null)
        {
            if (usingGamepad)
            { 
                
            }
            else
            {
                aimTarget = Mouse.current.position.ReadValue();
                aimTarget.z = 10;
                aimDirector.position = Camera.main.ScreenToWorldPoint(aimTarget);
            }
            aimRoot.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(aimDirector.localPosition.y, aimDirector.localPosition.x));
            aimingOnLeft = aimDirector.localPosition.x < 0;
            weaponScaleRoot.localScale = new(1, aimingOnLeft ? -1 : 1, 1);
        }
    }
    protected override void Timestep()
    {
        base.Timestep();
        if(dashDelay < dashDelayTimer)
        {
            dashDelay += Delta;
        }

        if (Input.jumpInput)
        {
            Input.jumpInput = false;
            Dash();
        }
        rb.AddForce((moveForce * Input.moveInput) + (moveDamping * -rb.linearVelocity));

    }
    public void Dash()
    {
        if (dashDelay >= dashDelayTimer && Input.moveInput != Vector2.zero)
        {
            rb.linearVelocity = Input.moveInput * dashSpeed;
            dashDelay = 0;
        }
    }



}
