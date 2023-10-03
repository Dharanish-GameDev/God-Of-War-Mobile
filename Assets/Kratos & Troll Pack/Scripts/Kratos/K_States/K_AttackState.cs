using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_AttackState : K_BaseState
{
    public override void Enter(K_Manager manager)
    {
        manager.StopMovement();
    }

    public override void Update(K_Manager manager)
    {
        manager.HandleRotation();

        #region axe attack
        // axe attack
        if (manager.Anim.GetBool(manager.anim_IsAxePicked))
        {
            if (!manager.canChangeAttack) return;

            // light attack
            if (InputManager.Instance.IsLAttackButtonPressed)
            {
                if (manager.attackStatus == 5) manager.attackStatus = 1;
                else manager.attackStatus++;

                UpdateAnimation(ref manager, 200);
            }
            // heavy attack
            else if (InputManager.Instance.IsHAttackButtonPressed)
            {
                if (manager.attackStatus < 5) manager.attackStatus = 5;
                else manager.attackStatus++;

                UpdateAnimation(ref manager, 250);
            }

            return;
        }
        #endregion

        // combat attack
        if (!manager.canChangeAttack) return;

        // light attack
        if (InputManager.Instance.IsLAttackButtonPressed)
        {
            if (manager.attackStatus < 4) manager.attackStatus++;
            UpdateAnimation(ref manager, 200);
        }
        // heavy attack
        else if (InputManager.Instance.IsHAttackButtonPressed)
        {
            manager.attackStatus = 5;
            UpdateAnimation(ref manager, 250);
        }
    }

    public override void Exit(K_Manager manager)
    {
        manager.attackStatus = 0;
        manager.Anim.SetInteger(manager.anim_AttackStatus, manager.attackStatus);
    }

    // Private Methods
    private void UpdateAnimation(ref K_Manager manager, int force)
    {
        // apply force
        manager.Rb.AddForce(force * manager.transform.forward, ForceMode.Impulse);

        // change animation
        manager.Anim.SetInteger(manager.anim_AttackStatus, manager.attackStatus);
        manager.canChangeAttack = false;
    }
}
