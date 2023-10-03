using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_IdleState : K_BaseState
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
            if (InputManager.Instance.IsRunPressed) manager.SwitchState(manager.runState);
            else manager.SwitchState(manager.walkState);
        }

        // update anim
        manager.Anim.SetFloat(manager.anim_SpeedX, 0);
        manager.Anim.SetFloat(manager.anim_SpeedZ, 0);

        // can able to switch these states
        manager.HandleAttacks();
        if (manager.K_Dodge) manager.K_Dodge.HandleDodge();
        if (manager.K_Shield) manager.K_Shield.HandleShieldOpen();
        if (manager.K_Axe)
        {
            manager.K_Axe.HandleAxePickup();
            manager.K_Axe.HandleAxeRecall();
        }
    }

}
