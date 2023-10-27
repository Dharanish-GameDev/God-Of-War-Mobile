using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for kratos to use the shield to block and perry attacks
/// </summary>
[RequireComponent(typeof(K_Manager))]
public class K_Shield : MonoBehaviour
{
    [SerializeField] private GameObject blockEffect;
    [SerializeField] private ParticleSystemStopCallback blockStopCallback;

    private K_Manager manager = null;

    // Properties
    public bool IsBlock { get; private set; }

    private void Start()
    {
        manager = GetComponent<K_Manager>();

        blockEffect.SetActive(false);
        blockStopCallback.OnParticleStopped += Event_OnParticleStopped;
    }

    private void OnDisable()
    {
        blockStopCallback.OnParticleStopped -= Event_OnParticleStopped;
    }

    // Event Methods
    private void Event_OnParticleStopped()
    {
        // deactivate block effect
        blockEffect.SetActive(false);
    }


    // Animation Events
    public void DisableUBLayer()
    {
        manager.Anim.SetLayerWeight(1, 0);  // disable upperbody layer
    }

    public void DisableStatic()
    {
        manager.Anim.SetBool(manager.anim_IsStatic, false);
    }

    public void BlockToIdle()
    {
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

    public void PlayShieldAudio(int state)
    {
        // shield opens
        if (state == 1) AudioManager.Instance.PlayKratosAudioAtPoint(KratosSfx.Name.ShieldOpen, manager.transform.position);
        else AudioManager.Instance.PlayKratosAudioAtPoint(KratosSfx.Name.ShieldClose, manager.transform.position);
    }


    // Public Methods
    public void HandleShieldOpen()
    {
        if (!manager.canSwitchAction) return;

        // press and hold "Q" to open shield
        if (InputManager.Instance.IsShieldButtonPressed)
        {
            IsBlock = true;

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
        // release "Q" to close the shield
        if (!InputManager.Instance.IsShieldButtonPressed)
        {
            IsBlock = false;

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

    public void CloseShield()
    {
        manager.Anim.SetBool(manager.anim_IsShieldOpen, false);
        manager.Anim.SetLayerWeight(1, 0);
    }

    public void SwitchToBlockState()
    {
        // stop movement and update anim
        manager.StopMovement();
        manager.Anim.SetLayerWeight(1, 0);
        manager.Anim.SetTrigger(manager.anim_IsShieldBlock);

        // add force
        manager.Rb.AddForce(800 * -manager.transform.forward, ForceMode.Impulse);
        LevelManager.Instance.CamCtrl.SetCameraZDamping(0.0f);

        // change state
        manager.SwitchState(manager.shieldBlockState);
    }

    public void ShowBlockEffect(Vector3 position)
    {
        blockEffect.transform.position = position;
        blockEffect.SetActive(true);
    }

    public void SetIsBlock(bool value)
    {
        IsBlock = value;
    }
}
