using UnityEngine;

public class Troll_WalkState : Troll_BaseState
{
    private float timer, percent;
    private bool isTimerSet;

    public override void Enter(Troll_Manager manager)
    {
        // initialzie
        isTimerSet = false;
        manager.Agent.speed = 6;
        manager.Agent.SetDestination(LevelManager.Instance.KratosManager.transform.position);

        // play walk anim
        manager.Anim.SetInteger(manager.anim_State, 1);
    }

    public override void Update(Troll_Manager manager)
    {
        // walk towards player 
        manager.Agent.SetDestination(LevelManager.Instance.KratosManager.transform.position);

        // switch to idle state
        if (manager.Agent.remainingDistance < manager.Agent.stoppingDistance)
            manager.SwitchState(manager.idleState);

        // check can attack while walk
        manager.HandleWalkAttacks();

        // check can switch to scream state
        HandleScream(manager);
    }

    // Private Methods
    private void HandleScream(Troll_Manager manager)
    {
        // set timer
        if (!isTimerSet)
        {
            isTimerSet = true;
            timer = Random.Range(1, 2);
        }

        // update timer
        if (timer > 0) timer -= Time.deltaTime;
        else
        {
            // calculate percent
            percent = Random.Range(0.0f, 1.0f);

            // 50% chance to scream
            if (percent >= 1 - 0.5f) manager.SwitchState(manager.screamState);
            else isTimerSet = false;
        }
    }
}
