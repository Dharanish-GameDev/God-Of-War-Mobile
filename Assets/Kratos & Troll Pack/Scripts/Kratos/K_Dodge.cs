using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for kratos to dodge and dodge roll
/// </summary>
public class K_Dodge : MonoBehaviour
{
    [SerializeField] private K_Manager manager = null;

    // Public Variables
    [HideInInspector] public int dodgeDir = 0;                  // decides which direction to dodge

    // Animation Events
    public void DodgeToIdle()
    {
        if (!manager) return;

        // reset dir and switch states
        if (manager.Anim.GetBool(manager.anim_IsAxePicked)) manager.SwitchState(manager.axeIdleState);
        else manager.SwitchState(manager.idleState);
    }

    // Public Methods
    public void HandleDodge()
    {
        if (!manager) return;
        if (manager.InputDir.magnitude < 0.01f || !manager.canSwitchAction) return;         // don't dodge while idle and switch actions

        // press "SPACE" to dodge
        if (InputManager.Instance.IsDodgeBottonPressed)
        {
            // stop movement and aiming
            manager.StopMovement();
            if (manager.K_Axe) manager.K_Axe.isAxeAiming = false;

            // update anim
            manager.Anim.SetInteger(manager.anim_AttackStatus, 0);
            manager.Anim.SetLayerWeight(1, 0);
            manager.Anim.SetBool(manager.anim_IsShieldOpen, false);
            manager.Anim.SetBool(manager.anim_IsStatic, false);
            manager.Anim.SetTrigger(manager.anim_IsDodge);

            // dodge based on the direction
            // front
            if (manager.InputDir.z > 0.0f)
            {
                dodgeDir = 1;
                manager.Anim.SetInteger(manager.anim_DodgeDirX, 0);
                manager.Anim.SetInteger(manager.anim_DodgeDirY, 1);
                manager.Rb.AddForce(800 * transform.forward, ForceMode.Impulse);
            }
            // back
            else if (manager.InputDir.z < 0.0f)
            {
                dodgeDir = 2;
                manager.Anim.SetInteger(manager.anim_DodgeDirX, 0);
                manager.Anim.SetInteger(manager.anim_DodgeDirY, -1);
                manager.Rb.AddForce(800 * (transform.forward * -1), ForceMode.Impulse);
            }
            // left
            else if (manager.InputDir.x < 0.0f)
            {
                dodgeDir = 3;
                manager.Anim.SetInteger(manager.anim_DodgeDirX, -1);
                manager.Anim.SetInteger(manager.anim_DodgeDirY, 0);
                manager.Rb.AddForce(800 * (transform.right * -1), ForceMode.Impulse);
            }
            // right
            else if (manager.InputDir.x > 0.0f)
            {
                dodgeDir = 4;
                manager.Anim.SetInteger(manager.anim_DodgeDirX, 1);
                manager.Anim.SetInteger(manager.anim_DodgeDirY, 0);
                manager.Rb.AddForce(800 * transform.right, ForceMode.Impulse);
            }

            // switch state
            manager.SwitchState(manager.dodgeState);
        }
    }
}
