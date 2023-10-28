using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_ShieldState : K_BaseState
{
    private Vector3 movement;

    public override void Enter(K_Manager manager)
    {
        // disable axe slash effect
        manager.K_Axe.ActivateSlashEffect(0);

        if (manager.K_Axe.IsAxePicked) manager.K_Axe.EnableThrowAxe();
        else manager.K_Axe.EnableHoldAxe();
    }

    public override void Update(K_Manager manager)
    {
        // movement and rotation
        manager.CalculateMovement(false);
        manager.HandleRotation();

        // can able to switch these states
        //manager.HandleAttacks();
        manager.K_Dodge.HandleDodge();
        manager.K_Shield.HandleShieldClose();

        //// switch to block state
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    SwitchToBlockState(manager);
        //}

        // switch to deflect state
        if (Input.GetKeyDown(KeyCode.T))
        {
            // stop movement and update anim
            manager.StopMovement();
            manager.Anim.SetLayerWeight(1, 0);
            manager.Anim.SetTrigger(manager.anim_IsShieldDeflect);

            // add force
            manager.Rb.AddForce(300 * manager.transform.forward, ForceMode.Impulse);

            // change state
            manager.SwitchState(manager.shieldBlockState);
        }
    }

    public override void FixedUpdate(K_Manager manager)
    {
        manager.CheckForHealOrbs();

        // move based on the player axis
        movement.x = manager.InputDir.x * manager.MoveSpeed / 2;
        movement.y = manager.Rb.velocity.y;
        movement.z = manager.InputDir.z * manager.MoveSpeed / 2;
        manager.Rb.velocity = manager.transform.TransformDirection(movement);
    }

    //private static void SwitchToBlockState(K_Manager manager)
    //{
    //    // stop movement and update anim
    //    manager.StopMovement();
    //    manager.anim.SetLayerWeight(1, 0);
    //    manager.anim.SetTrigger(manager.anim_IsShieldBlock);

    //    // add force
    //    manager.rb.AddForce(800 * (manager.transform.forward * -1), ForceMode.Impulse);
    //    manager.cameraCtrl.SetCameraZDamping(0.0f);

    //    // change state
    //    manager.SwitchState(manager.shieldBlockState);
    //}
}
