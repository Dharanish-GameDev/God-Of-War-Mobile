using UnityEngine;

public class K_AttackState : K_BaseState
{
    public override void Enter(K_Manager manager)
    {
        manager.canChangeAttack = false;
        manager.StopMovement();
    }

    public override void Update(K_Manager manager)
    {
        manager.HandleRotation();
        manager.K_Shield.HandleShieldOpen();            // cancel attack while open shield
        manager.K_Dodge.HandleDodge();                  // cancel attack while dodge

        #region axe attack
        // axe attack
        if (manager.Anim.GetBool(manager.anim_IsAxePicked))
        {
            if (!manager.canChangeAttack) return;

            // light attack
            if (InputManager.Instance.IsLAttackButtonPressed)
            {
                manager.canChangeAttack = false;

                switch (manager.attackStatus)
                {
                    case 1: manager.attackStatus = 2; break;
                    case 2: manager.attackStatus = 3; break;
                    case 5: manager.attackStatus = 1; break;
                }

                UpdateAnimation(ref manager, 200);
            }
            // heavy attack
            else if (InputManager.Instance.IsHAttackButtonPressed)
            {
                manager.canChangeAttack = false;

                switch (manager.attackStatus)
                {
                    case 5: manager.attackStatus = 6; break;
                    default: manager.attackStatus = 5; break;
                }
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
            manager.canChangeAttack = false;

            switch (manager.attackStatus)
            {
                case 1: manager.attackStatus = 2; break;
                case 2: manager.attackStatus = 3; break;
                case 3: manager.attackStatus = 4; break;
            }
            UpdateAnimation(ref manager, 200);
        }
        // heavy attack
        else if (InputManager.Instance.IsHAttackButtonPressed)
        {
            manager.canChangeAttack = false;

            manager.attackStatus = 5;
            UpdateAnimation(ref manager, 250);
        }
    }

    public override void Exit(K_Manager manager)
    {
        manager.attackStatus = 0;
        manager.canChangeAttack = true;
        manager.Anim.SetInteger(manager.anim_AttackStatus, manager.attackStatus);
    }

    // Private Methods
    private void UpdateAnimation(ref K_Manager manager, int force)
    {
        // apply force
        manager.Rb.velocity = Vector3.zero;
        manager.Rb.AddForce(force * manager.transform.forward, ForceMode.Impulse);

        // change animation
        manager.Anim.SetInteger(manager.anim_AttackStatus, manager.attackStatus);
    }
}
