using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_AxeIdleState : K_BaseState
{
    public override void Enter(K_Manager manager)
    {
        if (manager.InputDir.magnitude > 0.1f) return;
        manager.StopMovement();
    }

    public override void Update(K_Manager manager)
    {
        // switch states
        if (manager.InputDir.magnitude > 0.1f)
        {
            // walk only
            if (manager.K_Axe.isAxeAiming) manager.SwitchState(manager.walkState);
            else
            {
                if (InputManager.Instance.IsRunPressed) manager.SwitchState(manager.runState);     // run
                else manager.SwitchState(manager.walkState);                                    // walk
            }
        }

        // update anim
        manager.Anim.SetFloat(manager.anim_SpeedX, 0);
        manager.Anim.SetFloat(manager.anim_SpeedZ, 0);

        // can able to switch these states
        manager.K_Axe.HandleAxeAiming();
        manager.K_Axe.HandleAxePutdown();
        manager.K_Axe.HandleAxeThrow();
        manager.K_Shield.HandleShieldOpen();
        manager.HandleAttacks();
    }

    public override void FixedUpdate(K_Manager manager)
    {
        manager.CheckForHealOrbs();
    }
}
