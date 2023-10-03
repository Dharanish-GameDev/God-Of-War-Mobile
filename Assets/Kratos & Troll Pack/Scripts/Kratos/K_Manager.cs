using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the behaviour of the kratos [such as idle, walk, run, attack, dead]
/// </summary>
[RequireComponent(typeof(K_Dodge), typeof(K_Shield), typeof(K_Axe))]
public class K_Manager : MonoBehaviour
{
    #region States
    public K_BaseState currentState;
    public K_IdleState idleState = new();
    public K_WalkState walkState = new();
    public K_RunState runState = new();
    public K_DeadState deadState = new();
    public K_AttackState attackState = new();
    public K_DodgeState dodgeState = new();
    public K_ShieldState shieldState = new();
    public K_ShieldBlockState shieldBlockState = new();
    public K_AxeIdleState axeIdleState = new();
    public K_AimState aimState = new();
    public K_DamageState damageState = new();
    #endregion

    #region Animation Parameters
    [HideInInspector] public int anim_SpeedX = Animator.StringToHash("Speed_X");
    [HideInInspector] public int anim_SpeedZ = Animator.StringToHash("Speed_Z");
    [HideInInspector] public int anim_AxeStatus = Animator.StringToHash("AxeStatus");
    [HideInInspector] public int anim_IsStatic = Animator.StringToHash("IsStatic");
    [HideInInspector] public int anim_IsAxePicked = Animator.StringToHash("IsAxePicked");
    [HideInInspector] public int anim_IsAxeThrown = Animator.StringToHash("IsAxeThrown");
    [HideInInspector] public int anim_IsAxeRecall = Animator.StringToHash("IsAxeRecall");
    [HideInInspector] public int anim_IsAxeReturned = Animator.StringToHash("IsAxeReturned");
    [HideInInspector] public int anim_CanAim = Animator.StringToHash("CanAim");
    [HideInInspector] public int anim_IsShieldOpen = Animator.StringToHash("IsShieldOpen");
    [HideInInspector] public int anim_IsShieldDeflect = Animator.StringToHash("IsShieldDeflect");
    [HideInInspector] public int anim_IsShieldBlock = Animator.StringToHash("IsShieldBlock");
    [HideInInspector] public int anim_IsDodge = Animator.StringToHash("IsDodge");
    [HideInInspector] public int anim_DodgeDirX = Animator.StringToHash("DodgeDirX");
    [HideInInspector] public int anim_DodgeDirY = Animator.StringToHash("DodgeDirY");
    [HideInInspector] public int anim_IsDodgeRoll = Animator.StringToHash("IsDodgeRoll");
    [HideInInspector] public int anim_AttackStatus = Animator.StringToHash("AttackStatus");
    [HideInInspector] public int anim_DamageState = Animator.StringToHash("DamageState");
    #endregion

    // debug purpose only
    [SerializeField] private string activeState = "";

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float runSpeed = 8.0f;
    [SerializeField] [Range(0.1f, 1.0f)] private float rotationSmooth = 0.5f;

    [Header("Refereces")]
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rb;

    // Public Variables
    [HideInInspector] public bool canChangeAttack = false;
    [HideInInspector] public bool canSwitchAction = true;
    [HideInInspector] public int attackStatus = 0;

    // Private Variables
    private Vector3 tempVec;
    private bool isDead = false;
    [SerializeField] private LayerMask whatToHit;

    // Properties
    public Animator Anim { get { return anim; } }
    public Rigidbody Rb { get { return rb; } }
    public K_Dodge K_Dodge { get; private set; }
    public K_Shield K_Shield { get; private set; }
    public K_Axe K_Axe { get; private set; }
    public Vector3 InputDir { get; private set; }
    public Vector3 DamageMovePos { get; private set; }
    public float MoveSpeed { get { return moveSpeed; } }
    public float RunSpeed { get { return runSpeed; } }

    private void Start()
    {
        // initialise scripts
        K_Dodge = gameObject.GetComponent<K_Dodge>();
        K_Shield = gameObject.GetComponent<K_Shield>();
        K_Axe = gameObject.GetComponent<K_Axe>();

        // set starting state
        activeState = "Idle";
        currentState = idleState;
        currentState.Enter(this);

        if (LevelManager.Instance) LevelManager.Instance.TrollManager.OnKickAttack += Event_TrollOnKickAttack;
    }

    private void Update()
    {
        // calculate input
        tempVec.x = InputManager.Instance.Horizontal;
        tempVec.z = InputManager.Instance.Vertical;
        InputDir = tempVec;

        // dead
        if (!isDead && Input.GetKeyDown(KeyCode.K)) HandleDead();

        // update the active state
        currentState.Update(this);
    }

    private void FixedUpdate()
    {
        // update the active state
        currentState.FixedUpdate(this);
    }

    // Event Methods
    private void Event_TrollOnKickAttack(Vector3 pos)
    {
        HandleDamage(pos, 1);
    }

    // Property Methods
    public bool GetIsDead()
    {
        return isDead;
    }

    // Animation Events
    public void EnableDeadCamera()
    {
        StopMovement();
        LevelManager.Instance.CamCtrl.EnableDeadCamera();
    }

    public void CanChangeAttack()
    {
        canChangeAttack = true;
    }

    public void StopAttack(int status = 0)
    {
        // enable throw axe when axe is picked
        if (K_Axe && status == 1) K_Axe.EnableThrowAxe();

        attackStatus = 0;
        canChangeAttack = false;
        LevelManager.Instance.CamCtrl.SetCameraZDamping(0.4f);
        Anim.SetInteger(anim_AttackStatus, attackStatus);

        // switch state
        if (InputDir.magnitude > 0.1f) SwitchState(walkState);
        else
        {
            if (status == 1) SwitchState(axeIdleState);
            else SwitchState(idleState);
        }
    }

    public void ResetDamageAnim()
    {
        // reset performed attack combo
        //attackStatus = 0;
        //anim.SetInteger(anim_AttackStatus, attackStatus);
        Debug.Log("Reset");
        // reset damage
        Anim.SetInteger(anim_DamageState, 0);

        if (Anim.GetBool(anim_IsAxePicked))
        {
            K_Axe.EnableThrowAxe();
            SwitchState(axeIdleState);
        }
        else
        {
            K_Axe.EnableHoldAxe();
            SwitchState(idleState);
        }
    }

    public void SetAttackStatus(int status)
    {
        //Debug.Log("set attack: " + status);
        attackStatus = status;
        Anim.SetInteger(anim_AttackStatus, attackStatus);
    }

    // Public Methods
    public void SwitchState(K_BaseState newState)
    {
        // change current state
        if (currentState == newState) return;
        currentState.Exit(this);
        currentState = newState;

        #region DEBUG: Update Active State
#if UNITY_EDITOR
        if (newState == idleState) activeState = "Idle";
        else if (newState == walkState) activeState = "Walk";
        else if (newState == runState) activeState = "Run";
        else if (newState == attackState) activeState = "Attack";
        else if (newState == deadState) activeState = "Dead";
        else if (newState == dodgeState) activeState = "Dodge";
        else if (newState == shieldState) activeState = "Shield";
        else if (newState == shieldBlockState) activeState = "Shield Block";
        else if (newState == axeIdleState) activeState = "Axe Idle";
        else if (newState == aimState) activeState = "Aim";
        else if (newState == damageState) activeState = "Damaged";
        else activeState = "error";
#endif
        #endregion

        // enter the new changed state
        currentState.Enter(this);
    }

    public void CalculateMovement(bool canRun = true)
    {
        // switch to run
        if (InputManager.Instance.Type == InputManager.InputType.Pc)
        {
            if (canRun && InputDir.z > 0.0f && InputManager.Instance.IsRunPressed && InputDir.x == 0) SwitchState(runState);
        }
        else
        {
            if (canRun && InputDir.z > 0.0f && InputManager.Instance.IsRunPressed) SwitchState(runState);
        }

        //if (canRun && InputDir.z > 0.0f && InputManager.Instance.IsRunPressed && InputDir.x == 0) SwitchState(runState);
        //else    // walk
        //{
        //    movement.x = inputDir.x;
        //    movement.z = inputDir.z;
        //}

        // reduce the speed while moving diagonal
        if (Mathf.Abs(InputDir.x) >= 1 && Mathf.Abs(InputDir.z) >= 1) InputDir.Normalize();

        // update anim
        Anim.SetFloat(anim_SpeedX, InputDir.x);
        Anim.SetFloat(anim_SpeedZ, InputDir.z);
    }

    public void StopMovement()
    {
        // zero the rigidbody velocity
        //inputDir.x = 0;
        //inputDir.z = 0;
        //inputDir.y = 0;
        Rb.velocity = Vector3.zero;
    }

    public void HandleRotation()
    {
        // rotation based on camera forward direction
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, LevelManager.Instance.CamCtrl.transform.eulerAngles.y, 0.0f), rotationSmooth);
    }

    public void HandleAttacks()
    {
        // cannot attack while switch animations
        if (!canSwitchAction) return;

        // light attack
        if (InputManager.Instance.IsLAttackButtonPressed)
        {
            // stop runing in mobile once press attack btn
            InputManager.Instance.MovementBtn.StopRun();

            // apply forward force and slow down the camera follow
            Rb.AddForce(200 * transform.forward, ForceMode.Impulse);
            LevelManager.Instance.CamCtrl.SetCameraZDamping(1.0f);

            // update anim
            attackStatus = 1;
            Anim.SetLayerWeight(1, 0);
            Anim.SetBool(anim_IsShieldOpen, false);
            Anim.SetBool(anim_IsStatic, false);
            Anim.SetInteger(anim_AttackStatus, attackStatus);

            SwitchState(attackState);   // switch state
        }
        // heavy attack
        else if (InputManager.Instance.IsHAttackButtonPressed)
        {
            // stop runing in mobile once press attack btn
            InputManager.Instance.MovementBtn.StopRun();

            // apply more forward force and slowly the camera follow
            Rb.AddForce(250 * transform.forward, ForceMode.Impulse);
            LevelManager.Instance.CamCtrl.SetCameraZDamping(1.0f);

            // update anim
            attackStatus = 5;
            Anim.SetLayerWeight(1, 0);
            Anim.SetBool(anim_IsShieldOpen, false);
            Anim.SetBool(anim_IsStatic, false);
            Anim.SetInteger(anim_AttackStatus, attackStatus);

            SwitchState(attackState);   // switch state
        }
    }

    public void HandleDamage(Vector3 pos, int damageId)
    {
        // reset performed attack combo
        //attackStatus = 0;
        //anim.SetInteger(anim_AttackStatus, attackStatus);

        // play damage anim
        DamageMovePos = pos;
        Anim.SetInteger(anim_DamageState, damageId);

        // switch to damage state
        SwitchState(damageState);
    }

    public void SwitchToBlockState()
    {
        // stop movement and update anim
        StopMovement();
        Anim.SetLayerWeight(1, 0);
        Anim.SetTrigger(anim_IsShieldBlock);

        // add force
        Rb.AddForce(800 * (transform.forward * -1), ForceMode.Impulse);
        LevelManager.Instance.CamCtrl.SetCameraZDamping(0.0f);

        // change state
        SwitchState(shieldBlockState);
    }

    Collider[] rangeChecks;
    public void Attack()
    {
        rangeChecks = Physics.OverlapSphere(transform.position, 4, whatToHit);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;

            if (Vector3.Angle(transform.forward, target.position) < 180 / 2)
            {
                BaseHealth baseHealth;
                target.gameObject.TryGetComponent<BaseHealth>(out baseHealth);
                baseHealth.GiveDamage(20);
            }
        }
    }

    // Private Methods
    private void HandleDead()
    {
        isDead = true;
        Rb.AddForce(800 * (transform.forward * -1), ForceMode.Impulse); // apply backward force
        Anim.SetBool("IsDead", true);
        SwitchState(deadState);     // switch state
    }
}
