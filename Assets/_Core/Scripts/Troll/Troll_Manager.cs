using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// Manages the troll behaviours and handles its states.
/// </summary>
public class Troll_Manager : MonoBehaviour
{
    public event Action<Vector3> OnKickAttack;

    #region States
    private Troll_BaseState currentState;
    public Troll_WaitState waitState = new();
    public Troll_IdleState idleState = new();
    public Troll_WalkState walkState = new();
    public Troll_ScreamState screamState = new();
    public Troll_KickState kickState = new();
    public Troll_LAttackState lAttackState = new();
    public Troll_HAttackState hAttackState = new();
    public Troll_Dead deadState = new();
    #endregion

    #region Animation Parameters
    [HideInInspector] public int anim_State = Animator.StringToHash("State");
    [HideInInspector] public int anim_IsDead = Animator.StringToHash("IsDead");
    #endregion

    public enum Attacks
    {
        StoneSmash,
        RightLegStomp,
        LeftLegStomp,
        FirstSweep,
        SecondSweep,
        StoneDrop
    }

    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private BaseHealth trollHealth;
    [SerializeField] private Transform particleObject;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstcaleLayer;

    [Header("Values")]
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float kickRange = 1;
    [SerializeField] private float lAttackRange = 2;
    [SerializeField] private float hAttackMoveSpeed = 10;
    [SerializeField] private float playerRange = 5;
    [SerializeField] private float hAttackRange = 10;

    [Header("Debug")]
    [SerializeField] private string activeState = "";

    // Private Variables
    private readonly Collider[] playerColl = new Collider[1];
    private readonly Collider[] obstacleColl = new Collider[1];
    private VFXGraphholder vfxGraphholder;
    private float attackTimer, percent;
    private Vector3 dir, tempPos;
    private Quaternion rot, targetRot;
    private float minAttackTime = 0.8f, maxAttackTime = 2.0f;
    private float hattackChance = 0.2f;
    private bool canChooseAttack, isHAttackChecked;
    private bool canHAttack, isHAttackPointPicked;

    // Properties
    public Animator Anim { get { return anim; } }
    public NavMeshAgent Agent { get { return agent; } }
    public Vector3 HAttackPos { get; private set; }
    public float KickRange { get { return kickRange; } }
    public int KickSide { get; private set; }
    public float RotationSpeed { get { return rotationSpeed; } }
    public bool IsAttack { get; private set; }
    public bool IsLAttack { get; private set; }
    public Troll_BaseState CurrentState { get { return currentState; } } 
    public BaseHealth TrollHealth { get { return trollHealth; } }

    private void Start()
    {
        vfxGraphholder = particleObject.GetComponent<VFXGraphholder>();
        StopParticles();

        // set wait state as staring state
        currentState = screamState;
        currentState.Enter(this);

        trollHealth.OnDead += Event_OnDead;
    }

    private void OnDestroy()
    {
        trollHealth.OnDead -= Event_OnDead;
    }

    private void Update()
    {
        if (currentState != null)

        // update attack timer
        UpdateAttackTimer();

        // update the current state
        currentState.Update(this);
    }

    private void FixedUpdate()
    {
        if (currentState == null) return;

        // update the current state
        currentState.FixedUpdate(this);
    }

    private void OnDrawGizmosSelected()
    {
        // draw kick range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, kickRange);

        // draw lattack range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lAttackRange);

        // draw hattack range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, hAttackRange);

        // draw hattack position choose range
        if (LevelManager.Instance)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(LevelManager.Instance.KratosManager.transform.position, playerRange);
        }

        // draw hattackpos collider range
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(HAttackPos, 5);
    }

    // Event Methods
    private void Event_OnDead()
    {
        SwitchState(deadState);

        // show victory panel
        Invoke(nameof(StopGamePlay), 4f);
    }

    // Animation Event Methods
    public void KickAttackEvent()
    {
        CameraShake.Instance.StartShake(7f, 0.25f);
        AudioManager.Instance.PlayTrollAudioAtPoint(TrollSfx.Name.Kick, transform.position);

        // invoke kick attack event
        tempPos = transform.position + ((LevelManager.Instance.KratosManager.transform.position - transform.position).normalized * (kickRange + 2.0f));
        tempPos.y = LevelManager.Instance.KratosManager.transform.position.y;
        OnKickAttack?.Invoke(tempPos);
    }

    public void AttackPerformed(int value)
    {
        IsAttack = value != 0;
        IsLAttack = value == 1;
    }

    public void HAttackMovement(int value)
    {
        agent.speed = (value == 1) ? hAttackMoveSpeed : 0;
    }

    public void SwitchToIdle()
    {
        SwitchState(idleState);
    }

    public void PlayParticles(Attacks attack)
    {
        vfxGraphholder.Play(attack);
    }

    public void LAttackCameraShake() => CameraShake.Instance.StartShake(15f, 0.25f);

    public void PlayAudio(int value)
    {
        switch (value)
        {
            case 1: AudioManager.Instance.PlayTrollAudioAtPoint(TrollSfx.Name.Scream, transform.position); break;
            case 2: AudioManager.Instance.PlayTrollAudioAtPoint(TrollSfx.Name.StoneHit, transform.position); break;
        }
    }

    public void StoneDrop()
    {
        // shake camera
        CameraShake.Instance.StartShake(12f, 0.2f);
        AudioManager.Instance.PlayTrollAudioAtPoint(TrollSfx.Name.StoneDrop, transform.position);

        // play dust effect
        vfxGraphholder.Play(Attacks.StoneDrop);
    }

    public void TrollDead()
    {
        StoneDrop();
    }

    // Public Methods
    public void SwitchState(Troll_BaseState newState)
    {
        if (currentState == newState) return;

        currentState.Exit(this);        // exit from the current state
        currentState = newState;        // change current state to new state
        currentState.Enter(this);       // enter to the new state


        #region DEBUG: Update Active State
#if UNITY_EDITOR
        if (newState == idleState) activeState = "Idle";
        else if (newState == walkState) activeState = "Walk";
        else if (newState == screamState) activeState = "Scream";
        else if (newState == kickState) activeState = "Kick";
        else if (newState == lAttackState) activeState = "LAttack";
        else if (newState == hAttackState) activeState = "HAttack";

#endif
        #endregion
    }

    public bool RotateTowardsPosition(Vector3 position, float speed, int axis = 0)
    {
        // calculate rotation
        dir = position - transform.position;
        // rotate based on choosen which axis to face the position based on axis (axis = 1 then right axis face the target otherwise forward axis face the target
        rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir) * Quaternion.Euler(0, (axis == 1) ? -90 : 1, 0), speed);

        // update rotation
        targetRot.x = 0;
        targetRot.y = rot.y;
        targetRot.z = 0;
        targetRot.w = rot.w;
        transform.rotation = targetRot;

        // is faced the position
        if (Vector3.Dot((axis == 1) ? transform.right : transform.forward, dir.normalized) >= 0.98f) return true;
        return false;
    }

    public void StopParticles()
    {
        particleObject.GetComponent<VFXGraphholder>().Stop();
    }

    public float GetAnimationPercent()
    {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void ResetAttack()
    {
        attackTimer = -1;
        canChooseAttack = false;

        canHAttack = false;
        isHAttackChecked = false;
        isHAttackPointPicked = false;
    }

    public void HandleIdleAttacks()
    {
        // cannot choose attack
        if (!canChooseAttack) return;

        // kick attack
        if (Physics.OverlapSphereNonAlloc(transform.position, kickRange, playerColl, playerLayer) == 1)
        {
            // 4 - left side kick, 3 - right side kick
            KickSide = (transform.InverseTransformPoint(LevelManager.Instance.KratosManager.transform.position).x > 0) ? 3 : 4;
            SwitchState(kickState);
        }
        // light attack
        else if (Physics.OverlapSphereNonAlloc(transform.position, lAttackRange, playerColl, playerLayer) == 1) SwitchState(lAttackState);
        // chance to do heavy attack
        else if (Physics.OverlapSphereNonAlloc(transform.position, hAttackRange, playerColl, playerLayer) == 1) HandleHAttackChance();
        // walk state
        else SwitchState(walkState);
    }

    public void HandleWalkAttacks()
    {
        // cannot choose attack
        if (!canChooseAttack) return;

        // player not in hAttackRange or already checked can use hattack then return
        if (Physics.OverlapSphereNonAlloc(transform.position, hAttackRange, playerColl, playerLayer) == 0) return;
        if (isHAttackChecked) return;

        // calculate percent
        isHAttackChecked = true;
        percent = UnityEngine.Random.Range(0.0f, 1.0f);

        // chance to hattack attack
        if (percent >= 1 - hattackChance)
        {
            agent.SetDestination(transform.position);       // stop troll movement
            ChooseHAttackPoint();                           // choose hattack point around the player
            SwitchState(hAttackState);                      // switch to hattackstate
        }
    }

    public void SetIsAttack(bool value)
    {
        IsAttack = value;
        IsLAttack = false;
    }

    // Private Methods
    private void UpdateAttackTimer()
    {
        // set attack timer
        if (attackTimer == -1) attackTimer = UnityEngine.Random.Range(minAttackTime, maxAttackTime);

        if (attackTimer > 0) attackTimer -= Time.deltaTime;     // wait to trigger attack
        // trigger an attack
        else canChooseAttack = true;
    }

    private void HandleHAttackChance()
    {
        if (isHAttackChecked) return;

        // calculate percent
        isHAttackChecked = true;
        percent = UnityEngine.Random.Range(0.0f, 1.0f);

        // chance to walk attack
        if (percent < 1 - hattackChance) SwitchState(walkState);
        // chance to hattack attack
        else
        {
            agent.SetDestination(transform.position);       // stop troll movement
            ChooseHAttackPoint();                           // choose hattack point around the player
            SwitchState(hAttackState);                      // switch to hattackstate
        }
    }

    private void ChooseHAttackPoint()
    {
        // prevent choose hattack point multiple times
        if (canHAttack) return;

        canHAttack = true;
        for (int i = 0; i < 5; i++)
        {
            // choose random point around the player
            tempPos = LevelManager.Instance.KratosManager.transform.position + (UnityEngine.Random.insideUnitSphere * playerRange);
            tempPos.y = 0;
            HAttackPos = tempPos;

            // check choosen point is not collided with any obstacles, if collided with other objects then choose another point
            if (Physics.OverlapSphereNonAlloc(HAttackPos, 5, obstacleColl, obstcaleLayer) == 0)
            {
                isHAttackPointPicked = true;
                break;
            }
            else isHAttackPointPicked = false;
        }

        // if hattack point is not picked within 5 attemps, then choose player position
        if (!isHAttackPointPicked)
        {
            isHAttackPointPicked = true;
            tempPos = LevelManager.Instance.KratosManager.transform.position;
            tempPos.y = 0;
            HAttackPos = tempPos;
        }
    }

    private void StopGamePlay()
    {
        LevelManager.Instance.StopGameplay();
        Invoke(nameof(ShowVictoryPanel), 2f);
    }

    private void ShowVictoryPanel()
    {
        LevelManager.Instance.ShowVictoryPanel();
    }
}
