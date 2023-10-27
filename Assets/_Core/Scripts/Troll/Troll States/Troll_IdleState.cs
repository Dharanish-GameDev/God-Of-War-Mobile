using UnityEngine;

public class Troll_IdleState : Troll_BaseState
{
    private float timer;

    public override void Enter(Troll_Manager manager)
    {
        // stop agent movement
        manager.Agent.speed = 0;

        // play idle anim
        timer = 0.5f;
        manager.Anim.SetInteger(manager.anim_State, 0);
    }

    public override void Update(Troll_Manager manager)
    {
        if (LevelManager.Instance.KratosManager.IsDead) return;

        // wait 0.5sec before switch any states
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }

        // switch to walk state
        if (manager.Agent.remainingDistance > manager.Agent.stoppingDistance)
            manager.SwitchState(manager.walkState);

        // handle attacks
        manager.HandleIdleAttacks();
    }
}
