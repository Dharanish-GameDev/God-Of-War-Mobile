using System.Diagnostics;

public class Troll_HAttackState : Troll_BaseState
{
    public override void Enter(Troll_Manager manager)
    {
        manager.TrollStone.onPlayerHit += GiveDamage;

        // play hattack anim
        manager.Anim.SetInteger(manager.anim_State, 6);

        // stop agent and set destination towards the target point
        manager.Agent.speed = 0;
        manager.Agent.stoppingDistance = 5;
        manager.Agent.SetDestination(manager.HAttackPos);
    }

    private void GiveDamage(object sender, System.EventArgs e)
    {
        if(LevelManager.Instance.KratosHealth==null)
        {
            Debug.Print("It aint here");
            return;
        }
        LevelManager.Instance.KratosHealth.GiveDamage(30);
    }

    public override void Update(Troll_Manager manager)
    {
        // wait to troll rotate towards the target
        if (!manager.RotateTowardsPosition(manager.HAttackPos, manager.RotationSpeed)) return;

        // play hattack end anim
        if (manager.Agent.remainingDistance < manager.Agent.stoppingDistance)
        {
            manager.Agent.speed = 0;
            manager.Anim.SetInteger(manager.anim_State, 0);
        }      
    }

   

    public override void Exit(Troll_Manager manager)
    {
        manager.TrollStone.onPlayerHit -= GiveDamage;

        // reset agent values
        manager.Agent.stoppingDistance = 10;
        manager.Agent.SetDestination(LevelManager.Instance.KratosManager.transform.position);

        // reset attack timer
        manager.ResetAttack();
    }
}
