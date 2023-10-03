using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Behaviour for kratos to use axe [such as throw, attack]
/// </summary>
public class K_Axe : MonoBehaviour
{
    private event Action OnFOVChanged;                              // use the event to change the camera fov

    [SerializeField] private K_Manager manager = null;

    [Header("Movement")]
    [SerializeField] private int throwForce = 150;
    [SerializeField] [Range(0.5f, 3.0f)] private float returnSpeed = 1.0f;

    [Header("Axe")]
    [SerializeField] private SkinnedMeshRenderer holdAxe = null;
    [SerializeField] private GameObject throwAxe = null;
    [SerializeField] private Transform axeHolder = null;
    [SerializeField] private Transform throw_Point = null;
    [SerializeField] private Transform return_Point = null;
    [SerializeField] private Transform curvePoint = null;

    [Header("Crosshair")]
    [SerializeField] private Camera cam_Crosshair = null;
    [SerializeField] private Image crossHair = null;
    [SerializeField] private LayerMask hitMask;

    [Header("References")]
    [SerializeField] private Rigidbody axeRB = null;
    [SerializeField] private AxeCtrl axeCtrl = null;

    // Public Variables
    [HideInInspector] public bool isAxeAiming = false;
    public Image CrossHair { get { return crossHair; } set { crossHair = value; } }
    public bool IsAxeThrown { get; private set; }

    // Private Variables
    private bool isAxeRecall = false;
    private bool isThrowing = false;

    private Vector3 dir;                        // direction to throw axe              
    private float distance;                     // distance between hit object and crosshair
    private RaycastHit hitInfo;                 // check any object is hit the crosshair
    private Vector2 crosshairSize;              // change crosshair based on object distance
    private float percent;                      // how much distance axe travel while axe returning in pecent [0 - not recall, 1 - returned]
    private Vector3 startPos;                   // stores the start pos of the axe when return
    private Vector3 axeRotation;                // rotation of the axe while return

    private void Start()
    {
        crossHair.enabled = false;
        OnFOVChanged += ChangeCameraFOV;        // subscribe to the event
    }

    private void OnDestroy()
    {
        OnFOVChanged -= ChangeCameraFOV;        // unsubscribe to the event
    }

    private void Update()
    {
        if (!isAxeAiming) return;
        HandleCrosshair();  // handle crosshair behaviour
    }

    // Animation Events Methods
    public void DisableRHLayer()
    {
        if (!manager) return;
        IsAxeThrown = false;
        isAxeAiming = false;
        manager.Anim.SetLayerWeight(2, 0);
        manager.Anim.SetBool(manager.anim_CanAim, true);
        manager.Anim.SetBool(manager.anim_IsStatic, false);
    }           // disable righthand layer

    public void AxePickup()
    {
        if (!manager) return;
        holdAxe.enabled = false;
        throwAxe.SetActive(true);
        manager.Anim.SetLayerWeight(1, 0);
    }

    public void AxePutdown()
    {
        if (!manager) return;
        holdAxe.enabled = true;
        throwAxe.SetActive(false);
    }

    public void CanSwitchAction()
    {
        if (!manager) return;
        manager.canSwitchAction = true;
    }

    public void CannotSwitchAction()
    {
        if (!manager) return;
        manager.canSwitchAction = false;
    }

    public void EnableHoldAxe()
    {
        if (!manager) return;
        holdAxe.enabled = true;
        throwAxe.SetActive(false);
    }

    public void ThrowAxe()
    {
        if (!manager) return;

        manager.Anim.SetBool(manager.anim_CanAim, false);   // update anim

        // setup axe
        axeCtrl.SetIsActivate(true);
        throwAxe.SetActive(true);
        axeRB.isKinematic = false;
        throwAxe.transform.SetParent(null);
        throwAxe.transform.position = throw_Point.position;
        throwAxe.transform.right = transform.forward;

        // apply force
        axeRB.AddForce(dir.normalized * throwForce, ForceMode.Impulse);
    }

    public void DisableAxe()
    {
        if (!manager) return;

        isThrowing = false;
        manager.Anim.SetInteger(manager.anim_AttackStatus, 0);
        manager.canSwitchAction = true;
        holdAxe.enabled = false;
        LevelManager.Instance.CamCtrl.isAim = false;

        // update anim
        manager.Anim.SetLayerWeight(1, 0);
        manager.Anim.SetBool(manager.anim_IsStatic, false);
    }

    public void RecallAxe()
    {
        if (!manager) return;

        isAxeRecall = true;
        IsAxeThrown = true;
        axeRB.isKinematic = true;
        startPos = throwAxe.transform.position;
        axeRotation = throwAxe.transform.eulerAngles;
        axeRotation.x = 0.0f;
        throwAxe.transform.eulerAngles = axeRotation;
        manager.Anim.SetBool(manager.anim_IsStatic, true);
    }

    public void AxeReturned()
    {
        if (!manager) return;

        // convert recall btn into aim joystick
        InputManager.Instance.AimCtrlBtn.ChangeRecallBtnAimJoystick();

        // reset values
        percent = 0;
        isAxeRecall = false;
        IsAxeThrown = true;
        holdAxe.enabled = false;
        axeCtrl.SetIsActivate(false);
        axeCtrl.SetIsRecall(false);
        throwAxe.transform.SetParent(axeHolder);
        throwAxe.SetActive(true);
        throwAxe.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        throwAxe.transform.localScale = Vector3.one;

        // update anim
        manager.Anim.SetBool(manager.anim_IsAxePicked, true);
        manager.Anim.SetBool(manager.anim_IsAxeThrown, false);
        manager.Anim.SetBool(manager.anim_IsAxeRecall, false);
        manager.Anim.SetBool(manager.anim_IsAxeReturned, false);
        if (isAxeAiming) manager.Anim.SetFloat(manager.anim_AxeStatus, 0.5f);

        // switch states
        if (manager.InputDir.magnitude > 0.1f) manager.SwitchState(manager.walkState);
        else manager.SwitchState(manager.axeIdleState);
    }


    // Public Methods
    public void EnableThrowAxe()
    {
        holdAxe.enabled = false;
        throwAxe.SetActive(true);
    }

    public void HandleAxePickup()
    {
        if (!manager) return;
        if (IsAxeThrown) return;
        if (manager.Anim.GetLayerWeight(2) == 1) return;
        if (manager.Anim.GetBool(manager.anim_IsAxePicked)) return;

        // press "1" to pickup axe
        if (InputManager.Instance.IsAxePick)
        {
            InputManager.Instance.AimCtrlBtn.ChangeAttackBtnToAxe();    // change attack btn image to axe when pickup axe
            InputManager.Instance.AimCtrlBtn.EnableAimCtrlBtn();        // enable aim joystick
            manager.canSwitchAction = false;                            // cannot switch to another animation untill pickup

            // update anim
            manager.Anim.SetLayerWeight(2, 1);
            manager.Anim.SetBool(manager.anim_IsAxePicked, true);

            // switch state
            if (manager.InputDir.magnitude > 0.1f)
            {
                manager.Anim.SetBool(manager.anim_IsStatic, true);
                manager.SwitchState(manager.walkState);
            }
            else manager.SwitchState(manager.axeIdleState);
        }
    }

    public void HandleAxePutdown()
    {
        if (!manager) return;
        if (IsAxeThrown) return;
        if (manager.Anim.GetLayerWeight(2) == 1) return;
        if (!manager.Anim.GetBool(manager.anim_IsAxePicked)) return;

        // press "1" to putdown axe
        if (InputManager.Instance.IsAxePick)
        {
            InputManager.Instance.AimCtrlBtn.ChangeAttackBtnToHand();       // change attack btn image to hand when putdown axe
            InputManager.Instance.AimCtrlBtn.DisableAimCtrlBtn();           // disable aim joystick

            // update anim
            manager.Anim.SetLayerWeight(2, 1);
            manager.Anim.SetBool(manager.anim_IsAxePicked, false);

            // switch state
            if (manager.InputDir.magnitude > 0.1f)
            {
                manager.Anim.SetBool(manager.anim_IsStatic, true);
                manager.SwitchState(manager.walkState);
            }
            else manager.SwitchState(manager.idleState);
        }
    }

    public void HandleAxeAiming()
    {
        if (!manager) return;
        if (!manager.canSwitchAction) return;

        // press and hold "LEFTCONTROL" to aim
        if (!isAxeAiming && InputManager.Instance.IsAimJoystickBtnPressed) manager.SwitchState(manager.aimState);
    }

    public void StartAiming()
    {
        if (!manager) return;

        // enable crosshair and aim camera
        isAxeAiming = true;
        LevelManager.Instance.CamCtrl.isAim = true;
        crossHair.enabled = true;
        OnFOVChanged?.Invoke();                 // use the event to change the camera fov

        // update anim
        manager.canSwitchAction = false;
        manager.Anim.SetLayerWeight(1, 1);
        manager.Anim.SetBool(manager.anim_IsStatic, true);
    }

    public void StopAiming()
    {
        if (!manager) return;

        // disable crosshair and aim camera
        isAxeAiming = false;
        manager.canSwitchAction = true;
        LevelManager.Instance.CamCtrl.isAim = false;
        crossHair.enabled = false;
        OnFOVChanged?.Invoke();             // use the event to change the camera fov

        // anim
        if (!isThrowing) manager.Anim.SetLayerWeight(1, 0);
        manager.Anim.SetBool(manager.anim_IsStatic, false);
    }

    public void HandleAxeThrow()
    {
        if (!manager) return;
        if (!isAxeAiming || IsAxeThrown) return;

        // press "LEFTMOUSEBUTTON" to throw axe
        if (InputManager.Instance.IsAxeThrow)
        {
            // convert aim joystick into recall btn
            InputManager.Instance.AimCtrlBtn.ChangeAimJoystickToRecallBtn();

            IsAxeThrown = true;
            isThrowing = true;
            manager.canSwitchAction = false;
            crossHair.enabled = false;
            OnFOVChanged?.Invoke();             // use the event to change the camera fov

            // update anim
            if (manager.InputDir.magnitude < 0.1f) manager.Anim.SetBool(manager.anim_IsStatic, false);
            manager.Anim.SetLayerWeight(1, 1);
            manager.Anim.SetBool(manager.anim_IsAxePicked, false);
            manager.Anim.SetBool(manager.anim_IsAxeThrown, true);
            manager.Anim.SetFloat(manager.anim_AxeStatus, 0.0f);

            // switch state
            if (manager.InputDir.magnitude > 0.1f) manager.SwitchState(manager.walkState);
            else manager.SwitchState(manager.idleState);
        }
    }

    public void HandleAxeRecall()
    {
        if (!IsAxeThrown) return;

        // press "R" to recall the axe
        if (InputManager.Instance.IsAxeRecall && !manager.Anim.GetBool(manager.anim_IsAxeRecall))
        {
            axeCtrl.SetIsRecall(true);

            // update anim
            manager.Anim.SetLayerWeight(2, 1);
            manager.Anim.SetBool(manager.anim_IsAxeRecall, true);
            manager.Anim.SetBool(manager.anim_IsStatic, true);
        }

        if (!isAxeRecall) return;

        // move axe towards player hands
        if (percent < 1.0f)
        {
            // axe movement in curve
            throwAxe.transform.position = CalculateQudraticCurve(percent, startPos, curvePoint.position, return_Point.position);

            // handle axe rotation
            if (percent > 0.1f && !axeCtrl.GetIsActivate()) axeCtrl.SetIsActivate(true);
            else if (percent > 0.8f)
            {
                axeCtrl.SetIsActivate(false);
                throwAxe.transform.rotation = Quaternion.Slerp(throwAxe.transform.rotation, return_Point.rotation, Time.deltaTime * 20f);
                manager.Anim.SetBool(manager.anim_IsAxeReturned, true);
            }

            percent += Time.deltaTime * returnSpeed;
        }
        // axe returned
        else
        {
            IsAxeThrown = false;
            manager.Anim.SetBool(manager.anim_IsAxeRecall, false);
            manager.Anim.SetBool(manager.anim_IsAxeReturned, true);
        }
    }


    // Private Methods
    private Vector3 CalculateQudraticCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return (((1 - t) * (1 - t) * p0) + (2 * (1 - t) * t * p1) + (t * t * p2));
    }

    private void HandleCrosshair()
    {
        // calculate direction
        dir = crossHair.transform.position - throw_Point.position;

        // object hits the crosshair (chnage crosshair size)
        if (Physics.Raycast(throw_Point.position, dir, out hitInfo, dir.magnitude + 5.0f, hitMask))
        {
            // calculate distance
            dir = hitInfo.point - throw_Point.position;
            crossHair.transform.position = hitInfo.point;
            distance = Vector3.Distance(throw_Point.position, hitInfo.point);

            // change crosshair weight and height based on the object distance
            crosshairSize.x = Mathf.Lerp(70.0f, 450.0f, Mathf.InverseLerp(5.0f, 60.0f, distance));
            crosshairSize.y = Mathf.Lerp(70.0f, 450.0f, Mathf.InverseLerp(5.0f, 60.0f, distance));
            crossHair.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, crosshairSize.x);
            crossHair.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, crosshairSize.y);

            // for debug
            //Debug.DrawRay(throw_Point.position, dir, Color.red);
        }
        // object not hits the crosshair
        else
        {
            crossHair.transform.localPosition = Vector3.zero;
            dir = crossHair.transform.position - throw_Point.position;

            // change crosshair weight and height to default size
            crosshairSize.x = 450.0f;
            crosshairSize.y = 450.0f;
            crossHair.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, crosshairSize.x);
            crossHair.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, crosshairSize.y);

            // for debug
            //Debug.DrawRay(throw_Point.position, dir, Color.yellow);
        }
    }

    private void ChangeCameraFOV()
    {
        cam_Crosshair.fieldOfView = Camera.main.fieldOfView;
    }
}
