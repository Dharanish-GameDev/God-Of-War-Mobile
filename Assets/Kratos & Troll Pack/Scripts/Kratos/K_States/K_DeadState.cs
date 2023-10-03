using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_DeadState : K_BaseState
{
    public override void Enter(K_Manager manager)
    {
        manager.StopMovement();
        if (manager.K_Axe) manager.K_Axe.CrossHair.enabled = false;

        // update anim
        manager.Anim.SetLayerWeight(1, 0);
        manager.Anim.SetLayerWeight(2, 0);
    }
}
