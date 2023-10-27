using System;
using System.Collections;
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
    public K_HealState healState = new();
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
    [HideInInspector] public int anim_HealState = Animator.StringToHash("HealState");
    #endregion

    // debug purpose only
    [SerializeField] private string activeState = "";

    [Header("Values")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float runSpeed = 8.0f;
    [SerializeField] [Range(0.1f, 1.0f)] private float rotationSmooth = 0.5f;
    [SerializeField] private float collectRange = 1f;

    [Header("Refereces")]
    [SerializeField] private Animator anim; 
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BaseHealth kratosHealth;
    [SerializeField] private LayerMask whatToHit;
    [SerializeField] private LayerMask whatToBlock;
    [SerializeField] private LayerMask collectableLayer;

    [Header("HitPoints")]
    [SerializeField] private Transform handPoint;
    [SerializeField] private Transform kickPoint;
    [SerializeField] private Transform axePoint;
    [SerializeField] private Transform shieldPoint;
    [SerializeField] private float handRange = 1f;
    [SerializeField] private float kickRange = 1f;
    [SerializeField] private float axeRange = 1f;
    [SerializeField] private float shieldRange = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource feetAudioSrc;
    [SerializeField] private AudioSource breatheAudioSrc;

    // Public Variables
    [HideInInspector] public bool canChangeAttack = false;
    [HideInInspector] public bool canSwitchAction = true;
    [HideInInspector] public int attackStatus = 0;

    // Private Variables
    private readonly Collider[] rangeChecks = new Collider[1];
    private readonly Collider[] orbChecks = new Collider[1];
    private HealOrb healOrb;
    private Vector3 tempVec;

    // Properties
    public Animator Anim { get { return anim; } }
    public Rigidbody Rb { get { return rb; } }
    public BaseHealth KratosHealth { get { return kratosHealth; } }
    public K_Dodge K_Dodge { get; private set; }
    public K_Shield K_Shield { get; private set; }
    public K_Axe K_Axe { get; private set; }
    public Vector3 InputDir { get; private set; }
    public Vector3 DamageMovePos { get; private set; }
    public float MoveSpeed { get { return moveSpeed; } }
    public float RunSpeed { get { return runSpeed; } }
    public bool IsDead { get; private set; }

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
        IsDead = false;

        if (LevelManager.Instance) LevelManager.Instance.TrollManager.OnKickAttack += Event_TrollOnKickAttack;
        kratosHealth.OnDamage += Event_OnDamage;
        kratosHealth.OnHeal += Event_OnHeal;
        kratosHealth.OnDead += Event_OnDead;
    }

    private void OnDisable()
    {
        StopMovement();
        anim.SetFloat(anim_SpeedX, 0);
        anim.SetFloat(anim_SpeedZ, 0);
        anim.SetLayerWeight(1, 0);
        anim.SetLayerWeight(2, 0);

        SwitchState(idleState);
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance) LevelManager.Instance.TrollManager.OnKickAttack -= Event_TrollOnKickAttack;
        kratosHealth.OnDamage -= Event_OnDamage;
        kratosHealth.OnHeal -= Event_OnHeal;
        kratosHealth.OnDead -= Event_OnDead;
    }

    private void Update()
    {
        // dead
        if (IsDead) return;

        if (Input.GetKeyDown(KeyCode.Y))
        {
            kratosHealth.GiveDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            kratosHealth.GiveHealth(10);
        }

        // calculate input
        tempVec.x = InputManager.Instance.Horizontal;
        tempVec.z = InputManager.Instance.Vertical;
        InputDir = tempVec;

        // update the active state
        currentState.Update(this);
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        // update the active state
        currentState.FixedUpdate(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(handPoint.position, handRange);
        Gizmos.DrawWireSphere(axePoint.position, axeRange);
        Gizmos.DrawWireSphere(shieldPoint.position, shieldRange);
        Gizmos.DrawWireSphere(kickPoint.position, kickRange);
    }

    // Event Methods
    private void Event_TrollOnKickAttack(Vector3 pos)
    {
        HandleDamage(pos, 1, 15);
    }

    private void Event_OnDamage(float currentHealth)
    {
        if (currentHealth >= 30) return;

        // start breathe audio
        if (!breatheAudioSrc.isPlaying) breatheAudioSrc.Play();
    }

    private void Event_OnHeal(float currentHealth)
    {
        if (currentHealth < 30) return;

        // stop breathe audio
        if (breatheAudioSrc.isPlaying) breatheAudioSrc.Stop();
    }

    private void Event_OnDead()
    {
        // stop breathe
        if (breatheAudioSrc.isPlaying) breatheAudioSrc.Stop();

        HandleDead();
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

    public void CanSwitchAttack(int value)
    {
        canSwitchAction = value == 1;
    }

    public void StopAttack(int status = 0)
    {
        // enable throw axe when axe is picked
        if (status == 1) K_Axe.EnableThrowAxe();

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
        attackStatus = 0;
        K_Shield.SetIsBlock(false);
        anim.SetInteger(anim_AttackStatus, attackStatus);

        // reset damage
        anim.SetInteger(anim_DamageState, 0);

        if (anim.GetBool(anim_IsAxePicked) && !anim.GetBool(anim_IsAxeThrown))
        {
            K_Axe.EnableThrowAxe();
            SwitchState(axeIdleState);
        }
        else if (!anim.GetBool(anim_IsAxePicked) && anim.GetBool(anim_IsAxeThrown))
        {
            K_Axe.EnableThrowAxe();
            SwitchState(idleState);
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

    public void AttackDamage(int id)
    {
        switch (id)
        {
            case 1: HandleDamageAtPoint(handPoint.position, handRange, 10); break;           // normal attack
            case 2: HandleDamageAtPoint(shieldPoint.position, shieldRange, 10); break;
            case 3: HandleDamageAtPoint(handPoint.position, handRange, 10); break;
            case 4: HandleDamageAtPoint(shieldPoint.position, shieldRange, 10); break;
            case 5: HandleDamageAtPoint(shieldPoint.position, shieldRange, 10); break;
            case 6: HandleDamageAtPoint(kickPoint.position, kickRange, 15); break;
            case 7: HandleDamageAtPoint(axePoint.position, axeRange, 10); break;             // axe attack
            case 8: HandleDamageAtPoint(axePoint.position, axeRange, 10); break;
            case 9: HandleDamageAtPoint(axePoint.position, axeRange, 10); break;
            case 10: HandleDamageAtPoint(axePoint.position, axeRange, 15); break;
            case 11: HandleDamageAtPoint(axePoint.position, axeRange, 15); break;
        }
    }

    public void CollectHealOrb()
    {
        // no heal orb to collect
        if (orbChecks[0] == null) return;

        CameraShake.Instance.StartShake(0.75f, 0.15f);

        // collect heal orb
        if (healOrb) healOrb.Heal(this);
        healOrb.ShowActive(false);
        healOrb = null;
    }

    public void HealToIdle()
    {
        // update anim
        anim.SetInteger(anim_HealState, 0);

        // switch to idle state
        if (anim.GetBool(anim_IsAxePicked)) SwitchState(axeIdleState);
        else SwitchState(idleState);
    }

    public void ShowShieldBlockSlowMotion(int state)
    {
        // start slow motion
        if (state == 1) GameCtrl.Instance.StartSlowMotion(0.2f);
        else
        {
            GameCtrl.Instance.StopSlowMotion();
            StopMovement();
        }
    }

    public void PlayAttackAudio(int attackStatus)
    {
        switch (attackStatus)
        {
            // lattacks audio
            case 1: AudioManager.Instance.PlayKratosAudioAtPoint(KratosSfx.Name.LAttack1, transform.position); break;
            case 2: AudioManager.Instance.PlayKratosAudioAtPoint(KratosSfx.Name.LAttack2, transform.position); break;
            case 3: AudioManager.Instance.PlayKratosAudioAtPoint(KratosSfx.Name.LAttack3, transform.position); break;
            case 4: AudioManager.Instance.PlayKratosAudioAtPoint(KratosSfx.Name.LAttack4, transform.position); break;

            // hattacks audio
            case 5: AudioManager.Instance.PlayKratosAudioAtPoint(KratosSfx.Name.HAttack1, transform.position); break;
            case 6: AudioManager.Instance.PlayKratosAudioAtPoint(KratosSfx.Name.HAttack2, transform.position); break;
        }
    }

    public void PlayFeetLandedAudio() => feetAudioSrc.Play();

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
        else if (newState == healState) activeState = "Heal";
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

    public void HandleDamage(Vector3 pos, int damageId, int damageAmount)
    {
        if (IsDead) return;

        // reset performed attack combo
        attackStatus = 0;
        anim.SetInteger(anim_AttackStatus, attackStatus);

        // block the attack if possible
        if (K_Shield.IsBlock && CheckCanBlockAttack())
        {
            // play shield block audio
            AudioManager.Instance.PlayKratosAudioAtPoint(KratosSfx.Name.ShieldBlock, transform.position);

            CameraShake.Instance.StartShake(35.0f, 0.125f);
            K_Shield.ShowBlockEffect(shieldPoint.position);
            K_Shield.SwitchToBlockState();
            return;
        }

        // give damage to kratos
        LevelManager.Instance.KratosHealth.GiveDamage(damageAmount);

        // play damage anim
        DamageMovePos = pos;
        Anim.SetInteger(anim_DamageState, damageId);

        // switch to damage state
        SwitchState(damageState);
    }

    public void CheckForHealOrbs()
    {
        // no healorb within the range
        if (Physics.OverlapSphereNonAlloc(transform.position, collectRange, orbChecks, collectableLayer) != 1)
        {
            // deactivate healorb
            if (healOrb)
            {
                healOrb.ShowActive(false);
                healOrb = null;
            }

            return;
        }

        // activate the healorb
        if (!healOrb)
        {
            if (orbChecks[0].TryGetComponent(out healOrb))
                healOrb.ShowActive(true);
        }

        // Press 'F' key to collect heal orb
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Play heal audio
            AudioManager.Instance.PlayKratosAudioAtPoint(KratosSfx.Name.Heal, transform.position);

            anim.SetInteger(anim_HealState, 1);
            SwitchState(healState);
        }
    }

    public bool CheckCanBlockAttack()
    {
        return Physics.OverlapSphereNonAlloc(shieldPoint.position, shieldRange * 2, rangeChecks, whatToBlock) == 1;
    }

    // Private Methods
    private void HandleDamageAtPoint(Vector3 point, float attackRange, int damageAmount)
    {
        if (Physics.OverlapSphereNonAlloc(point, attackRange, rangeChecks, whatToHit) != 1) return;
        LevelManager.Instance.TrollManager.TrollHealth.GiveDamage(damageAmount);
    }

    private void HandleDead()
    {
        IsDead = true;
        Rb.AddForce(800 * -transform.forward, ForceMode.Impulse); // apply backward force
        Anim.SetInteger(anim_DamageState, 0);
        Anim.SetBool("IsDead", true);
        SwitchState(deadState);     // switch state
    }
}
