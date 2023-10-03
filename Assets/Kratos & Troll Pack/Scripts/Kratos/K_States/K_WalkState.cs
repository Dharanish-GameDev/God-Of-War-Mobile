using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_WalkState : K_BaseState
{
    private Vector3 movement;

    public override void Update(K_Manager manager)
    {
        // switch states
        if (manager.InputDir.magnitude < 0.1f)
        {
            if (!manager.Anim.GetBool(manager.anim_IsAxePicked)) manager.SwitchState(manager.idleState);
            else manager.SwitchState(manager.axeIdleState);
        }

        // movement and rotation
        manager.CalculateMovement();
        manager.HandleRotation();

        // can able to switch these states
        manager.HandleAttacks();
        if (manager.K_Dodge) manager.K_Dodge.HandleDodge();
        if (manager.K_Shield) manager.K_Shield.HandleShieldOpen();
        if (manager.K_Axe)
        {
            manager.K_Axe.HandleAxePickup();
            manager.K_Axe.HandleAxePutdown();
            if (manager.Anim.GetBool(manager.anim_IsAxePicked)) manager.K_Axe.HandleAxeAiming();
            manager.K_Axe.HandleAxeThrow();
            manager.K_Axe.HandleAxeRecall();
        }
    }

    public override void FixedUpdate(K_Manager manager)
    {
        // move based on the player axis
        movement.x = manager.InputDir.x * manager.MoveSpeed;
        movement.y = manager.Rb.velocity.y;
        movement.z = manager.InputDir.z * manager.MoveSpeed;
        manager.Rb.velocity = manager.transform.TransformDirection(movement);
    }
}
