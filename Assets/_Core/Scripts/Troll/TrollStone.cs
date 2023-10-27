using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle Troll stone behaviour such as push player back, damage player etc...
/// </summary>
public class TrollStone : MonoBehaviour
{
    //public event EventHandler onPlayerHit;

    [SerializeField] private Troll_Manager manager;
    [SerializeField] private Transform movePoint;
    [SerializeField] private Vector3 stoneSize;
    [SerializeField] private Vector3 offset;
    [SerializeField] private LayerMask playerLayer;

    // Private Methods
    private readonly Collider[] coll = new Collider[1];
    private Vector3 dir, pos;

    private void FixedUpdate()
    {
        // check troll is attacking
        if (!manager.IsAttack) return;
        HandlePlayerDamage();
    }

    private void OnDrawGizmosSelected()
    {
        // draw a wire cube around the troll stone
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + offset, stoneSize);
    }

    private void HandlePlayerDamage()
    {
        dir = LevelManager.Instance.KratosManager.transform.position - manager.transform.position;

        // light attack
        if (manager.IsLAttack)
        {
            // check trollstone hits the player
            if (Physics.OverlapBoxNonAlloc(transform.position + offset, stoneSize / 2, coll, transform.rotation, playerLayer) == 1 ||
                (dir.sqrMagnitude <= manager.KickRange * manager.KickRange && manager.GetAnimationPercent() >= 0.45f))
            {
                // push back the player and deals damage
                pos = movePoint.position;
                pos.y = LevelManager.Instance.KratosManager.transform.position.y;
                LevelManager.Instance.KratosManager.HandleDamage(pos, 2, 30);

                // prevent damage multiple times
                manager.SetIsAttack(false);
            }

            return;
        }

        // hatttack
        // check trollstone hits the player
        if (Physics.OverlapBoxNonAlloc(transform.position + offset, stoneSize / 2, coll, transform.rotation, playerLayer) != 1 &&
            dir.sqrMagnitude > manager.KickRange * manager.KickRange) return;

        // push back the player and deals damage
        pos = movePoint.position;
        pos.y = LevelManager.Instance.KratosManager.transform.position.y;
        LevelManager.Instance.KratosManager.HandleDamage(pos, 2, 30);

        // prevent damage multiple times
        manager.SetIsAttack(false);
    }
}
