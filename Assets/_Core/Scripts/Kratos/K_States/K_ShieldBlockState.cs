using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_ShieldBlockState : K_BaseState
{
    public override void Enter(K_Manager manager)
    {
        // disable axe slash effect
        manager.K_Axe.ActivateSlashEffect(0);

        manager.Anim.SetLayerWeight(1, 0);
        manager.Anim.SetBool(manager.anim_IsStatic, false);
    }

    public override void Exit(K_Manager manager)
    {
        // disable axe slash effect
        manager.K_Axe.ActivateSlashEffect(0);
        manager.Anim.SetBool(manager.anim_IsStatic, false);

        manager.StopMovement();
        manager.K_Shield.SetIsBlock(false);
        manager.Anim.ResetTrigger(manager.anim_IsShieldBlock);
        manager.Anim.ResetTrigger(manager.anim_IsShieldDeflect);
    }
}
