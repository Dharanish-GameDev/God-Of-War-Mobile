using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Controls the behaviour of the camera [such as smooth rotation, camera switching, etc...]
/// </summary>
public class CameraCtrl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform target = null;
    [SerializeField] [Range(0.0f, 1.0f)] private float smoothRate = 0.2f;

    [Header("Cameras")]
    [SerializeField] private GameObject followCam = null;
    [SerializeField] private GameObject aimCam = null;
    [SerializeField] private GameObject deadCam = null;
    [SerializeField] private CinemachineVirtualCamera followVcam = null;

    [Header("Kratos")]
    [SerializeField] private K_Manager manager = null;
    [SerializeField] private Transform k_Collider = null;

    [Header("Debug")]
    [SerializeField] private Transform test; 

    // Public Variables
    [HideInInspector] public bool isAim = false;

    // Private Variables
    private Cinemachine3rdPersonFollow cinemachineFollow;
    private Vector3 targetRot;
    private bool canRotate;

    // Properties
    public float DefaultFollowDistance { get; private set; }

    private void Awake()
    {
        canRotate = true;

        // set follow camera enabled
        followCam.SetActive(true);
        aimCam.SetActive(false);
        deadCam.SetActive(false);
    }

    private void Start()
    {
        // get cinemachine component
        cinemachineFollow = followVcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        DefaultFollowDistance = cinemachineFollow.CameraDistance;
    }

    private void FixedUpdate()
    {
        // kratos is dead
        if (manager.IsDead)
        {
            // set the camera to desired position
            //deadCam.transform.SetLocalPositionAndRotation(k_DeadCampoint.position, k_DeadCampoint.rotation);
            return;
        }

        // handle player rotation
        HandleRotation();

        // Handle Aiming
        if (isAim)
        {
            aimCam.SetActive(true);
            followCam.SetActive(false);
        }
        else
        {
            followCam.SetActive(true);
            aimCam.SetActive(false);
        }
    }

    // Public Methods
    public void SetCameraZDamping(float damping)
    {
        if (cinemachineFollow == null) return;
        cinemachineFollow.Damping.z = damping;
    }

    public void EnableDeadCamera()
    {
        // set the collider position
        k_Collider.SetLocalPositionAndRotation(new Vector3(0.0f, 0.75f, 0.0f), Quaternion.Euler(-90.0f, 0.0f, 0.0f));
        k_Collider.transform.up = -manager.transform.forward;

        // enable dead camera
        deadCam.SetActive(true);
        followCam.SetActive(false);
        aimCam.SetActive(false);
    }

    public void SetCanRotate(bool canRotate) => this.canRotate = canRotate;

    public void RotateTowardsPoint(Transform point)
    {
        if (!point) return;
        target.LookAt(point);
    }

    public void SetCameraFollowDistance(float distance)
    {
        if (cinemachineFollow == null) return;
        cinemachineFollow.CameraDistance = distance;
    }

    // Private Methods
    private void HandleRotation()
    {
        if (!canRotate) return;

        // calculate rotation
        // aim joystick rotation
        if (InputManager.Instance.IsAimJoystickBtnPressed && InputManager.Instance.Type == InputManager.InputType.Mobile)
        {
            // continuous rotation when aim joystick stick reach the edge
            if (InputManager.Instance.AimJoystick.Distance >= 90)           // 90 refers to the max aim joystick move distance 100 - 10 = 90
            {
                targetRot.x += InputManager.Instance.TouchDist.y;
                targetRot.y += InputManager.Instance.TouchDist.x;
            }
            // rotate when stick is moving
            else if (InputManager.Instance.AimJoystick.IsMoving)
            {
                targetRot.x += InputManager.Instance.TouchDist.y;
                targetRot.y += InputManager.Instance.TouchDist.x;
            }
        }
        // screen swipe rotation
        else
        {
            targetRot.x += InputManager.Instance.TouchDist.y;
            targetRot.y += InputManager.Instance.TouchDist.x;
        }

        // update the rotation
        targetRot.x = Mathf.Clamp(targetRot.x, -50, 30);
        target.rotation = Quaternion.Slerp(target.rotation, Quaternion.Euler(targetRot), smoothRate);
    }
}
