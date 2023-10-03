using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle Troll stone behaviour such as push player back, damage player etc...
/// </summary>
public class TrollStone : MonoBehaviour
{
    public event EventHandler onPlayerHit;

    [SerializeField] private Troll_Manager manager;
    [SerializeField] private Transform movePoint;
    [SerializeField] private Vector3 stoneSize;
    [SerializeField] private Vector3 offset;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float pushForce = 5;
    private bool hasAttacked = false;

    // Private Methods
    private readonly Collider[] coll = new Collider[1];
    private Vector3 dir, pos;

    private void FixedUpdate()
    {
        // when player hit troll stone while not performing lattack
        if (!manager.Anim.GetCurrentAnimatorStateInfo(0).IsTag("LAttack"))
        {
            NormalPushPlayer();
            return;
        }

        if(manager.IsLAttack)
        {
            PushPlayerWhileAttack(false);
        }
        else if(manager.CurrentState == manager.hAttackState)
        {
            PushPlayerWhileAttack(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // draw a wire cube around the troll stone
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + offset, stoneSize);
    }

    // Private Methods
    private void NormalPushPlayer()
    {
        // calculate dir
        dir = LevelManager.Instance.KratosManager.transform.position - manager.transform.position;
        dir.Normalize();

        // check player hits the troll stone
        if (Physics.OverlapBoxNonAlloc(transform.position + offset, stoneSize / 2, coll, transform.rotation, playerLayer) == 1)
        {
            pos = transform.position + (pushForce * dir);
            pos.y = LevelManager.Instance.KratosManager.transform.position.y;
            LevelManager.Instance.KratosManager.HandleDamage(pos, 2);
        }
    }

    private void PushPlayerWhileAttack(bool isHAttack)
    {
        // check player hits the troll stone or player near the troll
        if (Physics.OverlapBoxNonAlloc(transform.position + offset, stoneSize / 2, coll, transform.rotation, playerLayer) == 1 ||
            (Vector3.Distance(manager.transform.position, LevelManager.Instance.KratosManager.transform.position) <= manager.KickRange && manager.GetAnimationPercent() >= 0.5f))
        {
            pos = movePoint.position;
            pos.y = LevelManager.Instance.KratosManager.transform.position.y;
            LevelManager.Instance.KratosManager.HandleDamage(pos, 2);

            onPlayerHit?.Invoke(this, EventArgs.Empty);

            if(isHAttack)
            {
                Debug.Log("HAttack");
                manager.SetHAttack(0);
            }
            else
            {
                manager.SetLAttack(0);
            }
        }
    }
}
