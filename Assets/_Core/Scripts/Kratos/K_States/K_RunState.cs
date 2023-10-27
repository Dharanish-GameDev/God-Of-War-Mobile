using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_RunState : K_BaseState
{
    private Vector3 movement;

    public override void Enter(K_Manager manager)
    {
        LevelManager.Instance.CamCtrl.SetCameraZDamping(0.8f);
        manager.K_Axe.CancelAxeRecall();
    }

    public override void Update(K_Manager manager)
    {
        // stop aiming
        if (manager.K_Axe.isAxeAiming) manager.K_Axe.StopAiming();

        // switch states
        if (!InputManager.Instance.IsRunPressed)
        {
            if (manager.InputDir.magnitude < 0.1f) manager.SwitchState(manager.idleState);
            else manager.SwitchState(manager.walkState);
        }
        else
        {
            if (InputManager.Instance.Type == InputManager.InputType.Pc)
            {
                if (manager.InputDir.magnitude < 0.1f || manager.InputDir.x != 0)
                    manager.SwitchState(manager.walkState);
            }
            else
            {
                if (manager.InputDir.magnitude < 0.1f)
                    manager.SwitchState(manager.walkState);
            }
        }

        // movement and rotation
        //movement.z = Mathf.Lerp(movement.z, 2, Time.deltaTime * manager.moveSpeed);
        manager.HandleRotation();

        // can able to switch these states
        manager.K_Shield.HandleShieldOpen();
        manager.HandleAttacks();

        // updates anim
        manager.Anim.SetFloat(manager.anim_SpeedX, manager.InputDir.x);
        manager.Anim.SetFloat(manager.anim_SpeedZ, Mathf.Lerp(movement.z, 2, Time.deltaTime * manager.MoveSpeed));
    }

    public override void FixedUpdate(K_Manager manager)
    {
        manager.CheckForHealOrbs();

        // move based on the player axis
        movement.x = manager.InputDir.x * manager.MoveSpeed;
        movement.y = manager.Rb.velocity.y;
        movement.z = manager.InputDir.z * manager.RunSpeed;
        manager.Rb.velocity = manager.transform.TransformDirection(movement);
    }
}
