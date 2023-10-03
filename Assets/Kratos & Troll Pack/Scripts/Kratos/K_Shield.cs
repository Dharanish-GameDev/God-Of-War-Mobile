using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for kratos to use the shield to block and perry attacks
/// </summary>
public class K_Shield : MonoBehaviour
{
    [SerializeField] private K_Manager manager = null;

    // Animation Events
    public void DisableUBLayer()
    {
        if (!manager) return;
        manager.Anim.SetLayerWeight(1, 0);  // disable upperbody layer
    }

    public void DisableStatic()
    {
        if (!manager) return;
        manager.Anim.SetBool(manager.anim_IsStatic, false);
    }

    public void BlockToIdle()
    {
        if (!manager) return;

        // reset camera follow speed
        LevelManager.Instance.CamCtrl.SetCameraZDamping(0.4f);

        // update anim
        if (InputManager.Instance.IsShieldButtonPressed) manager.Anim.SetBool(manager.anim_IsShieldOpen, true);
        else manager.Anim.SetBool(manager.anim_IsShieldOpen, false);

        if (manager.Anim.GetBool(manager.anim_IsAxePicked))
        {
            manager.Anim.SetBool(manager.anim_IsStatic, true);
            manager.SwitchState(manager.axeIdleState);
        }
        else
        {
            manager.Anim.SetBool(manager.anim_IsStatic, false);
            manager.SwitchState(manager.idleState);
        }
    }


    // Public Methods
    public void HandleShieldOpen()
    {
        if (!manager) return;
        if (!manager.canSwitchAction) return;

        // press and hold "Q" to open shield
        if (InputManager.Instance.IsShieldButtonPressed)
        {
            // stop run in mobile once open shield
            InputManager.Instance.MovementBtn.StopRun();

            // make sure camera is not aiming
            LevelManager.Instance.CamCtrl.isAim = false;

            // update anim
            manager.Anim.SetLayerWeight(1, 1);
            manager.Anim.SetBool(manager.anim_IsStatic, true);
            manager.Anim.SetBool(manager.anim_IsShieldOpen, true);

            manager.SwitchState(manager.shieldState);   // switch state
        }
    }

    public void HandleShieldClose()
    {
        if (!manager) return;

        // release "Q" to close the shield
        if (!InputManager.Instance.IsShieldButtonPressed)
        {
            manager.Anim.SetBool(manager.anim_IsShieldOpen, false);

            // switch to walk state
            if (manager.InputDir.magnitude > 0.1f)
            {
                manager.Anim.SetLayerWeight(1, 0);
                manager.Anim.SetBool(manager.anim_IsStatic, false);
                manager.SwitchState(manager.walkState);
                return;
            }

            // switch to idle state
            if (manager.Anim.GetBool(manager.anim_IsAxePicked)) manager.SwitchState(manager.axeIdleState);
            else manager.SwitchState(manager.idleState);
        }
    }
}
