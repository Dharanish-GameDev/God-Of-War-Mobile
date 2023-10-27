using UnityEngine;

public class Troll_ScreamState : Troll_BaseState
{
    private Vector3 playerPos;
    private float timer;
    private bool isStartScream;

    public override void Enter(Troll_Manager manager)
    {
        isStartScream = false;

        // stop agent movement
        manager.Agent.speed = 0;
        playerPos = LevelManager.Instance.KratosManager.transform.position;

        // set wait timer and play idle anim
        timer = 0.5f;
        manager.Anim.SetInteger(manager.anim_State, 0);
    }

    public override void Update(Troll_Manager manager)
    {
        if (isStartScream) return;

        // face the player and start scream
        if (manager.RotateTowardsPosition(playerPos, manager.RotationSpeed))
        {
            if (timer > 0) timer -= Time.deltaTime;                     // wait to scream
            else
            {
                isStartScream = true;
                manager.Anim.SetInteger(manager.anim_State, 2);        // play scream anim
            }
        }
    }

    public override void Exit(Troll_Manager manager)
    {
        // reset attack timer
        manager.ResetAttack();
    }
}
