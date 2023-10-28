using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_DodgeState : K_BaseState
{
    private bool isDodgeRoll;

    public override void Enter(K_Manager manager)
    {
        // disable axe slash effect
        manager.K_Axe.ActivateSlashEffect(0);

        if (manager.K_Axe.IsAxePicked) manager.K_Axe.EnableThrowAxe();
        else manager.K_Axe.EnableHoldAxe();
    }

    public override void Update(K_Manager manager)
    {
        if (isDodgeRoll) return;

        manager.HandleRotation();

        // press "SPACE" while dodge to perform dodge roll
        if (InputManager.Instance.IsDodgeRollButtonPressed)
        {
            isDodgeRoll = true;
            manager.StopMovement();
            manager.Anim.SetTrigger(manager.anim_IsDodgeRoll);

            // apply force
            // front
            if (manager.K_Dodge.dodgeDir == 1) manager.Rb.AddForce(1200 * manager.transform.forward, ForceMode.Impulse);
            // back
            else if (manager.K_Dodge.dodgeDir == 2) manager.Rb.AddForce(-1200 * (manager.transform.forward), ForceMode.Impulse);
            // left
            else if (manager.K_Dodge.dodgeDir == 3) manager.Rb.AddForce(-1200 * (manager.transform.right), ForceMode.Impulse);
            // right
            else if (manager.K_Dodge.dodgeDir == 4) manager.Rb.AddForce(1200 * manager.transform.right, ForceMode.Impulse);
        }
    }

    public override void Exit(K_Manager manager)
    {
        // disable axe slash effect
        manager.K_Axe.ActivateSlashEffect(0);

        // reset dodge press count
        InputManager.Instance.DodgeCountBtn.ResetPressCount();

        manager.StopMovement();
        manager.K_Dodge.dodgeDir = 0;

        // reset dodgeroll
        if (isDodgeRoll)
        {
            isDodgeRoll = false;
            manager.Anim.ResetTrigger(manager.anim_IsDodgeRoll);
        }

        // anim
        manager.Anim.ResetTrigger(manager.anim_IsDodge);
        manager.Anim.SetInteger(manager.anim_DodgeDirX, 0);
        manager.Anim.SetInteger(manager.anim_DodgeDirY, 0);
    }
}
