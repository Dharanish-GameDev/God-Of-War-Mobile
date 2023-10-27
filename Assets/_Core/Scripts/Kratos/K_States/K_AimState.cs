using UnityEngine;

public class K_AimState : K_BaseState
{
    private float value;
    private Vector3 movement;

    public override void Enter(K_Manager manager)
    {
        manager.K_Axe.StartAiming();
    }

    public override void Update(K_Manager manager)
    {
        // press and hold "LEFTCONTROL" for aiming otherwise stop aiming
        if (InputManager.Instance.IsAimJoystickBtnPressed)
        {
            manager.K_Axe.isAxeAiming = true;
            manager.Anim.SetLayerWeight(1, 1);
            LevelManager.Instance.CamCtrl.isAim = true;                          // enable aim camera

            // smoothly transition to aiming state
            value = Mathf.MoveTowards(value, 0.5f, Time.deltaTime * 5);
            manager.Anim.SetFloat(manager.anim_AxeStatus, value);
        }
        else
        {
            LevelManager.Instance.CamCtrl.isAim = false;         // disable aim camera

            // smoothly transition to idle state
            value = Mathf.MoveTowards(value, 0, Time.deltaTime * 5);
            manager.Anim.SetFloat(manager.anim_AxeStatus, value);

            // stop aiming
            if (value <= 0.01f)
            {
                manager.K_Axe.StopAiming();

                // switch states
                if (manager.InputDir.magnitude > 0.1f) manager.SwitchState(manager.walkState);
                else manager.SwitchState(manager.axeIdleState);
            }
        }

        // movement and rotation
        manager.CalculateMovement();
        manager.HandleRotation();

        // can able to switch these states
        manager.K_Axe.HandleAxeThrow();
        manager.K_Shield.HandleShieldOpen();
        manager.K_Dodge.HandleDodge();
    }

    public override void FixedUpdate(K_Manager manager)
    {
        // move based on the player axis
        movement.x = manager.InputDir.x * manager.MoveSpeed / 2;
        movement.y = manager.Rb.velocity.y;
        movement.z = manager.InputDir.z * manager.MoveSpeed / 2;
        manager.Rb.velocity = manager.transform.TransformDirection(movement);
    }
}
