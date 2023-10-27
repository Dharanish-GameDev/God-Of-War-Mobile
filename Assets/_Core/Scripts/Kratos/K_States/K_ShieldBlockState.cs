using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_ShieldBlockState : K_BaseState
{
    public override void Enter(K_Manager manager)
    {
        manager.Anim.SetLayerWeight(1, 0);
    }

    public override void Exit(K_Manager manager)
    {
        manager.StopMovement();
        manager.K_Shield.SetIsBlock(false);
        manager.Anim.ResetTrigger(manager.anim_IsShieldBlock);
        manager.Anim.ResetTrigger(manager.anim_IsShieldDeflect);
    }
}
