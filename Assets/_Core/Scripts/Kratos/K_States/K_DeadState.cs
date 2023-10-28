using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_DeadState : K_BaseState
{
    public override void Enter(K_Manager manager)
    {
        // disable axe slash effect
        manager.K_Axe.ActivateSlashEffect(0);

        manager.StopMovement();
        manager.K_Axe.CancelAxeRecall();
        manager.K_Shield.CloseShield();
        manager.K_Axe.CrossHair.enabled = false;

        // update anim
        manager.Anim.SetLayerWeight(1, 0);
        manager.Anim.SetLayerWeight(2, 0);
    }
}
